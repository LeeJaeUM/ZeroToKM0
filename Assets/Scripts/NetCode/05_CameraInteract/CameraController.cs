using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    private float m_moveSpeed = 24f;       // 카메라 이동 속도
    private float m_rotationSpeed = 70f; // 카메라 회전 속도

    private Vector2 m_moveInput;        // 이동 입력 값
    private Vector2 m_lookInput;        // 회전 입력 값
    [SerializeField]
    private bool m_isMoveClickHeld;     // 이동버튼 클릭 상태 확인
    private bool m_isRightClickHeld;    // 우클릭 상태 확인

    private float m_yaw = 0f;           // 카메라의 좌우 회전
    private float m_pitch = 0f;         // 카메라의 상하 회전

    private Vector3 m_currentVelocity; // SmoothDamp에서 사용할 현재 속도

    private void HandleMovement()
    {
        if (m_isMoveClickHeld)
        {
            // 카메라가 바라보는 방향을 기준으로 이동
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            forward.Normalize();
            right.Normalize();

            //카메라의 앞 방향을 기준으로 설정
            Vector3 moveDirection = (forward * m_moveInput.y + right * m_moveInput.x);

            // Lerp를 사용하여 일정한 속도로 이동
            Vector3 targetPosition = transform.position + moveDirection * m_moveSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.8f);  // 0.1f는 이동 비율
        }
    }

    private void HandleRotation()
    {
        if (m_isRightClickHeld) // 우클릭 상태일 때만 회전 처리
        {
            // yaw(좌우 회전)과 pitch(상하 회전) 값 계산
            m_yaw += m_lookInput.x * m_rotationSpeed * Time.deltaTime;
            m_pitch -= m_lookInput.y * m_rotationSpeed * Time.deltaTime;

            // pitch 값 제한 (카메라가 너무 위/아래로 향하지 않도록)
            m_pitch = Mathf.Clamp(m_pitch, -90f, 90f);

            // 카메라 회전 적용
            transform.eulerAngles = new Vector3(m_pitch, m_yaw, 0f);
        }
    }

    #region Input System

    // Input System 콜백 함수: 이동 입력
    public void OnMove(InputValue value)
    {
        m_moveInput = value.Get<Vector2>();             // WASD 또는 방향키 입력
        m_isMoveClickHeld = m_moveInput != Vector2.zero;// 입력 값이 (0, 0)이 아니면 이동 중, 아니면 정지 상태
        Debug.Log($"{m_isMoveClickHeld}, moveVec : {m_moveInput.x},{m_moveInput.y}");
    }

    // Input System 콜백 함수: 마우스 회전 입력
    public void OnMouseMove(InputValue value)
    {
        m_lookInput = value.Get<Vector2>(); // 마우스 이동 입력
        //Debug.Log($"마우스 위치 바꾸는중 {m_lookInput.x}, {m_lookInput.y}");
    }

    // Input System 콜백 함수: 우클릭 상태 확인
    public void OnRightClick(InputValue value)
    {
        m_isRightClickHeld = value.isPressed; // 우클릭 여부 확인

    }


    #endregion

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

}
