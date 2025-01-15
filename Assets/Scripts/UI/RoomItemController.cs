using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItemController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject m_border; // Border 이미지 (테두리)
    RoomListController m_roomListController;

    [SerializeField] TextMeshProUGUI m_roomTitle;   // Row 1의 방 제목
    [SerializeField] TextMeshProUGUI m_playerCount; // Row 2의 인원 수
    [SerializeField] TextMeshProUGUI m_gameName;    // Row 3의 현재 게임 이름

    /// <summary>
    /// RoomListController 설정
    /// </summary>
    public void SetRoomListController(RoomListController controller)
    {
        m_roomListController = controller;
    }

    /// <summary>
    /// Room 데이터를 UI에 적용합니다.
    /// </summary>
    /// <param name="roomTitle">방 제목</param>
    /// <param name="playerCount">현재 인원 수</param>
    /// <param name="gameName">현재 게임 이름</param>
    public void SetRoomData(string roomTitle, string playerCount, string gameName)
    {
        if (m_roomTitle != null)
        {
            m_roomTitle.text = roomTitle;
        }
        if (m_playerCount != null)
        {
            m_playerCount.text = playerCount;
        }
        if (m_gameName != null)
        {
            m_gameName.text = gameName;
        }
    }

    public void SetSelected(bool isSelected)
    {
        // 테두리 활성화/비활성화
        if (m_border != null)
        {
            m_border.SetActive(isSelected);
        }
    }

    public void OnItemSelected()
    {
        if (m_roomListController != null)
        {
            // 선택된 상태를 RoomListController에 알림
            m_roomListController.SetSelectedItem(this);
        }
        
    }

    void Start()
    {
        // Border 기본 비활성화
        if (m_border != null)
        {
            m_border.SetActive(false);
        }    
    }
}
