using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestAltDrag : NetworkBehaviour
{
    private Camera m_camera; // 메인 카메라 참조
    private List<Transform> m_draggedObjects = new List<Transform>(); // 드래그 중인 오브젝트들 리스트
    private List<NetworkMove> m_draggedNetworkMoves = new List<NetworkMove>(); // 드래그 중인 오브젝트들의 NetworkMove 컴포넌트들
    private bool m_isDragging = false; // 드래그 상태 플래그
    [SerializeField] private float m_dragRadius = 5f; // 드래그 가능한 범위
    private Vector3 m_dragOffset = Vector3.zero; // 드래그 시작 위치에 대한 오프셋

    private bool m_isAlt = false; // Alt 키 상태
                                  
    private LayerMask layerMask;// 레이캐스트를 수행할 때 해당 레이어를 제외한 모든 레이어를 대상으로 할 LayerMask를 설정합니다.

            //if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) 레이어 마스크를 적용하는 예시

    // Alt 입력 ( Alt를 한번 누르면 드래그 기능 켜짐, 다시 누르면 꺼짐 )
    public void OnAlt(InputValue value)
    {
        if (value.isPressed)
        {
            SetIsAlt(); // Alt 키를 눌렀을 때 드래그 모드 토글
            Debug.Log("현재 alt : " + m_isAlt);
        }
    }

    // 마우스 클릭 이벤트 처리
    public void OnClick(InputValue value)
    {
        if (m_isAlt) // Alt가 켜져있을 때만 드래그 가능
        {
            if (value.isPressed)
            {
                StartDragging(); // 드래그 시작
            }
            else
            {
                StopDragging(); // 드래그 중단
            }
        }
    }

    // 마우스 이동 이벤트 처리
    public void OnMouseMove(InputValue value)
    {
        if (m_isDragging)
        {
            UpdateDraggedObjectsPosition(); // 드래그 중인 오브젝트들 위치 업데이트
        }
    }

    // alt를 껐다 켰다 하는 함수
    private void SetIsAlt()
    {
        m_isAlt = !m_isAlt;
    }

    // 드래그 시작 처리
    private void StartDragging()
    {
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        m_draggedObjects.Clear(); // 기존 드래그 리스트 초기화
        m_draggedNetworkMoves.Clear();

        foreach (RaycastHit hit in hits)
        {
            // Raycast가 맞은 지점이 일정 범위 내에 있고, NetworkMove 컴포넌트를 가지고 있는 경우
            if (Vector3.Distance(ray.origin, hit.point) <= m_dragRadius)
            {
                var networkMove = hit.transform.GetComponent<NetworkMove>();
                if (networkMove != null)
                {
                    m_draggedObjects.Add(hit.transform); // 드래그 가능한 오브젝트 추가
                    m_draggedNetworkMoves.Add(networkMove);
                }
            }
        }

        if (m_draggedObjects.Count > 0)
        {
            // 드래그 시작 시 첫 번째 오브젝트와의 위치 차이를 계산 (오프셋)
            m_dragOffset = m_draggedObjects[0].position - ray.origin;

            m_isDragging = true;
        }
    }

    // 드래그 중단 처리
    private void StopDragging()
    {
        m_isDragging = false;

        // 드래그된 오브젝트들의 위치를 서버/클라이언트에서 업데이트
        foreach (var networkMove in m_draggedNetworkMoves)
        {
            networkMove.IsMove(false); // 움직임 비활성화
            networkMove.SetGravity(true); // 중력 활성화
        }

        m_draggedObjects.Clear(); // 드래그된 오브젝트 리스트 초기화
        m_draggedNetworkMoves.Clear();
    }
    // 드래그 중인 오브젝트들의 위치 업데이트
    private void UpdateDraggedObjectsPosition()
    {
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Vector3 newPosition = ray.origin + m_dragOffset; // 마우스 위치 + 오프셋 계산

        // y값은 2로 고정
        newPosition.y = 2f;

        if (IsServer)
        {
            foreach (var draggedObject in m_draggedObjects)
            {
                draggedObject.position = newPosition; // 서버에서 드래그된 오브젝트 위치 업데이트
            }
        }
        else if (IsClient && !IsHost)
        {
            foreach (var networkMove in m_draggedNetworkMoves)
            {
                // 클라이언트에서 서버로 위치 업데이트 요청
                networkMove.RequestMoveServerRpc(newPosition);
            }
        }
    }
    //// 드래그 중인 오브젝트들의 위치 업데이트
    //private void UpdateDraggedObjectsPosition()
    //{
    //    Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
    //    Vector3 newPosition = ray.origin + m_dragOffset; // 드래그 위치 업데이트

    //    if (IsServer)
    //    {
    //        foreach (var draggedObject in m_draggedObjects)
    //        {
    //            draggedObject.position = newPosition; // 서버에서 드래그된 오브젝트 위치 업데이트
    //        }
    //    }
    //    else if (IsClient && !IsHost)
    //    {
    //        foreach (var networkMove in m_draggedNetworkMoves)
    //        {
    //            // 클라이언트에서 서버로 위치 업데이트 요청
    //            networkMove.RequestMoveServerRpc(newPosition);
    //        }
    //    }
    //}

    private void Start()
    {
        m_camera = GetComponent<Camera>(); // 메인 카메라 초기화        
        // NotMoveObject 레이어의 인덱스를 가져옵니다. (Unity 에디터에서 레이어 이름을 확인할 수 있습니다)
        int notMoveObjectLayer = LayerMask.NameToLayer("NotMoveObject");
        layerMask = ~(1 << notMoveObjectLayer);// ~ (bitwise NOT)을 사용하여 해당 레이어를 제외
    }
}
