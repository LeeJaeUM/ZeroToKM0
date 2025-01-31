using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

// 꼬치의 달인
public class Skewer : NetworkBehaviour
{
    [SerializeField] private GameObject[] m_skewerObjects;
    private int m_playerCount;
    [SerializeField] private SkewerAnswerCard[] m_allAnswers;   // 모든 정답 카드 배열
    [SerializeField] private SkewerAnswerCard m_currentAnswer;  // 현재 정답

    /// <summary>
    /// skewer게임을 시작할때 호출할 함수
    /// </summary>
    public void GameSetting()
    {                          
        m_playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        PrepareObjects();
        GetAnswerCard();
    }

    /// <summary>
    /// Skewer게임을 하기 위해 필요한 오브젝트들을 플레이어수만큼 세팅
    /// </summary>
    private void PrepareObjects()
    {
        for(int i = 0 ; i < m_playerCount; i++)
        {
            m_skewerObjects[i].SetActive(true);
        }
    }

    /// <summary>
    /// 정답카드를 뽑아주는 함수
    /// m_allAnswers 배열에서 랜덤하게 하나를 뽑아, m_currentAnswer에 값을 할당
    /// </summary>
    private void GetAnswerCard()
    {
        m_currentAnswer = m_allAnswers[Random.Range(0, m_allAnswers.Length)];
    }
    /// <summary>
    /// 정답을 체크하는 함수, 현재 정답 카드의 CheckAnswer함수를 통해 반환된 값을 다시 반환
    /// </summary>
    /// <param name="ingredientList">정답인지 확인할 리스트</param>
    /// <returns></returns>
    public bool IsCorrect(List<SkewerIngredient.IngredientType> ingredientList)
    {
        bool isCorrect = m_currentAnswer.CheckAnswer(ingredientList);
        // 정답이 맞으면 true를 반환하고, 정답을 바꿈
        if (isCorrect)
        {
            GetAnswerCard();
            return true;
        }
        // 정답이 아닐 경우 false 반환
        else
        {
            return false;
        }
    }

    void OnEnable()
    {
        GameSetting();
    }
    void Start()
    {
        // Resources 폴더에서 모든 RecipeCard 불러오기
        m_allAnswers = Resources.LoadAll<SkewerAnswerCard>("Data/Skewer/Answer");
    }
}
