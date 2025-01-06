using Unity.Netcode;
using UnityEngine;

public class HalliGalliCard : Card
{
    public enum AnimalType
    {
        Croc,
        Lion,
        Fox,
        Panda
    }
    public AnimalType m_AnimalType;       // 과일 종류
    public int m_fruitNum;              // 과일 개수
    public SpriteRenderer m_sprite;

    public void Initialize(AnimalType type, int num)
    {
        m_AnimalType = type;
        m_fruitNum = num;
    }

    void Awake()
    {
        m_sprite = GetComponentInChildren<SpriteRenderer>();
    }
}
