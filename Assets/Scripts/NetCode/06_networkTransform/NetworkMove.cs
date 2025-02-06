using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using System.Collections;
using System;

public class NetworkMove : NetworkBehaviour
{
    public NetworkVariable<Vector3> m_networkPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<bool> m_networkIsMoving = new NetworkVariable<bool>();
    private NetworkRigidbody rigid;
    private bool isMoving = false;
    private float moveDuration = 0.08f;
    public AnimationCurve smoothCurve; // 인스펙터에서 커브를 설정할 수 있도록

    public void IsMove(bool canMove)
    {
        //if (IsOwner)
        if (IsServer)
        {
            m_networkIsMoving.Value = canMove;
        }
    }

    [ServerRpc(RequireOwnership = false)] // 소유권 없이도 서버에 요청 가능
    public void RequestMoveServerRpc(Vector3 position)
    {
        // 서버에서 위치 변경
        m_networkPosition.Value = position;
        //Debug.Log("클라이언트가 드래그중임");
    }

    [ServerRpc(RequireOwnership = false)] // 소유권 없이도 서버에 요청 가능
    public void RequestIsMovingChangeServerRpc(bool canMove)
    {
        // 서버에서 위치 변경
        m_networkIsMoving.Value = canMove;
        Debug.Log("클라이언트가 드래그중임");
    }

    private void OnEnable()
    {
        m_networkPosition.OnValueChanged += OnPositionChanged;
        m_networkIsMoving.OnValueChanged += OnBoolChanged;
    }



    private void OnDisable()
    {
        m_networkIsMoving.OnValueChanged -= OnBoolChanged;
        m_networkPosition.OnValueChanged -= OnPositionChanged;
    }
    private void OnBoolChanged(bool previousValue, bool newValue)
    {
        isMoving = newValue;
        //rigidbody.useGravity = !newValue;
        rigid.Rigidbody.useGravity = !newValue;
    }

    private void OnPositionChanged(Vector3 oldPosition, Vector3 newPosition)
    {
       transform.position = newPosition;
       // StartCoroutine(SmoothMoveToPosition(newPosition));
       //TODO : 제한 해제
    }

    /// <summary>
    /// 입력받은 위치로 부드럽게 이동
    /// </summary>
    /// <param name="targetPosition">새 위치</param>
    /// <returns></returns>
    private IEnumerator SmoothMoveToPosition(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;

            // 애니메이션 커브를 사용하여 t 값을 계산
            t = smoothCurve.Evaluate(t); // 커브에 따라 t 값 계산

            transform.position = Vector3.Lerp(startPosition, targetPosition, t); // 보간

            yield return null; // 한 프레임 대기
        }
    }
    void Start()
    {
        rigid = GetComponent<NetworkRigidbody>();
    }

}