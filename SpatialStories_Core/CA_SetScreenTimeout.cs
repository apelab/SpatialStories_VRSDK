using Gaze;
using UnityEngine;

namespace SpatialStories
{
    public class CA_SetScreenTimeout : Gaze_AbstractBehaviour
    {
        public enum SleepTimeoutTypes { NeverSleep, SystemSettings }
        public SleepTimeoutTypes NewTimeout;

        public override void SetupUsingApi(GameObject _interaction){}

        protected override void OnTrigger()
        {
            //NOTE: I do it this way because I can't do an asignation (even if a try to cast)
            if (NewTimeout == SleepTimeoutTypes.NeverSleep)
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            else
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
    }

}
