using UnityEngine;

public class KushiExpressCard : Card
{
    public KushiExpressIngredient.IngredientType[] m_answer;        // ������ ���� �迭
    public int m_score;                                             // ����, ������ �� ����� ����

    public void Intialize(int score)
    {
        m_score = score;
        m_answer = new KushiExpressIngredient.IngredientType[m_score];        // ������ ����� ������ �����Ƿ� ������ŭ �迭 ũ�⸦ �Ҵ�
    }
}
