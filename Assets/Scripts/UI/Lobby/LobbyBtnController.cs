using Michsky.MUIP;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class LobbyBtnController : MonoBehaviour
{
    #region Contants and Fields
    [SerializeField] GameObject m_lobbyWindow;              // Lobby Window GameObject
    [SerializeField] GameObject m_roomListWindow;           // Room List Window GameObject
    [SerializeField] ModalWindowManager m_userInfoWindow;   // User Info Window ModalWindowManager
    [SerializeField] UserInfo m_userInfo;                   // UserInfo

    // TODO : TEST data. DB data로 변경 필요 (2025.02.03)
    [SerializeField] Sprite m_sampleIcon;
    [SerializeField] Sprite m_sampleOutline;

    Animator m_lobbyAnimator;
    Animator m_roomListAnimator;

    bool isTransitioning = false; // 전환 중인지 확인

    [SerializeField] WidgetController m_widgetController;
    #endregion

    #region Public Methods and Operators
    /// <summary>
    /// Join 버튼 클릭 시 04_OtherGames 씬으로 전환 후 클라이언트 시작
    /// </summary>
    public void OnJoinBtnClick()
    {
        Debug.Log("Join Button Clicked - Switching to Client Mode");

        // 씬 로드 후 클라이언트 시작
        // 개발 중에 확인할 코드
        //SceneManager.LoadScene(TargetSceneName);
        //SceneManager.sceneLoaded += (scene, mode) =>
        //{
        //    if (scene.name == TargetSceneName)
        //    {
        //        StartClient();
        //    }
        //};

        //

        // TODO : Client 연결로 변경 해놓으셔서 방목록창으로 변경 할시 아래코드 사용 요망 (2025.01.16)
        
        if (isTransitioning) return; // 이미 전환 중이면 중복 실행 방지
        isTransitioning = true;

        // LobbyWindow Fade-Out
        m_lobbyAnimator.Play("Fade-out");

        // RoomListWindow 활성화 후 Fade-In 시작
        StartCoroutine(TransitionToRoomList());

        //relay 사용
        m_widgetController.JoinSessionClick();
    }

    /// <summary>
    /// Create 버튼 클릭 시 04_OtherGames 씬으로 전환 후 호스트 시작
    /// </summary>
    public void OnCreateBtnClick()
    {
        Debug.Log("Create Button Clicked - Switching to Host Mode");

        // 씬 로드 후 호스트 시작 (개발용)
        //SceneManager.LoadScene(TargetSceneName);
        //SceneManager.sceneLoaded += (scene, mode) =>
        //{
        //    if (scene.name == TargetSceneName)
        //    {
        //        //
        //        StartHost();
        //    }
        //};

        //relay 사용
        m_widgetController.CreateSessionClick();
    }

    /// <summary>
    /// Power 버튼 클릭 시 게임 종료
    /// </summary>
    public void OnPowerBtnClick()
    {
        Debug.Log("Exiting the game");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        FBManager._instance.UserLogout();
#else
        Application.Quit();
        FBManager._instance.UserLogout();
#endif
    }

    /// <summary>
    /// Cancel 버튼 클릭 시 Lobby로 돌아가기
    /// </summary>
    public void OnCancelBtnClick()
    {
        StartCoroutine(TransitionToLobbyWindow());
    }

    /// <summary>
    /// UserInfo 클릭 시 UserInfo Window Open
    /// </summary>
    public void OnUserInfoClick()
    {
        // User Info 창 띄우기
        m_userInfoWindow.OpenWindow();

        // TODO : TEST data. DB data로 변경 필요 (2025.02.03)
        string playerName = "Player1";
        int playerWins = 10;
        int playerLosses = 3;
        int playerCoin = 500;

        m_userInfo.SetUserInfo(m_sampleIcon, m_sampleOutline, playerName, playerWins, playerLosses, playerCoin);
    }
    #endregion

    #region Methods
    /// <summary>
    /// 개발용 클라이언트 시작
    /// </summary>
    private void StartClient()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Client started successfully.");
        }
        else
        {
            Debug.LogError("NetworkManager is not set up in the scene.");
        }
    }

    /// <summary>
    /// 개발용 호스트 시작
    /// </summary>
    private void StartHost()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("Host started successfully.");
        }
        else
        {
            Debug.LogError("NetworkManager is not set up in the scene.");
        }
    }

    float GetAnimationClipLength(Animator animator, string clipName)
    {
        if (animator == null) return 0f;

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }

        Debug.LogWarning($"Animation clip '{clipName}' not found in {animator.gameObject.name}");
        return 0f;
    }
    #endregion

    #region Coroutine Methods
    IEnumerator TransitionToRoomList()
    {
        yield return new WaitForSecondsRealtime(GetAnimationClipLength(m_lobbyAnimator, "Fade-out"));

        // LobbyWindow 비활성화
        m_lobbyWindow.SetActive(false);

        // RoomListWindow 활성화
        m_roomListWindow.SetActive(true);

        // RoomListWindow Fade-In 애니메이션 재생
        m_roomListAnimator.Play("Fade-in");

        // RoomListWindow의 Fade-In 애니메이션 시간 대기
        yield return new WaitForSecondsRealtime(GetAnimationClipLength(m_roomListAnimator, "Fade-in"));

        isTransitioning = false; // 전환 종료
    }

    IEnumerator TransitionToLobbyWindow()
    {
        yield return new WaitForSecondsRealtime(GetAnimationClipLength(m_roomListAnimator, "Fade-out"));

        // RoomListWindow 비활성화
        m_roomListWindow.SetActive(false);

        // LobbyWindow 활성화
        m_lobbyWindow.SetActive(true);

        // RoomListWindow Fade-In 애니메이션 재생
        m_lobbyAnimator.Play("Fade-in");

        // RoomListWindow의 Fade-In 애니메이션 시간 대기
        yield return new WaitForSecondsRealtime(GetAnimationClipLength(m_lobbyAnimator, "Fade-in"));

        isTransitioning = false; // 전환 종료
    }
    #endregion

    void Start()
    {
        m_lobbyAnimator = m_lobbyWindow.GetComponent<Animator>();
        m_roomListAnimator = m_roomListWindow.GetComponent<Animator>();
    }
}
