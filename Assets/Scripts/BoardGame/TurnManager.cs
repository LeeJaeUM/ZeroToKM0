using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// Player 숫자를 받아서 Player들의 참여 정보와 턴을 관리해주는 스크립트
public class TurnManager : MonoBehaviour
{
    public List<int> m_alivePlayers;     // 살아있는 Player들의 인덱스
    public int m_currentTurn;            // 현재 차례인 Player의 m_alivePlayers에서의 인덱스
    public int AlivePlayerCount { get { return m_alivePlayers.Count; } }
    public int CurrentTurn { 
        get { return m_alivePlayers[m_currentTurn]; }               // 현재 차례인 Player의 번호 반환
        set { m_currentTurn = m_alivePlayers.IndexOf(value); }      // Player번호를 입력받아 m_alivePlayers내에서의 인덱스로 m_currentTurn의 값 변경
    }

    public event Action<int> OnCurTurnNum;      //TODO : 플레이어에게 턴 넘어갈 때 보낼 액션 (LJW)----

    public void InitPlayers(int playerCount)                               // 입력 받은 player의 숫자만큼 m_alivePlayers를 초기화해줌
    {
        m_alivePlayers = new List<int>();
        for(int i = 0; i < playerCount; i++)
        {
            m_alivePlayers.Add(0);
        }
    }
    public int NextTurn()                                                 // 다음 차례로 넘기기
    {
        m_currentTurn = (m_currentTurn + 1) % AlivePlayerCount;            // 마지막 차례인 플레이어일때, 다시 처음으로 돌아감.

        OnCurTurnNum?.Invoke(m_currentTurn);        //TODO : 플레이어에게 턴 넘어갈 때 보낼 액션 (LJW)----

        return m_alivePlayers[m_currentTurn];       // 다음 차례인 플레이어 인덱스 반환
    }
    public void RemovePlayer(int target)                                   // 인자로 player의 index를 받아서 그 player를 제거. 
    {
        m_alivePlayers.Remove(target);

        print("Player Out : " + (target + 1));
    }
}
