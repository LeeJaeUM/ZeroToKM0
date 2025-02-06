//using System.Collections;
//using Unity.Netcode;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class MousePositionSync : NetworkBehaviour
//{
//    public PlayerInput playerInput; // 플레이어 입력
//    public GameObject m_markerPrefab;  // 마우스 위치에 표시할 게임 오브젝트 프리팹
//    private GameObject m_markerInstance; // 게임 오브젝트 인스턴스
//    private Camera m_playerCamera; // 플레이어 카메라
//    public float m_distanceCameraToMarker = 5f; // 마커와 카메라 사이의 거리
//    private float m_maxRayDistance = 20f; // Ray 길이 제한

//    private float m_waitingTime = 0.1f; // 마커 동기화 대기 시간
//    private float m_nextSyncTime = 0f; // 다음 동기화 가능 시간
//    private float moveDuration = 0.1f; // 부드럽게 이동하는 데 걸리는 시간
//    private Vector3 m_targetPosition = Vector3.zero; // 마커 위치

//    public void OnMouseMove(InputValue value)
//    {
//        if (IsOwner) // 자신의 클라이언트에서만 처리
//        {
//            // 1. 현재 마우스 위치를 화면에서 얻기
//            Vector2 mousePosition = Mouse.current.position.ReadValue();

//            // 2. 마우스 위치에서 Ray 발사
//            Ray ray = m_playerCamera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0));

//            if (Physics.Raycast(ray, out RaycastHit hitInfo, m_maxRayDistance))
//            {
//                // 3. Ray가 물체에 닿았을 경우, 닿은 지점 사용
//                Vector3 pointOnPlane = hitInfo.point;

//                MarkerPositionUpdate(pointOnPlane);

//            }
//            else
//            {
//                // Ray가 물체에 닿지 않았을 경우, 카메라의 앞 방향 5 단위 거리 평면을 사용
//                Plane cameraPlane = new Plane(m_playerCamera.transform.forward, m_playerCamera.transform.position + m_playerCamera.transform.forward * m_distanceCameraToMarker);

//                if (cameraPlane.Raycast(ray, out float distance) && distance <= m_maxRayDistance)
//                {
//                    Vector3 pointOnPlane = ray.GetPoint(distance); // Ray와 평면의 교차점

//                    MarkerPositionUpdate(pointOnPlane);
//                }
//            }
//        }
//    }
//    /// <summary>
//    /// 마커 오브젝트 위치 업데이트
//    /// </summary>
//    /// <param name="pointOnPlane">입력받은 새로운 위치</param>
//    private void MarkerPositionUpdate(Vector3 pointOnPlane)
//    {
//        // 1. 로컬 마커 위치는 매 프레임 업데이트
//        if (m_markerInstance != null)
//        {
//            Debug.Log("마커 위치 업데이트");
//            m_markerInstance.transform.position = pointOnPlane; // 위치를 정확하게 맞추기
//        }
//        if (IsHost)
//        {
//            Debug.Log("마커 위치 업데이트 호스트");
//            // 호스트 -> 클라이언트로 동기화
//            UpdateMousePositionClientRpc(pointOnPlane);
//        }
//        else if (!IsHost && IsClient)
//        {
//            // 클라이언트 -> 서버로 동기화
//            UpdateMousePositionServerRpc(pointOnPlane);
//        }

//        // 2. 네트워크 통신은 제한된 주기로만 실행
//        //if (Time.time >= m_nextSyncTime)
//        //{
//        //    m_nextSyncTime = Time.time + m_waitingTime; // 다음 호출 시간 갱신


//        //}
//    }

//    /// <summary>
//    /// 서버 RPC: 서버가 호출하여 모든 클라이언트에게 마우스 위치 전달
//    /// </summary>
//    /// <param name="mousePosition">업데이트 할 새로운 위치</param>
//    [ServerRpc(RequireOwnership = false)] // 소유권을 필요로 하지 않음
//    private void UpdateMousePositionServerRpc(Vector3 mousePosition)
//    {
//        // 다른 클라이언트에게 마우스 위치 전송 (ClientRpc)
//        UpdateMousePositionClientRpc(mousePosition);
//    }

//    /// <summary>
//    /// 클라이언트 RPC: 마우스 위치를 받은 클라이언트에서 처리
//    /// </summary>
//    /// <param name="mousePosition">업데이트 할 새로운 위치</param>
//    [ClientRpc]
//    public void UpdateMousePositionClientRpc(Vector3 mousePosition)
//    {
//        // 마커 오브젝트가 없는 경우 생성
//        if (m_markerInstance == null)
//        {
//            m_markerInstance = Instantiate(m_markerPrefab);
//        }

//        // 클라이언트에서 마우스 위치 업데이트
//        if (m_markerInstance != null)
//        {
//            // 새 코루틴 실행
//            SetTargetPosition(mousePosition);
//        }
//    }

//    /// <summary>
//    /// 입력받은 위치로 부드럽게 이동
//    /// </summary>
//    /// <param name="targetPosition">새 위치</param>
//    /// <returns></returns>
//    private IEnumerator SmoothMoveToPosition(Vector3 targetPosition)
//    {
//        Vector3 startPosition = m_markerInstance.transform.position;
//        float elapsedTime = 0f;

//        while (elapsedTime < moveDuration)
//        {
//            elapsedTime += Time.deltaTime;
//            float t = elapsedTime / moveDuration;
//            t = Mathf.SmoothStep(0, 1, t); // 부드러운 곡선 비율 계산 (ease-in/out)

//            m_markerInstance.transform.position = Vector3.Lerp(startPosition, targetPosition, t); // 보간

//            yield return null; // 한 프레임 대기
//        }

//        // 목표 위치에 정확히 도달
//        m_markerInstance.transform.position = targetPosition;
//    }

//    /// <summary>
//    /// 새 위치를 설정할 때 이전과 같은 값이라면 실행하지 않는 함수
//    /// </summary>
//    /// <param name="targetPosition"></param>
//    private void SetTargetPosition(Vector3 targetPosition)
//    {
//        if(targetPosition != m_targetPosition)
//        {
//            m_targetPosition = targetPosition;
//            StartCoroutine(SmoothMoveToPosition(m_targetPosition));
//        }
//    }
//    private void Start()
//    {
//        // 네트워크 소유자에 해당하는 클라이언트에서만 마커를 생성
//        if (IsOwner)
//        {
//            m_markerInstance = Instantiate(m_markerPrefab);
//            m_playerCamera = GetComponent<Camera>();
//        }
//        //m_markerInstance = Instantiate(m_markerPrefab);
//        //m_playerCamera = GetComponent<Camera>();
//    }

//    private void OnEnable()
//    {
//        playerInput = GetComponent<PlayerInput>();
//    }
//}
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class MousePositionSync : NetworkBehaviour
{
    public GameObject m_markerPrefab;  // 마우스 위치에 표시할 게임 오브젝트 프리팹
    private GameObject m_markerInstance; // 게임 오브젝트 인스턴스
    private Camera m_playerCamera; // 플레이어 카메라
    public float m_distanceCameraToMarker = 5f; // 마커와 카메라 사이의 거리

    private void Start()
    {
        // 네트워크 소유자에 해당하는 클라이언트에서만 마커를 생성
        if (IsOwner)
        {
            m_markerInstance = Instantiate(m_markerPrefab);
            m_markerInstance.SetActive(false); // 처음에는 비활성화
            m_playerCamera = GetComponent<Camera>();
        }
    }

    private void Update()
    {
        if (IsOwner) // 자신의 클라이언트에서만 처리
        {
            // 1. 현재 마우스 위치를 화면에서 얻기
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            // 2. Y축에 평행한 평면을 생성 (y = 특정 값)
            float fixedY = 1.0f; // 원하는 높이 (예: y=1)
            Plane groundPlane = new Plane(Vector3.up, new Vector3(0, fixedY, 0));

            Ray ray = m_playerCamera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0)); // 마우스 위치에서 Ray 발사

            // 3. Ray가 어떤 오브젝트에 닿았는지 확인
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                // 오브젝트에 닿았다면 그 위치로 마커 이동
                if (m_markerInstance != null)
                {
                    m_markerInstance.transform.position = hitInfo.point;
                    m_markerInstance.SetActive(true);
                }

                // 다른 클라이언트에게 마우스 위치를 전송
                UpdateMousePositionServerRpc(hitInfo.point,
                (int)NetworkManager.Singleton.LocalClientId);
            }
            else if (new Plane(Vector3.up, new Vector3(0, fixedY, 0)).Raycast(ray, out float distance))
            {
                // 아무것도 닿지 않았다면, y = fixedY 평면에 마커 배치
                Vector3 pointOnPlane = ray.GetPoint(distance);

                if (m_markerInstance != null)
                {
                    m_markerInstance.transform.position = pointOnPlane;
                    m_markerInstance.SetActive(true);
                }

                // 다른 클라이언트에게 마우스 위치를 전송
                UpdateMousePositionServerRpc(pointOnPlane,
                (int)NetworkManager.Singleton.LocalClientId);
            }
        }
    }

    // 서버 RPC: 서버가 호출하여 모든 클라이언트에게 마우스 위치 전달
    [ServerRpc(RequireOwnership = false)] // 소유권을 필요로 하지 않음
    private void UpdateMousePositionServerRpc(Vector3 mousePosition, int playerNum)
    {
        // 다른 클라이언트에게 마우스 위치 전송 (ClientRpc)
        UpdateMousePositionClientRpc(mousePosition, playerNum); 
        SetRandomColor setRandom = m_markerInstance.GetComponent<SetRandomColor>();
        setRandom.AssignColor(playerNum);
    }

    // 클라이언트 RPC: 마우스 위치를 받은 클라이언트에서 처리
    [ClientRpc]
    public void UpdateMousePositionClientRpc(Vector3 mousePosition, int playerNum)
    {
        // 마커 오브젝트가 없는 경우 생성
        if (m_markerInstance == null)
        {
            m_markerInstance = Instantiate(m_markerPrefab);
            SetRandomColor setRandom = m_markerInstance.GetComponent<SetRandomColor>();
            setRandom.AssignColor(playerNum);
        }

        // 클라이언트에서 마우스 위치 업데이트
        if (m_markerInstance != null)
        {
            m_markerInstance.transform.position = mousePosition; // 정확한 위치로 동기화
            m_markerInstance.SetActive(true); // 마커 활성화
        }
    }
}