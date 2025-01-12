using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class MousePositionSync : NetworkBehaviour
{
    public GameObject m_markerPrefab;  // 마우스 위치에 표시할 게임 오브젝트 프리팹
    private GameObject m_markerInstance; // 게임 오브젝트 인스턴스
    private Camera m_playerCamera; // 플레이어 카메라
    public float m_distanceCameraToMarker = 5f; // 마커와 카메라 사이의 거리
    private float m_maxRayDistance = 30f; // Ray 길이 제한

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

            // 2. 마우스 위치에서 Ray 발사
            Ray ray = m_playerCamera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0));

            if (Physics.Raycast(ray, out RaycastHit hitInfo, m_maxRayDistance))
            {
                // 3. Ray가 물체에 닿았을 경우, 닿은 지점 사용
                Vector3 pointOnPlane = hitInfo.point;

                // 마커 오브젝트 위치 업데이트
                if (m_markerInstance != null)
                {
                    m_markerInstance.transform.position = pointOnPlane; // 위치를 정확하게 맞추기
                    m_markerInstance.SetActive(true); // 마커 활성화
                }

                // 다른 클라이언트에게 마우스 위치를 전송
                UpdateMousePositionServerRpc(pointOnPlane);
            }
            else
            {
                // Ray가 물체에 닿지 않았을 경우, 카메라의 앞 방향 5 단위 거리 평면을 사용
                Plane cameraPlane = new Plane(m_playerCamera.transform.forward, m_playerCamera.transform.position + m_playerCamera.transform.forward * m_distanceCameraToMarker);

                if (cameraPlane.Raycast(ray, out float distance) && distance <= m_maxRayDistance)
                {
                    Vector3 pointOnPlane = ray.GetPoint(distance); // Ray와 평면의 교차점

                    // 마커 오브젝트 위치 업데이트
                    if (m_markerInstance != null)
                    {
                        m_markerInstance.transform.position = pointOnPlane; // 위치를 정확하게 맞추기
                        m_markerInstance.SetActive(true); // 마커 활성화
                    }

                    // 다른 클라이언트에게 마우스 위치를 전송
                    UpdateMousePositionServerRpc(pointOnPlane);
                }
            }
        }

    }

    // 서버 RPC: 서버가 호출하여 모든 클라이언트에게 마우스 위치 전달
    [ServerRpc(RequireOwnership = false)] // 소유권을 필요로 하지 않음
    private void UpdateMousePositionServerRpc(Vector3 mousePosition)
    {
        // 다른 클라이언트에게 마우스 위치 전송 (ClientRpc)
        UpdateMousePositionClientRpc(mousePosition);
    }

    // 클라이언트 RPC: 마우스 위치를 받은 클라이언트에서 처리
    [ClientRpc]
    public void UpdateMousePositionClientRpc(Vector3 mousePosition)
    {
        // 마커 오브젝트가 없는 경우 생성
        if (m_markerInstance == null)
        {
            m_markerInstance = Instantiate(m_markerPrefab);
        }

        // 클라이언트에서 마우스 위치 업데이트
        if (m_markerInstance != null)
        {
            m_markerInstance.transform.position = mousePosition; // 정확한 위치로 동기화
            m_markerInstance.SetActive(true); // 마커 활성화
        }
    }
}
