//using System;
//using UnityEngine;

//// 꼬치의 달인
//public class KushiExpress : MonoBehaviour
//{
//    public KushiExpressCard[] m_cards;
//    public KushiExpressIngredient[] m_ingredients = new KushiExpressIngredient[24];
//    public KushiExpressIngredient[] m_kushi;
//    public void GameSetting()
//    {
//        // 재료 배분
//        int k = 0;
//        for (int i = 0; i < 6; i++)
//        {
//            for(int j = 0; j < GameManager.Instance.PlayerCount; j++)
//            {
//                GameManager.Instance.m_playerManager.m_players[j].m_ingredients[i] = m_ingredients[k++];
//            }
//        }

//        m_kushi = new KushiExpressIngredient[4];    // todo : 꼬치를 플레이어에게 각각 하는게 나을지 여기에 다 하는게 나을지 고민해보기
//                                                    // kushiexpress 이름 kushi로 줄이기
//        //GameManager.Instance.Shuffle(m_cards);  // 카드 섞기
//    }
//    void Awake()
//    {
//        m_cards = GetComponentsInChildren<KushiExpressCard>();
//        m_ingredients = GetComponentsInChildren<KushiExpressIngredient>();
//    }
//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        GameSetting();
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//}
