using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;

public class Card : NetworkBehaviour
{
    // 위치와 회전을 동기화할 네트워크 변수
    public NetworkVariable<Vector3> m_networkPosition = new NetworkVariable<Vector3>(new Vector3(0, 0, 0));
    public NetworkVariable<Quaternion> m_networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity);

    private CardDeck m_myCardDeck;                       // 카드 각각의 카드 덱 클래스, 카드 1장당 1개씩 가지고 있음.(변하지 않음)
    private CardDeck m_currentCardDeck;                  // 카드가 현재 속해있는 카드 덱의 레퍼런스.(위치가 옮겨질때 마다 변함)

    private bool isMoving = false;
    public bool m_isPlaced = false;                    // 테이블이나 카드위에 올려져있는지 확인하는 변수. Drag하고 있는중에 false가됨.
    public float m_cardSpacing = 0.1f;                 // 카드 사이 간격

    public int m_cardNum;             // 카드번호, Shuffle확인용
                                      // TODO : 나중에 지우기
    public CardDeck CurrentCardDeck {                   // m_currentCardDeck의 프로퍼티
        get { return m_currentCardDeck;}                // 현재 이 카드가 속해있는 덱의 주소를 반환.
        set {                                            
            if (value == null)                          // 아무값도 입력받지 않으면, m_currentCardDeck의 주소를 myCardDeck의 주소로 초기화.
                m_currentCardDeck = m_myCardDeck;
            else                                        // 이 카드가 속해있는 덱의 주소를 설정해줌.
                m_currentCardDeck = value;
        }
    }
    public void IsMove(bool canMove)
    {
        isMoving = canMove;
    }
    void Start()
    {
        m_myCardDeck = new CardDeck(this);              // myCardDeck 클래스 생성( 이 카드의 정보를 첫번째로 넣어줌 )
        CurrentCardDeck = m_myCardDeck;                 // 현재 속해있는 덱을 myCardDeck으로 설정.
    }
    void Update()
    {
        // 서버에서만 위치와 회전 업데이트 (클라이언트는 읽기만 함)
        if (IsServer)
        {
            // 위치와 회전 값 갱신
            m_networkPosition.Value = transform.position;
            m_networkRotation.Value = transform.rotation;
        }
        else if (IsClient && isMoving)
        {
            TestCardPosChangeServerRpc();
        }
    }
    [ServerRpc]
    void TestCardPosChangeServerRpc()
    {
        m_networkPosition.Value = transform.position;
    }

    // 위치와 회전이 변경되었을 때 클라이언트에서 처리
    private void OnEnable()
    {
        m_networkPosition.OnValueChanged += HandlePositionChanged;
        m_networkRotation.OnValueChanged += HandleRotationChanged;
    }

    private void OnDisable()
    {
        m_networkPosition.OnValueChanged -= HandlePositionChanged;
        m_networkRotation.OnValueChanged -= HandleRotationChanged;
    }

    private void HandlePositionChanged(Vector3 oldPosition, Vector3 newPosition)
    {
        // 위치가 변경되면 클라이언트에서 해당 위치로 이동
        transform.position = newPosition;
    }

    private void HandleRotationChanged(Quaternion oldRotation, Quaternion newRotation)
    {
        // 회전이 변경되면 클라이언트에서 해당 회전으로 회전
        transform.rotation = newRotation;
    }
    public void FlipCard()              // 카드를 뒤집어주는 함수
    {
        Debug.Log("FlipCard");
        //transform.Rotate(Vector3.right * 180);
        StartCoroutine(FlipAnimation());
    }

    private IEnumerator FlipAnimation()
    {
        isMoving = true;
        float elapsedTime = 0;
        float duration = 0.5f;
        Vector3 startRotation = transform.eulerAngles;
        Vector3 endRotation = transform.eulerAngles + new Vector3(180, 0, 0);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.eulerAngles = Vector3.Lerp(startRotation, endRotation, elapsedTime / duration);
            yield return null;
        }
        isMoving = false;
    }

    public  void CardSuffleAnimation()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        if (!m_isPlaced)
        {
            if (other.collider.CompareTag("Card"))
            {
                Debug.Log("Card");

                m_isPlaced = true;  // 카드가 놓였음을 표시

                // 카드 위치를 다른 카드와 일치시킴
                Vector3 newPos = other.transform.position;
                newPos.y += m_cardSpacing;  // 높이 조정

                transform.position = newPos;  // 최종 위치 설정

                // 카드의 회전 방향을 맞춤
                transform.rotation = other.transform.rotation;

                // m_cardDeck에 추가
                Card card = other.gameObject.GetComponent<Card>();
                card.CurrentCardDeck.AddToDeck(this);
            }
            else if (other.collider.CompareTag("Table"))
            {
                Debug.Log("Table");

                m_isPlaced = true;  // 테이블 위에 놓였음을 표시
            }
        }
    }
}
