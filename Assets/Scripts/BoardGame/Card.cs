using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
using Unity.Netcode.Components;

public class Card : NetworkBehaviour
{

    private CardDeck m_myCardDeck;                       // 카드 각각의 카드 덱 클래스, 카드 1장당 1개씩 가지고 있음.(변하지 않음)
    private CardDeck m_currentCardDeck;                  // 카드가 현재 속해있는 카드 덱의 레퍼런스.(위치가 옮겨질때 마다 변함)

    public bool m_isPlaced = false;                    // 테이블이나 카드위에 올려져있는지 확인하는 변수. Drag하고 있는중에 false가됨.
    public float m_cardSpacing = 0.1f;                 // 카드 사이 간격

    public int m_cardNum;             // 카드번호, Shuffle확인용
                                      // TODO : 나중에 지우기
    public CardDeck CurrentCardDeck {                   // m_currentCardDeck의 프로퍼티
        get { return m_currentCardDeck;}                // 현재 이 카드가 속해있는 덱의 주소를 반환.
        set {                                            
            if (value == null)                          // 아무값도 입력받지 않으면, m_currentCardDeck의 주소를 myCardDeck의 주소로 초기화.
                m_currentCardDeck = m_myCardDeck;
            else                                        // 이 카드가 속해있는 덱의 주소를 설정해줌.
                m_currentCardDeck = value;
        }
    }

    private CardAnimation m_cardAnimation;                             // 카드 애니메이션을 위한 클래스

    void Start()
    {
        m_myCardDeck = new CardDeck(this);          // myCardDeck 클래스 생성( 이 카드의 정보를 첫번째로 넣어줌 )
        CurrentCardDeck = m_myCardDeck;             // 현재 속해있는 덱을 myCardDeck으로 설정.
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
        //Debug.Log("FlipCard");
        //m_isOpen = !m_isOpen;               // isOpen값을 반대로 바꿔줌
        //if(IsServer)
        //    m_networkAnimator.Animator.SetBool("isOpen", m_isOpen); // 애니메이션 실행

    }

    public  void CardShuffleAnim()
    {
        m_cardAnimation.CardShuffleAnim();  // 애니메이션 실행
        //m_networkAnimator.SetTrigger("Shuffle");  // 애니메이션 실행
    }



    private void OnCollisionEnter(Collision other)
    {
        if (!m_isPlaced)
        {
            if (other.collider.CompareTag("Card"))
            {
                Debug.Log("Card");

                m_isPlaced = true;  // 카드가 놓였음을 표시

                // 카드 위치를 다른 카드와 일치시킴
                Vector3 newPos = other.transform.position;
                newPos.y += m_cardSpacing;  // 높이 조정

                transform.position = newPos;  // 최종 위치 설정

                // 카드의 회전 방향을 맞춤
                transform.rotation = other.transform.rotation;

                // m_cardDeck에 추가
                Card card = other.gameObject.GetComponent<Card>();
                card.CurrentCardDeck.AddToDeck(this);
            }
            else if (other.collider.CompareTag("Table"))
            {
                Debug.Log("Table");

                m_isPlaced = true;  // 테이블 위에 놓였음을 표시
            }
        }
    }
}
