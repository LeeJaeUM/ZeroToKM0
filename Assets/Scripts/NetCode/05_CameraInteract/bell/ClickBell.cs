using UnityEngine;
using UnityEngine.InputSystem;

public class ClickBell : MonoBehaviour
{
    private Camera m_camera; // 플레이어 카메라 참조
    public void OnClick(InputValue value)
    {
        if (value.isPressed)
        {
            RingBells();
        }
    }


    private void RingBells()
    {
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Card 컴포넌트가 있다면 추가 처리
            BellObject bell = hit.collider.GetComponent<BellObject>();
            if (bell != null)
            {
                print("Flip");
                bell.ClickedBell();
            }
        }
    }
    private void Start()
    {
        m_camera = GetComponent<Camera>(); // 플레이어 카메라 초기화
    }
}
