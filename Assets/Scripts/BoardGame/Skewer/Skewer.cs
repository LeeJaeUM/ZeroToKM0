using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

// 꼬치의 달인
public class Skewer : NetworkBehaviour
{
    public SkewerCard[] m_cards;
    public SkewerIngredient[] m_ingredients = new SkewerIngredient[24];
    public List<SkewerIngredient> m_kushi;
    public SkewerCard m_answerCard;
    public void GameSetting()
    {

                                                    //GameManager.Instance.Shuffle(m_cards);  // 카드 섞기
    }
    void Awake()
    {
        m_cards = GetComponentsInChildren<SkewerCard>();
        m_ingredients = GetComponentsInChildren<SkewerIngredient>();
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
