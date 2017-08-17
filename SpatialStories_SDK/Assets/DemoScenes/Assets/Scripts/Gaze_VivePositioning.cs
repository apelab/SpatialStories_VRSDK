using Gaze;
using UnityEngine;

public class Gaze_VivePositioning : MonoBehaviour
{
    public float viveHeigth = 0;

    void Awake()
    {
        Gaze_InputManager.OnControlerSetup += OnControlerSetup;
    }

    void OnControlerSetup(Gaze_Controllers _type)
    {
        if (_type == Gaze_Controllers.HTC_VIVE)
        {
            transform.position = new Vector3(transform.position.x, viveHeigth, transform.position.z);
            Gaze_Teleporter.playerHeightBeforeTeleport = viveHeigth;
        }
        Gaze_InputManager.OnControlerSetup -= OnControlerSetup;
        Destroy(this);
    }
}
