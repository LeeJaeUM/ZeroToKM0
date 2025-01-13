using UnityEngine;
using UnityEngine.InputSystem;

public class OldObjectDrag : MonoBehaviour
{
    private Camera m_camera;            // 플레이어의 카메라
    private bool m_isDragging = false;
    private bool m_shiftPressed = false; // Shift 키 상태
    [SerializeField]
    private Vector3 m_offset;
    [SerializeField]
    private float m_dragUpYPos = 0.3f;
    [SerializeField]
    private Transform m_draggedObject;
    private Rigidbody m_draggedRigidbody;   // 드래그 중인 오브젝트의 Rigidbody
    private Card m_draggedCard;             // 드래그 중인 카드

    #region InputSystem

    // 좌클릭 이벤트 처리
    public void OnClick(InputValue value)
    {
        if (value.isPressed) // 마우스 버튼을 누를 때
        {
            Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                m_draggedObject = hit.transform;

                // 클릭된 오브젝트를 약간 위로 올림
                m_draggedObject.position = new Vector3(
                    m_draggedObject.position.x,
                    m_draggedObject.position.y + m_dragUpYPos,
                    m_draggedObject.position.z
                );

                // Rigidbody의 useGravity를 false로 설정
                Rigidbody rb = m_draggedObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    m_draggedRigidbody = rb; // 드래그 중인 Rigidbody 저장
                    rb.useGravity = false;
                }
                Card card = hit.transform.GetComponent<Card>();
                if (card != null)
                {
                    m_draggedCard = card;
                    card.IsMove(true);
                }
                    m_isDragging = true;
            }
        }
        else // 마우스 버튼을 뗄 때
        {
            m_isDragging = false;

            // 드래그 중인 Rigidbody가 있다면 useGravity를 true로 복원
            if (m_draggedRigidbody != null)
            {
                m_draggedRigidbody.useGravity = true;
                m_draggedRigidbody = null;
            }
            if(m_draggedCard != null)
            {
                m_draggedCard.IsMove(false);
                m_draggedCard = null;
            }

            m_draggedObject = null;
        }
    }
    // Shift 키 상태 업데이트
    public void OnShift(InputValue value)
    {
        m_shiftPressed = value.isPressed;
    }

    // F버튼 이벤트 처리 (카드 뒤집기)  
    public void OnFlip(InputValue value)
    {
        if (value.isPressed) // 우클릭 시
        {
            Debug.Log("우클릭함");
            Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log($"뭔가에 닿음!!{hit.collider.name}");
                // Card 스크립트를 확인
                Card card = hit.transform.GetComponent<Card>();
                if (card != null)
                {
                    Debug.Log($"--------------카드에 닿음{hit.collider.name}");
                    card.FlipCard(); // FlipCard 함수 호출
                }
            }
            else
            {
                Debug.Log("d아무것도 안 닿았어");
            }
        }
    }

    #endregion
    void Start()
    {
        m_camera = GetComponent<Camera>();
    }

    void Update()
    {
        if (m_isDragging && m_draggedObject != null)
        {
            // 마우스 위치에서 Ray를 쏘아 드래그 중인 오브젝트 위치를 업데이트
            Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 newPosition = new Vector3(hit.point.x, m_draggedObject.position.y, hit.point.z); //+ m_offset;

                // Shift 키를 눌렀을 경우 Y값 변경가능
                if (m_shiftPressed)
                {
                    newPosition.y = hit.point.y;
                }

                m_draggedObject.position = newPosition;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (m_camera == null) return;

        // 현재 카메라가 바라보는 방향으로 선 그리기
        Gizmos.color = Color.red;
        Gizmos.DrawRay(m_camera.transform.position, m_camera.transform.forward * 100f);
    }
}
