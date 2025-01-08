using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : NetworkBehaviour
{
    public Camera m_playerCamera; // 네트워크로 제어되는 플레이어의 카메라
    private Camera m_localCamera; // 기존 로컬 카메라 (게임 시작 전에 있는 카메라)
    private PlayerInput playerInput;  // PlayerInput 컴포넌트

    private void Start()
    {
        // PlayerInput을 찾아서 설정
        playerInput = GetComponent<PlayerInput>();
        m_playerCamera = GetComponent<Camera>(); // 오브젝트에서 카메라 찾기

        // 기존의 로컬 카메라를 찾기
        m_localCamera = Camera.main; // 게임에서 기본 카메라를 가져옵니다 (main 카메라)

        if (IsOwner)
        {
            // 클라이언트가 자기 자신일 때만 카메라 활성화
            if (m_playerCamera != null)
            {
                m_playerCamera.gameObject.SetActive(true); // 자신의 카메라 활성화
            }

            // 기존 카메라는 비활성화
            if (m_localCamera != null)
            {
                m_localCamera.gameObject.SetActive(false); // 기존 로컬 카메라 비활성화
            }
        }
        else
        {
            // 다른 클라이언트는 카메라 비활성화
            if (m_playerCamera != null)
            {
                m_playerCamera.gameObject.SetActive(false); // 다른 클라이언트의 카메라 비활성화
            }
        }

        // 인풋 시스템 설정
        if (playerInput != null)
        {
            // 클라이언트에서만 사용할 컨트롤 스킴 지정
            playerInput.SwitchCurrentActionMap("Player");
            // 예시로 "Player"라는 Action Map을 사용할 경우, 이를 지정합니다.
        }
    }

 
}
