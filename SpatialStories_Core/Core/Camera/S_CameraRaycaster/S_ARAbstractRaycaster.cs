using Gaze;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    public abstract class S_ARAbstractRaycaster
    {
        protected Gaze_CameraRaycaster baseRaycaster;
        protected bool wasPointingIntoAPlane = false;
        protected List<RaycastHit> hits = new List<RaycastHit>();
        protected List<GameObject> previousGazedObjects = new List<GameObject>();
        protected List<GameObject> currentGazedObjects = new List<GameObject>();
        private Gaze_GazeEventArgs gaze_gazeEventArgs;

        public S_ARAbstractRaycaster(Gaze_CameraRaycaster _camRaycaster)
        {
            baseRaycaster = _camRaycaster;
            gaze_gazeEventArgs = new Gaze_GazeEventArgs();
        }
        
        public abstract void HandleNewGazedObjects();
        public abstract void LateUpdate();
        public abstract void ComputeClosestHits();
        public abstract void DetermineCurrentStateOfClosestHit(RaycastHit _closestHit);
        public abstract void ComputeCurrentDetectionStateData();

        public void FireGazeEvent(object _sender, bool _isGazed, Gaze_GazeConstraints _constraints)
        {
            // notify every listener with the new current gazed object
            gaze_gazeEventArgs.Sender = _sender;
            gaze_gazeEventArgs.IsGazed = _isGazed;
            gaze_gazeEventArgs.TargetType = _constraints;
            Gaze_EventManager.FireGazeEvent(gaze_gazeEventArgs);
        }

        protected void HandlePointingIntoAPlaneLogic(bool _pointingIntoPlane)
        {
            // If nothing changed just return
            if (_pointingIntoPlane == wasPointingIntoAPlane)
                return;

            FireGazeEvent(null, _pointingIntoPlane, Gaze_GazeConstraints.PLANE);
            wasPointingIntoAPlane = _pointingIntoPlane;
        }

        protected void PerformUnityRaycast()
        {
            // cast a ray
            //ray = new Ray(gazeCamera.transform.position, gazeCamera.transform.forward);
            Gaze_CameraRaycaster.LastRay.origin = baseRaycaster.GazeCamera.transform.position;
            Gaze_CameraRaycaster.LastRay.direction = baseRaycaster.GazeCamera.transform.forward;
            hits.Clear();
            hits.AddRange(Physics.RaycastAll(Gaze_CameraRaycaster.LastRay, baseRaycaster.RayLength));

            if (baseRaycaster.DebugMode)
                Debug.DrawRay(baseRaycaster.GazeCamera.transform.position, baseRaycaster.GazeCamera.transform.forward * baseRaycaster.RayLength, Color.red);
        }

        public void HandleGazeOutForPreviousObjects()
        {
            // and the there were previous gazed objects
            if (previousGazedObjects.Count > 0)
            {
                // for each one of the previous gazed object
                foreach (GameObject p in previousGazedObjects)
                {
                    Debug.LogError("--OUT--(2)COLLIDED OBJECT =" + p.name);
                    FireGazeEvent(p, false, Gaze_GazeConstraints.OBJECT);
                }

                // clear the previous gazed objects list
                previousGazedObjects.Clear();
            }

            if(!Gaze_CameraRaycaster.DetectedPlaneThisFrame)
                HandlePointingIntoAPlaneLogic(false);
        }
    }
}
