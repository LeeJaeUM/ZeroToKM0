using Unity.Netcode;
using UnityEngine;

public class BellObject : NetworkBehaviour
{
    public void ClickedBell()
    {
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        if(IsServer)
        {
            GameManager.Instance.RingBell((int)clientId);
        }
        else if(IsClient)
        {
            RequestRingBellServerRpc((int)clientId);
        }
    }


    // 클라이언트에서 서버로 카드 오픈 요청을 보내는 ServerRpc
    [ServerRpc(RequireOwnership = false)]
    public void RequestRingBellServerRpc(int playerNum)
    {
        // 서버에서 벨 누르기 처리
        GameManager.Instance.RingBell(playerNum);
    }
}
