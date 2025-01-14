using UnityEngine;

public class SkewerIngredient : MonoBehaviour
{
    public enum IngredientType          // 재료 종류
    {
        BellPepper,                     // 피망
        Shrimp,                         // 새우
        Tomato,                         // 토마토
        Steak,                          // 스테이크
        Bacon_OutSide,                  // 베이컨 바깥 부분( 3개 구멍 중 바깥 2개 구멍 )
        Bacon_InSide,                   // 베이컨 안쪽 부분( 3개 구멍 중 안쪽 1개 구멍 )
        Cheese_OutSide,                 // 치즈 바깥 부분
        Cheese_InSide,                  // 치즈 안쪽 부분
        Max
    }
    public IngredientType m_type;       // 재료의 종류
    public int m_length;                // 길이. 1이나 2. 몇번 꽂을 수 있는지.

}
