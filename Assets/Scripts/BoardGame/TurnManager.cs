using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Player 숫자를 받아서 Player들의 참여 정보와 턴을 관리해주는 스크립트
public class TurnManager : MonoBehaviour
{
    public List<int> m_alivePlayers;     // 살아있는 Player들의 인덱스
    public int m_currentTurn;            // 현재 차례인 Player의 m_alivePlayers에서의 인덱스
    public int AlivePlayerCount { get { return m_alivePlayers.Count; } }
    public int CurrentTurn { get { return m_alivePlayers[m_currentTurn]; } }    // 현재 차례인 Player의 인덱스 반환

    public void InitPlayers(int playerCount)                               // 입력 받은 player의 숫자만큼 m_alivePlayers를 초기화해줌
    {
        m_alivePlayers = new List<int>();
        for(int i = 0; i < playerCount; i++)
        {
            m_alivePlayers.Add(i);
        }
    }
    public int NextTurn()                                                 // 다음 차례로 넘기기
    {
        m_currentTurn = (m_currentTurn + 1) % AlivePlayerCount;            // 마지막 차례인 플레이어일때, 다시 처음으로 돌아감.

        return m_alivePlayers[m_currentTurn];       // 다음 차례인 플레이어 인덱스 반환
    }
    public void RemovePlayer(int target)                                   // 인자로 player의 index를 받아서 그 player를 제거. 
    {
        m_alivePlayers.Remove(target);

        print("Player Out : " + (target + 1));
    }

    void Awake()
    {
        InitPlayers(2);             // TODO : 임의로 초기화한 것. 나중에 지우기
    }
}