using UnityEngine;
using Michsky.MUIP; // Modern UI Pack 네임스페이스
using UnityEngine.UI;

public class DropdownController : MonoBehaviour
{
    [SerializeField] CustomDropdown m_dropdown;     // Dropdown

    public void OnDropdownItemSelected(int selectedIndex)
    {
        string selectedOption = m_dropdown.selectedText.text;
        
        // TODO : 추후 해당 씬에 맞게 변경 필요 (2025.01.13)
        switch (selectedOption)
        {
            case "Halli Galli":
                Debug.Log("Halli Galli Scene");
                LoadScene.Instance.LoadSceneAsync(SceneState.Lobby);
                break;

            case "Skewer":
                Debug.Log("Skewer Scene");
                //LoadScene.Instance.LoadSceneAsync(SceneState.Game);
                break;

            case "Jenga":
                Debug.Log("Jenga Scene");
                //LoadScene.Instance.LoadSceneAsync(SceneState.Game);
                break;
            default:
                Debug.LogWarning($"'{selectedOption}'에 해당하는 씬이 정의되지 않았습니다.");
                break;
        }
    }
 
    void Awake()
    {
        
    }
}