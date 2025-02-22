using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

// 카드관련 기능
// 1. F로 Flip
// 2. T로 Shuffle
// 3. G로 Sort
public class CardUse : MonoBehaviour
{
    private Camera m_camera; // 플레이어 카메라 참조
    private Card m_draggedCard;
    private HalliGalliCard m_draggedHalliGalliCard;
    private int m_playerNum;        // player번호

    private bool isTurnMode = true;

    private List<Card> m_scannedCards = new List<Card>(); // 드래그 중인 오브젝트들의 NetworkMove 컴포넌트들
    [SerializeField] private float m_dragRadius = 5f; // 드래그 가능한 범위
    private float m_cardSpacing = 0.1f;

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
            if (ScanCards())
            {
                ShuffleDeck();
            }
            m_scannedCards.Clear();
        }

    }
    public void OnSort(InputValue value)
    {
        if (value.isPressed)
        {
            if (ScanCards())
            {
                SortDeck();
            }
            m_scannedCards.Clear();
        }
    }
    /// <summary>
    /// 카드를 뒤집는 함수 OnFlip (키보드 f)으로 호출
    /// </summary>
    private void Flip()
    {
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            bool isMyTurn = false;
            int playerNum = (int)NetworkManager.Singleton.LocalClientId;
            if(isTurnMode)
            {
                isMyTurn = GameManager.Instance.IsMyTurn(playerNum);
            }
            else
            {
                isMyTurn = true;
            }
            Debug.Log($"현재 플레이어 넘버 : {playerNum}, 내턴 인지? = {isMyTurn}");
            //isMyTurn = GameManager.Instance.IsMyTurn((int)NetworkManager.Singleton.LocalClientId);
            if (isMyTurn)
            {            
                // HalliGalli Card 컴포넌트가 있다면 추가 처리
                m_draggedHalliGalliCard = hit.collider.GetComponent<HalliGalliCard>();
                if (m_draggedHalliGalliCard != null)
                {
                    //이 OpenCard는 GameManager에 연결된 HalliGalliNetwork의 Open카드를 실행시키는 함수
                    //그래서 자기 아이디와 현재 카드를 보냄
                    SoundManager.Instance.PlaySFX(SoundManager.SoundType.Flip);
                    GameManager.Instance.OpenCard((int)NetworkManager.Singleton.LocalClientId, m_draggedHalliGalliCard.m_CardIndex);
                }

                //내 카드가 맞다면 카드 뒤집는 애니메이션 실행
                //if(GameManager.Instance.IsOpenable())
                if(true)
                {
                    m_draggedCard = hit.collider.GetComponent<Card>();
                    if (m_draggedCard != null)
                    {
                        //m_draggedCard.FlipCardAnim(m_playerNum);
                        //사용자가 직접 카드에 애니메이션 작동하도록 하는 부분 
                        //그래서 여기서 직접 불러서 사용함
                        m_draggedCard.OpenCardInCard((int)NetworkManager.Singleton.LocalClientId);

                    }
                }
            }
        }
    }


    private void ShuffleDeck()                                               // 이 카드덱의 m_cardDeck을 섞어줌.
    {
        int cardCount = m_scannedCards.Count;
        Vector3[] newPos = new Vector3[cardCount];                   // 카드들의 위치를 저장할 임시 배열.

        SoundManager.Instance.PlaySFX(SoundManager.SoundType.Shuffle);
        // 현재 위치 정보 newPos에 저장
        for (int i = 0; i < cardCount; i++)
        {
            newPos[i] = m_scannedCards[i].transform.position;
        }
        // m_cardDeck 섞어줌.
        GameManager.Instance.ListShuffle(m_scannedCards);
        // 카드들의 실제 위치를 옮겨줌.
        for (int i = 0; i < cardCount; i++)
        {
            m_scannedCards[i].gameObject.transform.position = newPos[i];
            m_scannedCards[i].CardShuffleAnim();
        }
    }
    private void SortDeck()
    {
        Vector3 sortPos;
        // m_scannedCards y축 기준 오름차순 정렬
        m_scannedCards.Sort((obj1, obj2) => obj1.gameObject.transform.position.y.CompareTo(obj2.gameObject.transform.position.y));
        sortPos = m_scannedCards[0].transform.position;

        foreach (Card card in m_scannedCards)
        {
            sortPos.y += m_cardSpacing;
            card.gameObject.transform.position = sortPos;
        }
    }
    /// <summary>
    /// ray를 쏴서 맞은 카드들 m_scannedCards로 가져오기
    /// </summary>
    /// <returns>카드가 맞았으면 true, 맞지 않았으면 false반환 </returns>
    private bool ScanCards()
    {
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        m_scannedCards.Clear();

        foreach (RaycastHit hit in hits)
        {
            // Raycast가 맞은 지점이 일정 범위 내에 있고, Card 컴포넌트를 가지고 있는 경우
            if (Vector3.Distance(ray.origin, hit.point) <= m_dragRadius)
            {
                Card card = hit.transform.GetComponent<Card>();
                if (card != null)
                {
                    m_scannedCards.Add(card);
                }
            }
        }
        if(m_scannedCards.Count != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void OnChange(InputValue value)
    {
        if (value.isPressed)
        {
            SetTurnMode();
        }
    }
    public void SetTurnMode()
    {
        isTurnMode = !isTurnMode;
    }


    private void Start()
    {
        m_camera = GetComponent<Camera>(); // 플레이어 카메라 초기화
    }
}
