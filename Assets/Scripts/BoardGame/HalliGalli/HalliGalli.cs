using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class HalliGalli : MonoBehaviour
{
    public Sprite[] m_fruitSprite;                      // 과일 이미지
    public HalliGalliCard[] m_card;                   // 전체 카드
    [SerializeField]
    public Queue<HalliGalliCard>[] m_playerCard;      // 플레이어 각각의 카드
    public HalliGalliCard[] m_topCard;                // 각 플레이어의 맨 위 카드
    public List<HalliGalliCard> m_openedCard;         // 오픈된 카드

    public int[] m_playerCardCount;         // 각 플레이어의 카드 개수
    public float m_cardHeight;              // 카드의 높이
    public CardPos[] m_cardPos;           // 각 플레이어의 카드 위치

    public int[] m_fruitCount = { 3, 3, 3, 3, 2 };              // 과일 개수별 카드 개수, ex) 과일이 1개 그려진 카드는 총 m_fruitCount[0] = 3개

    public int m_roundCount = 1;

    public void GameSetting()                                                       // 게임 시작 전 실행
    {
        print("GameStart");
        CreateCard();
        m_playerCardCount = new int[GameManager.Instance.PlayerCount];
        GameManager.Instance.Calculatecard(m_card.Length, GameManager.Instance.PlayerCount, m_playerCardCount);
        Collectcard();
        GameManager.Instance.Shuffle(m_card);
        m_playerCard = new Queue<HalliGalliCard>[GameManager.Instance.PlayerCount];
        for (int i = 0; i < GameManager.Instance.PlayerCount; i++)                  // m_playerCard 초기화
        {
            m_playerCard[i] = new Queue<HalliGalliCard>();
        }
        m_topCard = new HalliGalliCard[GameManager.Instance.PlayerCount];

        DistributeCard();
        Dealcard();
    }
    public void CreateCard()                                                        // 카드 초기화 해주기( type, 숫자 )
    {
        int i = 0;
        foreach (HalliGalliCard.AnimalType fruit in Enum.GetValues(typeof(HalliGalliCard.AnimalType)))
        {
            for (int k = 0; k < m_fruitCount.Length; k++)
            {
                for (int j = 0; j < m_fruitCount[k]; j++)
                {
                    m_card[i++].Initialize(fruit, k + 1);
                    //TODO : 카드에 맞게 스프라이트 넣기 추가
                }
            }
        }
    }
    public void DistributeCard()                             // 카드 배분, 위치를 지정
    {
        int k = 0;      // 카드 번호 체크 용
        for (int i = 0; i < GameManager.Instance.PlayerCount; i++)              // i : 플레이어 번호
        {
            for (int j = 0; j < m_playerCardCount[i]; j++)   // j : 플레이어가 가진 카드 내에서의 카드 번호
            {
                m_playerCard[i].Enqueue(m_card[k++]);
            }
        }
    }
    public void Dealcard()                                   // 카드 딜링, 위치를 옮김
    {
        for (int i = 0; i < m_playerCard.Length; i++)
        {
            foreach (Card card in m_playerCard[i])
            {
                SetPos(i, card.gameObject);
            }
        }

        for (int i = 0; i < m_cardPos.Length; i++)
        {
            m_cardPos[i].m_cardCount = 0;
        }
    }
    public void SetPos(int playerNum, GameObject gameobj)       // 카드를 배치해주는 함수(누구의 카드를, 몇번째에 놓을지, 어떤 카드인지)
    {
        Vector3 cardPos = m_cardPos[playerNum].transform.position;
        cardPos.y += m_cardHeight * m_cardPos[playerNum].m_cardCount++;

        gameobj.transform.position = cardPos;
        gameobj.transform.forward = m_cardPos[playerNum].transform.forward;
        gameobj.transform.Rotate(Vector3.right * 90);

    }
    public void Collectcard()                          // 모든 카드 딜러가 가져오기
    {
        for (int i = 0; i < m_card.Length; i++)
        {
            SetPos(8, m_card[i].gameObject);
        }
    }

    public void OpenCard(int playerNum)     
    {
        playerNum = GameManager.Instance.GetCurrentPlayer();    // 일단은 현재 플레이어의 카드를 open하는 방식으로,
                                                                // todo : 멀티플레이 구현되면 지울것
        HalliGalliCard card;
        if (m_playerCard[playerNum].Count > 0)
        {
            card = m_playerCard[playerNum].Dequeue();           // 입력 받은 플레이어의 카드덱에서 가장 위의 카드를 가져옴
            m_topCard[playerNum] = card;                        // 그 카드를 m_topCard에 추가
            m_openedCard.Add(card);                             // m_openedCard에 추가

            SetPos(playerNum + 4, card.gameObject);
            card.FlipCard();                                    // card를 뒤집는 함수( 작동 안됨 )

            GameManager.Instance.NextTurn(playerNum);
            return;
        }
        else if (m_card.Length != m_openedCard.Count)
        {
            GameManager.Instance.NextTurn(playerNum);
            OpenCard(0);
        }
        print("All Card Used");
    }
    public void RingBell(int playernum)                         // player가 space바를 눌렀을 때 호출
    {
        if (isCorrect())
        {
            print("Round" + m_roundCount++ + " Winner : Player" + (playernum + 1));
            GameManager.Instance.RoundWinMessage(playernum + 1);
            GiveCard(playernum);
            RoundFinish();
        }
        else
        {
            print("That's not five");
        }
    }
    public bool isCorrect()                                 // 맨 위 카드들의 심볼의 합을 계산하여 true, false를 반환
    {
        int[] sum = new int[4];                             // 심볼 별 합을 담아줄 배열

        if (m_topCard == null)
        {
            return false;
        }
        for (int i = 0; i < GameManager.Instance.PlayerCount; i++)              // 각 심볼 별 합을 계산
        {
            if (m_topCard[i] != null)
                sum[(int)m_topCard[i].m_AnimalType] += m_topCard[i].m_fruitNum;
        }

        for (int i = 0; i < sum.Length; i++)                 // 합이 5가 되는 심볼이 있는지 확인
        {
            if (sum[i] == 5)
                return true;
        }
        return false;
    }
    public void GiveCard(int playerNum)                     // 승자에게 open된 카드를 전부 줌.
    {
        for (int i = 0; i < m_openedCard.Count; i++)
        {
            m_playerCard[playerNum].Enqueue(m_openedCard[i]);
        }
        Dealcard();

        m_openedCard.Clear();
        m_topCard = null;
        m_topCard = new HalliGalliCard[GameManager.Instance.PlayerCount];
    }
    public void RoundFinish()                               // 탈락자를 제거하고, 새 라운드를 시작하는 함수.
                                                            // 종을 쳐서 정답일 경우 호출됨.
    {
        for (int i = 0; i < m_playerCard.Length; i++)  // 카드가 0개인 플레이어 탈락
        {
            if (m_playerCard[i].Count == 0)
            {
                GameManager.Instance.RemovePlayer(i);
            }
        }
        if (GameManager.Instance.PlayerCount == 1)          // 혼자 남았을 경우 최종 승리.
        {
            GameOver();
        }
        else                                                // 플레이어가 2명 이상일 경우, 다시 진행
        {
            print(GameManager.Instance.PlayerCount);
            Array.Clear(m_topCard, 0, m_topCard.Length);    // topcard초기화
            print("new round");
        }
    }
    public void GameOver()                                  // 게임 종료
    {
        GameManager.Instance.FinalWinMessage();
        print("game over");
    }
    void Awake()
    {
        m_card = GetComponentsInChildren<HalliGalliCard>();
        m_playerCardCount = new int[GameManager.Instance.PlayerCount];
        m_cardHeight = 0.01f;
    }
    void Start()
    {
        GameSetting();
    }
}