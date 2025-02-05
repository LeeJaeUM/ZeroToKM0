using Unity.Netcode.Components;
using UnityEngine;

//클라이언트에게 권한을 주는 NetworkTransform 아직 사용예정 없음

[DisallowMultipleComponent]
public class MyClientNetworkTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
