using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

// 꼬치
public class SkewerStick : MonoBehaviour
{
    [SerializeField] private Transform m_start;
    [SerializeField] private Transform m_end;

    public List<SkewerIngredient.IngredientType> m_skewerIngredients = new List<SkewerIngredient.IngredientType>();

    public bool checkAnswer = false;
    public bool playeOnce = true;
    /// <summary>
    /// 이 꼬치에 ingredient를 추가하는 함수.
    /// </summary>
    /// <param name="ingredient">꼬치에 추가할 재료</param>
    /// <returns> 리스트의 인덱스 </returns>
    public void AddToSkewerStick(SkewerIngredient ingredient)
    {
        // 방향 조정
        ingredient.transform.rotation = transform.rotation;
        ingredient.transform.Rotate(0, 90f, 90f);       

        Rigidbody rb = ingredient.transform.GetComponent<Rigidbody>();
        if(rb != null)
        {
            // 물리로는 z축으로만 이동이 가능. 모든 축의 회전과 X, Y축의 이동을 막음.
            // -꼬치안에서 재료끼리 충돌했을 때, z축으로만 이동하도록 처리 + 중력으로 인한 이동을 막음
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;
        }
    }
    public void RemoveFromSkewerStcik(SkewerIngredient ingredient)
    {
        Rigidbody rb = ingredient.transform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
    /// <summary>
    /// Ray를 쏴서 현재 꼬치에 꽂혀있는 Ingredient들을 m_skewerIngredients리스트에 추가하는 함수
    /// </summary>
    public void CheckAnswer()
    {
        // Ray를 쏘아서 결과를 배열로 저장
        RaycastHit[] hits = Physics.RaycastAll(m_start.position, (m_end.position - m_start.position).normalized, Vector3.Distance(m_start.position, m_end.position));

        // 거리순으로 정렬
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        // 충돌된 객체들 확인
        foreach (RaycastHit hit in hits)
        {
            SkewerIngredient ingredient = hit.collider.GetComponent<SkewerIngredient>();
            if (ingredient != null)
            {
                // skewerIngredient 스크립트를 가진 객체를 리스트에 추가
                m_skewerIngredients.Add(ingredient.m_ingredientType);
            }
        }
        // GameManager의 IsCorrect함수를 통해 내 m_skwerIngredients리스트가 정답이 맞는지 확인
        if (GameManager.Instance.SkewerIsCorrect(m_skewerIngredients))
        {
            Debug.Log("정답입니다.");
        }
        else
        {
            Debug.Log("정답이 아닙니다.");
        }
        // m_skewerIngredients 초기화
        m_skewerIngredients.Clear();
    }
    private void Update()
    {
        if(checkAnswer && playeOnce)
        {
            CheckAnswer();
            playeOnce = false;
        }
    }
}

