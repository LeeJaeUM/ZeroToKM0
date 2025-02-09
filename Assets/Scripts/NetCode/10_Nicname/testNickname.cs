using Unity.Netcode;
using UnityEngine;

public class testNickname : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int i = (int)NetworkManager.Singleton.LocalClientId;
            Debug.Log($"{i}");
            GameManager.Instance.SetIconName();
        }
    }
}
