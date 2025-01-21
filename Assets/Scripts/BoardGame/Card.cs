using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
using Unity.Netcode.Components;

public class Card : NetworkBehaviour
{
    public bool m_isPlaced = false;                    // 테이블이나 카드위에 올려져있는지 확인하는 변수. Drag하고 있는중에 false가됨.
    public float m_cardSpacing = 1f;                 // 카드 사이 간격

    public int m_cardNum;             // 카드번호, Shuffle확인용
                                      // TODO : 나중에 지우기

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
<<<<<<< Updated upstream
=======

    public void OpenCardInCard(int player)
    {
        FlipCardAnim();
    }
    /// <summary>
    /// 이 카드 객체와 인자로 받은 카드의 y축 값을 비교하여 더 낮은 위치의 카드 위에 더 높은 위치의 카드를 올려줌
    /// </summary>
    /// <param name="placedCard"> 접촉한 카드 </param>
    public void PlaceOnCard(Card otherCard)
    {
        Vector3 newPos;

        if (otherCard.transform.position.y > transform.position.y)
        {
            print("otherCard Up");
            newPos = transform.position;
            newPos.y += m_cardSpacing;  // 높이 조정
            otherCard.transform.position = newPos;
            otherCard.transform.rotation = transform.rotation;
        }
        else if(otherCard.transform.position.y == transform.position.y)
        {
            print("Same");
        }
        else
        {
            print("otherCard Down");

            newPos = otherCard.transform.position;
            newPos.y += m_cardSpacing;  // 높이 조정
            transform.position = newPos;
            transform.rotation = otherCard.transform.rotation;
        }
    }

    //private void OnCollisionEnter(Collision other)
    //{
    //    if (!m_isPlaced)
    //    {
    //        if (other.collider.CompareTag("Card"))
    //        {
    //            m_isPlaced = true;
    //            Debug.Log("Card");
    //            //Vector3 newPos;
    //            //newPos = other.transform.position;
    //            //newPos.y += m_cardSpacing;  // 높이 조정
    //            //transform.position = newPos;
    //            //transform.rotation = other.transform.rotation;
    //            Card card = other.transform.GetComponent<Card>();
    //            PlaceOnCard(card);
    //        }
    //        else if (other.collider.CompareTag("Table"))
    //        {
    //            Debug.Log("Table");

    //            m_isPlaced = true;  // 테이블 위에 놓였음을 표시
    //        }
    //    }
    //}
    private void OnCollisionEnter(Collision other)
    {
        // 이미 놓여진 상태라면 충돌 무시
        if (m_isPlaced)
            return;

        // 충돌한 대상이 카드라면 처리
        if (other.collider.CompareTag("Card"))
        {
            Card otherCard = other.gameObject.GetComponent<Card>();

            if (otherCard != null && otherCard.m_isPlaced) // 다른 카드가 이미 놓여진 상태라면
            {
                PlaceOnTopOfCard(otherCard);
            }
        }
        else if (other.collider.CompareTag("Table"))
        {
            Debug.Log("Table");

            m_isPlaced = true;  // 테이블 위에 놓였음을 표시
        }
    }

    /// <summary>
    /// 다른 카드 위에 이 카드를 얹습니다.
    /// </summary>
    /// <param name="otherCard">대상 카드</param>
    private void PlaceOnTopOfCard(Card otherCard)
    {
    // 새 위치 계산
    Vector3 newPosition = otherCard.transform.position;
    newPosition.y += m_cardSpacing; // 높이를 대상 카드 위로 조정

    // 위치와 회전 설정
    transform.position = newPosition;
    transform.rotation = otherCard.transform.rotation;

    // 놓인 상태로 설정
    m_isPlaced = true;

    Debug.Log($"Card placed on top of another card: {otherCard.name}");
    }

    protected virtual void Start()
    {
        m_cardAnimation = GetComponentInChildren<CardAnimation>();        // 카드 애니메이션 컴포넌트 참조
    }

>>>>>>> Stashed changes
}
