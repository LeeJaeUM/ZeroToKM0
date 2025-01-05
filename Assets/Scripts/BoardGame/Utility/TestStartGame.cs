using System;
using UnityEngine;
using Unity.Netcode;

public class TestStartGame : NetworkBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // 1 키를 눌렀을 때
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        GameManager.Instance.m_halligalli.InitializeGame();
    }
}
