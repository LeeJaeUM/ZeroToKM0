using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Netcode;

public class HalliGalliNetwork : NetworkBehaviour
{
    public GameManager m_gameManager;
    public Sprite[] m_animalSprite;                      // 동물 이미지
    public HalliGalliCard[] m_card;                   // 전체 카드
    public Queue<HalliGalliCard>[] m_playerCard;      // 플레이어 각각의 카드
    public HalliGalliCard[] m_topCard;                // 각 플레이어의 맨 위 카드
    public List<HalliGalliCard> m_openedCard;         // 오픈된 카드

    public int[] m_playerCardCount;         // 각 플레이어의 카드 개수
    public float m_cardHeight;              // 카드의 높이
    public CardPos[] m_cardPos;           // 각 플레이어의 카드 위치
    public int[] m_shuffledIndexes;         // 랜덤으로 섞인 카드의 인덱스

    public int[] m_animalCount = { 3, 3, 3, 3, 2 };              // 동물 개수별 카드 개수. 인덱스는 동물 개수, 값은 카드 개수( 모든 동물이 동일 )
                                                                 // ex : 악어 1개짜리 3개, 2개짜리 3개, .. , 5개짜리 2개 => 악어 카드 총 3+3+3+3+2 = 14장
                                                                 // => 모든 동물이 카드개수가 같음. 따라서 총 카드 개수는 14 * 4 = 56

    public int m_roundCount = 1;

    // 길이 변경 이벤트
    public event Action<string, int> OnTopCardChanged;

    public void GameSetting()                                                       // 게임 시작 전 실행
    {
        print("GameStart");

        if (IsServer)
        {
            Collectcard();          //위치조절함수
            m_shuffledIndexes = m_gameManager.Shuffle(m_card);   //랜덤으로 섞인 카드의 인덱스를 받아옴
            ShuffleCards(m_shuffledIndexes);                            // 카드 섞기
        }
        SyncShuffledIndexesToClientRpc(m_shuffledIndexes);          // 클라이언트에게 섞인 인덱스 전달

        m_playerCard = new Queue<HalliGalliCard>[m_gameManager.GetPlayerCount()];
        for (int i = 0; i < m_gameManager.GetPlayerCount(); i++)                  // m_playerCard 초기화
        {
            m_playerCard[i] = new Queue<HalliGalliCard>();
        }
        m_topCard = new HalliGalliCard[m_gameManager.GetPlayerCount()];

        DistributeCard();       //위치조절함수
        Dealcard();             //위치조절함수

    }
    public void CreateCard()                                                        // 카드 초기화 해주기( type, 숫자 )
    {
        int i = 0;
        foreach (HalliGalliCard.AnimalType animal in Enum.GetValues(typeof(HalliGalliCard.AnimalType))) // animal = 동물 타입
        {
            for (int k = 0; k < m_animalCount.Length; k++)       // k = 동물 개수
            {
                for (int j = 0; j < m_animalCount[k]; j++)       // j = 카드 개수
                                                                 // => animal이 k개가 그려진 카드를 j개 생성.
                {
                    m_card[i].Initialize(animal, k + 1);
                    // 카드에 맞게 스프라이트 넣기
                    m_card[i].m_sprite.sprite = m_animalSprite[(int)animal * 5 + k];
                    i++;
                }
            }
        }
    }
    public void DistributeCard()                             // 카드 배분, 위치를 지정
    {
        int k = 0;      // 카드 번호 체크 용
        for (int i = 0; i < m_gameManager.GetPlayerCount(); i++)              // i : 플레이어 번호
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
                card.FlipCard();                                    // card를 뒤집어서 방향 맞춤
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
        HalliGalliCard card;

        // 1. 정상작동 : 플레이어의 카드가 존재할때(0개 이상일 때)
        if (m_playerCard[playerNum].Count > 0)
        {
            card = m_playerCard[playerNum].Dequeue();           // 입력 받은 플레이어의 카드덱에서 가장 위의 카드를 가져옴
            m_topCard[playerNum] = card;                        // 그 카드를 m_topCard에 추가
            m_openedCard.Add(card);                             // m_openedCard에 추가

            SetPos(playerNum + 4, card.gameObject);

            //CardInfoCheck에 액션으로 보낼 string값을 현재 Top 카드에서 찾아서 보냄
            OnTopCardChanged?.Invoke(m_topCard[playerNum].m_AnimalType.ToString() + m_topCard[playerNum].m_fruitNum, playerNum);

            int index;
            do
            {
                // GameManager의 NextTurn()함수를 통해 다음 차례로 넘김.
                index = m_gameManager.NextTurn();
                // 2. 예외처리(1) : 모든 플레이어의 카드가 Open되었다면 반복문 중지
                if(m_card.Length == m_openedCard.Count)
                {
                    print("All Card Used");
                    // TODO : 모든 카드가 사용되었을때의 룰 필요( ex : 게임을 다시 시작한다, 라운드를 다시 시작한다 )
                }

            // 3. 예외처리(2) : 다음 차례로 넘겼는데 그 플레이어의 카드가 0개라면 다시 다음 차례로 넘김.
            } while (m_playerCard[index].Count == 0);           
        }
    }
    public void RingBell(int playernum)                         // player가 space바를 눌렀을 때 호출
    {
        // 1. 정답이 맞을 때
        if (IsCorrect())
        {
            print("Round" + m_roundCount++ + " Winner : Player" + (playernum + 1));
            m_gameManager.RoundWinMessage(playernum + 1);
            GiveCard(playernum);
            RoundFinish();
        }
        // 2. 정답이 아닐 때
        else
        {
            print("That's not five");
        }
    }
    public bool IsCorrect()                                 // 맨 위 카드들의 심볼의 합을 계산하여 true, false를 반환
    {
        int[] sum = new int[4];                             // 심볼 별 합을 담아줄 배열

        // m_topCard가 완전히 비어 있을 때( 게임이 시작되고 아무 카드도 오픈이 안됬을 때 예외처리 )
        if (m_topCard == null)                              
        {
            return false;
        }
        for (int i = 0; i < m_gameManager.GetPlayerCount(); i++)              // 각 심볼 별 합을 계산
        {
            // 어떤 플레이어의 topCard가 존재하지 않을 때
            // ( ex : 플레이어 1, 2번은 턴을 진행하여 topCard가 존재하는데, 3번은 아직 턴을 진행하지 않아 topCard가 존재하지 않는 경우 )
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
        m_topCard = new HalliGalliCard[m_gameManager.GetPlayerCount()];
    }
    public void RoundFinish()                               // 탈락자를 제거하고, 새 라운드를 시작하는 함수.
                                                            // 종을 쳐서 정답일 경우 호출됨.
    {
        for (int i = 0; i < m_playerCard.Length; i++)  
        {
            if (m_playerCard[i].Count == 0) // 카드가 0개인 플레이어 탈락
            {
                m_gameManager.RemovePlayer(i);
            }
        }
        // 혼자 남았을 경우 최종 승리.
        if (m_gameManager.GetPlayerCount() == 1)         
        {
            GameOver();
        }
        // 플레이어가 2명 이상일 경우, 다시 진행
        else
        {
            print(m_gameManager.GetPlayerCount());
            //Array.Clear(m_topCard, 0, m_topCard.Length);    // topcard초기화
            print("new round");

            for (int i = 0; i < m_gameManager.GetPlayerCount(); i++)    // topcard초기화 후 CardInfoCheck에 초기화 된 string값을 액션으로 보냄 
            {
                OnTopCardChanged?.Invoke(" 라운드 초기화 ", i);
            }
        }
    }
    public void GameOver()                                  // 게임 종료
    {
        m_gameManager.FinalWinMessage();
        print("game over");
    }
    public void ShuffleCards(int[] shuffledIndexes) // 카드를 섞는다.
    {
        HalliGalliCard[] shuffledCards = new HalliGalliCard[m_card.Length];

        // 서버에서 전달된 인덱스를 기반으로 카드를 섞는다.
        for (int i = 0; i < shuffledIndexes.Length; i++)
        {
            shuffledCards[i] = m_card[shuffledIndexes[i]];
        }

        m_card = shuffledCards; // 섞인 카드를 m_card에 반영
    }
    #region Network Function
    public void InitializeGame()  //기존 Start 유니티 함수에 있던걸 직접 눌러서 실행하도록 함수로 뺌
    {
        GameSetting();
        SyncGameSettingClientRpc();
    }

    [ClientRpc]
    public void SyncGameSettingClientRpc()
    {
        GameSetting();
    }
    [ClientRpc]
    public void SyncShuffledIndexesToClientRpc(int[] shuffledIndexes)
    {
        // 클라이언트에서 받은 섞인 인덱스를 기반으로 카드를 섞는다.
        ShuffleCards(shuffledIndexes);
    }
    #endregion

    private void Start()
    {
        m_gameManager = GameManager.Instance;
        m_card = GetComponentsInChildren<HalliGalliCard>();
        m_playerCardCount = new int[m_gameManager.GetPlayerCount()];
        m_cardHeight = 0.01f;
        CreateCard();

        //m_gameManager.Calculatecard(m_card.Length, m_gameManager.PlayerCount, m_playerCardCount);
        m_gameManager.Calculatecard(m_card.Length, m_gameManager.GetPlayerCount(), m_playerCardCount);
    }
}