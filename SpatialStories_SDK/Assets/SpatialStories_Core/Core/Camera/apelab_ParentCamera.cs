using UnityEngine;

public class apelab_ParentCamera : MonoBehaviour
{
    private static float lastParentEventTime;
    private static GameObject cam;
    private static bool debug = false;

    private void Start()
    {
        lastParentEventTime = Time.time;
        cam = FindObjectOfType<Gaze_InputManager>().gameObject;
    }

    public static void Parent(bool _askForParenting, Transform _parent)
    {
        if (cam == null)
            cam = FindObjectOfType<Gaze_InputManager>().gameObject;

        // check if we're parenting or de-parenting
        if (!_askForParenting)
        {
            /// if we're deparenting, make sure there is no parenting just before,
            /// which indicates we've exit one parented zone to enter another parenting zone
            if (Time.time - lastParentEventTime > .4f)
            {
                if (debug)
                    Debug.Log("UNparent camera to " + _parent + " at position  " + _parent.transform.position);
                cam.transform.parent = null;
            }
        }
        else
        {
            if (debug)
                Debug.Log("parent camera to " + _parent + " at position  " + _parent.position);
            cam.transform.parent = _parent;
            cam.transform.position = new Vector3(_parent.position.x, cam.transform.position.y, _parent.position.z);
        }
        lastParentEventTime = Time.time;
    }
}
