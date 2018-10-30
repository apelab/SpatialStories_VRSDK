using System.Collections.Generic;
using UnityEngine;
using Gaze;
#if UNITY_IOS
using UnityEngine.XR.iOS;
#endif

public class ARKitCursor : MonoBehaviour
{
    public static ARKitCursor instance;
    
    public GameObject OnObject;
    public GameObject OnPlane;
    public GameObject OverNothing;

    private int initialOnPlaneCursor;

    [HideInInspector]
    public bool IsCursorActive = true;
    
    private void Awake()
    {
        initialOnPlaneCursor = OnPlane.GetInstanceID();
    }

    public void DeactivateCursors()
    {
        OnObject.SetActive(false);
        OnPlane.SetActive(false);
        OverNothing.SetActive(false);
    }

    void Update()
    {
        DeactivateCursors();
        if (!IsCursorActive)
            return; 

        switch (Gaze_CameraRaycaster.ActualDetectionState)
        {
            case Gaze_CameraRaycaster.CURSOR_DETECTION_STATE.OVER_NOTHING:
                if (IsCursorActive)
                {
                    if (OverNothing != null)
                        OverNothing.SetActive(true);
                }
                if (OverNothing != null)
                    OverNothing.transform.position = Camera.main.transform.position + 2f * Gaze_CameraRaycaster.LastRay.direction;
                break;
            case Gaze_CameraRaycaster.CURSOR_DETECTION_STATE.OVER_OBJECT:
                if (OnObject != null)
                {
                    OnObject.SetActive(true);
                    OnObject.transform.forward = Gaze_CameraRaycaster.ClosestHit.normal;
                    OnObject.transform.position = Gaze_CameraRaycaster.ClosestHit.point;
                }
                break;
            case Gaze_CameraRaycaster.CURSOR_DETECTION_STATE.OVER_PLANE:
                if (OnPlane != null)
                {
                    OnPlane.SetActive(true);
                    OnPlane.transform.position = Gaze_CameraRaycaster.ClosestHit.point;
                    if (initialOnPlaneCursor == OnPlane.GetInstanceID())
                        OnPlane.transform.forward = new Vector3(0, 1, 0);

                }
                break;
        }
    }
}
