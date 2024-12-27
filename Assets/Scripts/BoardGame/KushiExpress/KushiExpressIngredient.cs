using UnityEngine;

public class KushiExpressIngredient : MonoBehaviour
{
    public enum IngredientType          // 재료 종류
    {
        BellPepper,                     // 피망
        Shrimp,                         // 새우
        Tomato,                         // 토마토
        Steak,                          // 스테이크
        Bacon,                          // 베이컨
        Cheese,                         // 치즈
        Max
    }
    public IngredientType m_type;       // 재료의 종류
    public int m_length;                // 길이. 1이나 2. 몇번 꽂을 수 있는지.

}
