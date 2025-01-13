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
    //public void Shuffle(object[] obj)                           // Fisher-Yates 셔플 알고리즘
    //{
    //    System.Random random = new System.Random();     // 난수 생성기


    //    for (int i = obj.Length - 1; i > 0; i--)
    //    {
    //        int j = random.Next(i + 1);                 // 0 ~ i 범위에서 랜덤 인덱스
    //        object temp = obj[i];                     // 현재 인덱스와 랜덤 인덱스를 스왑
    //        obj[i] = obj[j];
    //        obj[j] = temp;
    //    }
    //}
    public int[] Shuffle(int length)    // 배열의 길이를 입력받음.
    {
        System.Random random = new System.Random();
        int[] shuffledIndexes = new int[length];

        // 인덱스를 랜덤하게 섞을 배열을 만든다.
        for (int i = 0; i < length; i++)
        {
            shuffledIndexes[i] = i;
        }

        for (int i = length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1); // 0 ~ i 범위에서 랜덤 인덱스
                                        // 인덱스를 섞는다.
            int temp = shuffledIndexes[i];
            shuffledIndexes[i] = shuffledIndexes[j];
            shuffledIndexes[j] = temp;
        }

        return shuffledIndexes;
    }

}
