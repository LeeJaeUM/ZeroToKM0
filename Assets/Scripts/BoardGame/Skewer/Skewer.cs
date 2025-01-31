using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

// 꼬치의 달인
public class Skewer : BoardGame
{
    [SerializeField] private GameObject m_skewerObjectPrefab;      // 꼬치의 달인에 사용되는 재료들과 꼬치
    [SerializeField] private List<GameObject> m_skewerObjects;
    private int m_playerCount;
    [SerializeField] private SkewerAnswerCard[] m_allAnswers;   // 모든 정답 카드 배열
    [SerializeField] private SkewerAnswerCard m_currentAnswer;  // 현재 정답
    [SerializeField] private SkewerStick[] m_sticks = new SkewerStick[4];
    [SerializeField] private Transform[] m_playerPos;                // skewerObject를 놓을 플레이어의 위치
    [SerializeField] private bool[] m_isObjectPrepared = new bool[4];              // 각 플레이어의 위치에 skwerObject가 준비가 되어있는지 확인.( 중복 방지용 )
    /// <summary>
    /// skewer게임을 시작할때 호출할 함수
    /// </summary>
    public override void InitializeGame()
    {                          
        m_playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        PrepareObjects();
        GetAnswerCard();
        FindSticks();
    }

    public override void EndGame()
    {
        for(int i = 0; i < m_skewerObjects.Count; i++)
        {
            Destroy(m_skewerObjects[i]);
            m_isObjectPrepared[i] = false;
        }
        m_skewerObjects.Clear();
    }
    /// <summary>
    /// Skewer게임을 하기 위해 필요한 오브젝트들을 플레이어수만큼 세팅
    /// </summary>
    private void PrepareObjects()
    {
        for(int i = 0 ; i < m_playerCount; i++)
        {
            if (!m_isObjectPrepared[i])
            {
                m_isObjectPrepared[i] = true;

                GameObject skewerObjects = Instantiate(m_skewerObjectPrefab);
                skewerObjects.transform.position = m_playerPos[i].position;
                skewerObjects.transform.rotation = m_playerPos[i].rotation;
                m_skewerObjects.Add(skewerObjects);
            }
        }
    }
    private void FindSticks()
    {
        m_sticks = GetComponentsInChildren<SkewerStick>();        
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
    void Awake()
    {
        // Resources 폴더에서 모든 RecipeCard 불러오기
        m_allAnswers = Resources.LoadAll<SkewerAnswerCard>("Data/Skewer/Answer");
    }
}
