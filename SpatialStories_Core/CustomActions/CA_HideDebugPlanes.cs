using System;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.XR.iOS;
#endif

namespace Gaze
{
    public class CA_HideDebugPlanes : Gaze_AbstractBehaviour
    {
        public override void SetupUsingApi(GameObject _interaction)
        {
            throw new NotImplementedException();
        }

        protected override void OnTrigger()
        {
            StudioPlaneManager.Instance.SetPlanesVisibility(false);
        }
    }
}

