using Gaze;
using UnityEngine.SceneManagement;

public class CA_LoadSceneImmediate : Gaze_AbstractBehaviour
{
    public string SceneName;

    protected override void OnTrigger()
    {
        SceneManager.LoadScene(SceneName);
    }
}