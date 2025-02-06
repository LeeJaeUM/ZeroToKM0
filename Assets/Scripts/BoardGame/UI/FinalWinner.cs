using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinalWinner : MonoBehaviour
{
    public TextMeshProUGUI m_text;
    private static readonly string[] playerNames = { "hyeon", "moonasd13", "yunjae999", "JaeUM" };

    public void SetText(int winner)
    {
        switch (winner)
        {
            case 1:
                m_text.text = "Final Win : hyeon";
                break;
            case 2:
                m_text.text = "Final Win : moonasd13";
                break;
            case 3:
                m_text.text = "Final Win : yunjae999";
                break;
            case 4:
                m_text.text = "Final Win : JaeUM";
                break;

        }
    }
    void Awake()
    {
        m_text = GetComponentInChildren<TextMeshProUGUI>();
    }
}
