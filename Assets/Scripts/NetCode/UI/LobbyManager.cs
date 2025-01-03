using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // TextMeshProUGUI 사용을 위한 네임스페이스 추가
using System.Collections.Generic;

public class LobbyManager : NetworkBehaviour
{
    [Header("UI Elements")]
    public Button createRoomButton;
    public TextMeshProUGUI roomListText;  // TextMeshProUGUI로 변경

    private List<string> roomList = new List<string>();

    private void Start()
    {
        if (IsHost) // 호스트일 때만 방을 생성할 수 있음
        {
            createRoomButton.onClick.AddListener(CreateRoom);
        }
    }

    // 세션 생성 (ServerRpc)
    [ServerRpc]
    private void CreateRoomServerRpc(ServerRpcParams rpcParams = default)
    {
        // 방을 만들고 이를 클라이언트에 전파
        string roomName = "Room_" + NetworkManager.Singleton.LocalClientId;
        roomList.Add(roomName);

        // 방 정보 클라이언트로 전달
        UpdateRoomListClientRpc();
    }

    // 클라이언트에서 방 목록 갱신 (ClientRpc)
    [ClientRpc]
    private void UpdateRoomListClientRpc()
    {
        roomListText.text = "Available Rooms:\n";
        foreach (var room in roomList)
        {
            roomListText.text += room + "\n";
        }
    }

    // 클라이언트에서 방 생성 버튼 클릭 시 호출 (UI에서 호출)
    private void CreateRoom()
    {
        if (IsOwner) // 자신이 소유자일 때만
        {
            CreateRoomServerRpc();
        }
    }
}
