using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneState
{
    None = -1,
    Title,
    Login,
    Lobby,
    Game,
    Max
}

public class LoadScene : SingletonDontDestroy<LoadScene>
{
    AsyncOperation m_loadState;
    SceneState m_state = SceneState.Title;
    SceneState m_loadScene = SceneState.None;

    public SceneState GetScene { get { return m_state; } }

    /// <summary>
    /// enum타입 SceneState의 현재 상태에 따라 씬 이등
    /// </summary>
    /// <param name="scene"></param>
    public void LoadSceneAsync(SceneState scene)
    {
        m_loadScene = scene;
        m_loadState = SceneManager.LoadSceneAsync((int)scene);
    }

    protected override void OnStart()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
