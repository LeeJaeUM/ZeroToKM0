using Unity.Netcode;
using UnityEngine;

public class ObjectNameBasedOnOwner : NetworkBehaviour
{
    private void Start()
    {
        if (IsOwner)
        {
            // OwnerClientId를 가져와서 이름을 설정
            SetObjectNameBasedOnOwnerServerRpc();
        }
    }

    // ServerRpc로 서버에게 이름 동기화 요청
    [ServerRpc]
    private void SetObjectNameBasedOnOwnerServerRpc(ServerRpcParams rpcParams = default)
    {        
        // 서버에서 자신의 이름을 변경
        gameObject.name = "Player_" + NetworkObject.OwnerClientId.ToString();
        // 서버는 모든 클라이언트에게 이름을 동기화하도록 요청
        SetObjectNameBasedOnOwnerClientRpc(NetworkObject.OwnerClientId);
    }

    // ClientRpc로 모든 클라이언트에게 이름을 변경하도록 전송
    [ClientRpc]
    private void SetObjectNameBasedOnOwnerClientRpc(ulong ownerId)
    {
        // 모든 클라이언트에서 이름을 갱신
        gameObject.name = "Player_" + ownerId.ToString();
    }
}
