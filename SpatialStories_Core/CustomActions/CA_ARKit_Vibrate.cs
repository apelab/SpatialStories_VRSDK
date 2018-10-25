using UnityEngine;

namespace Gaze
{
    public class CA_ARKit_Vibrate : Gaze_AbstractBehaviour
    {
        public override void SetupUsingApi(GameObject _interaction)
        {
        }

        protected override void OnTrigger()
        {
            Handheld.Vibrate();
        }
    }
}

