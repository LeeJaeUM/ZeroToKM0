using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SetUserColor : NetworkBehaviour
{
    public Renderer bodyRenderer;

    public int m_playerNum = 0;

    private static readonly Color32[] PlayerColors = new Color32[]
    {
        new Color32(173, 216, 230, 255), // 플레이어 0 → 연하늘색
        new Color32(220, 60, 60, 255),   // 플레이어 1 → 덜 쨍한 빨강
        new Color32(144, 238, 144, 255), // 플레이어 2 → 연한 초록
        new Color32(186, 85, 211, 255)   // 플레이어 3 → 연보라색
    };


    public void SetColorBasedOnOwner()
    {
            Debug.Log("이 함수");

        SettingColor(m_playerNum);
        //if (IsServer)
        //{
        //    Debug.Log("서버에서 색깔 세팅");
        //    SettingColor(m_playerNum);
        //}
        //else if (IsClient && !IsHost)
        //{
        //    Debug.Log("클라이언트에서 색깔 세팅");
        //    RequestSetColorServerRpc(m_playerNum);
        //}


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

    private void Start()
    {
        bodyRenderer = GetComponent<Renderer>();
    }

}
