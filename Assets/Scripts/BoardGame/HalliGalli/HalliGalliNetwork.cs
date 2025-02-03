using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;

public class HalliGalliNetwork : BoardGame
{
    public GameManager m_gameManager;
    public Sprite[] m_animalSprite;                      // 동물 이미지
    public HalliGalliCard[] m_card;                   // 전체 카드
    public List<HalliGalliCard>[] m_playerCard;      // 플레이어 각각의 카드
    public HalliGalliCard[] m_topCards;        // 각 플레이어의 오픈된 가장 윗 카드
    public List<HalliGalliCard> m_openedCard;         // 오픈된 카드

    public int[] m_playerCardCount;         // 각 플레이어의 카드 개수
    public float m_cardHeight;              // 카드의 높이
    public CardPos[] m_cardPos;           // 각 플레이어의 카드 위치
    public int[] m_shuffledIndexes;         // 랜덤으로 섞인 카드의 인덱스

    public int[] m_animalCount = { 3, 3, 3, 3, 2 };              // 동물 개수별 카드 개수. 인덱스는 동물 개수, 값은 카드 개수( 모든 동물이 동일 )
                                                                 // ex : 악어 1개짜리 3개, 2개짜리 3개, .. , 5개짜리 2개 => 악어 카드 총 3+3+3+3+2 = 14장
                                                                 // => 모든 동물이 카드개수가 같음. 따라서 총 카드 개수는 14 * 4 = 56

    public int m_roundCount = 1;
    public bool isUsePrivateMode = false;

    public bool isOpenable = false;

    // 길이 변경 이벤트
    public event Action<string, int> OnTopCardChanged;

    public void ServerGameSetting()                                                       // 게임 시작 전 실행
    {
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;          // 연결된 플레이어 숫자 가져옴
        InitPlayers(playerCount);
        InitCards(playerCount);
    }

    public void InitPlayers(int player)
    {
        m_playerCardCount = new int[player];
        GameManager.Instance.InitPlayers(player);                              // TurnManager에 플레이어 숫자 할당해줌.
    }
    public void InitCards(int player)
    {
        CreateCard();                                                               // 카드들에 정보+sprite 할당
        m_gameManager.Calculatecard(m_card.Length, player, m_playerCardCount); // 각 플레이어 별 카드 숫자 계산

        Collectcard();          //위치조절함수
        m_shuffledIndexes = m_gameManager.Shuffle(m_card.Length);   //랜덤으로 섞인 카드의 인덱스를 받아옴
        ShuffleCards(m_shuffledIndexes);                            // 카드 섞기

        m_playerCard = new List<HalliGalliCard>[player];
        for (int i = 0; i < player; i++)                  // m_playerCard 초기화
        {
            m_playerCard[i] = new List<HalliGalliCard>();
        }
        m_topCards = new HalliGalliCard[player];

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
                    if (IsServer)
                    {
                        m_card[i].Initialize(animal, k + 1, i, this);
                        // 카드에 맞게 스프라이트 인덱스를 네트워크로 동기화
                        int spriteIndex = (int)animal * 5 + k; // 스프라이트 인덱스를 계산
                        m_card[i].SetSprite(spriteIndex); // 서버에서 스프라이트 인덱스를 설정하여 동기화
                        i++;
                    }
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
                m_playerCard[i].Add(m_card[k++]);
            }
        }
    }
    public void Dealcard()                                   // 카드 딜링, 위치를 옮김
    {
        for (int i = 0; i < m_playerCard.Length; i++)
        {
            foreach (Card card in m_playerCard[i])
            {
                SetPos(i * 2 + 1, card);
                //card.FlipCardAnim();                                    // card를 뒤집어서 방향 맞춤
            }
        }

        for (int i = 0; i < m_cardPos.Length; i++)
        {
            m_cardPos[i].m_cardCount = 0;
        }
    }
    public void SetPos(int playerNum, Card card)       // 카드를 배치해주는 함수(누구의 카드를, 몇번째에 놓을지, 어떤 카드인지)
    {
        Vector3 cardPos = m_cardPos[playerNum].transform.position;
        cardPos.y += m_cardHeight * m_cardPos[playerNum].m_cardCount++;

        card.transform.position = cardPos;
        card.transform.forward = m_cardPos[playerNum].transform.forward;
        //card.transform.Rotate(Vector3.right * 90);

    }
    public void Collectcard()                          // 모든 카드 딜러가 가져오기
    {
        for (int i = 0; i < m_card.Length; i++)
        {
            SetPos(8, m_card[i]);
        }
    }


    public bool IsMyTurn(int playerNum)
    {
        // 룰 추가 : 자신의 턴에만 Open이 가능하게.
        // playerNum의 턴이 아니라면 false반환
 
        if (playerNum == m_gameManager.GetCurruntTurnPlayerNum())
        {
            return true;
        }
        Debug.Log("Not My Turn");
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerNum"></param>
    /// <param name="halliGalliCard"></param>
    /// <returns></returns>
    public bool IsOpenableCard(int playerNum, int cardIndex)
    {
        if(IsServer)
        {
            HalliGalliCard card = m_card[cardIndex];
            // 룰 추가 : 자신의 카드만 Open이 가능하게.
            // playerNum의 카드 덱에 halligalliCard가 존재하는지 확인
            if (m_playerCard[playerNum].Contains(card))
            {
                // 있다면 덱에서 제거
                m_playerCard[playerNum].Remove(card);
                isOpenable = true;
            }
            else
                isOpenable= false;
        }
        else if(!IsHost && IsClient)
        {
            RequestIsOpenableServerRpc(playerNum,cardIndex);
        }

        return isOpenable;
    }

    //턴이 있는 OpenCard
    //자신의 턴인지, 자신의 카드인지를 확인해서 맞다면 정상 진행하고 true반환
    //아니라면 함수를 중단하고 false반환
    public void OpenCard(int playerNum, int cardIndex)
    {
        if (isUsePrivateMode)//턴 제한 있을 때 로직
        {
            if (!IsOpenableCard(playerNum, cardIndex))
            {
                Debug.Log("내 카드가 아니면 카드를 오픈할 수 없음");
                return;
            }
        }
        if (IsServer)
        {
            HalliGalliCard findcard = null;
            foreach (HalliGalliCard _card in m_card)
            {
                if (_card.m_CardIndex == cardIndex)
                    findcard = _card;
            }
            if (findcard != null)
            {
                m_topCards[playerNum] = findcard;                        // 그 카드를 m_topCard에 추가
                m_openedCard.Add(findcard);                             // m_openedCard에 추가

                // OnTopCardChanged 이벤트 호출 : 텍스트 확인용 액션 - 할리갈리 개발 완료 시 삭제예정
                OnTopCardChanged?.Invoke(m_topCards[playerNum].m_AnimalType.ToString() + m_topCards[playerNum].m_fruitNum, playerNum);
            }
        }
        else if (!IsHost&&IsClient)
        {
            // halliGalliCard가 m_card 배열에서 몇 번째 인덱스인지 확인
            // 클라이언트에서 서버로 요청
            RequestOpenCardServerRpc(playerNum, cardIndex);
        }


        m_gameManager.NextTurn();
    }

    public void RingBell(int playernum)                         // player가 space바를 눌렀을 때 호출
    {
        // 1. 정답이 맞을 때
        if (IsCorrect())
        {
            print("Round" + m_roundCount++ + " Winner : Player" + (playernum + 1));
            m_gameManager.RoundWinMessage(playernum + 1);
            FlipOpenedCards();  // 뒤집혀있는 OpenedCard들을 전부 다시 뒤집어줌
            GiveCard(playernum);
            RoundFinish();
            m_gameManager.SetCurruntTurn(playernum);            // 승자부터 다시 시작
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
        if (m_topCards == null)
        {
            return false;
        }
        for (int i = 0; i < m_gameManager.GetPlayerCount(); i++)              // 각 심볼 별 합을 계산
        {
            // 어떤 플레이어의 topCard가 존재하지 않을 때
            // ( ex : 플레이어 1, 2번은 턴을 진행하여 topCard가 존재하는데, 3번은 아직 턴을 진행하지 않아 topCard가 존재하지 않는 경우 )
            if (m_topCards[i] != null)
                sum[(int)m_topCards[i].m_AnimalType] += m_topCards[i].m_fruitNum;
        }

        for (int i = 0; i < sum.Length; i++)                 // 합이 5가 되는 심볼이 있는지 확인
        {
            if (sum[i] == 5)
                return true;
        }
        return false;
    }

    /// <summary>
    ///  뒤집혀있는 OpenedCard들을 전부 다시 뒤집어줌
    /// </summary>
    public void FlipOpenedCards()
    {
        // Open되어있는 Card들을 다시 뒤집어줌.
        for (int i = 0; i < m_openedCard.Count; i++)
        {
            m_openedCard[i].FlipCardAnim();
        }
    }
    public void GiveCard(int playerNum)                     // 승자에게 open된 카드를 전부 줌.
    {
        for (int i = 0; i < m_openedCard.Count; i++)
        {
            m_playerCard[playerNum].Add(m_openedCard[i]);
        }
        m_openedCard.Clear();
        m_topCards = null;
        m_topCards = new HalliGalliCard[m_gameManager.GetPlayerCount()];
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
            Dealcard();
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

    //게임 시작 시 호출할 함수
    public override void InitializeGame()  //기존 Start 유니티 함수에 있던걸 직접 눌러서 실행하도록 함수로 뺌
    {
        ServerGameSetting();
    }
    public override void EndGame()
    {

    }

    public bool GetIsOpenable()
    {
        return isOpenable;
    }

    #region NetworkFunction

    // 클라이언트에서 서버로 카드 오픈 요청을 보내는 ServerRpc
    [ServerRpc(RequireOwnership = false)]
    public void RequestOpenCardServerRpc(int playerNum, int cardIndex)
    {
        // 서버에서 카드 오픈 처리
        OpenCardOnServer(playerNum, cardIndex);
    }

    // 서버에서 카드를 오픈하는 실제 처리 함수
    private void OpenCardOnServer(int playerNum, int cardIndex)
    {
        Debug.Log($"{cardIndex} 이 카드임");
        HalliGalliCard findcard = null;
        foreach (HalliGalliCard card in m_card)
        {
            if (card.m_CardIndex == cardIndex)
                findcard = card;
        }
        if (findcard != null)
        {
            m_topCards[playerNum] = findcard;                        // 그 카드를 m_topCard에 추가
            m_openedCard.Add(findcard);                             // m_openedCard에 추가

            // OnTopCardChanged 이벤트 호출 : 텍스트 확인용 액션 - 할리갈리 개발 완료 시 삭제예정
            OnTopCardChanged?.Invoke(m_topCards[playerNum].m_AnimalType.ToString() + m_topCards[playerNum].m_fruitNum, playerNum);
        }
        else
        {
            Debug.Log("클라이언트에서 OpenCArd했는데 못찾음");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestIsOpenableServerRpc(int playerNum, int cardIndex)
    {
        IsOpenableOnServer(playerNum, cardIndex); 
        SendMessageToClientRpc(isOpenable, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { (ulong)playerNum }
            }
        });
    }

    [ClientRpc]
    private void SendMessageToClientRpc(bool _isOpenable, ClientRpcParams clientRpcParams)
    {
        isOpenable = _isOpenable;
    }

    private void IsOpenableOnServer(int playerNum, int cardIndex)
    {
        HalliGalliCard card = m_card[cardIndex];
        // 룰 추가 : 자신의 카드만 Open이 가능하게.
        // playerNum의 카드 덱에 halligalliCard가 존재하는지 확인
        if (m_playerCard[playerNum].Contains(card))
        {
            // 있다면 덱에서 제거
            m_playerCard[playerNum].Remove(card);
            isOpenable = true;
        }
    }

    #endregion

    private void Start()
    {
        m_gameManager = GameManager.Instance;
        m_card = GetComponentsInChildren<HalliGalliCard>();
        m_cardHeight = 0.01f;

        m_cardPos = GetComponentsInChildren<CardPos>();
    }
}