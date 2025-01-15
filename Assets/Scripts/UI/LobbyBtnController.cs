using Michsky.MUIP;
using UnityEngine;

public class LobbyBtnController : MonoBehaviour
{
    [SerializeField] WindowManager m_windowManager;

    // TODO : 기능 구현 필요 (2025.01.14)

    /// <summary>
    /// Join 버튼 클릭 시 방 선택 창으로 변경
    /// </summary>
    public void OnJoinBtnClick()
    {
        Debug.Log("Join Button Clicked");
        m_windowManager.OpenWindow("RoomList_window");
    }

    /// <summary>
    /// Create 버튼 클릭 시 방 생성
    /// </summary>
    public void OnCreateBtnClick()
    {
        Debug.Log("Create Button Clicked");
    }

    /// <summary>
    /// Power 버튼 클릭 시 게임 종료
    /// </summary>
    public void OnPowerBtnClick()
    {
        Debug.Log("Exiting the game");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
