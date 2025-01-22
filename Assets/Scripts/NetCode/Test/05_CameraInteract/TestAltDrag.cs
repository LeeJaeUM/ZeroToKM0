using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestAltDrag : NetworkBehaviour
{
    private Camera m_camera; // 메인 카메라 참조
    private List<Transform> m_draggedObjects = new List<Transform>(); // 드래그 중인 오브젝트들 리스트
    private List<NetworkMove> m_draggedNetworkMoves = new List<NetworkMove>(); // 드래그 중인 오브젝트들의 NetworkMove 컴포넌트들
    private List<Card> m_draggedCards = new List<Card>();
    private bool m_isDragging = false; // 드래그 상태 플래그
    [SerializeField] private float m_dragRadius = 5f; // 드래그 가능한 범위
    private Vector3 m_dragUpYPos = new Vector3(0, 1.5f, 0);
    private List<Vector3> m_newDraggedPos = new List<Vector3>();
    private float m_cardSpacing = 0.2f;
    private LayerMask layerMask;// 레이캐스트를 수행할 때 해당 레이어를 제외한 모든 레이어를 대상으로 할 LayerMask를 설정합니다.
    //if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) 레이어 마스크를 적용하는 예시
    Card m_firstCard;
    // Alt 입력
    MousePositionSync m_mousePos;
    public void OnAlt(InputValue value)
    {
        if (value.isPressed && !m_isDragging)
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
                    //networkMove.SetGravity(false); // 중력 비활성화
                    Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                    if(rb != null)
                    {
                        rb.isKinematic = true;
                    }
                }
                Card card = hit.transform.GetComponent<Card>();
                if(card != null)
                {
                    card.m_isPlaced = true;
                    m_draggedCards.Add(card);
                }
            }
        }
        // m_draggedNetworkMoves안에 오브젝트들을 y축 기준으로 내림차순 정렬해줌( y축 값이 클수록 앞으로오게)
        m_draggedObjects.Sort((obj1, obj2) => obj2.gameObject.transform.position.y.CompareTo(obj1.gameObject.transform.position.y));
        foreach(Transform obj in m_draggedObjects)
        {
            Vector3 UpPos = obj.position + m_dragUpYPos;
            m_newDraggedPos.Add(UpPos);
        }
        if (m_draggedObjects.Count > 0)
        {
            m_isDragging = true;
        }
        m_draggedCards.Sort((obj1, obj2) => obj2.gameObject.transform.position.y.CompareTo(obj1.gameObject.transform.position.y));

        m_firstCard = m_draggedCards[m_draggedCards.Count - 1];
    }

    // 드래그 중단 처리
    private void StopAltDragging()
    {
        m_isDragging = false;

        // 드래그된 오브젝트들의 위치를 서버/클라이언트에서 업데이트
        foreach (var networkMove in m_draggedNetworkMoves)
        {
            networkMove.IsMove(false); // 움직임 비활성화
            //networkMove.SetGravity(true); // 중력 활성화
        }

        foreach (var draggedObject in m_draggedObjects)
        {
            Rigidbody rb = draggedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // 물리 계산을 다시 활성화
            }
        }
        Card card = BoxCastDown(m_firstCard.transform.position);
        if (card != null)
        {
            float cardSpacing = 0.01f;

            for (int i = 0; i < m_draggedCards.Count; i++)
            {
                Vector3 newPos = card.transform.position;
                newPos.y += cardSpacing * (m_draggedCards.Count - i);
                m_draggedCards[i].transform.position = newPos;
                m_draggedCards[i].transform.rotation = m_firstCard.transform.rotation;
            }
        }
        m_draggedCards.Clear();
        m_draggedObjects.Clear(); // 드래그된 오브젝트 리스트 초기화
        m_draggedNetworkMoves.Clear();
        m_newDraggedPos.Clear();
    }
    private void UpdateDraggedObjectsPosition()
    {
        // 마우스의 화면 위치를 월드 공간으로 변환
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        // 마우스가 평면에 닿는 지점 계산 (y 값을 2로 고정)
        Plane dragPlane = new Plane(Vector3.up, new Vector3(0, 2, 0));  // 평면을 Y=2로 설정
        float distance;
        float cardGap = 0.01f;
        if (dragPlane.Raycast(ray, out distance))
        {
            Vector3 newPosition = ray.GetPoint(distance); // 레이캐스트가 맞은 월드 좌표

            // 서버에서는 위치를 직접 업데이트
            if (IsServer)
            {
                for(int i = 0; i < m_draggedObjects.Count; i++)
                {
                    Vector3 newUpPosition = newPosition;
                    newUpPosition.y = m_newDraggedPos[i].y + cardGap * (m_draggedObjects.Count - i);
                    m_draggedObjects[i].position = newUpPosition; // 오브젝트의 위치 업데이트
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
    public Card BoxCastDown(Vector3 from)
    {
        // 아래로 쏘는 방향 (Vector3.down)
        Vector3 direction = Vector3.down;
        
        // 박스의 크기
        Vector3 boxHalfExtents = new Vector3(0.2f, 0.005f, 0.3f);
        // 박스 캐스트의 최대 거리
        float maxDistance = 10f;

        // 충돌 정보 저장 변수
        RaycastHit hit;

        // BoxCast를 수행
        if (Physics.BoxCast(from, boxHalfExtents, direction, out hit, Quaternion.identity, maxDistance))
        {
            Debug.Log("Hit object: " + hit.collider.name);

            // 박스 캐스트 가시화 (디버그 박스 그리기)
            Debug.DrawRay(from, direction * maxDistance, Color.red);
            Debug.DrawLine(from, hit.point, Color.red);

            // 충돌한 오브젝트에서 Card 컴포넌트 가져오기
            Card card = hit.transform.GetComponent<Card>();
            if (card != null)
            {
                return card;
            }
            else
            {
                return null;
            }
        }
        else
        {
            // 충돌이 없을 경우 가시화
            Debug.DrawRay(from, direction * maxDistance, Color.green);
            return null;
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
