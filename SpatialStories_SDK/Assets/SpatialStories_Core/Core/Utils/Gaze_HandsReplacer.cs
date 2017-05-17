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

        public enum HANDS { LEFT, RIGHT }
        public HANDS handToReplace = HANDS.RIGHT;

        /// <summary>
        /// This is mandatory and designates where the object will have its distant grab origin and grab target
        /// </summary>
        public Gaze_DistantGrabPointer DistantGrabObject;
        public Gaze_GrabPositionController GrabTarget;
        public Vector3 replaceRotation;
        public Transform handlePosition;

        private Gaze_GrabManager grabManager;
        private Gaze_InteractiveObject IO;

        private void Start()
        {
            IO = GetComponentInParent<Gaze_InteractiveObject>();
            handlePosition = IO.GetComponentInChildren<Gaze_Manipulation>().transform;

            bool searchingForLeftHand = handToReplace == HANDS.LEFT;

            foreach (Gaze_GrabManager gm in Gaze_GrabManager.GrabManagers)
            {
                if (gm.isLeftHand == searchingForLeftHand)
                {
                    grabManager = gm;
                }
            }
        }

        /// <summary>
        /// Deactivates the current hand visuals before destroying the game last hand game object
        /// </summary>
        private void UpdateVisuals(bool show)
        {
            GameObject Hand = grabManager.gameObject;
            MeshRenderer[] Visuals = Hand.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in Visuals)
                renderer.enabled = show;
        }

        private void ReplaceHand()
        {
            // tell the manager we detach this object (to parent it manually so the grab manager won't be able to detach it once hand's released)
            if (IO.GrabbingManager != null)
                IO.GrabbingManager.TryDetach();

            // Ensure that we are not parented anymore
            IO.transform.SetParent(null);

            // This object will not be able to be taken anymore
            IO.DisableManipulationMode(Gaze_ManipulationModes.GRAB);
            IO.DisableManipulationMode(Gaze_ManipulationModes.TOUCH);
            IO.DisableManipulationMode(Gaze_ManipulationModes.LEVITATE);

            // hide current hand's model
            UpdateVisuals(false);

            // destroy the actual hand's models
            GameObject Hand = grabManager.gameObject;
            foreach (Transform child in Hand.transform)
            {
                if (child.gameObject.GetInstanceID() != gameObject.GetInstanceID())
                {
                    Destroy(child.gameObject);
                }
            }

            // Parent to the hand and position to it
            IO.transform.SetParent(Hand.transform);
            IO.transform.localPosition = Vector3.zero;
            IO.transform.localRotation = Quaternion.Euler(replaceRotation);

            // Fire the event (for the Gaze_GrabManager to update locators and stuff)
            if (OnHandsReplaced != null)
                OnHandsReplaced(grabManager, GrabTarget.gameObject.transform, DistantGrabObject.gameObject);

            // show new hand's model
            UpdateVisuals(true);
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
