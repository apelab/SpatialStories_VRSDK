using Gaze;
#if UNITY_IOS
using UnityEngine.XR.iOS;
#endif

public class CC_OnFaceLost : Gaze_AbstractConditions
{
#if UNITY_IOS
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
#endif
}
