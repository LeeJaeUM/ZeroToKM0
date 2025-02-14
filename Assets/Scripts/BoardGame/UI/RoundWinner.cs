using UnityEngine;
using UnityEngine.UI;

public class RoundWinner : MonoBehaviour
{
    public Text m_text;
    [SerializeField]
    private readonly string[] playerNames = { "hyeon", "moonasd13", "yunjae999", "JaeUM" };
    public void SetText(int winner)
    {
        m_text.text = $"Round Winner : {playerNames[winner]}";
    }

    public void SetName(string name, int playerNum)
    {
        playerNames[playerNum] = name ;
    }


    void Awake()
    {
        m_text = GetComponentInChildren<Text>();
    }
}
