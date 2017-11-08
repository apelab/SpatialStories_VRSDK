
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gaze
{
    public class Gaze_HandsReplacerV2 : Gaze_AbstractBehaviour
    {
        public Gaze_HandsReplacer.HANDS HandToReplace;
        public Gaze_InteractiveObject HandReplacerIO;

        private Gaze_DistantGrabPointer newDistantGrabPointer;
        private Gaze_GrabPositionController newGrabPositionController;
        private Transform handlePosition;
        private Transform lastTransformPosition;
        private Gaze_GrabManager grabManager;

        public List<Renderer> VisualsToShowAfterReplace;

        private void Start()
        {
            newDistantGrabPointer = HandReplacerIO.gameObject.GetComponentInChildrenBFS<Gaze_DistantGrabPointer>();
            newGrabPositionController = HandReplacerIO.gameObject.GetComponentInChildrenBFS<Gaze_GrabPositionController>();

            SetActualPositionAsOld();

            bool searchingForLeftHand = HandToReplace == Gaze_HandsReplacer.HANDS.LEFT;
            foreach (Gaze_GrabManager gm in Gaze_GrabManager.GrabManagers)
            {
                if (gm.isLeftHand == searchingForLeftHand)
                    grabManager = gm;
            }
        }


        bool isReplacingHand = false;
        public void SetActualPositionAsOld()
        {
            if (lastTransformPosition == null)
            {
                lastTransformPosition = new GameObject(HandReplacerIO.name).transform;
                lastTransformPosition.name = HandReplacerIO.name;
            }

            lastTransformPosition.rotation = HandReplacerIO.transform.rotation;
            lastTransformPosition.position = HandReplacerIO.transform.position;
            isReplacingHand = false;
        }


        /// <summary>
        /// Deactivates the current hand visuals before destroying the game last hand game object
        /// </summary>
        private void UpdateVisuals(GameObject obj, bool show)
        {
            if (show && VisualsToShowAfterReplace != null && VisualsToShowAfterReplace.Count > 0)
            {
                foreach(Renderer rend in VisualsToShowAfterReplace)
                {
                    rend.enabled = true;
                }

                foreach(Gaze_GrabManager gm in Gaze_GrabManager.GrabManagers)
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

        public void DestroyInitialHandAndReplace()
        {
            // tell the manager we detach this object (to parent it manually so the grab manager won't be able to detach it once hand's released)
            if (HandReplacerIO.GrabLogic.GrabbingManager != null)
                HandReplacerIO.GrabLogic.GrabbingManager.TryDetach();

            // Ensure that we are not parented anymore
            HandReplacerIO.transform.SetParent(null);

            // This object will not be able to be taken anymore
            HandReplacerIO.EnableManipulationMode(Gaze_ManipulationModes.NONE);

            // hide current hand's model
            UpdateVisuals(grabManager.gameObject, false);

            GameObject Hand = grabManager.gameObject;
            Gaze_InteractiveObject io = null;

            foreach (Transform child in Hand.transform)
            {
                if (child.gameObject.GetInstanceID() != gameObject.GetInstanceID())
                {
                    Destroy(child.gameObject);
                }
            }

            Gaze_HandsReplacer.FireHandReplacedEvent(grabManager, newGrabPositionController.transform, newDistantGrabPointer.gameObject);

            // Parent to the hand and position to it
            HandReplacerIO.transform.SetParent(Hand.transform);

            HandReplacerIO.transform.localPosition = Vector3.zero;
            HandReplacerIO.transform.localRotation = Quaternion.Euler(Vector3.zero);
            
            
            // show new hand's model
            UpdateVisuals(grabManager.gameObject, true);
            Gaze_HandsReplacer.handHasBeenDestroyed = true;
        }

        public void ReplaceWandAndDropOldOne()
        {
            // tell the manager we detach this object (to parent it manually so the grab manager won't be able to detach it once hand's released)
            if (HandReplacerIO.GrabLogic.GrabbingManager != null)
                HandReplacerIO.GrabLogic.GrabbingManager.TryDetach();

            // Ensure that we are not parented anymore
            HandReplacerIO.transform.SetParent(null);

            // This object will not be able to be taken anymore
            HandReplacerIO.EnableManipulationMode(Gaze_ManipulationModes.NONE);

            // hide current hand's model
            UpdateVisuals(grabManager.gameObject, false);

            GameObject Hand = grabManager.gameObject;
            Gaze_InteractiveObject io = null;

            foreach (Transform child in Hand.transform)
            {
                if (child.gameObject.GetInstanceID() != gameObject.GetInstanceID())
                {
                    if (io == null)
                    {
                        io = Gaze_Utils.GetIOFromGameObject(child.gameObject);
                    }

                    io.EnableManipulationMode(Gaze_ManipulationModes.GRAB);
                    io.transform.position = lastTransformPosition.transform.position;
                    io.transform.rotation = lastTransformPosition.transform.rotation;
                    child.parent = null;
                    child.position = lastTransformPosition.position;
                    child.rotation = lastTransformPosition.rotation;

                    UpdateVisuals(child.gameObject, true);
                }
            }
            
            io.GetComponentInChildren<Gaze_HandsReplacer>().SetActualPositionAsOld();

            Gaze_HandsReplacer.FireHandReplacedEvent(grabManager, newGrabPositionController.transform, newDistantGrabPointer.gameObject);

            // Parent to the hand and position to it
            HandReplacerIO.transform.SetParent(Hand.transform);

            HandReplacerIO.transform.localPosition = Vector3.zero;
            HandReplacerIO.transform.localRotation = Quaternion.Euler(Vector3.zero);

            // show new hand's model
            UpdateVisuals(grabManager.gameObject, true);

            isReplacingHand = true;
            
            // show new hand's model
            UpdateVisuals(grabManager.gameObject, true);
            Gaze_HandsReplacer.handHasBeenDestroyed = true;

        }

        protected override void OnTrigger()
        {
            if (Gaze_HandsReplacer.handHasBeenDestroyed)
                ReplaceWandAndDropOldOne();
            else
                DestroyInitialHandAndReplace();
        }
        
        #region NonUsedMethods
        protected override void OnActive() { }
        protected override void OnAfter() { }
        protected override void OnBefore() { }
        protected override void OnReload() { }
        #endregion NonUsedMethods
    }
}
