using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WinMessage : MonoBehaviour
{
    public Text[] m_text;
    public Image[] m_image;
    [SerializeField]
    private readonly string[] playerNames = { "hyeon", "moonasd13", "yunjae999", "JaeUM" };
    public void SetRoundWinText(int winner)
    {
        m_text[0].text = $"Round Winner : {playerNames[winner]}";
        StartCoroutine(CoRoundWinMessage());
    }
    public void SetFinalWinText(int winner)
    {
        m_text[1].text = $"Final Win : {playerNames[winner]}";
        StartCoroutine(CoFinalWinMessage());
    }

    public void SetName(string name, int playerNum)
    {
        playerNames[playerNum] = name;
    }

    private void HideUI()
    {
        m_image[0].enabled = false;
        m_text[0].enabled = false;
        m_image[1].enabled = false;
        m_text[1].enabled = false;
    }

    #region Coroutine Methods
    IEnumerator CoRoundWinMessage()
    {
        m_image[0].enabled = true;
        m_text[0].enabled = true ;
        yield return new WaitForSeconds(2f);
        m_image[0].enabled = false;
        m_text[0].enabled = false ;
    }
    IEnumerator CoFinalWinMessage()
    {
        m_image[1].enabled = true;
        m_text[1].enabled = true;
        yield return new WaitForSeconds(2f);
        m_image[1].enabled = false;
        m_text[1].enabled = false;
    }
    #endregion

    void Awake()
    {
        m_text = GetComponentsInChildren<Text>();
        m_image = GetComponentsInChildren<Image>();
        HideUI();
    }
}
