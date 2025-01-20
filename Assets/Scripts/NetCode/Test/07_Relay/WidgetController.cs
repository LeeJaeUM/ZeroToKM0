using UnityEngine;
using UnityEngine.UI;

public class WidgetController : MonoBehaviour
{
    public Image m_background;
    public GameObject m_createSession;
    public GameObject m_showSessionList;
    public GameObject m_joinSessionList;

    public void CreateSessionClick()
    {
        m_background.enabled = true;
        m_createSession.SetActive(true);
        m_showSessionList.SetActive(false);
        m_joinSessionList.SetActive(false);
    }

    public void JoinSessionClick()
    {
        m_background.enabled = true;
        m_createSession.SetActive(false);
        m_showSessionList.SetActive(true);
        m_joinSessionList.SetActive(true);
    }
    public void AllHide()
    {
        m_background.enabled = false;
        m_createSession.SetActive(false);
        m_showSessionList.SetActive(false);
        m_joinSessionList.SetActive(false);
    }

    private void Start()
    {
       // AllHide();
    }

}
