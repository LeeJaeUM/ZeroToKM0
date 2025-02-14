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

        m_boardGames = new Dictionary<BoardGameType, BoardGame> {
            { BoardGameType.HalliGalli, m_halligalli },
            { BoardGameType.Skewer, m_skewer },
            { BoardGameType.Jenga, m_jenga }
        };
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
    private Dictionary<BoardGameType, BoardGame> m_boardGames;

    public BoardGameType m_currentBoardGame;                   // 현재 보드게임

    public Dealer m_dealer;
    public TurnManager m_turnManager;

    // 할리갈리 topcard 정보 체크용
    // 이미지 구해지면 지우기
    public WinMessage m_winMessage;



    [SerializeField] GameResultController m_gameResultController;

    /// <summary>
    /// 아이콘 닉네임
    /// </summary>
    public IconNameUI m_iconNameUI;

    public bool isTest = false;

    #region Public Methods and Operators
    public void SetBoardGame(BoardGameType type)        // 입력 받은 보드게임을 활성화해주는 함수
    {
        m_currentBoardGame = type;
        // 모든 보드게임 비활성화
        foreach (BoardGame boardGame in m_boardGames.Values)
        {
            boardGame.EndGame();
            boardGame.gameObject.SetActive(false);
        }
        // 입력받은 보드게임 활성화      
        m_boardGames[type].gameObject.SetActive(true);
    }
    public void StartGame()
    {
        m_boardGames[m_currentBoardGame].InitializeGame();
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

    #region Dealer Function

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

    #endregion


    #region TurnManager Function
    public void InitPlayers(int playerCount)
    {
        m_turnManager.InitPlayers(playerCount);
    }
    public int GetPlayerCount()
    {
        return m_turnManager.AlivePlayerCount;
    }
    public int GetCurruntTurnPlayerNum()                       // 지금 누구의 턴인지, index값 반환
    {
        return m_turnManager.GetCurruntTurnPlayerNum();
    }
    public void SetCurruntTurn(int playerNum)
    {
        m_turnManager.SetCurruntTurn(playerNum);
    }

    public void RemovePlayer(int player)
    {
        m_turnManager.RemovePlayer(player);
    }

    public void NextTurn()
    {
        m_turnManager.NextTurn();
    }
    #endregion

    #region HalliGalli Function
    public void RingBell(int playernum)
    {
        m_halligalli.RingBell(playernum);
    }

    public bool IsMyTurnHalliGalli(int playernum)
    {
       return m_halligalli.IsMyTurn(playernum);
    }

    public bool IsOpenable()
    {
        return m_halligalli.GetIsOpenable();
    }
    #endregion
    #region Skewer Function
    public bool SkewerIsCorrect(List<SkewerIngredient.IngredientType> ingredientList)
    {
        return m_skewer.IsCorrect(ingredientList);
    }
    #endregion
    public bool IsMyTurn(int playernum)
    {
        switch(m_currentBoardGame)
        {
            case BoardGameType.HalliGalli:
                return IsMyTurnHalliGalli(playernum);
            case BoardGameType.Skewer:
                break;
            default:
                break;
        }
        return false;
    }

    public void RoundWinMessage(int winner)
    {
        m_winMessage.SetRoundWinText(winner);
        RoundWinMessageClientRpc(winner);
    }
    public void FinalWinMessage()
    {
        int winner = m_turnManager.GetCurruntTurnPlayerNum();
        m_winMessage.SetFinalWinText(winner);
        FinalWinMessageClientRpc(winner);
    }

    [ClientRpc]
    private void RoundWinMessageClientRpc(int winner)
    {
        m_winMessage.SetRoundWinText(winner);
    }
    [ClientRpc]
    private void FinalWinMessageClientRpc(int winner)
    {
        m_winMessage.SetFinalWinText(winner);
    }

    public void OpenCard(int playerNum, int cardIndex)
    {
        m_halligalli.OpenCard(playerNum, cardIndex);
    }

    /// <summary>
    /// 유저의 닉네임과 아이콘 표시 함수 m_iconNameUI사용
    /// </summary>
    public void SetIconName()
    {
        ulong playerNum = NetworkManager.Singleton.LocalClientId;
        m_iconNameUI.SetUserInfo(playerNum);
    }

    public void SetUserNickName(int playerNum)
    {
        if (isTest) 
        { 
            
        }
        else
        {
            if (IsServer)
            {
                FBManager._instance.UserInfoLoad(() =>
                {
                    string userName = FBManager._instance.m_name;
                    FBSetUserNickName(userName, playerNum);
                    SetUserNickNameClientRpc(userName, playerNum);
                });
            }
            else if (IsClient && !IsHost)
            {
                //클라이언트에서 자기 자신의 닉네임을 전달
                FBManager._instance.UserInfoLoad(() =>
                {
                    string userName = FBManager._instance.m_name;
                    RequestSetUserNickNameServerRpc(userName, playerNum);
                });
            }
        }

    }

    /// <summary>
    /// 전광판에 보일 유저 닉네임 설정함수
    /// </summary>
    /// <param name="nickname"></param>
    /// <param name="playerNum"></param>
    private void FBSetUserNickName(string nickname, int playerNum)
    {
        m_winMessage.SetName(nickname, playerNum);
    }

    [ServerRpc]
    public void RequestSetUserNickNameServerRpc(string nickname, int playerNum)
    {
        FBSetUserNickName(nickname, playerNum); //서버에서 실행
        SetUserNickNameClientRpc(nickname, playerNum);
    }

    [ClientRpc]
    public void SetUserNickNameClientRpc(string nickname, int playerNum)
    {
        FBSetUserNickName(nickname, playerNum);
    }
    #endregion



    private void Start()
    {
        m_turnManager = GetComponentInChildren<TurnManager>();
        m_dealer = GetComponentInChildren<Dealer>();
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
