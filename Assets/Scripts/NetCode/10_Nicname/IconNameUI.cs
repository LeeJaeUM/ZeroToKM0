using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System;

public class IconNameUI : NetworkBehaviour
{
    #region Contants and Fields
    [SerializeField] Image[] m_userIcon;              // Icon Image
    [SerializeField] TextMeshProUGUI[] m_userName;    // User Name Text

    [SerializeField] Sprite[] m_SpritesIcon;
    #endregion

    #region Public Methods and Operators
    public void SetUserInfo(ulong playerNum)
    {
        if(GameManager.Instance.isTest)
        {
            //테스트용
            int testInt = (int)NetworkManager.Singleton.LocalClientId;
            SetInGameUserInfo($"{testInt} : player", testInt, (int)playerNum);
        }
        else
        {
            Debug.Log($"{playerNum} 이 닉네임 세팅을 함");
            FBManager._instance.UserInfoLoad(() =>
            {
                SetInGameUserInfo(FBManager._instance.m_name,
                            FBManager._instance.m_icon,
                            (int)playerNum);
            });
        }

    }
    #endregion
    private void SetInGameUserInfo(string userName, int icon, int playerNum)
    {
        if(IsServer)
        {
            OnClientSetInGameUserInfo(userName, icon, playerNum);
            SetIconNameClientRpc(userName, icon, playerNum);
        }
        else if (IsClient && !IsHost)
        {
            RequestSetIconNameServerRpc(userName, icon, playerNum);
        }
    }
    private void OnClientSetInGameUserInfo(string userName, int icon, int playerNum)
    {
        // 아이콘 & 테두리 변경
        if (m_userIcon != null) m_userIcon[playerNum].sprite = m_SpritesIcon[icon];

        // 텍스트 변경
        if (m_userName != null)
        {
            m_userName[playerNum].text = userName;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestSetIconNameServerRpc(string userName, int icon, int playerNum)
    {
        OnClientSetInGameUserInfo(userName, icon, playerNum);
        SetIconNameClientRpc(userName, icon, playerNum);
    }

    [ClientRpc]
    public void SetIconNameClientRpc(string userName, int icon, int playerNum)
    {
        OnClientSetInGameUserInfo(userName, icon, playerNum);
    }

    private void OnReadyGame(InputAction.CallbackContext context)
    {
        Debug.Log("눌렸나?");
        SetUserInfo(NetworkManager.Singleton.LocalClientId);
    }

    void Awake()
    {
        m_userIcon = GetComponentsInChildren<Image>();
        m_userName = GetComponentsInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (IsServer)
        {
            // NetworkManager.Singleton.OnClientConnectedCallback += SetUserInfo;
        }
    }


    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsServer)
        {
            //NetworkManager.Singleton.OnClientConnectedCallback -= SetUserInfo;
        }
    }
}
