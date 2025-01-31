using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipeCard", menuName = "Skewer/SkewerAnswerCard")]
public class SkewerAnswerCard : ScriptableObject
{
    [SerializeField] private List<SkewerIngredient.IngredientType> m_answer; // 정답 재료 리스트
    [SerializeField] private int m_score;
    // 정답과 비교하는 함수
    public bool CheckAnswer(List<SkewerIngredient.IngredientType> playerIngredients)
    {
        if (playerIngredients.Count != m_answer.Count)
            return false;

        for (int i = 0; i < m_answer.Count; i++)
        {
            if (playerIngredients[i] != m_answer[i])
                return false;
        }
        return true;
    }
}
