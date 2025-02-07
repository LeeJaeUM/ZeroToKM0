using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonDontDestroy<T> : MonoBehaviour where T : SingletonDontDestroy<T>
{
    public static T Instance { get; private set; }

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = (T)this;
            OnAwake();
            Debug.Log($"[Singleton] {typeof(T)} 생성됨, DontDestroyOnLoad 적용");
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log($"[Singleton] {typeof(T)} 이미 존재하므로 삭제됨");
            Destroy(gameObject);
            return;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == this)
        {
            OnStart();
        }
    }
}
