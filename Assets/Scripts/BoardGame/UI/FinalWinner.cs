using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinalWinner : MonoBehaviour
{
    public TextMeshProUGUI m_text;

    public void SetText(int winner)
    {
        m_text.text = "Final Win : Player" + winner;
    }
    void Awake()
    {
        m_text = GetComponentInChildren<TextMeshProUGUI>();
    }
}
