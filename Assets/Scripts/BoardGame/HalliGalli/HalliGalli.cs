using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class HalliGalli : MonoBehaviour
{
    public HalliGalliCard[] m_card;                   // ��ü ī��
    public Queue<HalliGalliCard>[] m_playerCard;      // �÷��̾� ������ ī��
    public HalliGalliCard[] m_topCard;                // �� �÷��̾��� �� �� ī��
    public List<HalliGalliCard> m_openedCard;         // ���µ� ī��

    public int[] m_playerCardCount;         // �� �÷��̾��� ī�� ����
    public float m_cardHeight;              // ī���� ����
    public CardPos[] m_cardPos;           // �� �÷��̾��� ī�� ��ġ

    public int[] m_fruitCount = { 3, 3, 3, 3, 2 };              // ���� ������ ī�� ����, ex) ������ 1�� �׷��� ī��� �� m_fruitCount[0] = 3��

    public int m_roundCount = 1;

    public void GameSetting()                                                       // ���� ���� �� ����
    {
        print("GameStart");

        CreateCard();
        m_playerCardCount = new int[GameManager.Instance.PlayerCount];
        GameManager.Instance.Calculatecard(m_card.Length, GameManager.Instance.PlayerCount, m_playerCardCount);
        Collectcard();
        GameManager.Instance.Shuffle(m_card);
        m_playerCard = new Queue<HalliGalliCard>[GameManager.Instance.PlayerCount];
        for (int i = 0; i < GameManager.Instance.PlayerCount; i++)                  // m_playerCard �ʱ�ȭ
        {
            m_playerCard[i] = new Queue<HalliGalliCard>();
        }
        m_topCard = new HalliGalliCard[GameManager.Instance.PlayerCount];

        DistributeCard();
        Dealcard();
    }
    public void CreateCard()                                                        // ī�� �ʱ�ȭ ���ֱ�( type, ���� )
    {
        int i = 0;
        foreach (HalliGalliCard.FruitType fruit in Enum.GetValues(typeof(HalliGalliCard.FruitType)))
        {
            for (int k = 0; k < m_fruitCount.Length; k++)
            {
                for (int j = 0; j < m_fruitCount[k]; j++)
                {
                    m_card[i++].Initialize(fruit, k + 1);
                }
            }
        }
    }
    public void DistributeCard()                             // ī�� ���, ��ġ�� ����
    {
        int k = 0;      // ī�� ��ȣ üũ ��
        for (int i = 0; i < GameManager.Instance.PlayerCount; i++)              // i : �÷��̾� ��ȣ
        {
            for (int j = 0; j < m_playerCardCount[i]; j++)   // j : �÷��̾ ���� ī�� �������� ī�� ��ȣ
            {
                m_playerCard[i].Enqueue(m_card[k++]);
            }
        }
    }
    public void Dealcard()                                   // ī�� ����, ��ġ�� �ű�
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
    public void SetPos(int playerNum, GameObject gameobj)       // ī�带 ��ġ���ִ� �Լ�(������ ī�带, ���°�� ������, � ī������)
    {
        Vector3 cardPos = m_cardPos[playerNum].transform.position;
        cardPos.y += m_cardHeight * m_cardPos[playerNum].m_cardCount++;

        gameobj.transform.position = cardPos;
        gameobj.transform.forward = m_cardPos[playerNum].transform.forward;
        gameobj.transform.Rotate(Vector3.right * 90);

    }
    public void Collectcard()                          // ��� ī�� ������ ��������
    {
        for (int i = 0; i < m_card.Length; i++)
        {
            SetPos(8, m_card[i].gameObject);
        }
    }

    public void OpenCard(int playerNum)     
    {
        playerNum = GameManager.Instance.GetCurrentPlayer();    // �ϴ��� ���� �÷��̾��� ī�带 open�ϴ� �������,
                                                                // todo : ��Ƽ�÷��� �����Ǹ� �����
        HalliGalliCard card;
        if (m_playerCard[playerNum].Count > 0)
        {
            card = m_playerCard[playerNum].Dequeue();           // �Է� ���� �÷��̾��� ī�嵦���� ���� ���� ī�带 ������
            m_topCard[playerNum] = card;                        // �� ī�带 m_topCard�� �߰�
            m_openedCard.Add(card);                             // m_openedCard�� �߰�

            SetPos(playerNum + 4, card.gameObject);
            card.OpenCard();                                    // card�� ������ �Լ�( �۵� �ȵ� )

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
    public void RingBell(int playernum)                         // player�� space�ٸ� ������ �� ȣ��
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
    public bool isCorrect()                                 // �� �� ī����� �ɺ��� ���� ����Ͽ� true, false�� ��ȯ
    {
        int[] sum = new int[4];                             // �ɺ� �� ���� ����� �迭

        if (m_topCard == null)
        {
            return false;
        }
        for (int i = 0; i < GameManager.Instance.PlayerCount; i++)              // �� �ɺ� �� ���� ���
        {
            if (m_topCard[i] != null)
                sum[(int)m_topCard[i].m_fruitType] += m_topCard[i].m_fruitNum;
        }

        for (int i = 0; i < sum.Length; i++)                 // ���� 5�� �Ǵ� �ɺ��� �ִ��� Ȯ��
        {
            if (sum[i] == 5)
                return true;
        }
        return false;
    }
    public void GiveCard(int playerNum)                     // ���ڿ��� open�� ī�带 ���� ��.
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
    public void RoundFinish()                               // Ż���ڸ� �����ϰ�, �� ���带 �����ϴ� �Լ�.
                                                            // ���� �ļ� ������ ��� ȣ���.
    {
        for (int i = 0; i < m_playerCard.Length; i++)  // ī�尡 0���� �÷��̾� Ż��
        {
            if (m_playerCard[i].Count == 0)
            {
                GameManager.Instance.RemovePlayer(i);
            }
        }
        if (GameManager.Instance.PlayerCount == 1)          // ȥ�� ������ ��� ���� �¸�.
        {
            GameOver();
        }
        else                                                // �÷��̾ 2�� �̻��� ���, �ٽ� ����
        {
            print(GameManager.Instance.PlayerCount);
            Array.Clear(m_topCard, 0, m_topCard.Length);    // topcard�ʱ�ȭ
            print("new round");
        }
    }
    public void GameOver()                                  // ���� ����
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