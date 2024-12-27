using UnityEngine;

public class BoardGame : MonoBehaviour
{
    public virtual void GameSetting() { }   // 게임 시작 전에 호출하여 필요한 것들을 세팅해놓는 함수
                                            // 게임 시작 전 호출할 함수들을 모아놓음.
    public virtual void GameOver() { }
}
