using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    public class Gaze_HandsReplacer : Gaze_AbstractBehaviour
    {
        /// <summary>
        ///  The event that will be called once the replacement is completely done
        /// </summary>
        /// <param name="grabManager"></param>
        public delegate void OnHandsReplacedHandler(Gaze_GrabManager grabManager, Transform GrabTarget, GameObject DistantGrabObject);
        public static event OnHandsReplacedHandler OnHandsReplaced;
        public static void FireHandReplacedEvent(Gaze_GrabManager _grabManager, Transform _grabTarget, GameObject _distantGrabObject)
        {
            if (OnHandsReplaced != null)
                OnHandsReplaced(_grabManager, _grabTarget, _distantGrabObject);
        }

        public enum HANDS { LEFT, RIGHT }
        public HANDS handToReplace = HANDS.RIGHT;

        /// <summary>
        /// This is mandatory and designates where the object will have its distant grab origin and grab target
        /// </summary>
        private Gaze_DistantGrabPointer DistantGrabObject;
        private Gaze_GrabPositionController GrabTarget;
        public Vector3 replaceRotation;
        public Transform handlePosition;
        public bool destroyOld;

        private Gaze_GrabManager grabManager;
        private Gaze_InteractiveObject IO;

        private Transform positionOld;
        public static bool handHasBeenDestroyed = false;
        private static float lastHandReplace = 0;

        public List<Renderer> VisualsToShowAfterReplace;

        private void Start()
        {
            IO = GetComponentInParent<Gaze_InteractiveObject>();
            DistantGrabObject = IO.gameObject.GetComponentInChildrenBFS<Gaze_DistantGrabPointer>();
            GrabTarget = IO.gameObject.GetComponentInChildrenBFS<Gaze_GrabPositionController>();

            if (!destroyOld)
                SetActualPositionAsOld();

            bool searchingForLeftHand = handToReplace == HANDS.LEFT;

            foreach (Gaze_GrabManager gm in Gaze_GrabManager.GrabManagers)
            {
                if (gm.isLeftHand == searchingForLeftHand)
                {
                    grabManager = gm;
                }
            }
            handHasBeenDestroyed = false;
        }

        private void OnLevelWasLoaded(int level)
        {
            handHasBeenDestroyed = false;
        }

        /// <summary>
        /// Deactivates the current hand visuals before destroying the game last hand game object
        /// </summary>
        private void UpdateVisuals(GameObject obj, bool show)
        {
            if (show && VisualsToShowAfterReplace != null && VisualsToShowAfterReplace.Count > 0)
            {
                foreach (Renderer rend in VisualsToShowAfterReplace)
                {
                    rend.enabled = true;
                }

                foreach (Gaze_GrabManager gm in Gaze_GrabManager.GrabManagers)
                {
                    if (!gm.isLeftHand)
                        gm.GetComponent<LineRenderer>().enabled = true;
                }

            }
            else
            {
                Renderer[] Visuals = obj.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in Visuals)
                    renderer.enabled = show;
            }
        }

        private void ReplaceHand()
        {
            if(lastHandReplace > Time.time + 0.5f)
                return;
            
            // Check if we really need to destroy a hand
            if (destroyOld && handHasBeenDestroyed)
                return;
            
            // tell the manager we detach this object (to parent it manually so the grab manager won't be able to detach it once hand's released)
            if (IO.GrabLogic.GrabbingManager != null)
                IO.GrabLogic.GrabbingManager.TryDetach();

            // Ensure that we are not parented anymore
            IO.transform.SetParent(null);

            // This object will not be able to be taken anymore
            IO.EnableManipulationMode(Gaze_ManipulationModes.NONE);

            // hide current hand's model
            UpdateVisuals(grabManager.gameObject, false);

            GameObject Hand = grabManager.gameObject;
            Gaze_InteractiveObject io = null;
            foreach (Transform child in Hand.transform)
            {
                if (child.gameObject.GetInstanceID() != gameObject.GetInstanceID())
                {
                    if (destroyOld)
                        Destroy(child.gameObject);
                    else
                    {
                        if(io == null)
                        {
                            io = Gaze_Utils.GetIOFromGameObject(child.gameObject);
                        }

                        io.EnableManipulationMode(Gaze_ManipulationModes.GRAB);
                        io.transform.position = positionOld.transform.position;
                        io.transform.rotation = positionOld.transform.rotation;
                        child.parent = null;
                        child.position = positionOld.position;
                        child.rotation = positionOld.rotation;
                       
                        UpdateVisuals(child.gameObject, true);
                    }
                }
            }

            if (!destroyOld)
                io.GetComponentInChildren<Gaze_HandsReplacer>().SetActualPositionAsOld();
           
            // Parent to the hand and position to it
            IO.transform.SetParent(Hand.transform);
            IO.transform.localPosition = handlePosition.localPosition;

            IO.transform.localRotation = Quaternion.Euler(replaceRotation);

            // Fire the event (for the Gaze_GrabManager to update locators and stuff)
            if (OnHandsReplaced != null)
                OnHandsReplaced(grabManager, GrabTarget.gameObject.transform, DistantGrabObject.gameObject);

            // show new hand's model
            UpdateVisuals(grabManager.gameObject, true);

            if (destroyOld)
            {
                handHasBeenDestroyed = true;
            }
            else
            {
                isReplacingHand = true;
                lastHandReplace = Time.time;
            }
        }

        bool isReplacingHand = false;
        public void SetActualPositionAsOld()
        {
            if(positionOld == null)
            {
                positionOld = new GameObject(IO.name).transform;
                positionOld.name = IO.name;
            }

            positionOld.rotation = IO.transform.rotation;
            positionOld.position = IO.transform.position;
            isReplacingHand = false;
        }

        protected override void OnTrigger()
        {
            ReplaceHand();
        }

        protected override void OnReload()
        {
        }

        protected override void OnBefore()
        {
        }

        protected override void OnActive()
        {
        }

        protected override void OnAfter()
        {
        }
    }

}
