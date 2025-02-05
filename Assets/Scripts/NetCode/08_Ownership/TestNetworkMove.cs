using Unity.Netcode.Components;
using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System;


public class TestNetworkMove : NetworkBehaviour
{
    private NetworkRigidbody rigid;
    private bool isMoving = false;
    private float moveDuration = 0.08f;
    public AnimationCurve smoothCurve; // 인스펙터에서 커브를 설정할 수 있도록
    public Coroutine cor;

    public void IsMove(bool canMove)
    {
        //if (IsOwner)
        if (IsServer)
        {
            IsMoveClientRpc(canMove);
        }
        else if (IsOwner)
        {
            RequestIsMovingChangeServerRpc(canMove);
        }
    }
    [ServerRpc] // 소유권 없이도 서버에 요청 가능
    public void RequestMoveServerRpc(Vector3 position)
    {
        // 서버에서 위치 변경
        MovedClientRpc(position);
        //Debug.Log("클라이언트가 드래그중임");
    }
    [ClientRpc]
    public void MovedClientRpc(Vector3 position)
    {
        OnPositionChanged(position);
    }

    [ServerRpc]
    public void RequestIsMovingChangeServerRpc(bool canMove)
    {
        // 서버에서 위치 변경
        IsMoveClientRpc(canMove);
        Debug.Log("클라이언트가 드래그중임");
    }
    [ClientRpc]
    public void IsMoveClientRpc(bool canMove)
    {
        OnBoolChanged(canMove);
    }


    public void StartSmoothMove(Vector3 newPosition)
    {
        if (cor != null)
        {
            StopCoroutine(cor);
        }
        cor = StartCoroutine(SmoothMoveToPosition(newPosition));
    }
    private void OnBoolChanged(bool newValue)
    {
        isMoving = newValue;
        //rigidbody.useGravity = !newValue;
        rigid.Rigidbody.useGravity = !newValue;
    }

    public void OnPositionChanged(Vector3 newPosition)
    {
        StartSmoothMove(newPosition);
    }

    public void PositionChanged(Vector3 newPosition)
    {
        //transform.position = newPosition;
        if (IsServer && IsHost)
        {
            Debug.Log("내꺼 : 서버에서");
            transform.position = newPosition;
        }
        else
        {
            Debug.Log("내꺼 : 클라이언트에서");
            RequestMoveServerRpc(newPosition);
        }
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
