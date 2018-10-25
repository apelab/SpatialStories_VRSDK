using System;
using UnityEngine;
using UnityEngine.XR.iOS;

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

