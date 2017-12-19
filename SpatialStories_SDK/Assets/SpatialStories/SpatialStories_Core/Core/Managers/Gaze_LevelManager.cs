using Gaze;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gaze_LevelManager : MonoBehaviour
{
    private static Gaze_LevelManager instance = null;
    public static string targetSceneName;
    [HideInInspector]
    public string targetSceneNameInspector;
    private Gaze_SceneLoader sceneLoader;

    // Game Instance Singleton
    public static Gaze_LevelManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        //DontDestroyOnLoad(this.gameObject);
        sceneLoader = GameObject.FindObjectOfType<Gaze_SceneLoader>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public string getNextLevelName()
    {
        // if we're in the loading screen scene
        if (SceneManager.GetActiveScene().name.Equals(sceneLoader.loadingScreen))
        {
            return targetSceneName;
        }

        return sceneLoader.loadingScreen;
    }

    public void setNextLevelName(string _name)
    {
        // if we're NOT in the loading screen scene
        if (!SceneManager.GetActiveScene().name.Equals(sceneLoader.loadingScreen))
        {
            targetSceneName = _name;

            // show in inspector for debug purposes
            targetSceneNameInspector = targetSceneName;
        }
    }

    private void OnSceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
        sceneLoader = GameObject.FindObjectOfType<Gaze_SceneLoader>();
    }
}