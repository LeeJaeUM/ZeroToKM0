using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

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
        Debug.Log(playerNum);
        FBManager._instance.UserInfoLoad(() =>
        {
            SetInGameUserInfo(FBManager._instance.m_name,
                        FBManager._instance.m_icon,
                        (int)playerNum);
        });
    }
    #endregion
    private void SetInGameUserInfo(string userName, int icon, int playerNum)
    {
        // 아이콘 & 테두리 변경
        if (m_userIcon != null) m_userIcon[playerNum].sprite = m_SpritesIcon[icon];

        // 텍스트 변경
        if (m_userName != null)
        {
            m_userName[playerNum].text = userName;
        }
    }

    private void Start()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += SetUserInfo;
        }
    }

    private void OnDestroy()
    {
        if (IsOwner)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= SetUserInfo;
        }
    }

    void Awake()
    {
        m_userIcon = GetComponentsInChildren<Image>();
        m_userName = GetComponentsInChildren<TextMeshProUGUI>();
        SetUserInfo(0); //기본은 0으로 초기화
    }
}
