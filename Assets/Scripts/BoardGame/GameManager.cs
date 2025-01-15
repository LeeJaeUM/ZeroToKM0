using System.Collections;
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using static GameManager;


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

        m_boardGames = new Dictionary<BoardGameType, NetworkBehaviour> {
            { BoardGameType.HalliGalli, m_halligalli },
            { BoardGameType.Skewer, m_skewer },
            { BoardGameType.Jenga, m_jenga }
        };
        m_deckManager = new DeckManager(10);
    }
    public enum BoardGameType
    {
        HalliGalli,
        Skewer,
        Jenga,
        Max
    }
    // 보드게임 종류
    public HalliGalliNetwork m_halligalli;
    public Skewer m_skewer;
    public Jenga m_jenga;
    
    // 보드게임 타입과 보드게임 게임오브젝트를 매핑
    private Dictionary<BoardGameType, NetworkBehaviour> m_boardGames;

    public BoardGameType m_boardGame;                   // 현재 보드게임

    public Dealer m_dealer;
    public TurnManager m_turnManager;
    public DeckManager m_deckManager;

    // 할리갈리 topcard 정보 체크용
    // 이미지 구해지면 지우기
    public RoundWinner m_roundWinner;
    public FinalWinner m_finalWinner;

    [SerializeField] GameResultController m_gameResultController;

    #region Public Methods and Operators
    public void SetBoardGame(BoardGameType type)        // 입력 받은 보드게임을 활성화해주는 함수
    {
        // 모든 보드게임 비활성화
        foreach(var boardGame in m_boardGames.Values)
        {
            boardGame.gameObject.SetActive(false);
        }
        // 입력받은 보드게임 활성화      
        m_boardGames[type].gameObject.SetActive(true);      
    }
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
    
    public CardDeck GetCardDeck(Card card)
    {
        return m_deckManager.GetDeck(card);
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
    public void ArrayShuffle(object[] obj)
    {
        m_dealer.ArrayShuffle(obj);
    }
    public void ListShuffle(List<Card> card)
    {
        m_dealer.ListShuffle(card);
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
    //----
    //public void OpenCard(int playerNum)
    //{
    //    m_halligalli.OpenCard(playerNum);
    //} 
    public void OpenCard(int playerNum, HalliGalliCard halliGalliCard)
    {
        m_halligalli.OpenCard(playerNum, halliGalliCard);
    }
    #endregion

    #region Coroutine Methods
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
    #endregion

 
    void Update()
    {
        // TODO : 데이터베이스 연결 시 변경필요
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            EndGame();
        }
        
    }
}
