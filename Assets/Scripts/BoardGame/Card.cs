using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Card : MonoBehaviour
{
    public void RotateCard()              // 카드를 뒤집어주는 함수
    {
        transform.Rotate(Vector3.right * 180);
    }
}
