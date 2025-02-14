using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using static UnityEngine.Rendering.DebugUI;

// Player 숫자를 받아서 Player들의 참여 정보와 턴을 관리해주는 스크립트
public class TurnManager : NetworkBehaviour
{
    [SerializeField] private List<int> m_alivePlayers;     // 살아있는 Player들의 인덱스
    [SerializeField] private int m_currentTurn;            // 현재 차례인 Player의 m_alivePlayers에서의 인덱스 : 현재 턴 번호
    [SerializeField] private int m_currentTurnPlayerNum;            // 현재 차례인 Player의 m_alivePlayers에서의 int값 : 현재 턴인 플레이어 번호
    [SerializeField] private NetworkVariable<int> m_NetCurrentTurnPlayerNum = new NetworkVariable<int>(0);
    public int AlivePlayerCount { get { return m_alivePlayers.Count; } }

    /// <summary>
    /// 현재 자기 턴인 플레이어 번호를 리턴하는 함수
    /// </summary>
    /// <returns>현재 자기 턴인 플레이어 번호</returns>
    public int GetCurruntTurnPlayerNum()
    {
        if(IsServer)
            return m_alivePlayers[m_currentTurn];
        else if(!IsHost && IsClient)
        {
            return m_currentTurnPlayerNum;
        }
        return -1;
    }

    /// <summary>
    /// 플레이어 번호를 입력받아 현재 턴을 설정하는 함수
    /// </summary>
    /// <param name="currentTurnPlayerNum">호출한 플레이어 번호</param>
    public void SetCurruntTurn(int currentTurnPlayerNum)
    {
        int index = m_alivePlayers.IndexOf(currentTurnPlayerNum);
        if (index != -1)
        {
            m_currentTurn = index;
            m_NetCurrentTurnPlayerNum.Value = m_alivePlayers[m_currentTurn];   // 네트워크상에서 동기화
        }
        else
        {
            Debug.LogWarning($"플레이어 {currentTurnPlayerNum}을 찾을 수 없습니다!");
        }
    }
    

    public void InitPlayers(int playerCount)                               // 입력 받은 player의 숫자만큼 m_alivePlayers를 초기화해줌
    {
        m_alivePlayers = new List<int>();
        for(int i = 0; i < playerCount; i++)
        {
            m_alivePlayers.Add(i);
        }
    }
    public void NextTurn()                                                 // 다음 차례로 넘기기
    {
        if(IsServer)
        {
            m_currentTurn = (m_currentTurn + 1) % AlivePlayerCount;            // 마지막 차례인 플레이어일때, 다시 처음으로 돌아감.
            m_NetCurrentTurnPlayerNum.Value = m_alivePlayers[m_currentTurn];   // 네트워크상에서 동기화
        }
        else if(!IsHost&&IsClient)
        {
            ServerNextTurnServerRpc();
        }
    }
    public void RemovePlayer(int target)                                   // 인자로 player의 index를 받아서 그 player를 제거. 
    {
        m_alivePlayers.Remove(target);

        print("Player Out : " + (target + 1));
    }


    [ServerRpc(RequireOwnership = false)]
    public void ServerNextTurnServerRpc()
    {
        NextTurn();
    }


    //private void HandleCurrentTurnChanged(int oldValue, int newValue)      // 현재 턴이 바뀌었을 때 호출되는 함수
    //{
    //    m_currentTurnPlayerNum = newValue;
    //}

    private void Start()
    {
        m_NetCurrentTurnPlayerNum.OnValueChanged += HandleCurrentTurnChanged;
        //m_NetCurrentTurnPlayerNum.Value = 0;
    }

    /// <summary>
    /// 네트워크상에서 동기화될 현재 턴인 플레이어 번호를 설정하는 함수
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void HandleCurrentTurnChanged(int previousValue, int newValue)
    {
        m_currentTurnPlayerNum = newValue;
    }
}
