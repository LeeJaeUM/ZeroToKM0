using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
using Unity.Netcode.Components;

public class Card : NetworkBehaviour
{
    public bool m_isPlaced = false;                    // 테이블이나 카드위에 올려져있는지 확인하는 변수. Drag하고 있는중에 false가됨.
    public float m_cardSpacing = 0.1f;                 // 카드 사이 간격

    public int m_cardNum;             // 카드번호, Shuffle확인용
                                      // TODO : 나중에 지우기

    [SerializeField]private CardAnimation m_cardAnimation;                             // 카드 애니메이션을 위한 클래스

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
        if(m_cardAnimation != null )
            m_cardAnimation.FlipCardAnim();     // 애니메이션 실행
    }

    public void CardShuffleAnim()
    {
        m_cardAnimation.CardShuffleAnim();  // 애니메이션 실행
    }

    public void OpenCardInCard(int player)
    {
        FlipCardAnim();
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
            }
            else if (other.collider.CompareTag("Table"))
            {
                Debug.Log("Table");

                m_isPlaced = true;  // 테이블 위에 놓였음을 표시
            }
        }
    }
    protected virtual void Start()
    {
        m_cardAnimation = GetComponent<CardAnimation>();        // 카드 애니메이션 컴포넌트 참조
    }

}
