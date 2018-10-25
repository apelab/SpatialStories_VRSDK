using UnityEngine;

#if UNITY_IOS
using UnityEngine.XR.iOS;
#endif

namespace SpatialStories
{
    public class S_ArkitCameraManager : MonoBehaviour
    {
        

        private void Start()
        {
#if UNITY_IOS
            // Ensure that ARkit reset the tracking when the scene gets restarted
            UnityARCameraManager arkitCameraManager = FindObjectOfType<UnityARCameraManager>();
            UnityARSessionRunOption options = new UnityARSessionRunOption();
            options = UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors | UnityARSessionRunOption.ARSessionRunOptionResetTracking;
            UnityARSessionNativeInterface nativeInterface = UnityARSessionNativeInterface.GetARSessionNativeInterface();

			ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration();
			config.planeDetection = arkitCameraManager.planeDetection;
			config.alignment = arkitCameraManager.startAlignment;
			config.getPointCloudData = arkitCameraManager.getPointCloud;
			config.enableLightEstimation = arkitCameraManager.enableLightEstimation;
            nativeInterface.RunWithConfigAndOptions(config, options);
#endif

        }
    }
}
