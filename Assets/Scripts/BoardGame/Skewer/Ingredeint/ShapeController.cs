using UnityEngine;

/// <summary>
/// Strip타입 Ingredient들의 모양을 애니메이션으로 컨트롤해주는 스크립트
/// 종류 :
/// 1. Idle(기본 1자형)
/// 2. Box모양
/// 3. S자(1단계, 2단계 있음)
/// </summary>
public class ShapeController : MonoBehaviour
{
    /// <summary>
    /// Strip형 재료의 현재 모양을 정의
    /// </summary>
    public enum Shape
    {
        Idle,
        Box,
        S
    }
    public Shape m_shape;
    public Animator m_animator;
    public int m_sStepNum = 0;      // Box나 S자로 모양을 바꿀때의 단계를 나타내줌.(2단계까지 존재)

    /// <summary>
    /// strip형 재료의 모양을 바꿔주는 함수
    /// </summary>
    /// <param name="shape">어떤 모양으로 바꿀지</param>
    public void SetShape(Shape shape)
    {
        switch (shape)
        {
            case Shape.Idle:
                if (m_shape != Shape.Idle)  // Idle상태가 아닐때만 전환 가능
                {
                    MakeIdle();
                }
                break;
            case Shape.Box:
                if (m_shape == Shape.Idle)  // Idle상태에서만 전환 가능
                {
                    MakeBox();
                }
                break;
            case Shape.S:
                if (m_shape != Shape.Box && m_sStepNum != 2)    // Box상태가 아니고, 단계가 2가 아닐때 가능
                {
                    MakeS();
                }
                break;
        }
    }
    private void MakeIdle()
    {
        // 지금 모양이 S이고, 그 단계가 2인 경우를 제외하고 m_shape를 Idle로 바꿈.
        // 반대 경우 m_shape를 그대로 둠.(S인 상태로)
        if (!(m_shape == Shape.S && m_sStepNum == 2))
        {
            m_shape = Shape.Idle;
        }

        m_sStepNum--;
        m_animator.SetTrigger("MakeIdle");
    }
    private void MakeBox()
    {
        m_shape = Shape.Box;
        m_animator.SetTrigger("MakeBox");
    }
    private void MakeS()
    {
        m_shape = Shape.S;
        m_sStepNum++;
        m_animator.SetTrigger("MakeS");
    }
    void Start()
    {
        m_animator = GetComponent<Animator>();
    }
}
