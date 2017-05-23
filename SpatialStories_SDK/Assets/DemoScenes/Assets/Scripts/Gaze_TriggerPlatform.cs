using UnityEngine;


namespace Gaze
{
    public class Gaze_TriggerPlatform : Gaze_AbstractBehaviour
    {

        public Transform objectToParent;
        public bool parented = true;




        protected override void OnActive()
        {
        }

        protected override void OnAfter()
        {
        }

        protected override void OnBefore()
        {
        }

        protected override void OnReload()
        {
        }

        protected override void OnTrigger()
        {

            if (parented)
            {
                objectToParent.parent = gazable.Root.transform;
            }
            else
            {

                objectToParent.parent = null;

            }
        }
    }
}

