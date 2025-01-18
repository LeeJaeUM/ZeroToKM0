//public class RelayLobbyUI : MonoBehaviour
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelayLobbyUI : MonoBehaviour
{
    public RelayLobbyManager relayLobbyManager;
    public TMP_InputField lobbyNameInputField; // 방 이름 입력
    public Button createLobbyButton;
    public Button refreshLobbyListButton;
    public Transform lobbyListParent; // 로비 목록 부모 오브젝트
    public GameObject lobbyListItemPrefab; // 로비 아이템 Prefab

    private void Start()
    {
        createLobbyButton.onClick.AddListener(() => CreateLobby());
        refreshLobbyListButton.onClick.AddListener(() => RefreshLobbyList());
    }

    private async void CreateLobby()
    {
        string lobbyName = lobbyNameInputField.text;
        if (!string.IsNullOrEmpty(lobbyName))
        {
            await relayLobbyManager.CreateLobby(lobbyName);
        }
    }

    private async void RefreshLobbyList()
    {
        // 1. 로비 목록 가져오기
        var lobbies = await relayLobbyManager.GetLobbies();

        // 2. 기존 로비 목록 UI 삭제
        foreach (Transform child in lobbyListParent)
        {
            Destroy(child.gameObject);
        }

        // 3. 새로운 로비 목록을 UI에 표시
        foreach (var lobby in lobbies)
        {
            // 3-1. 로비 아이템 생성
            var listItem = Instantiate(lobbyListItemPrefab, lobbyListParent);

            // 3-2. 로비 이름과 인원수를 UI 텍스트에 설정
            listItem.GetComponentInChildren<TextMeshProUGUI>().text = $"{lobby.Name} ({lobby.Players.Count}/{lobby.MaxPlayers})";

            // 3-3. 버튼 클릭 시 해당 로비에 참가하도록 이벤트 설정
            listItem.GetComponent<Button>().onClick.AddListener(async () =>
            {
                await relayLobbyManager.JoinLobby(lobby.Id);
            });
        }
    }

}
