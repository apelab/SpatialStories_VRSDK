using Gaze;
using UnityEngine;

/// <summary>
/// A persistent global identifier usefull in:
/// 1) Runtime.
/// 2) When gets stored.
/// 3) Before it's creation.
/// 4) When is stored.
/// </summary>
public class S_Guid : MonoBehaviour
{
#if UNITY_EDITOR
    [ReadOnly]
#endif
    public string GUID;

    public void SetGUID(string _guid)
    {
        GUID = _guid;
    }

#if UNITY_EDITOR
    // This is only used on the unity editor for ensure the integrity of the guids 
    const bool CHECK_GUIDS_INTEGRITY = true;
    private static S_Guid[] sceneGUIDs;
    private void Start()
    {
        if (!CHECK_GUIDS_INTEGRITY)
            return;

        // Test that the GUID's are unique, very important when developing the API in a future, this method will be deleted
        if (sceneGUIDs == null)
        {
            sceneGUIDs = FindObjectsOfType<S_Guid>();
            for (int i = 0; i < sceneGUIDs.Length; i++)
            {
                S_Guid guidToTest = sceneGUIDs[i];
                for(int j = 0; j < sceneGUIDs.Length; j++)
                {
                    // Don't test against the same GUID
                    if(i != j)
                    {
                        if(guidToTest.GUID.Equals(sceneGUIDs[j].GUID))
                        {
                            Debug.LogError(
                                string.Format("SpatialStoriesAPI: Error Gameobject {0} and {1} contains the same guid {2}",
                                guidToTest.name, sceneGUIDs[j].name, guidToTest.GUID));
                        }
                    }
                }
            }
        }
    }
#endif
}
