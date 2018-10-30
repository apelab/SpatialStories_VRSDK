using Gaze;
#if UNITY_IOS
using UnityEngine.XR.iOS;
#endif

public class CC_OnFaceDetected : Gaze_AbstractConditions
{
#if UNITY_IOS
    private void OnEnable()
    {
        UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdded;
    }

    private void FaceAdded(ARFaceAnchor anchorData)
    {
        ValidateCustomCondition(true);
    }

    private void OnDisable()
    {
        UnityARSessionNativeInterface.ARFaceAnchorAddedEvent -= FaceAdded;
    }
#endif
}
