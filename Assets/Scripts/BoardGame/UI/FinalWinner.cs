using UnityEngine;
using UnityEngine.UI;

public class FinalWinner : MonoBehaviour
{
    public Text m_text;
    private readonly string[] playerNames = { "hyeon", "moonasd13", "yunjae999", "JaeUM" };
    public void SetText(int winner)
    {
        m_text.text = $"Final Win : {playerNames[winner]}";
    }
    public void SetName(string name, int playerNum)
    {
        playerNames[playerNum] = name;
    }
    void Awake()
    {
        m_text = GetComponentInChildren<Text>();
    }
}
