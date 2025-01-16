using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
using Unity.Netcode.Components;

public class Card : NetworkBehaviour
{
    public enum CardState
    {
        OnTable,           // 테이블에 닿아있을 때( 맨 앞 카드인 경우 )
        Floating,          // 공중에 있을 때( 맨 앞 카드인 경우 )
        OnCard             // 카드 위에 올라가있을 때
    }
    private CardState m_state;
    public CardState State {  get { return m_state; } set { m_state = value; } }
    //private CardDeck m_cardDeck;                       // 이 카드가 속해있는 카드덱의 주소, 카드덱에 속하지 않는 경우 null

    public bool m_isPlaced = false;                    // 테이블이나 카드위에 올려져있는지 확인하는 변수. Drag하고 있는중에 false가됨.
    public bool m_isFirstCard = true;
    public float m_cardSpacing = 0.1f;                 // 카드 사이 간격

    public Card m_frontCard = null;             // 내 앞(밑) 카드
    public Card m_backCard = null;              // 내 뒤(위) 카드

    public int m_cardNum;             // 카드번호, Shuffle확인용
                                      // TODO : 나중에 지우기
    //public CardDeck CardDeck                            // m_cardDeck 프로퍼티
    //{
    //    get { return m_cardDeck; }                        // 현재 이 카드가 속해있는 덱의 주소를 반환.                      
    //    set { m_cardDeck = value; }
    //}

    public CardAnimation m_cardAnimation;                             // 카드 애니메이션을 위한 클래스

    void Start()
    {
        m_cardAnimation = GetComponent<CardAnimation>();        // 카드 애니메이션 컴포넌트 참조
    }
    
    private void HandlePositionChanged(Vector3 oldPosition, Vector3 newPosition)
    {
        // 위치가 변경되면 클라이언트에서 해당 위치로 이동
        transform.position = newPosition;
    }

    private void HandleRotationChanged(Quaternion oldRotation, Quaternion newRotation)
    {
        // 회전이 변경되면 클라이언트에서 해당 회전으로 회전
        transform.rotation = newRotation;
    }
    public void FlipCardAnim()              // 카드를 뒤집어주는 함수
    {
        m_cardAnimation.FlipCardAnim();     // 애니메이션 실행
        //m_isOpen = !m_isOpen;               // isOpen값을 반대로 바꿔줌
        //if(IsServer)
        //    m_networkAnimator.Animator.SetBool("isOpen", m_isOpen); // 애니메이션 실행

    }

    public void CardShuffleAnim()
    {
        m_cardAnimation.CardShuffleAnim();  // 애니메이션 실행
        //m_networkAnimator.SetTrigger("Shuffle");  // 애니메이션 실행
    }

    public void SetMyBackCards()
    {
        Vector3 newPos = transform.position;
        newPos.y += m_cardSpacing;
        m_backCard.transform.position = newPos;
        if (m_backCard != null)
        {
            m_backCard.SetMyBackCards();
        }
    }
    public Card FindFirstCard()
    {
        if (m_frontCard != null)
        {
            m_frontCard.FindFirstCard();
        }

        return this;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Table"))
        {
            State = CardState.OnTable;
        }
        else if (State == CardState.Floating && other.collider.CompareTag("Card"))
        {
            Card otherCard = other.transform.GetComponent<Card>();
            if(otherCard != null && otherCard.FindFirstCard() != this)
            {
                State = CardState.OnCard;
                m_frontCard = otherCard;
                otherCard.m_backCard = this;
                Vector3 newPos = otherCard.transform.position;
                newPos.y += m_cardSpacing;
                transform.position = newPos;
                SetMyBackCards();
            }
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag("Table"))
        {
            State = CardState.Floating;
        }
    }
    //private void OnCollisionEnter(Collision other)
    //{
    //    if (m_isFirstCard && !m_isPlaced)
    //    {
    //        if (other.collider.CompareTag("Card"))
    //        {
    //            Card otherCard = other.transform.GetComponent<Card>();
    //            if (otherCard.m_backCard == null)
    //            {
    //                m_frontCard = otherCard;
    //                otherCard.m_backCard = this;
    //                m_frontCard.SetMyBackCards();
    //            }
    //        }
    //        else
    //        {
    //            m_frontCard = null;
    //            m_isPlaced = true;
    //        }
    //    }
    //}
    //private void OnCollisionExit(Collision other)
    //{
    //    if (other.collider.CompareTag("Card"))
    //    {
    //        //Card otherCard = other.transform.GetComponent<Card>();
    //        //if (otherCard == m_frontCard)
    //        //{
    //        //    m_frontCard = null;
    //        //    otherCard.m_backCard = this;
    //        //    m_frontCard.SetMyBackCards();
    //        //}
    //        print("카드랑 떨어짐");
    //    }
    //    else
    //    {
    //        m_frontCard = null;
    //        m_isPlaced = true;
    //    }
    //}
    //private void OnCollisionEnter(Collision other)
    //{
    //    // 다른 카드와 충돌했을 때 처리
    //    Card otherCard = other.transform.GetComponent<Card>();
    //    if (otherCard != null)
    //    {
    //        if (otherCard.m_isPlaced || m_isPlaced)
    //        {
    //            m_isPlaced = true;
    //            otherCard.m_isPlaced = true;
    //            // 두 카드가 겹쳤을 때 정렬
    //            //SortCards(this, otherCard);
    //        }
    //    }
    //    else if (other.collider.CompareTag("Table"))
    //    {
    //        Debug.Log("Table");
    //        m_isPlaced = true;  // 테이블 위에 놓였음을 표시
    //    }
    //}
    //private void SortCards(Card card1, Card card2)
    //{
    //    // 카드들 사이의 위치를 정렬하는 로직
    //    // 예를 들어 Y 값을 기준으로 카드의 순서를 정렬하고 겹침을 방지
    //    Vector3 card1Pos = card1.transform.position;
    //    Vector3 card2Pos = card2.transform.position;

    //    // 두 카드 사이의 Y 위치 비교
    //    if (card1Pos.y == card2Pos.y)
    //    {
    //        card2.transform.position = new Vector3(card2Pos.x, card1Pos.y + 0.1f, card2Pos.z); // 카드 2는 카드 1 위로 살짝 이동
    //    }
    //    else if (card1Pos.y > card2Pos.y)
    //    {
    //        card2.transform.position = new Vector3(card2Pos.x, card1Pos.y + 0.1f, card2Pos.z); // 카드 2는 카드 1 위로 살짝 이동
    //    }
    //    else
    //    {
    //        card1.transform.position = new Vector3(card1Pos.x, card2Pos.y + 0.1f, card1Pos.z); // 카드 1은 카드 2 위로 살짝 이동
    //    }
    //}
}
