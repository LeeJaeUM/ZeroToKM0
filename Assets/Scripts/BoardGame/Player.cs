using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public int m_playerNum;
    public HalliGalliNetwork m_halligalli;
    //Wpublic bool m_isMyTurn;             // 내 턴인지 체크, true일때만 opencard가능

    public NetworkVariable<bool> nm_isMyTurn = new NetworkVariable<bool>(false); // 내 턴인지 체크, true일때만 opencard가능

    // 꼬치의 달인
    public KushiExpressIngredient[] m_ingredients = new KushiExpressIngredient[6];
    void Update()
    {
        if (IsOwner)
        {
            if (Input.GetMouseButtonDown(1) && nm_isMyTurn.Value)    // OpenCard 체크용 // 현재 턴인지 체크는 PlayerNaamger에서 실행중
            {
                GameManager.Instance.OpenCard(m_playerNum);
                //OpenCardServerRpc();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager.Instance.RingBell(m_playerNum);
                //RingBellServerRpc();
            }
        }
        else if(IsServer)
        {
            if (Input.GetMouseButtonDown(1) && nm_isMyTurn.Value)    // OpenCard 체크용 // 현재 턴인지 체크는 PlayerNaamger에서 실행중
            {
                GameManager.Instance.OpenCard(m_playerNum);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager.Instance.RingBell(m_playerNum);
            }
        }


    }

    [ServerRpc]
    public void OpenCardServerRpc(ServerRpcParams rpcParams = default)
    {
        GameManager.Instance.OpenCard(m_playerNum);
    }

    [ServerRpc]
    public void RingBellServerRpc(ServerRpcParams rpcParams = default)
    {
        GameManager.Instance.RingBell(m_playerNum);
    }

    // 플레이어 번호를 클라이언트 ID로 매핑 (필요 시 구현)
    //private int GetPlayerNumber(ulong clientId)
    //{
    //    // 예: 클라이언트 ID와 플레이어 번호를 매핑하는 로직
    //    return (int)clientId; // 단순 매핑 예시
    //}
}