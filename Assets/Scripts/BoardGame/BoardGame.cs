using Unity.Netcode;
using UnityEngine;

public class BoardGame : NetworkBehaviour
{
    /// <summary>
    /// 게임이 시작할 때 호출할 함수
    /// </summary>
    public virtual void InitializeGame() { }
    /// <summary>
    /// 게임이 끝날 때 호출할 함수
    /// </summary>
    public virtual void EndGame() { }
}
