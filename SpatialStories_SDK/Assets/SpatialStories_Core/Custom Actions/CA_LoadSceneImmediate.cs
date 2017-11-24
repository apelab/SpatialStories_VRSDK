using Gaze;
using UnityEngine.SceneManagement;

/// <summary>
/// This action loads a scene without any delay. (as soon as the belonging Gaze_Conditions are met)
/// </summary>

public class CA_LoadSceneImmediate : Gaze_AbstractBehaviour
{
    public string SceneName;

    protected override void OnTrigger()
    {
        SceneManager.LoadScene(SceneName);
    }
}