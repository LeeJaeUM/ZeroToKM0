using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SingletonNetworkBehaviour : NetworkBehaviour
{
    static SingletonNetworkBehaviour m_instance;
    public static SingletonNetworkBehaviour Instance { get { return m_instance; } private set { m_instance = value; } }
    protected virtual void OnAwake() { }

    protected virtual void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            OnAwake();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}