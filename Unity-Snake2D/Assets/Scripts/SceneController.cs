using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    #region Static Data
    public readonly string GamePlaySceneName = "GameplayScene";                         // Gameplay scene name.
    public readonly string MainMenuSceneName = "MainMenuScene";                         // Main Menu scene name.
    #endregion

    private static SceneController _instance = null;
    public static SceneController Instance => _instance;                                // Singleton

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else if(_instance != this)
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// Call this method to load new scene.
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName); 
    }
}
