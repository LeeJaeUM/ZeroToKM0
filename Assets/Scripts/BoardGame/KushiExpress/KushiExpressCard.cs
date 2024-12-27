using UnityEngine;

public class KushiExpressCard : Card
{
    public KushiExpressIngredient.IngredientType[] m_answer;        // 정답을 넣을 배열
    public int m_score;                                             // 점수, 점수가 곧 재료의 개수

    public void Intialize(int score)
    {
        m_score = score;
        m_answer = new KushiExpressIngredient.IngredientType[m_score];        // 점수가 재료의 개수와 같으므로 점수만큼 배열 크기를 할당
    }
}
