using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;

public class Card : NetworkBehaviour
{
    public void OpenCard()              // 카드를 뒤집어주는 함수
    {
        transform.Rotate(Vector3.right * 180);
    }
}
