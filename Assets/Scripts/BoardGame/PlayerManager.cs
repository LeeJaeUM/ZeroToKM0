using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public Player[] m_players;
    public List<Player> m_alivePlayers;
    public Player m_currentPlayer;

    public int CurrentPlayer { get { return m_currentPlayer.m_playerNum; } }
    public int PlayerCount {  get { return m_alivePlayers.Count; } }

    public void NextTurn(int currentPlayerIndex)
    {
        // todo : 멀티플레이 되면 주석 지우기
        //m_players[currentPlayerIndex].m_isMyTurn = false;

        currentPlayerIndex = (currentPlayerIndex + 1) % PlayerCount;            // 마지막 차례인 플레이어일때, 다시 처음으로 돌아감.

        m_currentPlayer = m_alivePlayers[currentPlayerIndex];
        // todo : 멀티플레이 되면 주석 지우기
        //m_currentPlayer.m_isMyTurn = true;
    }
    public void RemovePlayer(int player)
    {
        for(int i = 0; i < m_alivePlayers.Count; i++)
        {
            if (m_players[player] == m_alivePlayers[i])
            {
                m_alivePlayers.RemoveAt(i);
                print("Player Out : " + (player + 1));
            }
        }
    }
    void Awake()
    {
        m_players = GetComponentsInChildren<Player>();
        m_alivePlayers = new List<Player>();
        for(int i = 0 ; i < m_players.Length; i++)
        {
            m_alivePlayers.Add(m_players[i]);
        }
    }
    void Start()
    {
        m_currentPlayer = m_players[0];
    }
}
