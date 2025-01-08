using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectDrag : MonoBehaviour
{
    private Camera m_camera;
    private bool m_isDragging = false;
    private bool m_shiftPressed = false; // Shift 키 상태
    private Vector3 m_offset;
    private Transform m_draggedObject;

    void Start()
    {
        m_camera = GetComponent<Camera>();  
    }

    void Update()
    {
        if (m_isDragging)
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

    public void OnShift(InputValue value)
    {
        // Shift 키 상태 업데이트
        m_shiftPressed = value.isPressed;
    }

    private void OnDrawGizmos()
    {
        if (m_camera == null) return;

        // 1. 현재 카메라가 바라보는 방향으로 선 그리기
        Gizmos.color = Color.red;
        Gizmos.DrawRay(m_camera.transform.position, m_camera.transform.forward * 100f);


    //// 2. 카메라 앞 5 단위 거리에 위치한 평면에 마우스 포인터 위치의 구 그리기
    //Plane cameraPlane = new Plane(m_camera.transform.forward, m_camera.transform.position + m_camera.transform.forward * 5);
    //Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());

    //if (cameraPlane.Raycast(ray, out float distance))
    //{
    //    Vector3 pointOnPlane = ray.GetPoint(distance);

    //    // 3. 드래그 상태에 따른 구 색상 변경
    //    Gizmos.color = m_isDragging ? Color.red : Color.green;
    //    Gizmos.DrawSphere(pointOnPlane, 0.2f);
    //
    }
}


