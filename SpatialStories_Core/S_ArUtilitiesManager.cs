using UnityEngine;
using UnityEngine.XR.iOS;
using GoogleARCore;
using SpatialStories;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Used to enable or disable ARkit / Core components when needed
/// </summary>
[ExecuteInEditMode]
public class S_ArUtilitiesManager : MonoBehaviour {

    /// <summary>
    /// The selected camera acording to he active mode
    /// </summary>
    public static Camera SelectedCamera;

    /// <summary>
    /// A game object that wraps all the arkit dependencies
    /// </summary>
    public GameObject ArkitDependencies;

    /// <summary>
    /// A game object that wraps all the arcore dependencies
    /// </summary>
    public GameObject ArCoreDependencies;

    /// <summary>
    /// A reference to the camera object that holds references to arcore / arkit cameras
    /// </summary>
    private S_ARCameras camerasHolder;
    
#if UNITY_EDITOR

    // The selected build target on the last check
    private BuildTarget lastBuildTarget;

    // The current build target (IOS, Android, Windows ...)
    private BuildTarget currentBuildTarget;

    private void Awake()
    {
        // Set the last build target as the current
        lastBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        currentBuildTarget = lastBuildTarget;

        // Get the cameras holder for enable or disable dependencies
        camerasHolder = GetCamerasHolder();
        if (camerasHolder != null)
            PerformBuildTargetChangeLogic();
    }

    private void Update()
    {
        // If no camera just return
        if (camerasHolder == null)
            return;

        // Get the current build target from the editor build settings
        currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;

        // If the build target changed perform the change logic
        if(currentBuildTarget != lastBuildTarget || SelectedCamera == null)
        {
            PerformBuildTargetChangeLogic();
        }
    }

    /// <summary>
    /// Enable the dependencies for one mode
    /// ANDROID = Arcore
    /// IOS = Arkit
    /// </summary>
    private void PerformBuildTargetChangeLogic()
    {
		// If the dependencies have not been set just return
		if (ArkitDependencies == null || ArCoreDependencies == null)
			return;
		
        switch (currentBuildTarget)
        {
            case BuildTarget.iOS:
                SwitchToARkit();
                break;
            default:
                SwitchToArCore();
                break;
        }
    }

#elif UNITY_IOS
    void Awake()
    {
        camerasHolder = GetCamerasHolder();
        SwitchToARkit();
    }
#elif UNITY_ANDROID
    void Awake()
    {   
        camerasHolder = GetCamerasHolder();
        SwitchToArCore();
    }
#endif

    /// <summary>
    /// Get the objects that contains references to the arcore and arkit cameras
    /// very handy to disable them
    /// </summary>
    /// <returns></returns>
    private S_ARCameras GetCamerasHolder()
    {
        S_ARCameras holder = FindObjectOfType<S_ARCameras>();
        if (holder == null)
        {
            Debug.LogError("A Camera (IO) is required!");
            return null;
        }
        return holder;
    }
    
    /// <summary>
    /// Enables all the arcore dependencies making sure that the arkit ones are disabled
    /// </summary>
    private void SwitchToArCore()
    {
        SetARKitDependencies(false);
        SetArCoreDependencies(true);
        SelectedCamera.tag = "MainCamera";
    }

    /// <summary>
    /// Enables all the arkit dependencies making sure that the arcore ones are disabled
    /// </summary>
    private void SwitchToARkit()
    {
        SetArCoreDependencies(false);
        SetARKitDependencies(true);
        SelectedCamera.tag = "MainCamera";
    }

    /// <summary>
    /// Eanbles or disables the arkit dependencies
    /// </summary>
    /// <param name="_areEnabled"></param>
    private void SetARKitDependencies(bool _areEnabled)
    {
        ArkitDependencies.SetActive(_areEnabled);
        camerasHolder.ArKitCamera.SetActive(_areEnabled);

        if (_areEnabled && camerasHolder.ArKitCamera.GetComponent<S_ArkitCameraManager>() == null)
            camerasHolder.ArKitCamera.AddComponent<S_ArkitCameraManager>();

        if (_areEnabled)
            SelectedCamera = camerasHolder.ArKitCamera.GetComponentInChildren<Camera>();
    }

    /// <summary>
    /// Eanbles or disables the arcore dependencies
    /// </summary>
    /// <param name="_areEnabled"></param>
    private void SetArCoreDependencies(bool _areEnabled)
    {
        ArCoreDependencies.SetActive(_areEnabled);
        camerasHolder.ARCoreCamera.SetActive(_areEnabled);

        if (_areEnabled)
            SelectedCamera = camerasHolder.ARCoreCamera.GetComponentInChildren<Camera>();
    }
}