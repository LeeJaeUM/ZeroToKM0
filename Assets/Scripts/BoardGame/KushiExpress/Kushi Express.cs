using System;
using UnityEngine;
using System.Collections.Generic;

// 꼬치의 달인
public class KushiExpress : MonoBehaviour
{
    public KushiExpressCard[] m_cards;
    public KushiExpressIngredient[] m_ingredients = new KushiExpressIngredient[24];
    public List<KushiExpressIngredient> m_kushi;
    public KushiExpressCard m_answerCard;
    public void GameSetting()
    {

                                                    //GameManager.Instance.Shuffle(m_cards);  // 카드 섞기
    }
    void Awake()
    {
        m_cards = GetComponentsInChildren<KushiExpressCard>();
        m_ingredients = GetComponentsInChildren<KushiExpressIngredient>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameSetting();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
