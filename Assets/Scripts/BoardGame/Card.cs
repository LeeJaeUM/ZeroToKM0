using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;

public class Card : NetworkBehaviour
{  
    // 위치와 회전을 동기화할 네트워크 변수
    public NetworkVariable<Vector3> m_networkPosition = new NetworkVariable<Vector3>(new Vector3(0, 0, 0));
    public NetworkVariable<Quaternion> m_networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity);

    private bool isMoving = false;

    void Update()
    {
        // 서버에서만 위치와 회전 업데이트 (클라이언트는 읽기만 함)
        if (IsServer)
        {
            // 위치와 회전 값 갱신
            m_networkPosition.Value = transform.position;
            m_networkRotation.Value = transform.rotation;
        }
        else if (IsClient && isMoving)
        {
            TestCardPosChangeServerRpc();
        }
    }

    [ServerRpc]
    void TestCardPosChangeServerRpc()
    {
        m_networkPosition.Value = transform.position;
    }

    // 위치와 회전이 변경되었을 때 클라이언트에서 처리
    private void OnEnable()
    {
        m_networkPosition.OnValueChanged += HandlePositionChanged;
        m_networkRotation.OnValueChanged += HandleRotationChanged;
    }

    private void OnDisable()
    {
        m_networkPosition.OnValueChanged -= HandlePositionChanged;
        m_networkRotation.OnValueChanged -= HandleRotationChanged;
    }

    private void HandlePositionChanged(Vector3 oldPosition, Vector3 newPosition)
    {
        // 위치가 변경되면 클라이언트에서 해당 위치로 이동
        transform.position = newPosition;
    }

    private void HandleRotationChanged(Quaternion oldRotation, Quaternion newRotation)
    {
        // 회전이 변경되면 클라이언트에서 해당 회전으로 회전
        transform.rotation = newRotation;
    }
    public void FlipCard()              // 카드를 뒤집어주는 함수
    {
        transform.Rotate(Vector3.right * 180);
    }
}
