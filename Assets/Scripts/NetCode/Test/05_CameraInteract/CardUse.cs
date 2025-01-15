using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class CardUse : MonoBehaviour
{
    private Camera m_camera; // 플레이어 카메라 참조
    private Card m_draggedCard;

    private List<Transform> m_scannedObjects = new List<Transform>(); // 드래그 중인 오브젝트들 리스트
    private List<Card> m_scannedCards = new List<Card>(); // 드래그 중인 오브젝트들의 NetworkMove 컴포넌트들
    private bool m_isScanning = false; // 드래그 상태 플래그
    [SerializeField] private float m_dragRadius = 5f; // 드래그 가능한 범위
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
            ScanCards();
            ShuffleDeck();
            StopScanning();
        }
    }
 
    private void Flip()
    {
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Card 컴포넌트가 있다면 추가 처리
            m_draggedCard = hit.collider.GetComponent<Card>();
            if (m_draggedCard != null)
            {
                print("Flip");
                m_draggedCard.FlipCardAnim();
            }
        }
    }
    private void ShuffleDeck()                                               // 이 카드덱의 m_cardDeck을 섞어줌.
    {
        int cardCount = m_scannedCards.Count;
        Vector3[] newPos = new Vector3[cardCount];                   // 카드들의 위치를 저장할 임시 배열.
        // Shuffle확인용
        Debug.Log("Shuffle 전 : ");
        // 현재 위치 정보 newPos에 저장
        for (int i = 0; i < cardCount; i++)
        {
            newPos[i] = m_scannedCards[i].transform.position;
            Debug.Log(m_scannedCards[i].m_cardNum);
        }
        // m_cardDeck 섞어줌.
        GameManager.Instance.ListShuffle(m_scannedCards);
        // 카드들의 실제 위치를 옮겨줌.
        // Shuffle확인용
        Debug.Log("Shuffle 후 : ");
        for (int i = 0; i < cardCount; i++)
        {
            m_scannedCards[i].gameObject.transform.position = newPos[i];
            Debug.Log(m_scannedCards[i].m_cardNum);
        }
    }
    // ray를 쏴서 Shuffle할 카드 가져오기
    private void ScanCards()
    {
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        m_scannedObjects.Clear(); // 기존 드래그 리스트 초기화
        m_scannedCards.Clear();

        foreach (RaycastHit hit in hits)
        {
            // Raycast가 맞은 지점이 일정 범위 내에 있고, NetworkMove 컴포넌트를 가지고 있는 경우
            if (Vector3.Distance(ray.origin, hit.point) <= m_dragRadius)
            {
                var card = hit.transform.GetComponent<Card>();
                if (card != null)
                {
                    m_scannedObjects.Add(hit.transform); // 드래그 가능한 오브젝트 추가
                    m_scannedCards.Add(card);
                }
            }
        }

        if (m_scannedObjects.Count > 0)
        {
            m_isScanning = true;
        }
    }

    // 드래그 중단 처리
    private void StopScanning()
    {
        m_isScanning = false;

        m_scannedObjects.Clear(); // 드래그된 오브젝트 리스트 초기화
        m_scannedCards.Clear();
    }
    private void Start()
    {
        m_camera = GetComponent<Camera>(); // 플레이어 카메라 초기화
    }
}
