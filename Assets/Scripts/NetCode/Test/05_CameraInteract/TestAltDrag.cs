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
                                  
    private LayerMask layerMask;// 레이캐스트를 수행할 때 해당 레이어를 제외한 모든 레이어를 대상으로 할 LayerMask를 설정합니다.

    //if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) 레이어 마스크를 적용하는 예시

    // Alt 입력 ( Alt를 한번 누르면 드래그 기능 켜짐, 다시 누르면 꺼짐 )
    public void OnAlt(InputValue value)
    {
        if (value.isPressed)
        {
            StartAltDragging(); // 드래그 시작
        }
        else
        {
            StopAltDragging(); // 드래그 중단
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
    // 드래그 시작 처리
    private void StartAltDragging()
    {
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        m_draggedObjects.Clear(); // 기존 드래그 리스트 초기화
        m_draggedNetworkMoves.Clear();

        bool isFirstCard = true;
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
                    networkMove.SetGravity(false); // 중력 비활성화
                    Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                    if(rb != null)
                    {
                        rb.isKinematic = true;
                    }
                }
            }
            Card card = hit.transform.GetComponent<Card>();
            if (card != null)
            {
                if (isFirstCard)
                {
                    card.m_isOnCard = false;
                }
                else
                {
                    card.m_isOnCard = true;
                }
                card.m_isPlaced = false;                
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
    private void StopAltDragging()
    {
        m_isDragging = false;

        // 드래그된 오브젝트들의 위치를 서버/클라이언트에서 업데이트
        foreach (var networkMove in m_draggedNetworkMoves)
        {
            networkMove.IsMove(false); // 움직임 비활성화
            networkMove.SetGravity(true); // 중력 활성화
        }

        foreach (var draggedObject in m_draggedObjects)
        {
            Rigidbody rb = draggedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // 물리 계산을 다시 활성화
            }
        }

        m_draggedObjects.Clear(); // 드래그된 오브젝트 리스트 초기화
        m_draggedNetworkMoves.Clear();
    }
    private void UpdateDraggedObjectsPosition()
    {
        // 마우스의 화면 위치를 월드 공간으로 변환
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        // 마우스가 평면에 닿는 지점 계산 (y 값을 2로 고정)
        Plane dragPlane = new Plane(Vector3.up, new Vector3(0, 2, 0));  // 평면을 Y=2로 설정
        float distance;
        if (dragPlane.Raycast(ray, out distance))
        {
            Vector3 newPosition = ray.GetPoint(distance); // 레이캐스트가 맞은 월드 좌표

            // Y 값 고정 (기존 Y값을 고정시킴)
            newPosition.y = 2f;  // Y 값 고정 (필요시 다른 값으로 수정 가능)

            // 서버에서는 위치를 직접 업데이트
            if (IsServer)
            {
                foreach (var draggedObject in m_draggedObjects)
                {
                    draggedObject.position = newPosition; // 오브젝트의 위치 업데이트
                }
            }
            else if (IsClient)
            {
                // 클라이언트에서는 서버로 위치 업데이트 요청
                foreach (var networkMove in m_draggedNetworkMoves)
                {
                    networkMove.RequestMoveServerRpc(newPosition); // 서버로 위치 전송
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
