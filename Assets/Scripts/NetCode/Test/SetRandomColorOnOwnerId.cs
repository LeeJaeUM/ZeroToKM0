using System;
using Unity.Netcode;
using UnityEngine;
/// <summary>
/// A basic example of setting the color of a GameObject based on the owner's client ID.
/// If you want to modify this Script please copy it into your own project and add it to your Player Prefab.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class SetRandomColor : NetworkBehaviour
{
    public Renderer bodyRenderer;
    /// <summary>
    /// Gets called when the <see cref="NetworkObject"/> gets spawned, message handlers are ready to be registered
    /// and the network is setup.
    /// Here we use it to set the color of the object based on the owner's client ID.
    /// </summary>
    /// 
    private NetworkVariable<Color> playerColor = new NetworkVariable<Color>(
          Color.white, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private static readonly Color[] playerColors = {
    Color.red, Color.blue, Color.green, Color.yellow, Color.cyan, Color.magenta
};
    ////public override void OnNetworkSpawn()
    ////{
    ////    base.OnNetworkSpawn();
    ////    SetColorBasedOnOwner();
    ////}
    public void OnEnable()
    {
          //  AssignColor();

        playerColor.OnValueChanged += OnColorChanged;
        OnColorChanged(Color.white, playerColor.Value); // 초기화
    }
    public void AssignColor(int playerNum)
    {
        int playerIndex = playerNum;
        if (IsServer)
            playerColor.Value = playerColors[playerIndex]; // 서버에서 색상 지정
        //else(!IsHost && IsClient)
        //{
        //    TestSetCOLORServerRpc();
        //}
    }

    [ServerRpc]
    void TestSetCOLORServerRpc(int a)
    {

    }

    private void OnColorChanged(Color oldColor, Color newColor)
    {
        if (bodyRenderer != null)
        {
            bodyRenderer.material.color = newColor;
        }
    }

    /// <summary>
    /// Get notified when the ownership of the object changes to change the color.
    /// </summary>
    /// <param name="previous">the previous owner</param>
    /// <param name="current">the current owner</param>
    protected override void OnOwnershipChanged(ulong previous, ulong current)
    {
        SetColorBasedOnOwner();
    }

    void SetColorBasedOnOwner()
    {
        // OwnerClientId is used here for debugging purposes. A live game should use a session manager to make sure
        // reconnecting players still get the same color, as client IDs could be reused for other clients between
        // disconnect and reconnect. See Boss Room for a session manager example.
        UnityEngine.Random.InitState((int)OwnerClientId);

        //GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV();
        bodyRenderer.material.color = UnityEngine.Random.ColorHSV();
    }
   public  void SetColor()
    {
        // OwnerClientId is used here for debugging purposes. A live game should use a session manager to make sure
        // reconnecting players still get the same color, as client IDs could be reused for other clients between
        // disconnect and reconnect. See Boss Room for a session manager example.
        UnityEngine.Random.InitState((int)OwnerClientId);

        //GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV();
        bodyRenderer.material.color = UnityEngine.Random.ColorHSV();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetColor();
        }
    }
}

