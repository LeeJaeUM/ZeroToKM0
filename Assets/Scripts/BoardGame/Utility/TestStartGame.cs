using System;
using UnityEngine;
using Unity.Netcode;

public class TestStartGame : NetworkBehaviour
{
    public int m_testplayerNum = 0; 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // 1 키를 눌렀을 때
        {
            if (IsServer)
                StartGame();
            else
                Debug.Log("서버만 게임을 시작할 수 있습니다.");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) // 2 키를 눌렀을 때
        {
            GameManager.Instance.NextTurn(m_testplayerNum);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) // 2 키를 눌렀을 때
        {
            GameManager.Instance.OpenCard(m_testplayerNum);
        }
    }

    private void StartGame()
    {
        GameManager.Instance.m_halligalli.InitializeGame();
    }
}
