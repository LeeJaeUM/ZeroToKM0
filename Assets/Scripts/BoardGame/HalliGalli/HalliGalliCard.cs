using Unity.Netcode;
using UnityEngine;

public class HalliGalliCard : Card
{
    // NetworkVariable로 네트워크 동기화하는 변수들
    NetworkVariable<int> m_NetAnimalType = new NetworkVariable<int>(0);
    NetworkVariable<int> m_NetFruitNum = new NetworkVariable<int>(0);
    NetworkVariable<int> m_NetSpriteIndex = new NetworkVariable<int>(0); // Sprite 인덱스를 네트워크로 동기화
    NetworkVariable<int> m_NetCardIndex = new NetworkVariable<int>(0); // 카드 고정 인덱스 (클라이언트에서 보내는 용도)

    public enum AnimalType
    {
        Croc,
        Lion,
        Fox,
        Panda
    }
    public HalliGalliNetwork m_halligalli;
    public AnimalType m_AnimalType;       // 과일 종류
    public int m_fruitNum;                // 과일 개수
    public SpriteRenderer m_sprite;
    public int m_CardIndex;


    // Initialize 함수에서 값을 설정하고 NetworkVariable로 동기화
    public void Initialize(AnimalType type, int num, int cardIndex, HalliGalliNetwork halligalli)
    {
        m_halligalli = halligalli;
        m_AnimalType = type;
        m_fruitNum = num;

        // 네트워크 변수에 값을 설정하여 클라이언트로 동기화
        m_NetAnimalType.Value = (int)type; // NetworkVariable은 int로 설정되므로 타입을 int로 변환
        m_NetFruitNum.Value = num;
        m_NetCardIndex.Value = cardIndex;
    }

    #region Network Functions

    // 동기화된 값이 변경되면 호출될 함수들
    private void HandleAnimalTypeChanged(int oldValue, int newValue)
    {
        m_AnimalType = (AnimalType)newValue; // 서버에서 동기화된 값을 AnimalType으로 변환
        UpdateCardAppearance();
    }

    private void HandleFruitNumChanged(int oldValue, int newValue)
    {
        m_fruitNum = newValue; // 서버에서 동기화된 값을 fruitNum으로 설정
        UpdateCardAppearance();
    }

    private void HandleSpriteIndexChanged(int oldValue, int newValue)
    {
        m_sprite.sprite = GameManager.Instance.m_halligalli.m_animalSprite[newValue]; // 서버에서 동기화된 스프라이트 인덱스를 사용하여 스프라이트 변경
    }

    // 카드 모양을 동적으로 업데이트하는 함수 (예시)
    private void UpdateCardAppearance()
    {
        // AnimalType에 따라 카드의 스프라이트를 변경하는 코드 작성 가능
        // 예시로 각 동물 타입에 맞는 스프라이트를 설정할 수 있습니다.
    }

    // 스프라이트를 네트워크로 동기화하려면 이 함수를 호출
    public void SetSprite(int spriteIndex)
    {
        if (IsServer)
        {
            m_NetSpriteIndex.Value = spriteIndex; // 서버에서 스프라이트 인덱스를 설정하여 클라이언트로 동기화
        }
    }    
    
    // 서버에서 카드 인덱스가 변경되었을 때, 클라이언트에서 호출되는 함수
    private void OnCardIndexChanged(int oldValue, int newValue)
    {
        // 인덱스가 변경되었을 때 클라이언트에서 처리할 추가 작업이 있으면 여기에 추가
        m_CardIndex = newValue;
    }

    #endregion
    void Awake()
    {
        m_sprite = GetComponentInChildren<SpriteRenderer>();

        // 네트워크 값 변경 시 동기화되는 콜백을 등록
        m_NetAnimalType.OnValueChanged += HandleAnimalTypeChanged;
        m_NetFruitNum.OnValueChanged += HandleFruitNumChanged;
        m_NetSpriteIndex.OnValueChanged += HandleSpriteIndexChanged; // 스프라이트 변경 처리
        m_NetCardIndex.OnValueChanged += OnCardIndexChanged;
    }


    protected override void Start()
    {
        base.Start();
        m_halligalli = GameManager.Instance.m_halligalli;
    }
}


