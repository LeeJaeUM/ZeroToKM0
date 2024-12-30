using UnityEngine;

public class ChangeCardSprite : MonoBehaviour
{
    [SerializeField]
    Sprite[] m_spriteToChange;          // 랜덤으로 변경할 Sprite 배열

    SpriteRenderer m_spriteRenderer;    // 하위에 있는 SpriteRenderer를 참조

    void ChangeSprite()
    {
        if (m_spriteToChange == null || m_spriteToChange.Length == 0)
        {
            Debug.LogWarning("m_spriteToChange 배열이 비어 있습니다.");
            return;
        }

        // Sprite 배열에서 랜덤으로 하나 선택
        int randomIndex = Random.Range(0, m_spriteToChange.Length);
        Sprite randomSprite = m_spriteToChange[randomIndex];

        m_spriteRenderer.sprite = randomSprite;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (m_spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer를 찾을 수 없습니다.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 카드 뒤집었을 때 → 특정 키 누를 때(Test)
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            ChangeSprite();
        }
    }
}
