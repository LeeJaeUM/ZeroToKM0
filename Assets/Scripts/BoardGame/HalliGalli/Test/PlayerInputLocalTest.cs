using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

// Local OpenCard 테스트용.  m_playerNum, camera Inspector에서 설정하고, OpenCard가능.
public class PlayerInputLocalTest : MonoBehaviour
{
    public Camera m_camera; // 플레이어 카메라 참조
    private Card m_draggedCard;
    private HalliGalliCard m_draggedHalliGalliCard;
    public int m_playerNum;        // player번호

    public void OnTestOpenCard(InputValue value)
    {
        if (value.isPressed)
        {
            TestFlip();
        }
    }

    private void TestFlip()
    {
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            m_draggedCard = hit.collider.GetComponent<Card>();
            if (m_draggedCard != null)
            {
                m_draggedCard.OpenCard(m_playerNum);
                m_playerNum++;
                if(m_playerNum == 4)
                    m_playerNum = 0;
            }
        }
    }
}
