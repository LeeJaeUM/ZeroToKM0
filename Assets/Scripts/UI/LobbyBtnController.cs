using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyBtnController : MonoBehaviour
{
    // 이동할 씬 이름
    private const string TargetSceneName = "04_OtherGames";

    /// <summary>
    /// Join 버튼 클릭 시 04_OtherGames 씬으로 전환 후 클라이언트 시작
    /// </summary>
    public void OnJoinBtnClick()
    {
        Debug.Log("Join Button Clicked - Switching to Client Mode");

        // 씬 로드 후 클라이언트 시작
        SceneManager.LoadScene(TargetSceneName);
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name == TargetSceneName)
            {
                StartClient();
            }
        };
    }

    /// <summary>
    /// Create 버튼 클릭 시 04_OtherGames 씬으로 전환 후 호스트 시작
    /// </summary>
    public void OnCreateBtnClick()
    {
        Debug.Log("Create Button Clicked - Switching to Host Mode");

        // 씬 로드 후 호스트 시작
        SceneManager.LoadScene(TargetSceneName);
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name == TargetSceneName)
            {
                StartHost();
            }
        };
    }

    /// <summary>
    /// 클라이언트 시작
    /// </summary>
    private void StartClient()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client started successfully.");
        }
        else
        {
            Debug.LogError("NetworkManager is not set up in the scene.");
        }
    }

    /// <summary>
    /// 호스트 시작
    /// </summary>
    private void StartHost()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Host started successfully.");
        }
        else
        {
            Debug.LogError("NetworkManager is not set up in the scene.");
        }
    }


    /// <summary>
    /// Power 버튼 클릭 시 게임 종료
    /// </summary>
    public void OnPowerBtnClick()
    {
        Debug.Log("Exiting the game");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
