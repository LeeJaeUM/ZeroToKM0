using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Dealer : NetworkBehaviour
{
    public void Calculatecard(int cardCount, int playerCount, int[] playerCardCount)                            // 각 플레이어가 가질 카드의 수를 계산
    {
        int divide = cardCount / playerCount;    // 1인당 카드의 수
        int mod = cardCount % playerCount;
        for (int i = 0; i < playerCount; i++)             // 카드가 나누어 떨어지지 않을 경우, 첫번째 플레이어부터 한장씩 추가
        {
            playerCardCount[i] = divide + (mod > i ? 1 : 0);
        }
    }
    public void Shuffle(object[] obj)                           // Fisher-Yates 셔플 알고리즘
    {
        System.Random random = new System.Random();     // 난수 생성기
        for (int i = obj.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);                 // 0 ~ i 범위에서 랜덤 인덱스
            object temp = obj[i];                     // 현재 인덱스와 랜덤 인덱스를 스왑
            obj[i] = obj[j];
            obj[j] = temp;
        }
    }
}
