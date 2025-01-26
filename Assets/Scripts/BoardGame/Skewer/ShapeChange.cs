using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// 플레이어가 버튼 입력을 통해 ray를 쏴서 Strip형태의 재료의 모양을 바꿀 수 있게 하는 스크립트
/// </summary>
/// 
public class ShapeChange : MonoBehaviour
{
    Camera m_camera;
    GameObject m_scannedObj;
    public LayerMask skewerLayer;  // "skewer" 레이어를 지정할 수 있도록 public으로 추가


    /// <summary>
    /// Z버튼 입력을 통해 재료를 box모양으로 바꿈
    /// </summary>
    /// <param name="value">z버튼 입력</param>
    public void OnMakeBoxshape(InputValue value)
    {
        if(value.isPressed)
        {
            CheckForSkewerObject();
            if(m_scannedObj != null)
            {
                ShapeController shapeController = m_scannedObj.GetComponent<ShapeController>();
                if(shapeController != null)
                {
                    shapeController.SetShape(ShapeController.Shape.Box);
                }
                else
                {
                    Debug.Log("Can't change shape");
                }
            }
        }
    }
    /// <summary>
    /// X버튼 입력을 통해 재료를 S자 모양으로 바꿈
    /// </summary>
    /// <param name="value">X버튼 입력</param>
    public void OnMakeSshape(InputValue value)
    {
        if (value.isPressed)
        {
            CheckForSkewerObject();
            if (m_scannedObj != null)
            {
                ShapeController shapeController = m_scannedObj.GetComponent<ShapeController>();
                if (shapeController != null)
                {
                    shapeController.SetShape(ShapeController.Shape.S);
                }
                else
                {
                    Debug.Log("Can't change shape");
                }
            }
        }
    }
    /// <summary>
    /// C버튼 입력을 통해 재료를 기본 1자 모양으로 바꿈
    /// </summary>
    /// <param name="value">C버튼 입력</param>
    public void OnMakeIdleshape(InputValue value)
    {
        if (value.isPressed)
        {
            CheckForSkewerObject();
            if (m_scannedObj != null)
            {
                ShapeController shapeController = m_scannedObj.GetComponent<ShapeController>();
                if (shapeController != null)
                {
                    shapeController.SetShape(ShapeController.Shape.Idle);
                }
                else
                {
                    Debug.Log("Can't change shape");
                }
            }
        }
    }
    /// <summary>
    /// 레이캐스트를 통해 "skewer" 레이어에 속한 오브젝트를 검색하는 함수
    /// </summary>
    public void CheckForSkewerObject()
    {
        RaycastHit hit;
        Ray ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue()); // 마우스 포인터 위치로 레이 쏘기

        // 레이가 skewer 레이어에 맞은 경우
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, skewerLayer))
        {
            // 레이가 맞은 오브젝트를 m_scannedObj에 저장
            m_scannedObj = hit.collider.gameObject;

            // 여기에 추가적인 처리를 넣을 수 있음
            Debug.Log("Hit object: " + m_scannedObj.name);
        }
        else
        {
            m_scannedObj = null;  // 레이가 아무것도 맞지 않으면 null로 설정
        }
    }
    void Start()
    {
        m_camera = GetComponent<Camera>();
    }
}
