using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class NetworkMove : NetworkBehaviour
{
    public NetworkVariable<Vector3> m_networkPosition = new NetworkVariable<Vector3>();
    private NetworkRigidbody networkRigidbody;
    private bool isMoving = false;

    public void IsMove(bool canMove)
    {
        isMoving = canMove;
    }

    [ServerRpc(RequireOwnership = false)] // 소유권 없이도 서버에 요청 가능
    public void RequestMoveServerRpc(Vector3 position)
    {
        // 서버에서 위치 변경
        m_networkPosition.Value = position;
        Debug.Log("클라이언트가 드래그중임");
    }

    private void OnEnable()
    {
        m_networkPosition.OnValueChanged += OnPositionChanged;
    }

    private void OnDisable()
    {
        m_networkPosition.OnValueChanged -= OnPositionChanged;
    }

    private void OnPositionChanged(Vector3 oldPosition, Vector3 newPosition)
    {
        if(!isMoving)
        {
            transform.position = newPosition;
        }
    }
    void Start()
    {
        networkRigidbody = GetComponent<NetworkRigidbody>();
    }

    public void SetGravity(bool useGravity)
    {
        networkRigidbody.Rigidbody.useGravity = useGravity;
    }
}
