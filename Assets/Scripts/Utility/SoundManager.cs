using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    public enum SoundType
    {
        Flip,
        Shuffle
    }

    [SerializeField] AudioSource m_audioSource;     // 사운드 재생을 위한 AudioSource
    [SerializeField] List<AudioClip> m_audioClip;   // 재생할 사운드 클립 추가

    public void PlaySound(SoundType type)
    {
        if(m_audioSource != null && m_audioClip[(int)type]!= null)
        {
            m_audioSource.Stop();                               // 겹쳐서 재생되는 현상 방지
            m_audioSource.PlayOneShot(m_audioClip[(int)type]);
        }
    }
}
