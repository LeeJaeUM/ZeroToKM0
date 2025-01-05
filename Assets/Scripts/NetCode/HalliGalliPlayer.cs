using UnityEngine;
using Unity.Netcode;

public class HalliGalliPlayer : NetworkBehaviour
{
    public NetworkVariable<int> m_playerNum = new NetworkVariable<int>(0); // 기본값 0
    private static int nextPlayerNum = 1; // 서버에서 고유 번호를 관리
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
    private void Update()
    {
        if(!IsOwner) return; // 소유자가 아니면 실행하지 않음

        if (Input.GetMouseButtonDown(1)) // OpenCard 체크용
        {
            // 클라이언트가 서버에서 OpenCardServerRpc 호출
            RequestOpenCardServerRpc(m_playerNum.Value);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RequestRingBellServerRpc(m_playerNum.Value);
        }

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
}
