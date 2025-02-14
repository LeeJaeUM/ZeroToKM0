using UnityEngine;
using UnityEngine.UI;
using TMPro;        // TextMeshPro용 네임스페이스
using Michsky.MUIP; // Modern UI Pack 네임스페이스

public class SettingsController : MonoBehaviour
{
    [SerializeField] SliderManager m_bgmVolumeSlider;     // 전체 볼륨 슬라이더
    [SerializeField] SliderManager m_sfxVolumeSlider;   // 효과음 볼륨 슬라이더
    [SerializeField] SliderManager m_moveSpeedSlider;     // 전체 볼륨 슬라이더
    [SerializeField] SliderManager m_rotateSpeedSlider;   // 효과음 볼륨 슬라이더

    // 임시로 값을 저장하는 변수
    float m_tempFullVolume;
    float m_tempEffectVolume;   
    float m_tempMoveSpeed;
    float m_tempRotateSpeed;


    // 저장된 설정 값 (PlayerPrefs 키)
    const string BGM_VOLUME_KEY = "BGMVolume";
    const string SFX_VOLUME_KEY = "SFXVolume";    
    const string MOVESPEED_KEY= "MoveSpeed";
    const string ROTATESPEED_KEY= "RotateSpeed";

    public void OnSaveSettings()
    {
        // 슬라이더 값 저장
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, m_bgmVolumeSlider.mainSlider.value);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, m_sfxVolumeSlider.mainSlider.value);

        // 실제 음량 조절
        SoundManager.Instance.SetVolume(m_bgmVolumeSlider.mainSlider.value, m_sfxVolumeSlider.mainSlider.value);

        // 설정 저장 완료
        PlayerPrefs.Save();
        Debug.Log("Settings Saved!");

        // 설정 창 닫기
        CloseSettings();
    }
    public void OnCancelSettings()
    {
        // 임시로 저장된 값으로 복원
        m_bgmVolumeSlider.mainSlider.value = m_tempFullVolume;
        m_sfxVolumeSlider.mainSlider.value = m_tempEffectVolume;

        Debug.Log("Settings Canceled!");

        // 설정 창 닫기
        CloseSettings();
    }
    public void OnOpenSettings()
    {
        // 설정 창 활성화
        gameObject.SetActive(true);

        // 현재 값을 임시 저장
        m_tempFullVolume = m_bgmVolumeSlider.mainSlider.value;
        m_tempEffectVolume = m_sfxVolumeSlider.mainSlider.value;

        Debug.Log("Settings Window Opened");
    }
    void CloseSettings()
    {
        gameObject.SetActive(false);
    }
}
