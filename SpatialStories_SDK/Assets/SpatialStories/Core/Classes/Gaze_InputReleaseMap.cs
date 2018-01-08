using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gaze
{
    public class Gaze_InputReleaseMap
    {
        private static Dictionary<Gaze_InputTypes, List<Gaze_InputTypes>> pressToReleaseMap = new Dictionary<Gaze_InputTypes, List<Gaze_InputTypes>>()
        {
            // Regular buttons
            // BUTON                                                   RELEASE BUTTONS
            { Gaze_InputTypes.A_BUTTON, new List<Gaze_InputTypes>() { Gaze_InputTypes.A_BUTTON_UP }},
            { Gaze_InputTypes.A_BUTTON, new List<Gaze_InputTypes>() { Gaze_InputTypes.A_BUTTON_UP }},

            { Gaze_InputTypes.B_BUTTON, new List<Gaze_InputTypes>() { Gaze_InputTypes.B_BUTTON_UP }},
            { Gaze_InputTypes.B_BUTTON_DOWN, new List<Gaze_InputTypes>() { Gaze_InputTypes.B_BUTTON_UP }},

            { Gaze_InputTypes.X_BUTTON, new List<Gaze_InputTypes>() { Gaze_InputTypes.X_BUTTON_UP }},
            { Gaze_InputTypes.X_BUTTON_DOWN, new List<Gaze_InputTypes>() { Gaze_InputTypes.X_BUTTON_UP }},
            
            { Gaze_InputTypes.Y_BUTTON, new List<Gaze_InputTypes>() { Gaze_InputTypes.Y_BUTTON_UP }},
            { Gaze_InputTypes.Y_BUTTON_DOWN, new List<Gaze_InputTypes>() { Gaze_InputTypes.Y_BUTTON_UP }},

            { Gaze_InputTypes.INDEX_LEFT, new List<Gaze_InputTypes>() { Gaze_InputTypes.INDEX_LEFT_UP }},
            { Gaze_InputTypes.INDEX_LEFT_DOWN, new List<Gaze_InputTypes>() { Gaze_InputTypes.INDEX_LEFT_UP }},

            { Gaze_InputTypes.INDEX_RIGHT, new List<Gaze_InputTypes>() { Gaze_InputTypes.INDEX_RIGHT_UP }},
            { Gaze_InputTypes.INDEX_RIGHT_DOWN, new List<Gaze_InputTypes>() { Gaze_InputTypes.INDEX_RIGHT_UP }},

            { Gaze_InputTypes.HAND_LEFT, new List<Gaze_InputTypes>() { Gaze_InputTypes.HAND_LEFT_UP }},
            { Gaze_InputTypes.HAND_LEFT_DOWN, new List<Gaze_InputTypes>() { Gaze_InputTypes.HAND_LEFT_UP }},

            { Gaze_InputTypes.HAND_RIGHT, new List<Gaze_InputTypes>() { Gaze_InputTypes.HAND_RIGHT_UP }},
            { Gaze_InputTypes.HAND_RIGHT_DOWN, new List<Gaze_InputTypes>() { Gaze_InputTypes.HAND_RIGHT_UP }},

            { Gaze_InputTypes.STICK_LEFT, new List<Gaze_InputTypes>() { Gaze_InputTypes.STICK_LEFT_UP }},
            { Gaze_InputTypes.STICK_LEFT_DOWN, new List<Gaze_InputTypes>() { Gaze_InputTypes.STICK_LEFT_UP }},

            { Gaze_InputTypes.STICK_RIGHT, new List<Gaze_InputTypes>() { Gaze_InputTypes.STICK_RIGHT_UP }},
            { Gaze_InputTypes.STICK_RIGHT_DOWN, new List<Gaze_InputTypes>() { Gaze_InputTypes.STICK_RIGHT_UP }},
            
            // Axis buttons treated as button
            { Gaze_InputTypes.PAD_LEFT_TOUCH_WEST, new List<Gaze_InputTypes>() {
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_EAST,
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_NORTH,
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_SOUTH,
                                                        Gaze_InputTypes.PAD_LEFT_RELEASE_WEST
                                                    }
            },

        };
    }
}
