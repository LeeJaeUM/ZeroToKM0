using Unity.Netcode.Components;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

public class CardAnimation : NetworkBehaviour
{
    private NetworkAnimator m_networkAnimator;
    private bool isOpen;

    void Start()
    {
        m_networkAnimator = GetComponent<NetworkAnimator>();
    }

    // 클라이언트가 애니메이션 실행을 서버에 요청
    public void FlipCardAnim()
    {
        Debug.Log("FlipCard");

        if (IsServer)
        {
            // 서버에서 애니메이션을 실행
            isOpen = !isOpen;
            m_networkAnimator.Animator.SetBool("isOpen", isOpen);
            m_networkAnimator.SetTrigger("Open");

            // 클라이언트에게 동기화
            FlipCardAnimClientRpc(isOpen);
        }
        else if (IsClient)
        {
            // 클라이언트에서 서버에 애니메이션 실행 요청
            FlipCardAnimServerRpc();
        }
    }

    // 서버로부터 카드 셔플 애니메이션 실행 요청을 처리
    public void CardShuffleAnim()
    {
        if (IsServer)
        {
            m_networkAnimator.SetTrigger("Shuffle");  // 서버에서 셔플 애니메이션 실행
            CardShuffleAnimClientRpc();  // 클라이언트 동기화
        }
        else if (IsClient)
        {
            // 클라이언트에서 서버에 셔플 애니메이션 실행 요청
            CardShuffleAnimServerRpc();
        }
    }

    // 클라이언트에서 서버로 애니메이션 실행 요청을 보내는 ServerRpc
    [ServerRpc(RequireOwnership = false)]
    private void FlipCardAnimServerRpc(ServerRpcParams rpcParams = default)
    {
        // 서버에서 애니메이션 상태 변경 및 클라이언트에게 동기화
        isOpen = !isOpen;
        m_networkAnimator.Animator.SetBool("isOpen", isOpen);
        m_networkAnimator.SetTrigger("Open");

        FlipCardAnimClientRpc(isOpen);  // 클라이언트들에게 동기화
    }

    // 클라이언트에서 서버로 셔플 애니메이션 실행 요청을 보내는 ServerRpc
    [ServerRpc(RequireOwnership = false)]
    private void CardShuffleAnimServerRpc(ServerRpcParams rpcParams = default)
    {
        // 서버에서 셔플 애니메이션 실행
        m_networkAnimator.SetTrigger("Shuffle");
        CardShuffleAnimClientRpc();  // 클라이언트들에게 동기화
    }

    // 클라이언트에게 애니메이션 상태를 동기화하는 ClientRpc
    [ClientRpc]
    private void FlipCardAnimClientRpc(bool isOpenState)
    {
        isOpen = isOpenState;  // 서버에서 받은 값으로 상태 업데이트
        m_networkAnimator.Animator.SetBool("isOpen", isOpen);  // 클라이언트 애니메이션 실행
        m_networkAnimator.SetTrigger("Open");

    }

    // 셔플 애니메이션을 클라이언트에게 동기화하는 ClientRpc
    [ClientRpc]
    private void CardShuffleAnimClientRpc()
    {
        m_networkAnimator.SetTrigger("Shuffle");  // 클라이언트에서 셔플 애니메이션 실행
    }
}
