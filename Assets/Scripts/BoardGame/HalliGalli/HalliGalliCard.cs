using UnityEngine;

public class HalliGalliCard : Card
{
    public enum AnimalType
    {
        Corc,
        Lion,
        Fox,
        Panda
    }
    public AnimalType m_AnimalType;       // 과일 종류
    public int m_fruitNum;              // 과일 개수

    public void Initialize(AnimalType type, int num)
    {
        m_AnimalType = type;
        m_fruitNum = num;
    }
}
