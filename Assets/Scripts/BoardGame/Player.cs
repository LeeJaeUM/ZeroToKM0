using UnityEngine;

public class Player : MonoBehaviour
{
    public int m_playerNum;
    public HalliGalliNetwork m_halligalli;
    public bool m_isMyTurn;             // 내 턴인지 체크, true일때만 opencard가능
    public bool m_isGetInput;           // space bar input 하나만 받기 위해, 멀티플레이가 되면 없앨 것

    // 꼬치의 달인
    public KushiExpressIngredient[] m_ingredients = new KushiExpressIngredient[6];
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && m_isMyTurn && m_isGetInput)    // OpenCard 체크용
        {
            m_halligalli.OpenCard(m_playerNum);
        }
        if(Input.GetKeyDown(KeyCode.Space) && m_isGetInput)
        {
            GameManager.Instance.RingBell(m_playerNum);
        }
    }
}