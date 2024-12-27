using UnityEngine;
using UnityEngine.UI;

public class FinalWinner : MonoBehaviour
{
    public Text m_text;

    public void SetText(int winner)
    {
        m_text.text = "Final Win : Player" + winner;
    }
    void Awake()
    {
        m_text = GetComponentInChildren<Text>();
    }
}
