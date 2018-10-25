using Gaze;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialStories
{
    public class S_ARKitCameraRaycaster : S_ARAbstractRaycaster
    {
        // Arkit specific features
        int arkitLayer = -1;

        public S_ARKitCameraRaycaster(Gaze_CameraRaycaster _camRaycaster) : base(_camRaycaster)
        {
            arkitLayer = LayerMask.NameToLayer("ARKitPlane");
        }
        
        public override void HandleNewGazedObjects()
        {
            // if camera's ray hits something
            if (hits != null && hits.Count > 0)
            {
                bool pointingIntoPlane = false;
                // construct new current gazed objects list
                foreach (RaycastHit h in hits)
                {
                    if (h.collider.gameObject.layer == arkitLayer)
                    {
                        pointingIntoPlane = true;
                        Gaze_CameraRaycaster.DetectedPlaneThisFrame = true;
                    }
                    else
                    {
                        // add it to the current gazed objects list
                        currentGazedObjects.Add(h.collider.gameObject);
                        Gaze_CameraRaycaster.DetectedObjectThisFrame = true;
                        // if it wasn't in the previous gazed objects list
                        if (!previousGazedObjects.Contains(h.collider.gameObject))
                        {
                            FireGazeEvent(h.collider.gameObject, true, Gaze_GazeConstraints.OBJECT);
                        }
                    }
                }

                HandlePointingIntoAPlaneLogic(pointingIntoPlane);

                // if there were any previous gazed object
                if (previousGazedObjects.Count > 0)
                {

                    // check for each one of them
                    foreach (GameObject p in previousGazedObjects)
                    {

                        // if they don't exist anymore in the current gazed objects
                        if (!currentGazedObjects.Contains(p))
                        {
                            FireGazeEvent(p, false, Gaze_GazeConstraints.OBJECT);
                        }
                    }
                }

                // update previous gazed objects list with the new ones
                previousGazedObjects = new List<GameObject>(currentGazedObjects);
            }
        }

        public override void LateUpdate()
        {
            Gaze_CameraRaycaster.DetectedObjectThisFrame = false;
            Gaze_CameraRaycaster.DetectedPlaneThisFrame = false;
            // clear current gazed objects list
            currentGazedObjects.Clear();
            PerformUnityRaycast();
            if (hits != null && hits.Count > 0)
            {
                HandleNewGazedObjects();
            }
            else
            {
                HandleGazeOutForPreviousObjects();
            }

            ComputeCurrentDetectionStateData();
        }

        /// <summary>
        /// This method does 3 things:
        /// 1) Gets the closest hit of the current raycast
        /// 2) Gets the closest hit into a plane for the curent raycast.
        /// 3) Gets the closest hit into an object for the current raycast.
        /// </summary>
        public override void ComputeClosestHits()
        {
            int count = hits.Count;
            float minDistance = float.MaxValue;
            float minDistanceObj = float.MaxValue;
            float minDistancePlane = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                RaycastHit hit = hits[i];

                // Ignore proximity and gaze in the computations
                if(hit.collider.GetComponent<Gaze_Proximity>() != null || hit.collider.GetComponent<Gaze_Gaze>() != null)
                    continue;
                
                if (hit.collider.gameObject.layer == arkitLayer)
                {
                    Gaze_CameraRaycaster.ActualDetectionState = Gaze_CameraRaycaster.CURSOR_DETECTION_STATE.OVER_PLANE;
                    if (hit.distance < minDistancePlane)
                    {
                        Gaze_CameraRaycaster.ClosestHitOverPlane = hit;
                        minDistancePlane = hit.distance;
                    }
                }
                else
                {
                    if (hit.distance < minDistanceObj)
                    {
                        Gaze_CameraRaycaster.ClosestHitOverObject = hit;
                        minDistanceObj = hit.distance;
                    }
                }

                if (hit.distance < minDistance)
                {
                    Gaze_CameraRaycaster.ClosestHit = hit;
                    minDistance = hit.distance;
                }
            }
        }

        /// <summary>
        /// Gets the state of the closest hit in order to know which cursor use
        /// </summary>
        /// <param name="_closestHit"></param>
        public override void DetermineCurrentStateOfClosestHit(RaycastHit _closestHit)
        {
            if (_closestHit.collider.gameObject.layer == arkitLayer)
            {
                Gaze_CameraRaycaster.ActualDetectionState = Gaze_CameraRaycaster.CURSOR_DETECTION_STATE.OVER_PLANE;
            }
            else
            {
                Gaze_CameraRaycaster.ActualDetectionState = Gaze_CameraRaycaster.CURSOR_DETECTION_STATE.OVER_OBJECT;
            }
        }

        /// <summary>
        /// This method is used in ARKIt to compute all the data needed to know
        /// where the user is looking at and where to place te cursor.
        /// </summary>
        public override void ComputeCurrentDetectionStateData()
        {
            if (hits != null && hits.Count > 0)
            {
                ComputeClosestHits();
                DetermineCurrentStateOfClosestHit(Gaze_CameraRaycaster.ClosestHit);
            }
            else
            {
                Gaze_CameraRaycaster.ActualDetectionState = Gaze_CameraRaycaster.CURSOR_DETECTION_STATE.OVER_NOTHING;
            }
        }
    }
}
