using System.Collections.Generic;
using UnityEngine;

// 카드가 2개 이상 모였을 시 관리하는 클래스.
public class CardDeck
{
    private List<Card> m_cardDeck = new List<Card>();

    private DeckManager m_deckManager;
    public int DeckCount
    {
        get { return m_cardDeck.Count; }
    }
    public CardDeck(DeckManager deckManager)
    {
        m_deckManager = deckManager;
    }
    public void AddToDeck(Card card)            // 카드덱에 card를 넣어줌.
    {       
        m_cardDeck.Add(card);                   // 입력받은 card추가
        card.CardDeck = this;
    }
    public void RemoveFromDeck(Card card)       // 카드덱에서 card값 제거.
    {
        m_cardDeck.Remove(card);
        // 덱에 카드가 1장 이하 있다면, 덱이 아니므로 데이터 전부 삭제 후 deckManager에게 반환
        if(DeckCount <= 1)
        {
            m_cardDeck.Clear();
            m_deckManager.ReturnDeck(this);
        }
        card.CardDeck = null;       
    }
    public void ShuffleDeck()                                               // 이 카드덱의 m_cardDeck을 섞어줌.
    {
        Vector3[] newPos = new Vector3[DeckCount];                   // 카드들의 위치를 저장할 임시 배열.
        // 현재 위치 정보 newPos에 저장
        for (int i = 0; i < DeckCount; i++)
        {
            newPos[i] = m_cardDeck[i].transform.position;
        }
        // m_cardDeck 섞어줌.
        GameManager.Instance.ListShuffle(m_cardDeck);
        // 카드들의 실제 위치를 옮겨줌.
        for (int i = 0; i < DeckCount; i++)
        {
            m_cardDeck[i].gameObject.transform.position = newPos[i];
        }    
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
