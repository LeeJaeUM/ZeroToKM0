using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Card : MonoBehaviour
{
    public void OpenCard()              // ī�带 �������ִ� �Լ�
    {
        transform.Rotate(Vector3.right * 180);
    }
}
