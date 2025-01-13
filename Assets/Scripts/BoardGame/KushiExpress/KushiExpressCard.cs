using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class KushiExpressCard : Card
{
    public List<KushiExpressIngredient.IngredientType> m_answer;    // 정답을 넣을 배열
    public int m_score;                                             // 점수, 점수가 곧 재료의 개수

    public void Intialize(int score)
    {
        m_score = score;
    }
}
