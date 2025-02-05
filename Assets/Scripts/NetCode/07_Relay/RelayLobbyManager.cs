//RelayManager
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
///using Unity.Services.Lobby;
///using Unity.Services.Lobby.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;

public class RelayLobbyManager : MonoBehaviour
{
    public static RelayLobbyManager Instance { get; private set; }

    private const int MaxPlayers = 4; // 최대 플레이어 수 (호스트 포함)
    private Lobby currentLobby; // 현재 생성된 로비 정보

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private async void Start()
    {
        await InitializeServices();
    }

    private async Task InitializeServices()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Signed in as {AuthenticationService.Instance.PlayerId}");
            }
        } catch (System.Exception e)
        {
            Debug.LogError($"Failed to initialize services: {e.Message}");
        }
    }

    // 방 생성
    public async Task CreateLobby(string lobbyName)
    {
        try
        {
            // Relay 세션 생성
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayers);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // Lobby 생성
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false, // 공개 방
                Data = new Dictionary<string, DataObject>
                {
                    { "RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode) } // Relay Join Code 저장
                }
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MaxPlayers, options);

            // Relay 설정
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();
            Debug.Log($"Lobby created: {lobbyName}, Relay Join Code: {joinCode}");
        } catch (System.Exception e)
        {
            Debug.LogError($"Failed to create lobby: {e.Message}");
        }
    }

    // 로비 목록 가져오기
    public async Task<List<Lobby>> GetLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0"
                    )
                }
            };

            var response = await LobbyService.Instance.QueryLobbiesAsync(options);
            return response.Results;
        } catch (System.Exception e)
        {
            Debug.LogError($"Failed to get lobbies: {e.Message}");
            return null;
        }
    }

    // 로비 참가
    public async Task JoinLobby(string lobbyId)
    {
        try
        {
            var lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            // Relay 설정
            string joinCode = lobby.Data["RelayJoinCode"].Value;
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
            Debug.Log($"Joined lobby: {lobby.Name}");
        } catch (System.Exception e)
        {
            Debug.LogError($"Failed to join lobby: {e.Message}");
        }
    }
}
