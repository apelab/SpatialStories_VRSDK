using Gaze;
using UnityEngine;
using System.Collections.Generic;
using GoogleARCore;
using YourCommonTools;

namespace SpatialStories
{
    public class S_ARCoreCameraRaycaster : S_ARAbstractRaycaster
    {
        public const string EVENT_ARCORECAMERA_RAYCAST_IMAGE_ANCHOR = "EVENT_ARCORECAMERA_RAYCAST_IMAGE_ANCHOR";

        // ARCORE SESSION
        private ARCoreSession m_arCoreSession;

        // PLANE DETECTOR
        private RaycastHit m_planeHit;

        // IMAGE DETECTOR
        private Dictionary<int, GameObject> m_goImageReferences = new Dictionary<int, GameObject>();
        private List<AugmentedImage> m_tempAugmentedImages = new List<AugmentedImage>();
        

        public S_ARCoreCameraRaycaster(Gaze_CameraRaycaster _camRaycaster) : base(_camRaycaster)
        {
        }

        public override void HandleNewGazedObjects()
        {
            if (hits != null && hits.Count > 0)
            {
                foreach (RaycastHit h in hits)
                {
                    currentGazedObjects.Add(h.collider.gameObject);
                    Gaze_CameraRaycaster.DetectedObjectThisFrame = true;
                    if (!previousGazedObjects.Contains(h.collider.gameObject))
                    {
                        Debug.LogError("++IN++COLLIDED OBJECT =" + h.collider.gameObject.name);
                        FireGazeEvent(h.collider.gameObject, true, Gaze_GazeConstraints.OBJECT);
                    }
                }

                if (previousGazedObjects.Count > 0)
                {
                    foreach (GameObject p in previousGazedObjects)
                    {
                        if (!currentGazedObjects.Contains(p))
                        {
                            Debug.LogError("--OUT--COLLIDED OBJECT =" + p.name);
                            FireGazeEvent(p, false, Gaze_GazeConstraints.OBJECT);
                        }
                    }
                }
                previousGazedObjects = new List<GameObject>(currentGazedObjects);
            }
        }

        
        public override void LateUpdate()
        {
            Gaze_CameraRaycaster.DetectedObjectThisFrame = false;
            Gaze_CameraRaycaster.DetectedPlaneThisFrame = false;
            currentGazedObjects.Clear();

            PerformUnityRaycast();
            PerformArCoreRaycast();
            
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

        private void PerformArCoreRaycast()
        {
            m_arCoreSession = GameObject.FindObjectOfType<ARCoreSession>();

            if (m_arCoreSession != null)
            {
                if (m_arCoreSession.SessionConfig.PlaneFindingMode != DetectedPlaneFindingMode.Disabled) PerformArCoreRaycastPlane();
                if (m_arCoreSession.SessionConfig.AugmentedImageDatabase != null) PerformArCoreImageRecognition();
            }
        }

        private void PerformArCoreRaycastPlane()
        {
            bool pointingIntoPlane = false;
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;
            Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
            if (Frame.Raycast(center.x, center.y, raycastFilter, out hit))
            {
                pointingIntoPlane = true;
                Gaze_CameraRaycaster.DetectedPlaneThisFrame = true;
                m_planeHit = new RaycastHit();
                m_planeHit.distance = hit.Distance;
                m_planeHit.point = hit.Pose.position;
                m_planeHit.normal = hit.Pose.up;
                Gaze_CameraRaycaster.ClosestHitOverPlane = m_planeHit;
                Gaze_CameraRaycaster.ClosestHitOverPlaneTrackable = new TrackableHit(hit.Pose, hit.Distance, hit.Flags, hit.Trackable);
                
                FireGazeEvent(null, true, Gaze_GazeConstraints.PLANE);
            }
            else
            {
                Gaze_CameraRaycaster.DetectedPlaneThisFrame = false;
                FireGazeEvent(null, false, Gaze_GazeConstraints.PLANE);
            }

            HandlePointingIntoAPlaneLogic(pointingIntoPlane);
        }

        private void PerformArCoreImageRecognition()
        {
            if (Session.Status != SessionStatus.Tracking)
            {
                return;
            }

            Session.GetTrackables<AugmentedImage>(m_tempAugmentedImages, TrackableQueryFilter.Updated);

            foreach (var image in m_tempAugmentedImages)
            {
                GameObject goImage = null;
                m_goImageReferences.TryGetValue(image.DatabaseIndex, out goImage);
                if (image.TrackingState == TrackingState.Tracking && goImage == null)
                {
                    Anchor anchor = image.CreateAnchor(image.CenterPose);
                    GameObject goReference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    goReference.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    goReference.transform.position = anchor.transform.position;
                    goReference.transform.parent = anchor.transform;
                    goReference.GetComponent<Renderer>().enabled = false;

                    m_goImageReferences.Clear();
                    m_goImageReferences.Add(image.DatabaseIndex, goReference);
                    BasicSystemEventController.Instance.DispatchBasicSystemEvent(EVENT_ARCORECAMERA_RAYCAST_IMAGE_ANCHOR, anchor);

                    FireGazeEvent(image.Name, true, Gaze_GazeConstraints.IMAGE);
                }
                else if (image.TrackingState == TrackingState.Stopped && goImage != null)
                {
                    m_goImageReferences.Remove(image.DatabaseIndex);
                    GameObject.Destroy(goImage);
                }
            }
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

            if (Gaze_CameraRaycaster.DetectedPlaneThisFrame)
            {
                Gaze_CameraRaycaster.ActualDetectionState = Gaze_CameraRaycaster.CURSOR_DETECTION_STATE.OVER_PLANE;
                Gaze_CameraRaycaster.ClosestHit = m_planeHit;
                minDistancePlane = m_planeHit.distance;
                minDistance = m_planeHit.distance;
            }

            for (int i = 0; i < count; i++)
            {
				RaycastHit hit = hits[i];
    
                // Ignore proximity and gaze in the computations
                if (hit.collider.GetComponent<Gaze_Proximity>() != null || hit.collider.GetComponent<Gaze_Gaze>() != null)
                    continue;
                
                
                if (hit.distance < minDistanceObj)
                {
                    Gaze_CameraRaycaster.ClosestHitOverObject = hit;
                    minDistanceObj = hit.distance;
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
            // NOTE: Plane collisions doesn't have a collider attached
            if (_closestHit.collider == null)
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
            else if (Gaze_CameraRaycaster.DetectedPlaneThisFrame == true)
            {
                Gaze_CameraRaycaster.ActualDetectionState = Gaze_CameraRaycaster.CURSOR_DETECTION_STATE.OVER_PLANE;
            }
            else
            {
                Gaze_CameraRaycaster.ActualDetectionState = Gaze_CameraRaycaster.CURSOR_DETECTION_STATE.OVER_NOTHING;
            }
        }
    }
}
