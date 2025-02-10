using Michsky.MUIP;
using TMPro;
using UnityEngine;
using System;

[Serializable]
public class PlayerResult
{
    public string playerName;   // 유저명
    public bool isWinner;       // 승리 여부
    public int winCount;        // 총 승리 횟수
    public int totalGames;      // 총 게임 횟수
}

public class GameResultController : MonoBehaviour
{
    [SerializeField] GameObject m_modalWindow;      // Modal Window
    [SerializeField] Transform m_listParent;        // List View의 List (Item 부모)
    [SerializeField] GameObject m_listItemPrefab;   // List View Item Prefab (Item)
    [SerializeField] TMP_Text m_turnsText;          // 진행한 턴 횟수 텍스트
    
    ModalWindowManager m_modalWindowManager;        // Modal Window Manager

    /// <summary>
    /// 게임 결과를 표시합니다.
    /// </summary>
    /// <param name="results">플레이어 결과 배열</param>
    /// <param name="totalTurns">진행된 턴 횟수</param>
    public void ShowGameResult(PlayerResult[] results, int totalTurns)
    {
        m_modalWindow.SetActive(true);
        m_modalWindowManager.OpenWindow();

        // 기존 List View Item 삭제
        foreach (Transform child in m_listParent)
        {
            Destroy(child.gameObject);
        }

        // 새로운 Item 추가
        foreach(PlayerResult result in results)
        {
            // List Item 생성
            GameObject listItem = Instantiate(m_listItemPrefab, m_listParent);

            // Row 1: 유저명
            Transform row = listItem.transform.Find("Row 1");
            TMP_Text rowText = row.Find("Text").GetComponent<TMP_Text>();
            rowText.text = result.playerName;

            // Row 2: 승/패 여부
            row = listItem.transform.Find("Row 2");
            rowText = row.Find("Text").GetComponent<TMP_Text>();
            rowText.text = result.isWinner ? "Win" : "Lose";

            // Row 3: 승리 횟수
            row = listItem.transform.Find("Row 3");
            rowText = row.Find("Text").GetComponent<TMP_Text>();
            rowText.text = $"{result.winCount}/{result.totalGames}";
        }

        // 전체 회차
        m_turnsText.text = $"Round : {totalTurns}";
    }
    
    public void OnCofirm()
    {
        Debug.Log("Click Confirm");
    }

    public void Test()
    {
        // TODO: 예제 데이터 (2025.01.17)
        PlayerResult[] results = new PlayerResult[]
        {
            new PlayerResult { playerName = "Player1", isWinner = true, winCount = 1, totalGames = 3 },
            new PlayerResult { playerName = "Player2", isWinner = false, winCount = 0, totalGames = 3 },
            new PlayerResult { playerName = "Player3", isWinner = false, winCount = 0, totalGames = 3 },
            new PlayerResult { playerName = "Player4", isWinner = false, winCount = 2, totalGames = 3 },
        };

        ShowGameResult(results, 3);
    }

    void Start()
    {
        m_modalWindowManager = m_modalWindow.GetComponent<ModalWindowManager>();
        m_modalWindow.SetActive(false);
    }
}
