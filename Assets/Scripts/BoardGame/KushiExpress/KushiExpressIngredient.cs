using UnityEngine;

public class KushiExpressIngredient : MonoBehaviour
{
    public enum IngredientType          // ��� ����
    {
        BellPepper,                     // �Ǹ�
        Shrimp,                         // ����
        Tomato,                         // �丶��
        Steak,                          // ������ũ
        Bacon,                          // ������
        Cheese,                         // ġ��
        Max
    }
    public IngredientType m_type;       // ����� ����
    public int m_length;                // ����. 1�̳� 2. ��� ���� �� �ִ���.

}
