using UnityEngine;

public class OpeningAnimHandler : MonoBehaviour
{
    [SerializeField] ProgressBarManager m_progressBarController;

    public void AnimEvent_StartProgressBar()
    {
        if (m_progressBarController != null)
        {
            m_progressBarController.AnimEvent_StartProgressBar();
        }
    }
    
    public void AnimEvent_CompleteProgressBar()
    {
        if(m_progressBarController != null)
        {
            m_progressBarController.AnimEvent_CompleteProgressBar();
        }
    }
}
