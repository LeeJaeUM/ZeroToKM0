using UnityEngine;

public class CardInfoCheck : MonoBehaviour
{
    public TextMesh myText;
    public int index;
    private void Awake()
    {
        myText = GetComponent<TextMesh>();
    }
    // Update is called once per frame
    void Update()
    {

        if (GameManager.Instance.m_halligalli.m_topCard[index] != null)
            myText.text = string.Format("{0:F0} {1:F0}", GameManager.Instance.m_halligalli.m_topCard[index].m_fruitType.ToString(), GameManager.Instance.m_halligalli.m_topCard[index].m_fruitNum);
        else
            myText.text = " ";
    }
}