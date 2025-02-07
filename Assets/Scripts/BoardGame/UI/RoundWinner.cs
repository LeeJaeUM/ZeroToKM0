using UnityEngine;
using UnityEngine.UI;

public class RoundWinner : MonoBehaviour
{
    public Text m_text;

    public void SetText(int winner)
    {
        m_text.text = "Player" + winner + "Win!";
    }
    void Awake()
    {
        m_text = GetComponentInChildren<Text>();
    }
}
