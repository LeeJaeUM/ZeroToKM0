using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool<T> where T : Component
{
    Queue<T> m_pool;
    int m_count;              // 몇 개를 만들지
    Func<T> m_createFunc;   // 어떻게 만들지
    public GameObjectPool(int count, Func<T> createFunc)
    {
        m_count = count;
        m_createFunc = createFunc;
        m_pool = new Queue<T>(count);
        Allocate();
    }
    void Allocate() // 메모리 할당하는 함수
    {
        for(int i = 0; i < m_count; i++)
        {
            m_pool.Enqueue(m_createFunc()); // createFunc의 반환값을 pool에다가 하나씩 쌓아준다.
        }
    }
    public T Get()  // pool에서 객체를 빼오는 함수
    {
        if(m_pool.Count > 0)
        {
            return m_pool.Dequeue();
        }
        else
        {
            return m_createFunc();
        }
    }
    public void Set(T obj)  // pool에 객체를 다시 넣는 함수
    {
        m_pool.Enqueue(obj);
    }
}
