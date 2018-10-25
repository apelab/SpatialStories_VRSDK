using Gaze;
using UnityEngine.XR.iOS;

public class CC_OnFaceDetected : Gaze_AbstractConditions
{
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
}
