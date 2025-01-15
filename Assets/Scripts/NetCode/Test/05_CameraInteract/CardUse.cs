using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class CardUse : MonoBehaviour
{
    private Camera m_camera; // 플레이어 카메라 참조
    private Card m_draggedCard;
    private HalliGalliCard m_draggedHalliGalliCard;

    public void OnFlip(InputValue value)
    {
        if (value.isPressed)
        {
            Flip();
        }
    }

    public void OnShuffle(InputValue value)
    {
        if (value.isPressed)
        {
            Shuffle();
        }

    }
 
    private void Flip()
    {
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // HalliGalli Card 컴포넌트가 있다면 추가 처리
            m_draggedHalliGalliCard = hit.collider.GetComponent<HalliGalliCard>();
            if (m_draggedHalliGalliCard != null)
            {
                GameManager.Instance.OpenCard((int)NetworkManager.Singleton.LocalClientId, m_draggedHalliGalliCard);
            }

            m_draggedCard = hit.collider.GetComponent<Card>();
            if (m_draggedCard != null)
            {
                print("Flip");
                m_draggedCard.FlipCardAnim();
            }
            // Card 컴포넌트가 있다면 추가 처리

            //m_draggedCard = hit.collider.GetComponent<Card>();
            //if (m_draggedCard != null)
            //{
            //    print("Flip");
            //    m_draggedCard.FlipCardAnim();
            //    GameManager.Instance.OpenCard((int)NetworkManager.Singleton.LocalClientId, );
            //}
        }
    }

    private void Shuffle()
    {
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Card 컴포넌트가 있다면 추가 처리
            m_draggedCard = hit.collider.GetComponent<Card>();
            if (m_draggedCard != null && m_draggedCard.CardDeck != null)
            {
                print("Shuffle");
                m_draggedCard.CardDeck.ShuffleDeck();
                m_draggedCard.CardShuffleAnim();
            }
        }
    }
    private void Start()
    {
        m_camera = GetComponent<Camera>(); // 플레이어 카메라 초기화
    }
}
