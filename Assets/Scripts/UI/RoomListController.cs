using Michsky.MUIP;
using UnityEngine;
using UnityEngine.UI;

public class RoomListController : MonoBehaviour
{
    [Header("Room List Settings")]
    [SerializeField] private Transform itemListParent; // ItemList (Test Item 부모)
    //[SerializeField] private Button enterButton;       // Enter 버튼
    [SerializeField] private Color selectedColor = Color.cyan;  // 선택된 항목 색상
    [SerializeField] private Color defaultColor = Color.white;  // 기본 항목 색상

    private GameObject selectedItem = null; // 현재 선택된 Room Item

    private void Start()
    {
        // 초기 상태에서 Enter 버튼 비활성화
        //enterButton.interactable = false;

        // Enter 버튼 클릭 이벤트 설정
        //enterButton.onClick.AddListener(OnEnterButtonClick);
    }

    /// <summary>
    /// Test Item이 클릭되었을 때 호출
    /// </summary>
    /// <param name="clickedItem">클릭된 Room Item</param>
    public void OnRoomItemClick(GameObject clickedItem)
    {
        // 이전 선택 항목 초기화
        if (selectedItem != null)
        {
            SetItemColor(selectedItem, defaultColor);
        }

        // 새로 선택된 항목 업데이트
        selectedItem = clickedItem;
        SetItemColor(selectedItem, selectedColor);

        // Enter 버튼 활성화
        //enterButton.interactable = true;

        Debug.Log($"Selected Room: {selectedItem.name}");
    }

    /// <summary>
    /// Enter 버튼 클릭 시 호출
    /// </summary>
    private void OnEnterButtonClick()
    {
        if (selectedItem != null)
        {
            Debug.Log($"Entering Room: {selectedItem.name}");
            // 방 입장 처리 로직 추가
            EnterRoom(selectedItem.name);
        }
    }

    /// <summary>
    /// 선택된 Room Item의 색상을 변경
    /// </summary>
    private void SetItemColor(GameObject item, Color color)
    {
        Image itemImage = item.GetComponent<Image>();
        if (itemImage != null)
        {
            itemImage.color = color;
        }
    }

    /// <summary>
    /// 방 입장 처리 (예제)
    /// </summary>
    /// <param name="roomName">방 이름</param>
    private void EnterRoom(string roomName)
    {
        Debug.Log($"Room '{roomName}'에 입장했습니다!");
        // 실제 입장 로직 추가 가능
    }
}