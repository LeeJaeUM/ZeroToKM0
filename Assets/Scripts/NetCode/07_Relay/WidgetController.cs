using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WidgetController : MonoBehaviour
{
    public Image m_background;
    public GameObject m_createSession;
    public GameObject m_showSessionList;
    public GameObject m_joinSessionList;
    public GameObject m_cancelBtn;    
    
    // 이동할 씬 이름
    private const string TargetSceneName = "04_OtherGames";

    /// <summary>
    /// 세션 생성 버튼 클릭
    /// </summary>
    public void CreateSessionClick()
    {
        m_background.enabled = true;
        m_createSession.SetActive(true);
        m_showSessionList.SetActive(false);
        m_joinSessionList.SetActive(false);
        m_cancelBtn.gameObject.SetActive(true);
    }

    /// <summary>
    /// 세션 입장 버튼 클릭
    /// </summary>
    public void JoinSessionClick()
    {
        m_background.enabled = true;
        m_createSession.SetActive(false);
        m_showSessionList.SetActive(true);
        m_joinSessionList.SetActive(true);
        m_cancelBtn.gameObject.SetActive(true);
    }
    public void AllHide()
    {
        m_background.enabled = false;
        m_createSession.SetActive(false);
        m_showSessionList.SetActive(false);
        m_joinSessionList.SetActive(false);
        m_cancelBtn.gameObject.SetActive(false);
    }

    /// <summary>
    /// create, join 버튼 클릭 시 04_OtherGames 씬으로 전환
    /// </summary>
    public void SceneChange()
    {
        SceneManager.LoadScene(TargetSceneName);
    }

    private void Start()
    {
       AllHide();

        //if(m_cancelBtn != null)
        //    m_cancelBtn.onClick.AddListener(AllHide);
    }

}
