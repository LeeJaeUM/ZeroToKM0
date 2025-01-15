using Michsky.MUIP;
using UnityEngine;
using UnityEngine.UI;

public class RoomListController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] GameObject m_itemPrefab;       // Test Item Prefab

    Transform m_itemListParent;                     // ItemList (Test Item 부모)
    RoomItemController m_selectedItem;              // 현재 선택된 Item

    /// <summary>
    /// 선택된 Item 설정
    /// </summary>
    /// <param name="item">선택된 Room Item</param>
    public void SetSelectedItem(RoomItemController item)
    {
        // 이전 선택 항목 해제
        if (m_selectedItem != null)
        {
            m_selectedItem.SetSelected(false);
        }

        // 새로 선택된 항목 설정
        m_selectedItem = item;
        m_selectedItem.SetSelected(true);
    }

    public void AddRoomItem(string roomName)
    {
        // Test Item Prefab 생성
        GameObject newItem = Instantiate(m_itemPrefab, m_itemListParent);
        newItem.name = roomName;

        // RoomItemController 연결
        RoomItemController m_itemController = newItem.GetComponent<RoomItemController>();
        if (m_itemController != null)
        {
            m_itemController.SetRoomListController(this);

            // TODO : 자식 Row들 값 추가, 매개변수 (2025.01.15)
            m_itemController.SetRoomData("roomTitle", "gameName", "1/2");
        }

        // Click Event 연결
        Button itemButton = newItem.GetComponent<Button>();
        if (itemButton != null)
        {
            itemButton.onClick.AddListener(m_itemController.OnItemSelected);
        }
    }

    public void OnEnterBtnClick()
    {
        if (m_selectedItem != null)
        {
            Debug.Log($"Entering Room: {m_selectedItem.name}");
            // TODO : 해당 방으로 Scene 넘어가도록 추가 필요(data 필요) (2025.01.15)
        }
    }

    /// <summary>
    /// 선택된 Item 반환
    /// </summary>
    public RoomItemController GetSelectedItem()
    {
        return m_selectedItem;
    }

    void Start()
    {
        m_itemListParent = GetComponent<Transform>();

        // TODO : database 활성화 시 - for문으로 방 개수만큼 호출 및 AddRoomItem 매개변수 추가하여 Row(항목 값) 추가 필요(2025.01.25)
        AddRoomItem("Room 1");
        AddRoomItem("Room 2");
        AddRoomItem("Room 3");
        AddRoomItem("Room 4");
        AddRoomItem("Room 5");
    }
}
