using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnLobbyUI : MonoBehaviour
{
    public Button reLobbyBtn; 
    private string lobbySceneName = "02_Lobby";  // 클라이언트 종료 후 이동할 씬

    private void OnEnable()
    {
        reLobbyBtn = GetComponent<Button>();
        reLobbyBtn.onClick.AddListener(ReLobby);
    }

    void ReLobby()
    {
        SceneManager.LoadScene(lobbySceneName);
    }

}
