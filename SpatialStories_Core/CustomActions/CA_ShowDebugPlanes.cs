using System;
using UnityEngine;

namespace Gaze
{
    public class CA_ShowDebugPlanes : Gaze_AbstractBehaviour
    {
        public override void SetupUsingApi(GameObject _interaction)
        {
            throw new NotImplementedException();
        }

        protected override void OnTrigger()
        {
            StudioPlaneManager.Instance.SetPlanesVisibility(true);
        }
    }
}

