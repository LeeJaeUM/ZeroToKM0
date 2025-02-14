using UnityEngine;

public class TestLogInCkeak : MonoBehaviour
{
    void Awake()
    {
        FBManager._instance.TestCurrentUser();
    }
}
