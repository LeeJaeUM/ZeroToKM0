using UnityEngine;

public class SkewerIngredient : MonoBehaviour
{
    public enum IngredientType          // 재료 종류
    {
        Pimento,                        // 피망
        Shrimp,                         // 새우
        Tomato,                         // 토마토
        Meat,                          // 고기
        Bacon,                          // 베이컨
        Cheese,                         // 치즈
        Max
    }
    public IngredientType m_ingredientType;
    public SkewerStick m_touchedStick = null;

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("SkewerStick"))
        {
            m_touchedStick = other.transform.GetComponent<SkewerStick>();
        }
    }
}
