using Unity.Netcode;
using UnityEngine;

public class TestDondon : NetworkBehaviour
{
    public static TestDondon Instance { get; private set; }

    public static void DontDestroyOnLoad()
    {

    }
    private void Awake()
    {

        if (Instance == null)
        {
            Debug.Log($"[Singleton]생성됨, DontDestroyOnLoad 적용");
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log($"[Singleton]이미 존재하므로 삭제됨");
            Destroy(gameObject);
            return;
        }
    }

    //public override void OnNetworkSpawn()
    //{
    //    base.OnNetworkSpawn();

    //    if (IsServer)
    //    {
    //        NetworkManager.SceneManager.OnSceneEvent += SceneChanged;
    //    }
    //}

    //private void SceneChanged(SceneEvent sceneEvent)
    //{
    //    if (sceneEvent.SceneEventType == SceneEventType.LoadComplete)
    //    {
    //        DontDestroyOnLoad(gameObject);
    //    }
    //}

    //private void OnDestroy()
    //{
    //    if (IsServer)
    //    {
    //        NetworkManager.SceneManager.OnSceneEvent -= SceneChanged;
    //    }
    //}
}
