using UnityEngine;
using Michsky.MUIP; // Modern UI Pack namesapce

public class ProgressBarManager : MonoBehaviour
{
    #region Constants and Fields
    [SerializeField] ProgressBar m_progressBar;
    
    bool m_isAnimating = false;             // Progress Bar Update Flag
    float m_animationDuration = 1.75f;     // Animation 길이
    float m_elapsedTime = 0f;               // 진행된 시간
    #endregion

    #region Animation Event Methods
    public void AnimEvent_StartProgressBar()
    {
        if(m_progressBar == null)
        {
            Debug.LogError("ProgressBar is not assigned");
            return;
        }

        m_isAnimating = true;
        m_elapsedTime = 0f;
        m_progressBar.currentPercent = 0;
        m_progressBar.UpdateUI();
    }

    public void AnimEvent_CompleteProgressBar()
    {
        if (m_progressBar == null)
        {
            Debug.LogError("ProgressBar is not assigned");
            return;
        }

        m_isAnimating = false;
        m_progressBar.currentPercent = 100;
        m_progressBar.UpdateUI();
    }
    #endregion

    void Start()
    {

    }

    void Update()
    {
        if(m_isAnimating)
        {
            m_elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(m_elapsedTime / m_animationDuration);
            m_progressBar.currentPercent = progress * 100;
            m_progressBar.UpdateUI();

            if(m_elapsedTime >= m_animationDuration)
            {
                m_isAnimating = false;
            }
        }
    }
}
