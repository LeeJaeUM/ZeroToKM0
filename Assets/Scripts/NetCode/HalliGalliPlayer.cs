using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System.Collections;

public class HalliGalliPlayer : NetworkBehaviour
{
    public NetworkVariable<int> m_playerNum = new NetworkVariable<int>(0); // 기본값 0
    private static int nextPlayerNum = 0; // 서버에서 고유 번호를 관리
    private bool m_isMyTurn = false;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer) // 서버에서만 값을 설정
        {
            m_playerNum.Value = nextPlayerNum;
            nextPlayerNum++;
        }
    }

    public int GetPlayerNumber()
    {
        return m_playerNum.Value;
    }


    [ServerRpc]  // 소유자(IsOwner == true)만 호출 가능
    private void RequestOpenCardServerRpc(int playerNum)
    {
        // 서버에서 OpenCard 실행
        GameManager.Instance.OpenCard(playerNum);
    }
    [ServerRpc]
    private void RequestRingBellServerRpc(int playerNum)
    {
        // 서버에서 OpenCard 실행
        GameManager.Instance.RingBell(playerNum);
    }

    public void TestMyTurnCheck(int curTurn)    //TODO: TurnManager에서 보낸 액션을 받아 현재 자신의 턴인지 체크
    {
        if (m_playerNum.Value == curTurn)
        {
            m_isMyTurn = true;
        }
        else
        {
            m_isMyTurn = false;
        }
    }

    private void Start()
    {
        GameManager.Instance.m_turnManager.OnCurTurnNum += TestMyTurnCheck;
    }

    private void OnDisable()
    {
        GameManager.Instance.m_turnManager.OnCurTurnNum -= TestMyTurnCheck;
    }

    private void Update()
    {
        if (!IsOwner) return; // 소유자가 아니면 실행하지 않음

        if (m_isMyTurn)           //자기 턴 일때만 OpenCard실행
        {
            if (Input.GetMouseButtonDown(1)) // OpenCard 체크용
            {
                // 클라이언트가 서버에서 OpenCardServerRpc 호출
                RequestOpenCardServerRpc(m_playerNum.Value);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))        //벨 울리는건 언제나 가능
        {
            RequestRingBellServerRpc(m_playerNum.Value);
        }

    }
}
