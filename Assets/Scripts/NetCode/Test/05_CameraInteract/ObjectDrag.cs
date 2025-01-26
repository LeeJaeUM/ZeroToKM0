using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class ObjectDrag : NetworkBehaviour
{
    private Camera m_camera; // 메인 카메라 참조
    [SerializeField] private Transform m_draggedObject; // 드래그 중인 오브젝트
    private Card m_draggedCard; // 드래그 중인 카드
    private SkewerIngredient m_draggedIngredient;   // 꼬치의 달인의 재료
    private bool m_isDragging = false; // 드래그 상태 플래그
    [SerializeField] private float m_dragUpYPos = 2f; // 오브젝트를 올릴 높이
    private Vector3 m_newDraggedPos = Vector3.zero;

    private NetworkMove m_draggedNetworkMove;
    private float m_waitingTime = 0.2f; // 마커 동기화 대기 시간
    private float m_nextSyncTime = 0.3f; // 다음 동기화 가능 시간

    // 레이캐스트를 수행할 때 해당 레이어를 제외한 모든 레이어를 대상으로 할 LayerMask를 설정합니다.
    private LayerMask layerMask;


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
    /// <summary>
    ///드래그 시작 처리 
    /// </summary>
    private void StartDragging()
    {
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            //현재 드래그 중할 오브젝트의 트랜스폼을 저장
            m_draggedObject = hit.transform;

            // 클릭된 오브젝트를 약간 위로 올림
            m_newDraggedPos = m_draggedObject.position + new Vector3(0, m_dragUpYPos, 0);

            //드래그 가능한 물체인 경우 NetworkMove 컴포넌트를 추가
            m_draggedNetworkMove = m_draggedObject.GetComponent<NetworkMove>();
            if (m_draggedNetworkMove != null)
            {
                if (IsServer)
                {
                    m_draggedNetworkMove.IsMove(true);
                }
                else if (IsClient && !IsHost)
                {
                    m_draggedNetworkMove.RequestIsMovingChangeServerRpc(true);
                }
            }

            // Card 컴포넌트가 있다면 추가 처리
            m_draggedCard = m_draggedObject.GetComponent<Card>();
            if (m_draggedCard != null)
            {
                m_draggedCard.m_isPlaced = false;
            }
            m_isDragging = true;

            // SkewerIngredient 컴포넌트가 있는지 확인용
            m_draggedIngredient = m_draggedObject.GetComponent<SkewerIngredient>();
        }
    }
    /// <summary>
    /// 드래그 중단 처리 
    /// </summary>
    private void StopDragging()
    {
        m_isDragging = false;

        if (m_draggedNetworkMove != null)
        {
            if (IsServer)
            {
                m_draggedNetworkMove.IsMove(false);
            }
            else if (IsClient && !IsHost)
            {
                m_draggedNetworkMove.RequestIsMovingChangeServerRpc(false);
            }
        }

        // Card 컴포넌트 처리
        if (m_draggedCard != null)
        {
            m_draggedCard = null;
        }

        m_draggedObject = null;
        m_newDraggedPos = Vector3.zero;
    }
    /// <summary>
    /// 드래그 중인 오브젝트 위치 업데이트 
    /// </summary>
    private void UpdateDraggedObjectPosition()
    {
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 newPosition = hit.point; //+ m_offset;
            newPosition.y = m_newDraggedPos.y; //y값 고정
            if (IsServer)
            {
                m_draggedObject.position = newPosition;
            }
            else if (!IsHost && IsClient)
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
            // SkewerIngredient 컴포넌트가 있다면 추가 처리
            if (m_draggedIngredient != null)
            {
                // 이 오브젝트가 꼬치에 닿았다면
                if(m_draggedIngredient.m_touchedStick != null)
                {
                    // Dragging을 멈추고 이 오브젝트를 꼬치에 추가
                    StopDragging();
                    m_draggedIngredient.m_touchedStick.AddToSkewerStick(m_draggedIngredient);
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
