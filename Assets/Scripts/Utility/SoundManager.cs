using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SoundManager : SingletonDontDestroy<SoundManager>
{
    #region Contants and Fields
    public enum SoundType
    {
        Flip,
        Shuffle
    }

    [SerializeField] AudioSource m_sfxSource;       // 효과음용 AnudioSource
    [SerializeField] AudioSource m_bgmSource;       // 배경음악용 AudioSource
    [SerializeField] List<AudioClip> m_audioClip;   // 효과음 List
    [SerializeField] AudioClip m_defaultBGM;          // 기본 BGM

    SettingsController m_settingsController;
    #endregion

    #region Public Methods and Operators
    public void PlaySFX(SoundType type)
    {
        int index = (int)type;
        if(m_sfxSource != null && index < m_audioClip.Count && m_audioClip[index] != null)
        {
            m_sfxSource.Stop();                               // 겹쳐서 재생되는 현상 방지
            m_sfxSource.PlayOneShot(m_audioClip[index]);
        }
    }
    public void PlayBGM(AudioClip bgm)
    {
        if(m_bgmSource != null && bgm != null)
        {
            if (m_bgmSource.clip == bgm) return; // 같은 BGM이면 실행 안 함

            m_bgmSource.clip = bgm;
            m_bgmSource.loop = true;
            m_bgmSource.Play();
        }
    }
    public void SetVolume(float bgmVolume, float sfxVolume)
    {
        if(m_bgmSource != null)
        {
            m_bgmSource.volume = Mathf.Clamp01(bgmVolume); // 배경음 볼륨
        }

        if(m_sfxSource != null)
        {
            m_sfxSource.volume = Mathf.Clamp01(sfxVolume); // 효과음 볼륨
        }
    }
    #endregion

    protected override void OnAwake()
    {
        base.OnAwake();

        // 저장된 볼륨 불러오기
    }
    protected override void OnStart()
    {
        PlayBGM(m_defaultBGM); // 처음 시작할 때 BGM 실행
    }
}
