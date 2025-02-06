using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundWinner : MonoBehaviour
{
    public TextMeshProUGUI m_text;
    private static readonly string[] playerNames = { "hyeon", "moonasd13", "yunjae999", "JaeUM" };

    public void SetText(int winner)
    {
        switch (winner)
        {
            case 1:
                m_text.text = "Round Winner : hyeon";
                break;
            case 2:
                m_text.text = "Round Winner : moonasd13";
                break;
            case 3:
                m_text.text = "Round Winner : yunjae999";
                break;
            case 4:
                m_text.text = "Round Winner : JaeUM";
                break;

        }
    }
    void Awake()
    {
        m_text = GetComponentInChildren<TextMeshProUGUI>();
    }
}
