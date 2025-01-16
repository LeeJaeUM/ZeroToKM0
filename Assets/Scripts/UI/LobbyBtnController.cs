using Michsky.MUIP;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyBtnController : MonoBehaviour
{
    #region Contants and Fields
    [SerializeField] GameObject m_lobbyWindow;    // Lobby Window GameObject
    [SerializeField] GameObject m_roomListWindow; // Room List Window GameObject

    // 이동할 씬 이름
    private const string TargetSceneName = "04_OtherGames";

    Animator m_lobbyAnimator;
    Animator m_roomListAnimator;

    bool isTransitioning = false; // 전환 중인지 확인
    #endregion

    #region Public Methods and Operators
    /// <summary>
    /// Join 버튼 클릭 시 04_OtherGames 씬으로 전환 후 클라이언트 시작
    /// </summary>
    public void OnJoinBtnClick()
    {
        Debug.Log("Join Button Clicked - Switching to Client Mode");

        // 씬 로드 후 클라이언트 시작
        SceneManager.LoadScene(TargetSceneName);
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name == TargetSceneName)
            {
                StartClient();
            }
        };

        // TODO : Client 연결로 변경 해놓으셔서 방목록창으로 변경 할시 아래코드 사용 요망 (2025.01.16)
        /*
        if (isTransitioning) return; // 이미 전환 중이면 중복 실행 방지
        isTransitioning = true;

        // LobbyWindow Fade-Out
        m_lobbyAnimator.Play("Fade-out");

        // RoomListWindow 활성화 후 Fade-In 시작
        StartCoroutine(TransitionToRoomList());*/
    }

    /// <summary>
    /// Create 버튼 클릭 시 04_OtherGames 씬으로 전환 후 호스트 시작
    /// </summary>
    public void OnCreateBtnClick()
    {
        Debug.Log("Create Button Clicked - Switching to Host Mode");

        // 씬 로드 후 호스트 시작
        SceneManager.LoadScene(TargetSceneName);
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name == TargetSceneName)
            {
                StartHost();
            }
        };
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
    #endregion

    #region Methods
    /// <summary>
    /// 클라이언트 시작
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
    /// 호스트 시작
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
    #endregion

    void Start()
    {
        m_lobbyAnimator = m_lobbyWindow.GetComponent<Animator>();
        m_roomListAnimator = m_roomListWindow.GetComponent<Animator>();
    }
}
