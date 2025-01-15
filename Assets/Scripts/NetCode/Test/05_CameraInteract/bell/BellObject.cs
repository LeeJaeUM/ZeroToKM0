using Unity.Netcode;
using UnityEngine;

public class BellObject : MonoBehaviour
{
    public void ClickedBell()
    {
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        GameManager.Instance.RingBell((int)clientId);
    }
}
