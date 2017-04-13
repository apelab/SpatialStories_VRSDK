using UnityEngine;

public class ScreenShot : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            Application.CaptureScreenshot("Screenshot.png");
        }
    }
}
