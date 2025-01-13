using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectDrag : MonoBehaviour
{
    private Camera m_camera; // 메인 카메라 참조
    private Transform m_draggedObject; // 드래그 중인 오브젝트
    private Rigidbody m_draggedRigidbody; // 드래그 중인 오브젝트의 Rigidbody
    private Card m_draggedCard; // 드래그 중인 카드
    private bool m_isDragging = false; // 드래그 상태 플래그
    private bool m_shiftPressed = false; // Shift 키 상태 플래그
    [SerializeField] private float m_dragUpYPos = 0.1f; // 오브젝트를 올릴 높이

    private Vector3 m_offset; // 마우스와 오브젝트의 상대적 위치 오프셋

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

    // 마우스 이동 이벤트 처리
    public void OnMouseMove(InputValue value)
    {
        if (m_isDragging && m_draggedObject != null)
        {
            UpdateDraggedObjectPosition();
        }
    }

    // Shift 키 입력 이벤트
    public void OnShift(InputValue value)
    {
        m_shiftPressed = value.isPressed;
    }

    // 드래그 시작 처리
    private void StartDragging()
    {
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            m_draggedObject = hit.transform;

            // 클릭된 오브젝트를 약간 위로 올림
            m_draggedObject.position += new Vector3(0, m_dragUpYPos, 0);

            // Rigidbody의 useGravity를 false로 설정
            m_draggedRigidbody = m_draggedObject.GetComponent<Rigidbody>();
            if (m_draggedRigidbody != null)
            {
                m_draggedRigidbody.useGravity = false;
            }

            // Card 컴포넌트가 있다면 추가 처리
            m_draggedCard = m_draggedObject.GetComponent<Card>();
            if (m_draggedCard != null)
            {
                m_draggedCard.IsMove(true);
                m_draggedCard.m_isPlaced = false;        
                // Card가 Deck에 속해있었다면, 해당 Card를 CardDeck에서 제거.
                if(m_draggedCard.m_cardDeck.Count != 0)
                {
                    m_draggedCard.RemoveFromDeck(m_draggedCard);
                }
            }
            m_isDragging = true;
        }
    }

    // 드래그 중단 처리
    private void StopDragging()
    {
        m_isDragging = false;

        // Rigidbody의 useGravity를 true로 복원
        if (m_draggedRigidbody != null)
        {
            m_draggedRigidbody.useGravity = true;
            m_draggedRigidbody = null;
        }

        // Card 컴포넌트 처리
        if (m_draggedCard != null)
        {
            m_draggedCard.IsMove(false);
            m_draggedCard = null;
        }

        m_draggedObject = null;
    }

    // 드래그 중인 오브젝트 위치 업데이트
    private void UpdateDraggedObjectPosition()
    {
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 newPosition = hit.point + m_offset;

            // Shift 키 상태에 따라 Y축 변경
            if (!m_shiftPressed)
            {
                newPosition.y = m_draggedObject.position.y;
            }

            m_draggedObject.position = newPosition;
        }
    }
    private void Start()
    {
        m_camera = GetComponent<Camera>(); // 메인 카메라 초기화
    }

}
