using UnityEngine;
using System.Collections.Generic;

// CardDeck을 관리하는 스크립트
public class DeckManager : MonoBehaviour
{
    public GameObject m_cardDeckPrefab;
    public GameObjectPool<CardDeck> m_deckPool;
    public void RemoveDeck(CardDeck deck)
    {
        deck.gameObject.SetActive(false);
        m_deckPool.Set(deck);
    }
    public CardDeck CreateDeck(Vector3 deckPos)
    {
        var deck = m_deckPool.Get();
        deck.gameObject.SetActive(true);
        deck.transform.position = deckPos;

        return deck;
    }
    void Start()
    {
        var prefab = m_cardDeckPrefab;
        m_deckPool = new GameObjectPool<CardDeck>(10, () =>
        {
            var obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            var deck = obj.GetComponent<CardDeck>();
            deck.Initialize(this);
            return deck;
        });        
    }
    //private Queue<CardDeck> m_cardDeckPool;

    //public DeckManager(int count)
    //{
    //    m_cardDeckPool = new Queue<CardDeck>();
    //    // count만큼 CardDeck미리 생성
    //    for (int i = 0; i < count; i++)
    //    {
    //        CreateDeck();
    //    }
    //}
    //public CardDeck GetDeck(Card card)
    //{
    //    // 풀에 사용 가능한 덱이 있는지 확인
    //    if (m_cardDeckPool.Count == 0)
    //    {
    //        // 풀에 사용 가능한 덱이 없으면 새로 생성
    //        CreateDeck();
    //    }
    //    // 풀에서 사용 가능한 덱을 꺼내서 card를 추가해줌. 그후 반환
    //    CardDeck deck = m_cardDeckPool.Dequeue();
    //    deck.AddToDeck(card);
    //    return deck;
    //}

    //// 덱을 생성 하는 함수
    //private void CreateDeck()
    //{
    //    CardDeck newDeck = new CardDeck(this);
    //    m_cardDeckPool.Enqueue(newDeck);
    //}
    //// 덱을 반환 받는 함수.
    //public void ReturnDeck(CardDeck deck)
    //{
    //    m_cardDeckPool.Enqueue(deck); // 사용된 덱을 다시 큐에 추가
    //}
}
