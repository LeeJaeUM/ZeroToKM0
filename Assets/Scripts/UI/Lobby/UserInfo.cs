using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInfo : MonoBehaviour
{
    #region Contants and Fields
    [SerializeField] Image m_userIcon;              // Icon Image
    [SerializeField] Image m_userOutline;           // Outline Image
    [SerializeField] TextMeshProUGUI m_userName;    // User Name Text
    
    // TODO : TEST data. DB data로 변경 필요 (2025.02.03)
    [SerializeField] Sprite m_sampleIcon;
    [SerializeField] Sprite m_sampleOutline;

    [SerializeField] Sprite[] m_SpritesIcon;
    [SerializeField] Sprite[] m_SpritesOutline;
    #endregion

    #region Public Methods and Operators
    /// <summary>
    /// UserInfo 데이터를 세팅
    /// </summary>
    /// <param name="icon">User Icon</param>
    /// <param name="outline">User Outline</param>
    /// <param name="userName">User Nickname</param>
    /// <param name="wins">User 승리 기록</param>
    /// <param name="losses">User 패배 기록</param>
    /// <param name="coin">User Coin</param>
    public void SetUserInfo(string userName, int wins, int losses, int coin, int icon, int outline)
    {
        // 아이콘 & 테두리 변경
        if (m_userIcon != null) m_userIcon.sprite = m_SpritesIcon[icon];
        if (m_userOutline != null) m_userOutline.sprite = m_SpritesOutline[outline];

        // 텍스트 변경
        if (m_userName != null)
        {
            // Lobby Window의 UserInfo 경우
            if(wins < 0)
            {
                m_userName.text = userName;
            }
            // UserInfo Window의 UserInfo 경우
            else
            {
                m_userName.text = $"User Name : {userName}\n\n" +
                                $"Record\u2003\u2002\u2002 : {wins} win {losses} Lose\n\n" +
                                $"Coin\u2003\u2003\u2003\u2002: {coin}";
            }
        }
    }
    #endregion

    void Awake()
    {
        // TODO : TEST data. DB data로 변경 필요 (2025.02.03) get set으로 받기 
        //string playerName = "Player1";
        FBManager._instance.UserInfoLoad(() =>
        {
            Debug.Log($"이름: {FBManager._instance.m_name}");
            Debug.Log($"승리: {FBManager._instance.m_win}");
            Debug.Log($"패배: {FBManager._instance.m_loss}");
            Debug.Log($"코인: {FBManager._instance.m_coin}");
            Debug.Log($"아이콘: {FBManager._instance.m_icon}");
            Debug.Log($"아웃라인: {FBManager._instance.m_outline}");

            SetUserInfo(FBManager._instance.m_name,
                        FBManager._instance.m_win,
                        FBManager._instance.m_loss,
                        FBManager._instance.m_coin,
                        FBManager._instance.m_icon,
                        FBManager._instance.m_outline);
        });

    }
}
