using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelayUI : MonoBehaviour
{
    public RelayTest1 relayManager;
    public TMP_InputField joinCodeInputField; // Join Code 입력 필드
    public TextMeshProUGUI joinCodeDisplay;// 생성된 Join Code 표시용 텍스트
    public Button createRelayButton; // Relay 생성 버튼
    public Button joinRelayButton; // Relay 참가 버튼
    private void Start()
    {
        // 버튼 클릭 이벤트 리스너 추가
        createRelayButton.onClick.AddListener(CreateRelaySession);
        joinRelayButton.onClick.AddListener(JoinRelaySession);
    }
    public async void CreateRelaySession()
    {
        // Relay 세션 생성
        string joinCode = await relayManager.CreateRelay();
        if (!string.IsNullOrEmpty(joinCode))
        {
            joinCodeDisplay.text = $"Join Code: {joinCode}"; // 생성된 Join Code 표시
            Debug.Log($"Relay Session Created! Join Code: {joinCode}");
        }
        else
        {
            joinCodeDisplay.text = "Failed to create Relay session.";
        }
    }

    public async void JoinRelaySession()
    {
        // Join Code를 입력받아 Relay 세션에 참가
        string joinCode = joinCodeInputField.text;
        if (!string.IsNullOrEmpty(joinCode))
        {
            await relayManager.JoinRelay(joinCode);
            Debug.Log($"Joined Relay Session with Code: {joinCode}");
        }
        else
        {
            Debug.LogError("Join Code is empty. Please enter a valid Join Code.");
        }
    }
}
