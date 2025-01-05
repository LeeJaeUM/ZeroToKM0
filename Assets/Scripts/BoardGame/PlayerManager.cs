using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class PlayerManager : NetworkBehaviour
{
    public Player[] m_players;
    public List<Player> m_alivePlayers;

    public int CurrentPlayer { get { return nm_currentPlayer.Value; } }
    public int PlayerCount { get { return m_alivePlayers.Count; } }


    public NetworkVariable<int> nm_currentPlayer = new NetworkVariable<int>(0);  // 현재 턴의 플레이어 번호
    //public NetworkVariable<int> nm_playerCount = new NetworkVariable<int>(0);  // 살아있는 플레이어 수

    public void NextTurn(int currentPlayerIndex)
    {
        Debug.Log("NextTurn");
        if (IsServer)
        {
            Debug.Log("NextTurnIn Server");
            currentPlayerIndex = (currentPlayerIndex + 1) % PlayerCount;            // 마지막 차례인 플레이어일때, 다시 처음으로 돌아감.
            nm_currentPlayer.Value = currentPlayerIndex;

            for (int i = 0; i < m_alivePlayers.Count; i++)
            {
                if(m_alivePlayers[i].m_playerNum == nm_currentPlayer.Value)
                {
                    m_alivePlayers[currentPlayerIndex].nm_isMyTurn.Value = true;
                }
                m_alivePlayers[i].nm_isMyTurn.Value = false;
            }
        }
        Debug.Log("NextTur - Endn");
    }
    public void RemovePlayer(int player)
    {
        for (int i = 0; i < m_alivePlayers.Count; i++)
        {
            if (m_players[player] == m_alivePlayers[i])
            {
                m_alivePlayers.RemoveAt(i);
                print("Player Out : " + (player + 1));
            }
        }
    }

    private void Awake()
    {
        m_players = GetComponentsInChildren<Player>();
        m_alivePlayers = new List<Player>();
        for (int i = 0; i < m_players.Length; i++)
        {
            m_alivePlayers.Add(m_players[i]);
        }
    }

    void Start()
    {

    }
}
