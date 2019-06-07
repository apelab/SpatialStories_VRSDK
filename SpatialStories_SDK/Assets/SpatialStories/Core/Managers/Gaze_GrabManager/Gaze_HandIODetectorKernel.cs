using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

namespace Gaze
{
    /// <summary>
    /// This class is represents the sensors of the user's hands, it decides
    /// if the hand is able to interact with some object
    /// </summary>
    public class Gaze_HandIODetectorKernel
    {
        public RaycastHit[] Hits;

        private float nextAllowedUpdate = 0;
        private int pointLayerMask;
        private const float BASE_RAYCAST_INTERVAL = 0.2f;
        
        private List<GameObject> raycastIOs;
        private Gaze_GrabManager grabManager;
        private Gaze_HandIODetectorFeedback feedbackManager;
        private Gaze_ControllerPointingEventArgs gaze_ControllerPointingEventArgs;
        private KeyValuePair<UnityEngine.XR.XRNode, GameObject> keyValue;

        /// <summary>
        /// Used to raycast every frame when touching an IO or at the specified interval.
        /// </summary>
        public bool ObjectPointerInPreviousFrame = false;

        public Gaze_HandIODetectorKernel(Gaze_GrabManager _owner)
        {
            grabManager = _owner;
            pointLayerMask = 1 << LayerMask.NameToLayer(Gaze_HashIDs.LAYER_HANDHOVER);
            raycastIOs = new List<GameObject>();
        }

        public void Setup()
        {
            feedbackManager = grabManager.IoDetectorFeedback;
            grabManager.distantGrabOrigin = grabManager.gameObject.GetComponentInChildren<Gaze_DistantGrabPointer>().gameObject;
            gaze_ControllerPointingEventArgs = new Gaze_ControllerPointingEventArgs(grabManager.gameObject, keyValue, true);
            gaze_ControllerPointingEventArgs.Sender = grabManager.gameObject;
            keyValue = new KeyValuePair<UnityEngine.XR.XRNode, GameObject>();
        }

        public void Update()
        {
            if(Time.time > nextAllowedUpdate || feedbackManager.ActualFeedbackMode == Gaze_HandIODetectorFeedback.FeedbackModes.Colliding)
            {
                FindDistantObjects();
                nextAllowedUpdate = Time.time + BASE_RAYCAST_INTERVAL;
            }
        }

        public void FindDistantObjects()
        {
            if (grabManager.distantGrabOrigin == null)
                return;

            Hits = Physics.RaycastAll(grabManager.distantGrabOrigin.transform.position, grabManager.distantGrabOrigin.transform.forward, pointLayerMask);
            grabManager.HitsIos.Clear();
            grabManager.closerIO = null;

            float visualRayLength = float.MaxValue;

            // if the raycast hits nothing
            if (Hits.Length < 1)
            {
                // notify every previously pointed object they are no longer pointed
                for (int i = 0; i < raycastIOs.Count; i++)
                {
                    gaze_ControllerPointingEventArgs.IsPointed = false;
                    Gaze_EventManager.FireControllerPointingEvent(gaze_ControllerPointingEventArgs);
                }

                // clear the list
                raycastIOs.Clear();

                ObjectPointerInPreviousFrame = false;
            }
            else
            {
                // 1° notify new raycasted objects in hits
                for (int i = 0; i < Hits.Length; i++)
                {
                    if (Hits[i].collider.GetComponent<Gaze_HandHover>() != null)
                    {
                        // get the pointed object
                        Gaze_InteractiveObject interactiveObject = Hits[i].collider.transform.GetComponentInParent<Gaze_InteractiveObject>();

                        if (Hits[i].transform.name == "Cube (IO)")
                        {
                           // Debug.Log("Trala");
                        }

                        // populate the list of IOs hit
                        grabManager.HitsIos.Add(interactiveObject.gameObject);

                        // Add a dynamic grab position points to objects that doesn't need to snap.
                        if (!interactiveObject.SnapOnGrab && !interactiveObject.GrabLogic.IsBeingGrabbed)
                            interactiveObject.GrabLogic.AddDynamicGrabPositioner(Hits[i].point, grabManager);

                        // if pointed object is not in the list
                        if (!raycastIOs.Contains(interactiveObject.gameObject))
                        {
                            // notify the new pointed object
                            raycastIOs.Add(interactiveObject.gameObject);
                            gaze_ControllerPointingEventArgs.Dico = new KeyValuePair<UnityEngine.XR.XRNode, GameObject>(grabManager.isLeftHand ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand, interactiveObject.gameObject);
                            gaze_ControllerPointingEventArgs.IsPointed = true;
                            Gaze_EventManager.FireControllerPointingEvent(gaze_ControllerPointingEventArgs);
                        }

                        if (!interactiveObject.GrabLogic.IsBeingGrabbed && interactiveObject.IsGrabEnabled && interactiveObject.GrabModeIndex.Equals((int)Gaze_GrabMode.GRAB) && Hits[i].distance < interactiveObject.GrabDistance)
                        {
                            grabManager.closerIO = interactiveObject;
                            grabManager.closerDistance = Hits[i].distance;
                            break;
                        }
                        if (interactiveObject.IsTouchEnabled && Hits[i].distance < interactiveObject.TouchDistance)
                        {
                            grabManager.closerIO = interactiveObject;
                            grabManager.closerDistance = Hits[i].distance;
                            break;
                        }
                        if (interactiveObject.IsGrabEnabled && interactiveObject.GrabModeIndex.Equals((int)Gaze_GrabMode.LEVITATE) && Hits[i].distance < interactiveObject.GrabDistance && !interactiveObject.GrabLogic.IsBeingGrabbed)
                        {
                            // update the hit position until we grab something
                            if (grabManager.grabState != Gaze_GrabManagerState.GRABBED)
                                grabManager.hitPosition = Hits[i].point;

                            grabManager.closerIO = interactiveObject;
                            grabManager.closerDistance = Hits[i].distance;
                            break;
                        }

                        // Get the visual ray length
                        visualRayLength = interactiveObject.IsGrabEnabled && interactiveObject.GrabModeIndex.Equals((int)Gaze_GrabMode.GRAB) && visualRayLength > Hits[i].distance ? Hits[i].distance : visualRayLength;
                    }
                }

                // 2 : notify no longer raycasted objects in raycastIOs
                for (int i = 0; i < raycastIOs.Count; i++)
                {
                    if (!grabManager.HitsIos.Contains(raycastIOs[i]))
                    {
                        // notify
                        gaze_ControllerPointingEventArgs.Dico = new KeyValuePair<UnityEngine.XR.XRNode, GameObject>(grabManager.isLeftHand ? UnityEngine.XR.XRNode.LeftHand : UnityEngine.XR.XRNode.RightHand, raycastIOs[i]);
                        gaze_ControllerPointingEventArgs.IsPointed = false;

                        Gaze_EventManager.FireControllerPointingEvent(gaze_ControllerPointingEventArgs);

                        // remove it
                        raycastIOs.RemoveAt(i);
                    }
                }

                if (grabManager.HitsIos.Count > 0)
                    ObjectPointerInPreviousFrame = true;
            }

            if (grabManager.closerIO == null)
            {
                feedbackManager.ActualFeedbackMode = Gaze_HandIODetectorFeedback.FeedbackModes.Default;
            }
            else
            {
                feedbackManager.ShowDistantGrabFeedbacks(grabManager.distantGrabOrigin.transform.position, grabManager.distantGrabOrigin.transform.forward, visualRayLength, grabManager.closerIO != null);
                feedbackManager.ActualFeedbackMode = Gaze_HandIODetectorFeedback.FeedbackModes.Colliding;
            }
        }

        public void ClearRaycasts()
        {
            raycastIOs.Clear();
        }
        
        public void RemoveDestroyedIOFromRaycasts(GameObject _destoyedObj)
        {
            if (raycastIOs != null)
                raycastIOs.Remove(_destoyedObj);
        }

    }
}
