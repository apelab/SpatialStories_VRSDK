using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    public class Gaze_PlatformSpecificToGenericInputMapper
    {
        private static Dictionary<Gaze_HTCViveInputTypes, Gaze_InputTypes> viveToGenericMap = new Dictionary<Gaze_HTCViveInputTypes, Gaze_InputTypes>()
        {
            { Gaze_HTCViveInputTypes.LeftTrackpadCenterPress, Gaze_InputTypes.STICK_LEFT_DOWN },
            { Gaze_HTCViveInputTypes.LeftTrackpadNorthPress, Gaze_InputTypes.PAD_LEFT_PRESS_NORTH },
            { Gaze_HTCViveInputTypes.LeftTrackpadSouthPress, Gaze_InputTypes.PAD_LEFT_PRESS_SOUTH },
            { Gaze_HTCViveInputTypes.LeftTrackpadEastPress, Gaze_InputTypes.PAD_LEFT_PRESS_EAST },
        };

        public static Gaze_InputTypes ToGenericInput(Gaze_Controllers _platform, int _inputType)
        {
            switch (_platform)
            {
                case Gaze_Controllers.HTC_VIVE:
                    return ViveToGenericInput((Gaze_HTCViveInputTypes)_inputType);
                    break;
                default:
                    Debug.LogError("Translation not implemented for this platform: " + _platform);
                    return Gaze_InputTypes.A_BUTTON;
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
                return Gaze_InputTypes.A_BUTTON;
            }
        }
    }
}
