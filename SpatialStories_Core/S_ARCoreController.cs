using Gaze;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using SpatialStories;
using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

public class S_ARCoreController : MonoBehaviour
{
    // -------------------------------------------
    // EVENTS
    // -------------------------------------------
    public const string EVENT_ARCORE_CONTROLLER_DISABLE_DETECTION = "EVENT_ARCORE_CONTROLLER_DISABLE_DETECTION";

    // -------------------------------------------
    // PUBLIC MEMBERS
    // -------------------------------------------
    public bool EnablePlaneRecognition = true;
    public bool EnableImageRecognition = true;

    public ARCoreSessionConfig ARCoreSessionConfig;
    public GameObject SearchingForPlaneUI;
    public GameObject FitToScanOverlay;

    // -------------------------------------------
    // PRIVATE MEMBERS
    // -------------------------------------------
    // PLANE DETECTOR
    private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();

    private bool m_IsQuitting = false;

    /// <summary>
    /// Establish the initial configuration
    /// </summary>
    private void Awake()
    {
        ARCoreSession arCoreSession = GameObject.FindObjectOfType<ARCoreSession>();
        arCoreSession.SessionConfig = ARCoreSessionConfig;
        SpatialStoriesEventController.Instance.SpatialStoriesEvent += new SpatialStoriesEventHandler(OnSpatialStoriesEvent);
        if (FitToScanOverlay != null) FitToScanOverlay.SetActive(true);
    }

    /// <summary>
    /// Basic System Event manager
    /// </summary>
    private void OnSpatialStoriesEvent(string _nameEvent, object[] _list)
    {
        if (_nameEvent == EVENT_ARCORE_CONTROLLER_DISABLE_DETECTION)
        {
            DetectedPlaneGenerator planeGenerator = GameObject.FindObjectOfType<DetectedPlaneGenerator>();
            if (planeGenerator != null)
            {
                Destroy(planeGenerator.gameObject);
            }
        }
        if (_nameEvent == S_ARCoreCameraRaycaster.EVENT_ARCORECAMERA_RAYCAST_IMAGE_ANCHOR)
        {
            if (FitToScanOverlay != null) FitToScanOverlay.SetActive(false);
        }
    }

    /// <summary>
    /// Release resources
    /// </summary>
    private void OnDestroy()
    {
        SpatialStoriesEventController.Instance.SpatialStoriesEvent -= OnSpatialStoriesEvent;
    }

    /// <summary>
    /// Main loop where we will look with the method we prefer
    /// </summary>
    private void Update()
    {
        _UpdateApplicationLifecycle();

        if (EnablePlaneRecognition)
        {
            UpdatePlaneRecognition();
        }
        if (EnableImageRecognition)
        {
            UpdateImageRecognition();
        }
    }

    /// <summary>
    /// Logic to look for the planes
    /// </summary>
    private void UpdatePlaneRecognition()
    {
        Session.GetTrackables<DetectedPlane>(m_AllPlanes);
        bool showSearchingUI = true;
        for (int i = 0; i < m_AllPlanes.Count; i++)
        {
            if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
            {
                showSearchingUI = false;
                break;
            }
        }

        if (SearchingForPlaneUI!=null) SearchingForPlaneUI.SetActive(showSearchingUI);
    }

    /// <summary>
    /// Logic to look for the images
    /// </summary>
    private void UpdateImageRecognition()
    {
    }

    /// <summary>
    /// Quit the application if there was a connection error for the ARCore session.
    /// </summary>
    private void _UpdateApplicationLifecycle()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting)
        {
            return;
        }

        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// _DoQuit
    /// </summary>
    private void _DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// _ShowAndroidToastMessage
    /// </summary>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
