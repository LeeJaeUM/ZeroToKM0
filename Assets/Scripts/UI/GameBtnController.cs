using UnityEngine;

public class GameBtnController : MonoBehaviour
{
    public void OnPowerBtnClick()
    {
        Debug.Log("Exiting the game");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void OnRestartBtnClick()
    {
        // TODO : Restart 함수 추가 필요 (2025.01.20) 임시로 할리갈리만 추가
        GameManager.Instance.StartHalliGalli();
    }
    public void OnFlipBtnClick()
    {
        // TODO : Flip 함수 추가 필요 (2025.01.20)
    }
}
