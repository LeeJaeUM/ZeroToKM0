using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class TestDrags : NetworkBehaviour
{
    public TestNetworkMove m_draggedNetworkMove; 
    private Camera m_camera; // 메인 카메라 참조
    private LayerMask layerMask;
    public bool m_isDragging = false; // 드래그 상태 플래그

    private float m_waitingTime = 0.15f; // 마커 동기화 대기 시간
    private float m_nextSyncTime = 0.3f; // 다음 동기화 가능 시간


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
    public void OnMouseMove(InputValue value)
    {
        if (m_isDragging && m_draggedNetworkMove != null)
        {
            UpdateDraggedObjectPosition();
        }
    }
    private void StartDragging()
    {
        m_isDragging=true;
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            m_draggedNetworkMove = hit.transform.GetComponent<TestNetworkMove>();

            //if (m_draggedNetworkMove != null)
            //{
            //    if (IsOwner)
            //    {
            //        m_draggedNetworkMove.IsMove(true);
            //    }
            //    else //if (IsClient && !IsHost)
            //    {
            //        m_draggedNetworkMove.RequestIsMovingChangeServerRpc(true);
            //    }
            //}
        }
    }
    private void StopDragging()
    {
        m_isDragging=false;
    }
    private void UpdateDraggedObjectPosition()
    {
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 newPosition = hit.point; //+ m_offset;
            if (IsOwner)
            {
                if(IsServer && IsHost)
                    m_draggedNetworkMove.PositionChanged(newPosition);
                else
                {
                    if (Time.time >= m_nextSyncTime)
                    {
                        m_nextSyncTime = Time.time + m_waitingTime; // 다음 호출 시간 갱신

                        if (m_draggedNetworkMove != null)
                        {
                            m_draggedNetworkMove.RequestMoveServerRpc(newPosition);
                        }
                    }
                }
            }
        }
    }
    private void Start()
    {
        m_camera = GetComponent<Camera>(); // 메인 카메라 초기화

        // NotMoveObject 레이어의 인덱스를 가져옵니다. (Unity 에디터에서 레이어 이름을 확인할 수 있습니다)
        int notMoveObjectLayer = LayerMask.NameToLayer("NotMoveObject");
        layerMask = ~(1 << notMoveObjectLayer);// ~ (bitwise NOT)을 사용하여 해당 레이어를 제외
    }
}
