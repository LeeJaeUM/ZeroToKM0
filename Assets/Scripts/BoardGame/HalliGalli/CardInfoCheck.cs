using UnityEngine;
using Unity.Netcode;

public class CardInfoCheck : NetworkBehaviour
{
    public TextMesh myText;
    public int index;

    void UpdateText()
    {
        if (GameManager.Instance.m_halligalli.m_topCard[index] != null)
            myText.text = string.Format("{0:F0} {1:F0}", GameManager.Instance.m_halligalli.m_topCard[index].m_AnimalType.ToString(), GameManager.Instance.m_halligalli.m_topCard[index].m_fruitNum);
        else
            myText.text = " ";
    }

    private void Awake()
    {
        myText = GetComponent<TextMesh>();
        myText.text = " ";
    }
    private void Start()
    {
        GameManager.Instance.m_halligalli.OnTopCardChanged += UpdateText;
    }
    private void OnDisable()
    {
        GameManager.Instance.m_halligalli.OnTopCardChanged -= UpdateText;
    }
}