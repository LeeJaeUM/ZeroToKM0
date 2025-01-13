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
    public TurnManager m_turnManager;
    //public HalliGalli m_halligalli_old;
    public HalliGalliNetwork m_halligalli;
    public BoardGameType m_boardGame;                   // 현재 보드게임

    // 할리갈리 topcard 정보 체크용
    // 이미지 구해지면 지우기
    public RoundWinner m_roundWinner;
    public FinalWinner m_finalWinner;

    [SerializeField] GameResultController m_gameResultController;

    public void EndGame()
    {
        // TODO : 예제 데이터, 데이터베이스 데이터로 변경 필요
        PlayerResult[] results = new PlayerResult[]
        {
            new PlayerResult { playerName = "Player1", isWinner = true, winCount = 1, totalGames = 3 },
            new PlayerResult { playerName = "Player2", isWinner = false, winCount = 0, totalGames = 3 },
            new PlayerResult { playerName = "Player3", isWinner = false, winCount = 0, totalGames = 3 },
            new PlayerResult { playerName = "Player4", isWinner = false, winCount = 2, totalGames = 3 },
        };

        int totalTurns = 3;

        // Modal 창 열기
        m_gameResultController.ShowGameResult(results, totalTurns);
    }

    public void InitPlayers(int playerCount)
    {
        m_turnManager.InitPlayers(playerCount);
    }
    public int GetPlayerCount()
    {
        return m_turnManager.AlivePlayerCount;
    }
    public int GetCurrentTurn()                       // 지금 누구의 턴인지, index값 반환
    {
        return m_turnManager.CurrentTurn;
    }
    public void SetCurrentTurn(int turn)
    {
        m_turnManager.CurrentTurn = turn;
    }
    public int[] Shuffle(int length)
    {
        return m_dealer.Shuffle(length);
    }
    public void Calculatecard(int cardCount, int playerCount, int[] playerCardCount)
    {
        m_dealer.Calculatecard(cardCount, playerCount, playerCardCount);
    }    
    public void RemovePlayer(int player)
    {
        m_turnManager.RemovePlayer(player);
    }

    public int NextTurn()
    {
        return m_turnManager.NextTurn();
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
        int winner = m_turnManager.m_alivePlayers[0];
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

    //----
    public void OpenCard(int playerNum)
    {
        m_halligalli.OpenCard(playerNum);
    }

    void Update()
    {
        // TODO : 데이터베이스 연결 시 변경필요
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            EndGame();
        }
        
    }
}
