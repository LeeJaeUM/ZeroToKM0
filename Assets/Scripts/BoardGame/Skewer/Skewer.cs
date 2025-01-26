using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

// 꼬치의 달인
public class Skewer : NetworkBehaviour
{
    [SerializeField] private GameObject[] m_skewerObjects;
    private int m_playerCount;

    /// <summary>
    /// skewer게임을 시작할때 호출할 함수
    /// </summary>
    public void GameSetting()
    {                          
        m_playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        PrepareObjects();
    }
    /// <summary>
    /// Skewer게임을 하기 위해 필요한 오브젝트들을 플레이어수만큼 세팅
    /// </summary>
    private void PrepareObjects()
    {
        for(int i = 0 ; i < m_playerCount; i++)
        {
            m_skewerObjects[i].SetActive(true);
        }
    }
    void OnEnable()
    {
        GameSetting();
    }
}
