using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

// 꼬치
public class SkewerStick : MonoBehaviour
{
    public List<SkewerIngredient> m_skeweredList;      // 꼬치에 꽂아진 재료의 리스트( 정답 확인 용 )
    public Transform[] m_cubeIngredientPos;         // 큐브형 재료를 넣을 위치
    public Transform[] m_stripIngredientPos;        // 스트립형 재료를 넣을 위치

    public int m_cubeCount = 0;         // 몇번째 위치에 추가할지

    /// <summary>
    /// 이 꼬치에 ingredient를 추가하는 함수.
    /// </summary>
    /// <param name="ingredient">꼬치에 추가할 재료</param>
    public void AddToSkewerStick(SkewerIngredient ingredient)
    {
        m_skeweredList.Add(ingredient);

        StripIngredient stripIngredient = ingredient.transform.GetComponent<StripIngredient>();

        // ingredient가 strip모양일 때( 치즈, 베이컨 )
        if(stripIngredient != null )
        {
            // 위치, 방향 조정
            stripIngredient.transform.position = m_stripIngredientPos[m_cubeCount].position;
            stripIngredient.transform.rotation = transform.rotation;
            stripIngredient.transform.Rotate(0, 90f, 90f);

        }

        // ingredient가 cube모양일 때( 고기, 피망, 새우, 토마토 )
        else
        {
            // 위치, 방향 조정
            ingredient.transform.position = m_cubeIngredientPos[m_cubeCount].position;
            ingredient.transform.rotation = transform.rotation;

            // 재료 모양별로 개수 관리
            m_cubeCount++;
        }


        Rigidbody rb = ingredient.transform.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.isKinematic = true;
        }

    }
}
