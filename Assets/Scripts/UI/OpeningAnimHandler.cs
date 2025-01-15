using UnityEngine;

public class OpeningAnimHandler : MonoBehaviour
{
    [SerializeField] ProgressBarManager m_progressBarController;

    // progressBar 0%
    public void AnimEvent_StartProgressBar()
    {
        if (m_progressBarController != null)
        {
            m_progressBarController.AnimEvent_StartProgressBar();
        }
    }
    
    // progressBar 100%
    public void AnimEvent_CompleteProgressBar()
    {
        if(m_progressBarController != null)
        {
            m_progressBarController.AnimEvent_CompleteProgressBar();
        }
    }

    // Animation 종료
    public void AnimEvent_FinishAnimation()
    {
        // TODO : 최종 Scene에 따라 넘어갈 Scene 조정필요 (2024.01.05)
        LoadScene.Instance.LoadSceneAsync(SceneState.Lobby);
    }
}
