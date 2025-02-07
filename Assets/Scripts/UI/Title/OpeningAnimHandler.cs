using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadSceneAsync(1);
    }
}
