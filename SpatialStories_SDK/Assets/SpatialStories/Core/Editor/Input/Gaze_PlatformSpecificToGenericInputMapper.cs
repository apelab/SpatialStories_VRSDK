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

        private static Dictionary<Gaze_OculusInputTypes, Gaze_InputTypes> oculusToGeneric = new Dictionary<Gaze_OculusInputTypes, Gaze_InputTypes>()
        {
            {Gaze_OculusInputTypes.LeftJoystickPress, Gaze_InputTypes.STICK_LEFT_DOWN },
            {Gaze_OculusInputTypes.LeftJoystickRelease, Gaze_InputTypes.STICK_LEFT_UP },
            {Gaze_OculusInputTypes.LeftJoystickEast, Gaze_InputTypes.PAD_LEFT_TOUCH_EAST },
            {Gaze_OculusInputTypes.LeftJoystickEastRelease, Gaze_InputTypes.PAD_LEFT_UNTOUCH_EAST },
            {Gaze_OculusInputTypes.LeftJoystickSouth, Gaze_InputTypes.PAD_LEFT_TOUCH_SOUTH },
            {Gaze_OculusInputTypes.LeftJoystickSouthRelease, Gaze_InputTypes.PAD_LEFT_UNTOUCH_SOUTH },
            {Gaze_OculusInputTypes.LeftJoystickNorth, Gaze_InputTypes.PAD_LEFT_TOUCH_NORTH },
            {Gaze_OculusInputTypes.LeftJoystickNorthRelease, Gaze_InputTypes.PAD_LEFT_UNTOUCH_NORTH },
            {Gaze_OculusInputTypes.LeftJoystickWest, Gaze_InputTypes.PAD_LEFT_TOUCH_WEST },
            {Gaze_OculusInputTypes.LeftJoystickWestRelease, Gaze_InputTypes.PAD_LEFT_UNTOUCH_WEST },
            {Gaze_OculusInputTypes.LeftJoystickTouch, Gaze_InputTypes.STICK_LEFT_TOUCH },
            {Gaze_OculusInputTypes.LeftJoystickHandTriggerPress, Gaze_InputTypes.HAND_LEFT_DOWN },
            {Gaze_OculusInputTypes.LeftJoystickHandTriggerRelease, Gaze_InputTypes.HAND_LEFT_UP },
            {Gaze_OculusInputTypes.LeftJoystickIndexTriggerPress, Gaze_InputTypes.INDEX_LEFT_DOWN },
            {Gaze_OculusInputTypes.LeftJoystickIndexTriggerRelease, Gaze_InputTypes.INDEX_LEFT_UP },

            {Gaze_OculusInputTypes.RightJoystickPress, Gaze_InputTypes.STICK_RIGHT_DOWN },
            {Gaze_OculusInputTypes.RightJoystickRelease, Gaze_InputTypes.STICK_RIGHT_UP },
            {Gaze_OculusInputTypes.RightJoystickEast, Gaze_InputTypes.PAD_RIGHT_TOUCH_EAST },
            {Gaze_OculusInputTypes.RightJoystickEastRelease, Gaze_InputTypes.PAD_RIGHT_UNTOUCH_EAST },
            {Gaze_OculusInputTypes.RightJoystickSouth, Gaze_InputTypes.PAD_RIGHT_TOUCH_SOUTH },
            {Gaze_OculusInputTypes.RightJoystickSouthRelease, Gaze_InputTypes.PAD_RIGHT_UNTOUCH_SOUTH },
            {Gaze_OculusInputTypes.RightJoystickNorth, Gaze_InputTypes.PAD_RIGHT_TOUCH_NORTH },
            {Gaze_OculusInputTypes.RightJoystickNorthRelease, Gaze_InputTypes.PAD_RIGHT_UNTOUCH_NORTH },
            {Gaze_OculusInputTypes.RightJoystickWest, Gaze_InputTypes.PAD_RIGHT_TOUCH_WEST },
            {Gaze_OculusInputTypes.RightJoystickWestRelease, Gaze_InputTypes.PAD_RIGHT_UNTOUCH_WEST },
            {Gaze_OculusInputTypes.RightJoystickTouch, Gaze_InputTypes.STICK_RIGHT_TOUCH },
            {Gaze_OculusInputTypes.RightJoystickHandTriggerPress, Gaze_InputTypes.HAND_RIGHT_DOWN },
            {Gaze_OculusInputTypes.RightJoystickHandTriggerRelease, Gaze_InputTypes.HAND_RIGHT_UP },
            {Gaze_OculusInputTypes.RightJoystickIndexTriggerPress, Gaze_InputTypes.INDEX_RIGHT_DOWN },
            {Gaze_OculusInputTypes.RightJoystickIndexTriggerRelease, Gaze_InputTypes.INDEX_RIGHT_UP },

            {Gaze_OculusInputTypes.AButtonPress, Gaze_InputTypes.A_BUTTON_DOWN },
            {Gaze_OculusInputTypes.AButtonRelease, Gaze_InputTypes.A_BUTTON_UP },
            {Gaze_OculusInputTypes.BButtonPress, Gaze_InputTypes.B_BUTTON_DOWN },
            {Gaze_OculusInputTypes.BButtonRelease, Gaze_InputTypes.B_BUTTON_UP },
            {Gaze_OculusInputTypes.XButtonPress, Gaze_InputTypes.X_BUTTON_DOWN },
            {Gaze_OculusInputTypes.XButtonRelease, Gaze_InputTypes.X_BUTTON_UP },
            {Gaze_OculusInputTypes.YButtonPress, Gaze_InputTypes.Y_BUTTON_DOWN },
            {Gaze_OculusInputTypes.YButtonRelease, Gaze_InputTypes.Y_BUTTON_UP },

            {Gaze_OculusInputTypes.StartButtonPress, Gaze_InputTypes.START_BUTTON },

            {Gaze_OculusInputTypes.AButtonTouch, Gaze_InputTypes.A_BUTTON_TOUCH },
            {Gaze_OculusInputTypes.BButtonTouch, Gaze_InputTypes.B_BUTTON_TOUCH },
            {Gaze_OculusInputTypes.XButtonTouch, Gaze_InputTypes.X_BUTTON_TOUCH },
            {Gaze_OculusInputTypes.YButtonTouch, Gaze_InputTypes.Y_BUTTON_TOUCH },

            {Gaze_OculusInputTypes.LeftThumbrestTouch, Gaze_InputTypes.THUMBREST_LEFT_TOUCH },
            {Gaze_OculusInputTypes.LeftIndexTouch, Gaze_InputTypes.INDEX_LEFT_TOUCH },


            {Gaze_OculusInputTypes.RightThumbrestTouch, Gaze_InputTypes.THUMBREST_RIGHT_TOUCH },
            {Gaze_OculusInputTypes.RightIndexTouch, Gaze_InputTypes.INDEX_RIGHT_TOUCH },

        };

        public static Gaze_InputTypes ToGenericInput(Gaze_Controllers _platform, int _inputType)
        {
            switch (_platform)
            {
                case Gaze_Controllers.HTC_VIVE:
                    return ViveToGenericInput((Gaze_HTCViveInputTypes)_inputType);
                    break;
                case Gaze_Controllers.OCULUS_RIFT:
                    return OculusToGenericInput((Gaze_OculusInputTypes)_inputType);
                    break;
                default:
                    Debug.LogError("Translation not implemented for this platform: " + _platform);
                    return Gaze_InputTypes.NONE;
                    break;
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

        private static Gaze_InputTypes OculusToGenericInput(Gaze_OculusInputTypes _inputType)
        {
            if (oculusToGeneric.ContainsKey(_inputType))
            {
                return oculusToGeneric[_inputType];
            }
            else
            {
                Debug.LogError("Translation not implemented for this inputType: " + _inputType);
                return Gaze_InputTypes.NONE;
            }
        }
    }
}
