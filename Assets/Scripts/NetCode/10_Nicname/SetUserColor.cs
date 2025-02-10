using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SetUserColor : MonoBehaviour
{
    public Renderer bodyRenderer;

    public void SetColorBasedOnOwner()
    {
        // OwnerClientId is used here for debugging purposes. A live game should use a session manager to make sure
        // reconnecting players still get the same color, as client IDs could be reused for other clients between
        // disconnect and reconnect. See Boss Room for a session manager example.
        UnityEngine.Random.InitState((int)NetworkManager.Singleton.LocalClientId);

        //GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV();
        bodyRenderer.material.color = UnityEngine.Random.ColorHSV();
    }

    private void Start()
    {
        bodyRenderer = GetComponent<Renderer>();
    }

}
