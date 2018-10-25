using Gaze;
using UnityEngine.XR.iOS;

public class CC_OnFaceLost : Gaze_AbstractConditions
{
    private void OnEnable()
    {
        UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent += OnFaceLost;
    }

    private void OnDisable()
    {
        UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent -= OnFaceLost;
    }

    private void OnFaceLost(ARFaceAnchor anchorData)
    {
        ValidateCustomCondition(true);
    }
}
