using UnityEngine;

public class HalliGalliCard : Card
{
    public enum FruitType
    {
        Strawberry,
        Banana,
        Plum,
        Kiwi
    }
    public FruitType m_fruitType;       // ���� ����
    public int m_fruitNum;              // ���� ����

    public void Initialize(FruitType type, int num)
    {
        m_fruitType = type;
        m_fruitNum = num;
    }
}
