using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectDrag : MonoBehaviour
{
    private Camera m_camera;            // 플레이어의 카메라
    private bool m_isDragging = false;
    private bool m_shiftPressed = false; // Shift 키 상태
    private Vector3 m_offset;
    private Transform m_draggedObject;

    void Start()
    {
        m_camera = GetComponent<Camera>();
    }
    //TODO : 현재 우클릭이 두 번클릭되는 문제가 있는데 Update에서 이미 돌아가고 있어서(Mouse.current.position.ReadValue())가 중복 호출의심됨

    void Update()
    {
        if (m_isDragging && m_draggedObject != null)
        {
            // 마우스 위치에서 Ray를 쏘아 드래그 중인 오브젝트 위치를 업데이트
            Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 newPosition = hit.point + m_offset;

                // Shift 키를 누르지 않았을 경우 Y값 고정
                if (!m_shiftPressed)
                {
                    newPosition.y = m_draggedObject.position.y;
                }

                m_draggedObject.position = newPosition;
            }
        }
    }

    // 좌클릭 이벤트 처리
    public void OnClick(InputValue value)
    {
        if (value.isPressed) // 마우스 버튼을 누를 때
        {
            Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                m_draggedObject = hit.transform;
                m_offset = m_draggedObject.position - hit.point;
                m_isDragging = true;
            }
        }
        else // 마우스 버튼을 뗄 때
        {
            m_isDragging = false;
            m_draggedObject = null;
        }
    }

    // Shift 키 상태 업데이트
    public void OnShift(InputValue value)
    {
        m_shiftPressed = value.isPressed;
    }

    // 우클릭 이벤트 처리
    public void OnRightClick(InputValue value)
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

    private void OnDrawGizmos()
    {
        if (m_camera == null) return;

        // 현재 카메라가 바라보는 방향으로 선 그리기
        Gizmos.color = Color.red;
        Gizmos.DrawRay(m_camera.transform.position, m_camera.transform.forward * 100f);
    }
}
