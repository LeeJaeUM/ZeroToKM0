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
<<<<<<< Updated upstream

    public void Initialize(AnimalType type, int num)
=======
    public int m_CardIndex;
    public GameObject m_skin;

    // Initialize 함수에서 값을 설정하고 NetworkVariable로 동기화
    public void Initialize(AnimalType type, int num, int cardIndex, HalliGalliNetwork halligalli)
>>>>>>> Stashed changes
    {
        m_AnimalType = type;
        m_fruitNum = num;
    }

    void Awake()
    {
        m_sprite = GetComponentInChildren<SpriteRenderer>();
    }
}
