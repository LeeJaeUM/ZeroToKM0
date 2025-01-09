using UnityEngine;
using UnityEngine.UI;
using TMPro;        // TextMeshPro용 네임스페이스
using Michsky.MUIP; // Modern UI Pack 네임스페이스

public class SettingsController : MonoBehaviour
{
    [SerializeField] SliderManager m_fullVolumeSlider;     // 전체 볼륨 슬라이더
    [SerializeField] SliderManager m_effectVolumeSlider;   // 효과음 볼륨 슬라이더
    
    [SerializeField] SwitchManager m_outlineSwitch;        // 외곽선 ON/OFF 스위치
    [SerializeField] SwitchManager m_usernameSwitch;       // 닉네임 표시 ON/OFF 스위치

    // 임시로 값을 저장하는 변수
    float m_tempFullVolume;
    float m_tempEffectVolume;
    bool m_tempOutlineState;
    bool m_tempUsernameState;

    const string FULL_VOLUME_KEY = "FullVolume";
    const string EFFECT_VOLUME_KEY = "FullVolume";
    const string OUTLINE_TOGGLE_KEY = "FullVolume";
    const string USERNAME_TOGGLE_KEY = "FullVolume";

    public void OnSaveSettings()
    {
        // 슬라이더 값 저장
        PlayerPrefs.SetFloat(FULL_VOLUME_KEY, m_fullVolumeSlider.mainSlider.value);
        PlayerPrefs.SetFloat(EFFECT_VOLUME_KEY, m_effectVolumeSlider.mainSlider.value);

        // 스위치 값 저장
        PlayerPrefs.SetInt(OUTLINE_TOGGLE_KEY, m_outlineSwitch.isOn ? 1 : 0);
        PlayerPrefs.SetInt(USERNAME_TOGGLE_KEY, m_usernameSwitch.isOn ? 1 : 0);

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
        m_outlineSwitch.isOn = m_tempOutlineState;
        m_usernameSwitch.isOn = m_tempUsernameState;

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
        m_tempOutlineState = m_outlineSwitch.isOn;
        m_tempUsernameState = m_usernameSwitch.isOn;

        Debug.Log("Settings Window Opened");
    }
    void CloseSettings()
    {
        gameObject.SetActive(false);
    }
}
