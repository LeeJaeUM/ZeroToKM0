using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SetUserColor : NetworkBehaviour
{
    public Renderer bodyRenderer;

    public int m_playerNum = 0;

    //private  Color[] PlayerColors = new Color[]
    //{
    //    new Color(173, 216, 230, 255), // 플레이어 0 → 연하늘색
    //    new Color(220, 60, 60, 255),   // 플레이어 1 → 덜 쨍한 빨강
    //    new Color(144, 238, 144, 255), // 플레이어 2 → 연한 초록
    //    new Color(186, 85, 211, 255)   // 플레이어 3 → 연보라색
    //};

    [SerializeField]private Color[] PlayerColors = new Color[]
{
    new Color(173f / 255f, 216f / 255f, 230f / 255f, 1f), // 플레이어 0 → 연하늘색
    new Color(220f / 255f, 60f / 255f, 60f / 255f, 1f),   // 플레이어 1 → 덜 쨍한 빨강
    new Color(144f / 255f, 238f / 255f, 144f / 255f, 1f), // 플레이어 2 → 연한 초록
    new Color(186f / 255f, 85f / 255f, 211f / 255f, 1f)   // 플레이어 3 → 연보라색
};


    [ClientRpc]
    private void SetColorClientRpc()
    {
        SettingColor(m_playerNum);
    }
    public void SetColorBasedOnOwner()
    {
        //    SettingColor(m_playerNum);
        if (IsServer)
        {
            Debug.Log($"서버에서 색깔 설정: 플레이어 {m_playerNum}");

            // m_playerNum을 기반으로 색상을 설정
            Color newColor = PlayerColors[m_playerNum];

            // NetworkVariable을 통해 모든 클라이언트에 색상 동기화
            bodyRenderer.material.color  = newColor;
            SetColorClientRpc();
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        GameManager.Instance.m_onGameEnd += SetColorBasedOnOwner; 
        
        if (IsServer)
        {
            SetColorBasedOnOwner();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestSetColorServerRpc(int playerNum)
    {
        SettingColor(playerNum);
    }


    private void SettingColor(int playerNum)
    {
        bodyRenderer.material.color = PlayerColors[playerNum];
        //UnityEngine.Random.InitState(playerNum);
        //ObjectColor.Value = UnityEngine.Random.ColorHSV();
    }
    #region 기존함수



    //private void SettingColor()
    //{
    //    if (playerNum == (int)NetworkManager.Singleton.LocalClientId)
    //    {
    //        UnityEngine.Random.InitState((int)NetworkManager.Singleton.LocalClientId);

    //        //GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV();
    //        bodyRenderer.material.color = UnityEngine.Random.ColorHSV();
    //    }
    //}
    //private void SettingColor(int playerNum)
    //{
    //    if (playerNum == (int)NetworkManager.Singleton.LocalClientId)
    //    {
    //        UnityEngine.Random.InitState((int)NetworkManager.Singleton.LocalClientId);

    //        //GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV();
    //        bodyRenderer.material.color = UnityEngine.Random.ColorHSV();
    //    }
    //}
    #endregion
    private void TestAc()
    {
        Debug.Log("액션 받음");
    }
    private void Start()
    {
        bodyRenderer = GetComponent<Renderer>();
        // NetworkVariable 값이 변경될 때 색상 업데이트


    }

   

}
