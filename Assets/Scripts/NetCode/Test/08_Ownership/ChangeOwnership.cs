using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class ChangeOwnership : NetworkBehaviour
{
    public List<HalliGalliCard>[] m_playerCard; // 각 플레이어의 카드 리스트
    public HalliGalliCard[] m_card; // 전체 카드 (56장)
    public int playerCount = 0; // 플레이어 수

    private void Start()
    {
        if (IsServer) // 서버에서만 실행
        {
            DistributeCards();
        }
    }

    // 카드 배분 및 소유권 설정
    [ServerRpc(RequireOwnership = false)]
    public void DistributeCards()
    {
        if (!IsServer) return;

        // 플레이어당 받을 카드 수 계산
        int cardsPerPlayer = m_card.Length / playerCount;

        // 플레이어별 카드 리스트 초기화
        m_playerCard = new List<HalliGalliCard>[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            m_playerCard[i] = new List<HalliGalliCard>();
        }

        // 카드 나누기
        for (int i = 0; i < m_card.Length; i++)
        {
            int playerIndex = i % playerCount;
            m_playerCard[playerIndex].Add(m_card[i]);

            // 소유권 이전
            NetworkObject netObj = m_card[i].GetComponent<NetworkObject>();
            if (netObj != null)
            {
                netObj.ChangeOwnership((ulong)playerIndex); // 해당 플레이어에게 소유권 이전
            }
        }
    }

    // 모든 카드의 소유권을 서버로 회수
    [ServerRpc(RequireOwnership = false)]
    public void ReclaimAllOwnership()
    {
        if (!IsServer) return;

        foreach (var card in m_card)
        {
            NetworkObject netObj = card.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                netObj.ChangeOwnership(NetworkManager.ServerClientId); // 서버로 소유권 반환
            }
        }
    }
}
