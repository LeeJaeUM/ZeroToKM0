using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneState
{
    None = -1,
    Title,
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

    public void LoadSceneAsync(SceneState scene)
    {
        if (m_loadScene != SceneState.None) return;
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
