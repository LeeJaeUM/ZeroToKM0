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

    private bool isMoving = false;
    public bool m_isPlaced = false;                    // 테이블이나 카드위에 올려져있는지 확인하는 변수. Drag하고 있는중에 false가됨.
    public float m_cardSpacing = 0.05f;                 // 카드 사이 간격
    public void IsMove(bool canMove)
    {
        isMoving = canMove;
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

                // 카드의 회전 방향을 맞추고, x축을 기준으로 90도 회전
                transform.forward = other.transform.forward;
            }
            else if (other.collider.CompareTag("Table"))
            {
                Debug.Log("Table");

                m_isPlaced = true;  // 테이블 위에 놓였음을 표시
            }
        }
    }
}
