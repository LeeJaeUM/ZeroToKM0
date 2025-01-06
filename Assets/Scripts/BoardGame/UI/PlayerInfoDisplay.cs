using UnityEngine;
using TMPro;
using Unity.Netcode;

public class PlayerInfoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerInfoText; // TextMeshProUGUI 연결

    private void Start()
    {
        // 초기화
        UpdatePlayerInfo();

        // 네트워크 이벤트 구독
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        UpdatePlayerInfo();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        UpdatePlayerInfo();
    }

    private void UpdatePlayerInfo()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
        {
            playerInfoText.text = "Server not running.";
            return;
        }

        // 접속된 플레이어 수
        int connectedPlayers = NetworkManager.Singleton.ConnectedClients.Count;

        // 각 플레이어 ID 수집
        string playerIds = "";
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            playerIds += $"Player ID: {client.ClientId}\n";
        }

        // TextMeshProUGUI 업데이트
        playerInfoText.text = $"players connected: {connectedPlayers}\n{playerIds}";
    }
}
