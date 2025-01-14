using System.Collections.Generic;
using UnityEngine;

public class CardDeck
{
    public List<Card> m_cardDeck;

    public int DeckCount
    {
        get { return m_cardDeck.Count; }
    }
    public CardDeck(Card card)
    {
        m_cardDeck = new List<Card>
        {
            card
        };
    }
    public void AddToDeck(Card card)            // 카드덱에 card를 넣어줌.
    {       
        m_cardDeck.Add(card);                   // 입력받은 card추가
        card.CurrentCardDeck = this;
    }
    public void RemoveFromDeck(Card card)       // 카드덱에서 card값 제거.
    {
        m_cardDeck.Remove(card);
        card.CurrentCardDeck = null;
    }
    public void ShuffleDeck()                                               // 이 카드덱의 m_cardDeck을 섞어줌.
    {
        List<Card> shuffledCards = new List<Card>();                        // 섞은 카드의 정보를 저장할 임시 리스트.
        Vector3[] newPos = new Vector3[m_cardDeck.Count];                   // 카드들의 위치를 저장할 임시 배열.
        int[] shuffledIndex;                                                // GameManager의 Shuffle함수를 통해 반환될 int형 배열의 정보를 가질 임시 배열.
        shuffledIndex = GameManager.Instance.Shuffle(m_cardDeck.Count);     // GameManager의 Shuffle함수를 통해 값을 반환받음.

        // 반환받은 shuffledIndex의 데이터를 토대로 shuffleCards에 값을 넣어줌.
        for (int i = 0; i < m_cardDeck.Count; i++)
        {
            shuffledCards.Add(m_cardDeck[shuffledIndex[i]]);

            newPos[i] = m_cardDeck[i].transform.position;
        }
        // 카드들의 실제 위치를 옮겨줌.
        for (int i = 0; i < m_cardDeck.Count; i++)
        {
            shuffledCards[i].gameObject.transform.position = newPos[i];
        }
        m_cardDeck = shuffledCards; // 섞인 카드를 m_cardDeck에 반영

        // 확인용
        // TODO : 나중에 지우기
        PrintDeck();
    }
    // 확인용
    // TODO : 나중에 지우기
    public void PrintDeck()
    {
        for(int i = 0; i < DeckCount ; i++)
        {
            Debug.Log(m_cardDeck[i].m_cardNum);
        }
    }
}
