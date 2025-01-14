using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class CardUse : MonoBehaviour
{
    private Camera m_camera; // 메인 카메라 참조
    private Card m_draggedCard;

    // 마우스 클릭 이벤트 처리
    public void OnClick(InputValue value)
    {
        if (value.isPressed)
        {
            StartDragging();
        }
        else
        {
            StopDragging();
        }
    }

    public void OnShuffle(InputValue value)
    {
        if (value.isPressed)
        {
            Shuffle();
        }

    }
    // 드래그 시작 처리
    private void StartDragging()
    {
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Card 컴포넌트가 있다면 추가 처리
            m_draggedCard = hit.collider.GetComponent<Card>();
            if (m_draggedCard != null)
            {
                m_draggedCard.IsMove(true);
                m_draggedCard.m_isPlaced = false;
                // Card가 Deck에 속해있었다면, 해당 Card를 CardDeck에서 제거.
                m_draggedCard.CurrentCardDeck.RemoveFromDeck(m_draggedCard);
            }
        }
    }

    // 드래그 중단 처리
    private void StopDragging()
    {

    }
    private void Shuffle()
    {
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Card 컴포넌트가 있다면 추가 처리
            m_draggedCard = hit.collider.GetComponent<Card>();
            if (m_draggedCard != null)
            {
                print("Shuffle");
                m_draggedCard.CurrentCardDeck.ShuffleDeck();
                m_draggedCard.CardSuffleAnimation();
            }
        }
    }
    private void Start()
    {
        m_camera = GetComponent<Camera>(); // 메인 카메라 초기화
    }
}
