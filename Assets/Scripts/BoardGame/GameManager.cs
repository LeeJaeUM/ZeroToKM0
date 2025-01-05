using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    static GameManager m_instance;
    public static GameManager Instance { get { return m_instance; } private set { m_instance = value; } }
    protected virtual void OnAwake() { }

    protected virtual void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            OnAwake();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public enum BoardGameType
    {
        HalliGalli,
        KushiExpress,
        Max
    }
    public Dealer m_dealer;
    public PlayerManager m_playerManager;
    public HalliGalli m_halligalli_old;
    public HalliGalliNetwork m_halligalli;
    public BoardGameType m_boardGame;                   // 현재 보드게임

    // 할리갈리 topcard 정보 체크용
    // 이미지 구해지면 지우기
    public RoundWinner m_roundWinner;
    public FinalWinner m_finalWinner;
    public int PlayerCount                              // 몇 명의 플레이어가 참여중인지
    {
        get{ return m_playerManager.PlayerCount; }
    }

    public int GetCurrentPlayer()                       // 지금 누구의 턴인지, index값 반환
    {
        return m_playerManager.CurrentPlayer;
    }
    public void Shuffle(object[] obj)
    {
        m_dealer.Shuffle(obj);
    }
    public void Calculatecard(int cardCount, int playerCount, int[] playerCardCount)
    {
        m_dealer.Calculatecard(cardCount, playerCount, playerCardCount);
    }    
    public void RemovePlayer(int player)
    {
        m_playerManager.RemovePlayer(player);
    }

    public void NextTurn(int player)
    {
        m_playerManager.NextTurn(player);
    }
    public void RingBell(int playernum)
    {
        m_halligalli.RingBell(playernum);
    }
    public void RoundWinMessage(int winner)
    {
        StartCoroutine("CoRoundWinMessage");
        m_roundWinner.SetText(winner);
    }
    public void FinalWinMessage()
    {
        int winner = m_playerManager.m_alivePlayers[0].m_playerNum;
        StartCoroutine("CoFinalWinMessage");
        m_finalWinner.SetText(winner + 1);
    }
    IEnumerator CoRoundWinMessage()
    {
        m_roundWinner.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        m_roundWinner.gameObject.SetActive(false);
    }
    IEnumerator CoFinalWinMessage()
    {
        m_finalWinner.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        m_finalWinner.gameObject.SetActive(false);
    }
}
