using UnityEngine;
using UnityEngine.UI;
using TMPro;        // TextMeshPro용 네임스페이스
using Michsky.MUIP; // Modern UI Pack 네임스페이스

public class SettingsController : MonoBehaviour
{
    [SerializeField] SliderManager m_fullVolumeSlider;     // 전체 볼륨 슬라이더
    [SerializeField] SliderManager m_effectVolumeSlider;   // 효과음 볼륨 슬라이더

    // 임시로 값을 저장하는 변수
    float m_tempFullVolume;
    float m_tempEffectVolume;
    bool m_tempOutlineState;
    bool m_tempUsernameState;

    // 저장된 설정 값 (PlayerPrefs 키)
    const string FULL_VOLUME_KEY = "FullVolume";
    const string EFFECT_VOLUME_KEY = "EffectVolume";
    const string OUTLINE_SWITCH_KEY = "OutlineSwitch";
    const string USERNAME_SWITCH_KEY = "UsernameSwitch";

    public void OnSaveSettings()
    {
        // 슬라이더 값 저장
        PlayerPrefs.SetFloat(FULL_VOLUME_KEY, m_fullVolumeSlider.mainSlider.value);
        PlayerPrefs.SetFloat(EFFECT_VOLUME_KEY, m_effectVolumeSlider.mainSlider.value);

        // 설정 저장 완료
        PlayerPrefs.Save();
        Debug.Log("Settings Saved!");

        // 설정 창 닫기
        CloseSettings();
    }
    public void OnCancelSettings()
    {
        // 임시로 저장된 값으로 복원
        m_fullVolumeSlider.mainSlider.value = m_tempFullVolume;
        m_effectVolumeSlider.mainSlider.value = m_tempEffectVolume;

        Debug.Log("Settings Canceled!");

        // 설정 창 닫기
        CloseSettings();
    }
    public void OnOpenSettings()
    {
        // 설정 창 활성화
        gameObject.SetActive(true);

        // 현재 값을 임시 저장
        m_tempFullVolume = m_fullVolumeSlider.mainSlider.value;
        m_tempEffectVolume = m_effectVolumeSlider.mainSlider.value;

        Debug.Log("Settings Window Opened");
    }
    void CloseSettings()
    {
        gameObject.SetActive(false);
    }
}
