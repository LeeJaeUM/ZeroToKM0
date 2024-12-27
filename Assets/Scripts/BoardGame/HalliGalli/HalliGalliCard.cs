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
    public FruitType m_fruitType;       // 과일 종류
    public int m_fruitNum;              // 과일 개수

    public void Initialize(FruitType type, int num)
    {
        m_fruitType = type;
        m_fruitNum = num;
    }
}
