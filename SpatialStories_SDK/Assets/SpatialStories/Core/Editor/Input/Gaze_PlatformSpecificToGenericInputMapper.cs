using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    public class Gaze_PlatformSpecificToGenericInputMapper
    {
        private static Dictionary<Gaze_HTCViveInputTypes, Gaze_InputTypes> viveToGenericMap = new Dictionary<Gaze_HTCViveInputTypes, Gaze_InputTypes>()
        {
            // Left Controller
            { Gaze_HTCViveInputTypes.LeftTrackpadCenterPress, Gaze_InputTypes.STICK_LEFT_DOWN },

            // Press
            { Gaze_HTCViveInputTypes.LeftTrackpadNorthPress, Gaze_InputTypes.PAD_LEFT_PRESS_NORTH },
            { Gaze_HTCViveInputTypes.LeftTrackpadSouthPress, Gaze_InputTypes.PAD_LEFT_PRESS_SOUTH },
            { Gaze_HTCViveInputTypes.LeftTrackpadEastPress, Gaze_InputTypes.PAD_LEFT_PRESS_EAST },
            { Gaze_HTCViveInputTypes.LeftTrackpadWestPress, Gaze_InputTypes.PAD_LEFT_PRESS_WEST },

            // Release
            { Gaze_HTCViveInputTypes.LeftTrackpadCenterRelease, Gaze_InputTypes.STICK_LEFT_UP },
            { Gaze_HTCViveInputTypes.LeftTrackpadNorthRelease, Gaze_InputTypes.PAD_LEFT_RELEASE_NORTH },
            { Gaze_HTCViveInputTypes.LeftTrackpadSouthRelease, Gaze_InputTypes.PAD_LEFT_RELEASE_SOUTH },
            { Gaze_HTCViveInputTypes.LeftTrackpadWestRelease, Gaze_InputTypes.PAD_LEFT_RELEASE_WEST },
            { Gaze_HTCViveInputTypes.LeftTrackpadEastRelease, Gaze_InputTypes.PAD_LEFT_RELEASE_EAST },

            // Touch
            { Gaze_HTCViveInputTypes.LeftTrackpadNorthTouch, Gaze_InputTypes.PAD_LEFT_TOUCH_NORTH },
            { Gaze_HTCViveInputTypes.LeftTrackpadSouthTouch, Gaze_InputTypes.PAD_LEFT_TOUCH_SOUTH },
            { Gaze_HTCViveInputTypes.LeftTrackpadEastTouch, Gaze_InputTypes.PAD_LEFT_TOUCH_EAST },
            { Gaze_HTCViveInputTypes.LeftTrackpadWestTouch, Gaze_InputTypes.PAD_LEFT_TOUCH_WEST },
            { Gaze_HTCViveInputTypes.LeftTrackpadCenterTouch, Gaze_InputTypes.PAD_LEFT_TOUCH },

            // Untouch
            { Gaze_HTCViveInputTypes.LeftTrackpadNorthUntouch, Gaze_InputTypes.PAD_LEFT_UNTOUCH_NORTH },
            { Gaze_HTCViveInputTypes.LeftTrackpadSouthUntouch, Gaze_InputTypes.PAD_LEFT_UNTOUCH_SOUTH },
            { Gaze_HTCViveInputTypes.LeftTrackpadEastUntouch, Gaze_InputTypes.PAD_LEFT_UNTOUCH_EAST },
            { Gaze_HTCViveInputTypes.LeftTrackpadWestUntouch, Gaze_InputTypes.PAD_LEFT_UNTOUCH_WEST },
            { Gaze_HTCViveInputTypes.LeftTrackpadCenterUntouch, Gaze_InputTypes.PAD_LEFT_UNTOUCH },

            // Index
            { Gaze_HTCViveInputTypes.LeftTriggerPress, Gaze_InputTypes.INDEX_LEFT_DOWN },
            { Gaze_HTCViveInputTypes.LeftTriggerRelease, Gaze_InputTypes.INDEX_LEFT_UP },

            // Hand
            { Gaze_HTCViveInputTypes.LeftGripPress, Gaze_InputTypes.HAND_LEFT_DOWN },
            { Gaze_HTCViveInputTypes.LeftGripRelease, Gaze_InputTypes.HAND_LEFT_UP },
            
            // Right Controller

            // Press
            { Gaze_HTCViveInputTypes.RightTrackpadCenterPress, Gaze_InputTypes.STICK_RIGHT_DOWN },
            { Gaze_HTCViveInputTypes.RightTrackpadNorthPress, Gaze_InputTypes.PAD_RIGHT_PRESS_NORTH },
            { Gaze_HTCViveInputTypes.RightTrackpadSouthPress, Gaze_InputTypes.PAD_RIGHT_PRESS_SOUTH },
            { Gaze_HTCViveInputTypes.RightTrackpadEastPress, Gaze_InputTypes.PAD_RIGHT_PRESS_EAST },
            { Gaze_HTCViveInputTypes.RightTrackpadWestPress, Gaze_InputTypes.PAD_RIGHT_PRESS_WEST },

            // Release
            { Gaze_HTCViveInputTypes.RightTrackpadCenterRelease, Gaze_InputTypes.STICK_RIGHT_UP },
            { Gaze_HTCViveInputTypes.RightTrackpadNorthRelease, Gaze_InputTypes.PAD_RIGHT_RELEASE_NORTH },
            { Gaze_HTCViveInputTypes.RightTrackpadSouthRelease, Gaze_InputTypes.PAD_RIGHT_RELEASE_SOUTH },
            { Gaze_HTCViveInputTypes.RightTrackpadWestRelease, Gaze_InputTypes.PAD_RIGHT_RELEASE_WEST },
            { Gaze_HTCViveInputTypes.RightTrackpadEastRelease, Gaze_InputTypes.PAD_RIGHT_RELEASE_EAST },

            // Touch
            { Gaze_HTCViveInputTypes.RightTrackpadNorthTouch, Gaze_InputTypes.PAD_RIGHT_TOUCH_NORTH },
            { Gaze_HTCViveInputTypes.RightTrackpadSouthTouch, Gaze_InputTypes.PAD_RIGHT_TOUCH_SOUTH },
            { Gaze_HTCViveInputTypes.RightTrackpadEastTouch, Gaze_InputTypes.PAD_RIGHT_TOUCH_EAST },
            { Gaze_HTCViveInputTypes.RightTrackpadWestTouch, Gaze_InputTypes.PAD_RIGHT_TOUCH_WEST },
            { Gaze_HTCViveInputTypes.RightTrackpadCenterTouch, Gaze_InputTypes.PAD_RIGHT_TOUCH },

            // Untouch
            { Gaze_HTCViveInputTypes.RightTrackpadNorthUntouch, Gaze_InputTypes.PAD_RIGHT_UNTOUCH_NORTH },
            { Gaze_HTCViveInputTypes.RightTrackpadSouthUntouch, Gaze_InputTypes.PAD_RIGHT_UNTOUCH_SOUTH },
            { Gaze_HTCViveInputTypes.RightTrackpadEastUntouch, Gaze_InputTypes.PAD_RIGHT_UNTOUCH_EAST },
            { Gaze_HTCViveInputTypes.RightTrackpadWestUntouch, Gaze_InputTypes.PAD_RIGHT_UNTOUCH_WEST },
            { Gaze_HTCViveInputTypes.RightTrackpadCenterUntouch, Gaze_InputTypes.PAD_RIGHT_UNTOUCH },

            // Index
            { Gaze_HTCViveInputTypes.RightTriggerPress, Gaze_InputTypes.INDEX_RIGHT_DOWN },
            { Gaze_HTCViveInputTypes.RightTriggerRelease, Gaze_InputTypes.INDEX_RIGHT_UP },

            // Hand
            { Gaze_HTCViveInputTypes.RightGripPress, Gaze_InputTypes.HAND_RIGHT_DOWN },
            { Gaze_HTCViveInputTypes.RightGripRelease, Gaze_InputTypes.HAND_RIGHT_UP },
        };

        private static Dictionary<Gaze_GearVRInputTypes, Gaze_InputTypes> gearVrToGenericMap = new Dictionary<Gaze_GearVRInputTypes, Gaze_InputTypes>()
        {
            { Gaze_GearVRInputTypes.TrackpadPress, Gaze_InputTypes.STICK_RIGHT_DOWN },
            { Gaze_GearVRInputTypes.TrackpadNorthPress, Gaze_InputTypes.PAD_RIGHT_PRESS_SOUTH },
            { Gaze_GearVRInputTypes.TrackpadSouthPress, Gaze_InputTypes.PAD_RIGHT_PRESS_NORTH },
            { Gaze_GearVRInputTypes.TrackpadWestPress, Gaze_InputTypes.PAD_RIGHT_PRESS_EAST },
            { Gaze_GearVRInputTypes.TrackpadEastPress, Gaze_InputTypes.PAD_RIGHT_PRESS_WEST },
            { Gaze_GearVRInputTypes.TrackpadRelease, Gaze_InputTypes.STICK_RIGHT_UP },
            { Gaze_GearVRInputTypes.TrackpadNorthRelease, Gaze_InputTypes.PAD_RIGHT_RELEASE_SOUTH },
            { Gaze_GearVRInputTypes.TrackpadSouthRelease, Gaze_InputTypes.PAD_RIGHT_RELEASE_NORTH },
            { Gaze_GearVRInputTypes.TrackpadWestRelease, Gaze_InputTypes.PAD_RIGHT_RELEASE_EAST },
            { Gaze_GearVRInputTypes.TrackpadEastRelease, Gaze_InputTypes.PAD_RIGHT_RELEASE_WEST },
            { Gaze_GearVRInputTypes.TrackpadNorthTouch, Gaze_InputTypes.PAD_RIGHT_TOUCH_SOUTH },
            { Gaze_GearVRInputTypes.TrackpadSouthTouch, Gaze_InputTypes.PAD_RIGHT_TOUCH_NORTH },
            { Gaze_GearVRInputTypes.TrackpadEastTouch, Gaze_InputTypes.PAD_RIGHT_TOUCH_EAST },
            { Gaze_GearVRInputTypes.TrackpadWestTouch, Gaze_InputTypes.PAD_RIGHT_TOUCH_WEST },
            { Gaze_GearVRInputTypes.TrackpadCenterTouch, Gaze_InputTypes.PAD_RIGHT_TOUCH },
            { Gaze_GearVRInputTypes.TrackpadNorthUnTouch, Gaze_InputTypes.PAD_RIGHT_UNTOUCH_SOUTH },
            { Gaze_GearVRInputTypes.TrackpadSouthUnTouch, Gaze_InputTypes.PAD_RIGHT_UNTOUCH_NORTH },
            { Gaze_GearVRInputTypes.TrackpadEastUnTouch, Gaze_InputTypes.PAD_RIGHT_UNTOUCH_EAST },
            { Gaze_GearVRInputTypes.TrackpadWestUnTouch, Gaze_InputTypes.PAD_RIGHT_UNTOUCH_WEST },
            { Gaze_GearVRInputTypes.TrackpadCenterUnTouch, Gaze_InputTypes.PAD_RIGHT_UNTOUCH },
            { Gaze_GearVRInputTypes.TriggerDown, Gaze_InputTypes.HAND_RIGHT_DOWN },
            { Gaze_GearVRInputTypes.TriggerUp, Gaze_InputTypes.HAND_RIGHT_UP },
        };

        public static Gaze_InputTypes ToGenericInput(Gaze_Controllers _platform, int _inputType)
        {
            switch (_platform)
            {
                case Gaze_Controllers.HTC_VIVE:
                    return ViveToGenericInput((Gaze_HTCViveInputTypes)_inputType);
                    break;
                case Gaze_Controllers.GEARVR_CONTROLLER:
                    return GearVRToGenericInput((Gaze_GearVRInputTypes)_inputType);
                    break;
                default:
                    Debug.LogError("Translation not implemented for this platform: " + _platform);
                    return Gaze_InputTypes.NONE;
                    break;
            }
        }

        private static Gaze_InputTypes GearVRToGenericInput(Gaze_GearVRInputTypes _inputType)
        {
            if (gearVrToGenericMap.ContainsKey(_inputType))
            {
                return gearVrToGenericMap[_inputType];
            }
            else
            {
                Debug.LogError("Translation not implemented for this inputType: " + _inputType);
                return Gaze_InputTypes.NONE;
            }
        }

        private static Gaze_InputTypes ViveToGenericInput(Gaze_HTCViveInputTypes _inputType)
        {
            if (viveToGenericMap.ContainsKey(_inputType))
            {
                return viveToGenericMap[_inputType];
            }
            else
            {
                Debug.LogError("Translation not implemented for this inputType: " + _inputType);
                return Gaze_InputTypes.NONE;
            }
        }
    }
}
