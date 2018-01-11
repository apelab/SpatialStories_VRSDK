using System.Collections.Generic;

namespace Gaze
{
    public class Gaze_InputReleaseMap
    {
        private static Dictionary<Gaze_InputTypes, List<Gaze_InputTypes>> pressToReleaseMap = new Dictionary<Gaze_InputTypes, List<Gaze_InputTypes>>()
        {
            // Regular buttons
            // BUTON                                                   RELEASE BUTTONS
            { Gaze_InputTypes.A_BUTTON, new List<Gaze_InputTypes>() { Gaze_InputTypes.A_BUTTON_UP }},
            { Gaze_InputTypes.A_BUTTON_DOWN, new List<Gaze_InputTypes>() { Gaze_InputTypes.A_BUTTON_UP }},

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
            // WEST
            { Gaze_InputTypes.PAD_LEFT_TOUCH_WEST, new List<Gaze_InputTypes>() {
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_EAST,
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_NORTH,
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_SOUTH,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_EAST,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_NORTH,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_SOUTH,
                                                        Gaze_InputTypes.PAD_LEFT_RELEASE_WEST,
                                                        Gaze_InputTypes.PAD_LEFT_UNTOUCH_WEST,
                                                    }
            },

            { Gaze_InputTypes.PAD_LEFT_PRESS_WEST, new List<Gaze_InputTypes>() {
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_EAST,
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_NORTH,
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_SOUTH,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_EAST,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_NORTH,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_SOUTH,
                                                        Gaze_InputTypes.PAD_LEFT_RELEASE_WEST
                                                    }
            },

            { Gaze_InputTypes.PAD_RIGHT_TOUCH_WEST, new List<Gaze_InputTypes>() {
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_EAST,
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_NORTH,
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_SOUTH,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_EAST,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_NORTH,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_SOUTH,
                                                        Gaze_InputTypes.PAD_RIGHT_RELEASE_WEST,
                                                        Gaze_InputTypes.PAD_RIGHT_UNTOUCH_WEST,
                                                    }
            },

            { Gaze_InputTypes.PAD_RIGHT_PRESS_WEST, new List<Gaze_InputTypes>() {
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_EAST,
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_NORTH,
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_SOUTH,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_EAST,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_NORTH,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_SOUTH,
                                                        Gaze_InputTypes.PAD_RIGHT_RELEASE_WEST
                                                    }
            },

            // EAST
            { Gaze_InputTypes.PAD_LEFT_TOUCH_EAST, new List<Gaze_InputTypes>() {
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_WEST,
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_NORTH,
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_SOUTH,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_WEST,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_NORTH,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_SOUTH,
                                                        Gaze_InputTypes.PAD_LEFT_RELEASE_EAST,
                                                        Gaze_InputTypes.PAD_LEFT_UNTOUCH_EAST,
                                                    }
            },
            { Gaze_InputTypes.PAD_LEFT_PRESS_EAST, new List<Gaze_InputTypes>() {
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_WEST,
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_NORTH,
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_SOUTH,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_WEST,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_NORTH,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_SOUTH,
                                                        Gaze_InputTypes.PAD_LEFT_RELEASE_EAST
                                                    }
            },
            { Gaze_InputTypes.PAD_RIGHT_TOUCH_EAST, new List<Gaze_InputTypes>() {
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_WEST,
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_NORTH,
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_SOUTH,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_WEST,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_NORTH,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_SOUTH,
                                                        Gaze_InputTypes.PAD_RIGHT_RELEASE_EAST,
                                                        Gaze_InputTypes.PAD_RIGHT_UNTOUCH_EAST,
                                                    }
            },
            { Gaze_InputTypes.PAD_RIGHT_PRESS_EAST, new List<Gaze_InputTypes>() {
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_WEST,
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_NORTH,
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_SOUTH,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_WEST,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_NORTH,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_SOUTH,
                                                        Gaze_InputTypes.PAD_RIGHT_RELEASE_EAST
                                                    }
            },

            // NORTH
            { Gaze_InputTypes.PAD_LEFT_TOUCH_NORTH, new List<Gaze_InputTypes>() {
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_WEST,
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_EAST,
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_SOUTH,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_WEST,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_EAST,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_SOUTH,
                                                        Gaze_InputTypes.PAD_LEFT_RELEASE_NORTH,
                                                        Gaze_InputTypes.PAD_LEFT_UNTOUCH_NORTH,
                                                    }
            },
            { Gaze_InputTypes.PAD_LEFT_PRESS_NORTH, new List<Gaze_InputTypes>() {
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_WEST,
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_EAST,
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_SOUTH,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_WEST,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_EAST,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_SOUTH,
                                                        Gaze_InputTypes.PAD_LEFT_RELEASE_NORTH,
                                                    }
            },
            { Gaze_InputTypes.PAD_RIGHT_TOUCH_NORTH, new List<Gaze_InputTypes>() {
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_WEST,
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_EAST,
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_SOUTH,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_WEST,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_EAST,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_SOUTH,
                                                        Gaze_InputTypes.PAD_RIGHT_RELEASE_NORTH,
                                                        Gaze_InputTypes.PAD_RIGHT_UNTOUCH_NORTH,
                                                    }
            },
            { Gaze_InputTypes.PAD_RIGHT_PRESS_NORTH, new List<Gaze_InputTypes>() {
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_WEST,
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_EAST,
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_SOUTH,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_WEST,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_EAST,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_SOUTH,
                                                        Gaze_InputTypes.PAD_RIGHT_RELEASE_NORTH
                                                    }
            },

            //SOUTH
            { Gaze_InputTypes.PAD_LEFT_TOUCH_SOUTH, new List<Gaze_InputTypes>() {
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_WEST,
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_EAST,
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_NORTH,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_WEST,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_EAST,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_NORTH,
                                                        Gaze_InputTypes.PAD_LEFT_RELEASE_SOUTH,
                                                        Gaze_InputTypes.PAD_LEFT_UNTOUCH_SOUTH,
                                                    }
            },
            { Gaze_InputTypes.PAD_LEFT_PRESS_SOUTH, new List<Gaze_InputTypes>() {
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_WEST,
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_EAST,
                                                        Gaze_InputTypes.PAD_LEFT_TOUCH_NORTH,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_WEST,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_EAST,
                                                        Gaze_InputTypes.PAD_LEFT_PRESS_NORTH,
                                                        Gaze_InputTypes.PAD_LEFT_RELEASE_SOUTH
                                                    }
            },
            { Gaze_InputTypes.PAD_RIGHT_TOUCH_SOUTH, new List<Gaze_InputTypes>() {
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_WEST,
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_EAST,
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_NORTH,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_WEST,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_EAST,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_NORTH,
                                                        Gaze_InputTypes.PAD_RIGHT_RELEASE_SOUTH,
                                                        Gaze_InputTypes.PAD_RIGHT_UNTOUCH_SOUTH,
                                                    }
            },
            { Gaze_InputTypes.PAD_RIGHT_PRESS_SOUTH, new List<Gaze_InputTypes>() {
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_WEST,
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_EAST,
                                                        Gaze_InputTypes.PAD_RIGHT_TOUCH_NORTH,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_WEST,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_EAST,
                                                        Gaze_InputTypes.PAD_RIGHT_PRESS_NORTH,
                                                        Gaze_InputTypes.PAD_RIGHT_RELEASE_SOUTH
                                                    }
            },

            // Touch
            { Gaze_InputTypes.PAD_LEFT_TOUCH, new List<Gaze_InputTypes>() { Gaze_InputTypes.PAD_LEFT_UNTOUCH } },
            { Gaze_InputTypes.PAD_RIGHT_TOUCH, new List<Gaze_InputTypes>() { Gaze_InputTypes.PAD_RIGHT_UNTOUCH } },

            { Gaze_InputTypes.A_BUTTON_TOUCH, new List<Gaze_InputTypes>() { Gaze_InputTypes.A_BUTTON_UNTOUCH} },
            { Gaze_InputTypes.B_BUTTON_TOUCH, new List<Gaze_InputTypes>() { Gaze_InputTypes.B_BUTTON_UNTOUCH} },
            { Gaze_InputTypes.X_BUTTON_TOUCH, new List<Gaze_InputTypes>() { Gaze_InputTypes.X_BUTTON_UNTOUCH} },
            { Gaze_InputTypes.Y_BUTTON_TOUCH, new List<Gaze_InputTypes>() { Gaze_InputTypes.Y_BUTTON_UNTOUCH} },
            { Gaze_InputTypes.THUMBREST_LEFT_TOUCH, new List<Gaze_InputTypes>() { Gaze_InputTypes.THUMBREST_LEFT_UNTOUCH} },
            { Gaze_InputTypes.THUMBREST_RIGHT_TOUCH, new List<Gaze_InputTypes>() { Gaze_InputTypes.THUMBREST_RIGHT_UNTOUCH} },
            { Gaze_InputTypes.STICK_LEFT_TOUCH, new List<Gaze_InputTypes>() { Gaze_InputTypes.STICK_LEFT_UNTOUCH} },
            { Gaze_InputTypes.STICK_RIGHT_TOUCH, new List<Gaze_InputTypes>() { Gaze_InputTypes.STICK_RIGHT_UNTOUCH} },
            { Gaze_InputTypes.INDEX_LEFT_TOUCH, new List<Gaze_InputTypes>() { Gaze_InputTypes.INDEX_LEFT_UNTOUCH} },
            { Gaze_InputTypes.INDEX_RIGHT_TOUCH, new List<Gaze_InputTypes>() { Gaze_InputTypes.INDEX_RIGHT_UNTOUCH} },

        };

        private static Dictionary<Gaze_InputTypes, Gaze_InputTypes> PressToRelease = new Dictionary<Gaze_InputTypes, Gaze_InputTypes>()
        {
            { Gaze_InputTypes.PAD_LEFT_TOUCH, Gaze_InputTypes.PAD_LEFT_UNTOUCH },
            { Gaze_InputTypes.PAD_RIGHT_TOUCH, Gaze_InputTypes.PAD_RIGHT_UNTOUCH },

            { Gaze_InputTypes.PAD_LEFT_TOUCH_WEST, Gaze_InputTypes.PAD_LEFT_UNTOUCH_WEST },
            { Gaze_InputTypes.PAD_LEFT_TOUCH_EAST, Gaze_InputTypes.PAD_LEFT_UNTOUCH_EAST },
            { Gaze_InputTypes.PAD_LEFT_TOUCH_NORTH, Gaze_InputTypes.PAD_LEFT_UNTOUCH_NORTH },
            { Gaze_InputTypes.PAD_LEFT_TOUCH_SOUTH, Gaze_InputTypes.PAD_LEFT_UNTOUCH_SOUTH },

            { Gaze_InputTypes.PAD_RIGHT_TOUCH_WEST, Gaze_InputTypes.PAD_RIGHT_UNTOUCH_WEST },
            { Gaze_InputTypes.PAD_RIGHT_TOUCH_EAST, Gaze_InputTypes.PAD_RIGHT_UNTOUCH_EAST },
            { Gaze_InputTypes.PAD_RIGHT_TOUCH_NORTH, Gaze_InputTypes.PAD_RIGHT_UNTOUCH_NORTH },
            { Gaze_InputTypes.PAD_RIGHT_TOUCH_SOUTH, Gaze_InputTypes.PAD_RIGHT_UNTOUCH_SOUTH },

            { Gaze_InputTypes.PAD_LEFT_PRESS_WEST, Gaze_InputTypes.PAD_LEFT_RELEASE_WEST },
            { Gaze_InputTypes.PAD_LEFT_PRESS_EAST, Gaze_InputTypes.PAD_LEFT_RELEASE_EAST },
            { Gaze_InputTypes.PAD_LEFT_PRESS_NORTH, Gaze_InputTypes.PAD_LEFT_RELEASE_NORTH },
            { Gaze_InputTypes.PAD_LEFT_PRESS_SOUTH, Gaze_InputTypes.PAD_LEFT_RELEASE_SOUTH },

            { Gaze_InputTypes.PAD_RIGHT_PRESS_WEST, Gaze_InputTypes.PAD_RIGHT_RELEASE_WEST },
            { Gaze_InputTypes.PAD_RIGHT_PRESS_EAST, Gaze_InputTypes.PAD_RIGHT_RELEASE_EAST },
            { Gaze_InputTypes.PAD_RIGHT_PRESS_NORTH, Gaze_InputTypes.PAD_RIGHT_RELEASE_NORTH },
            { Gaze_InputTypes.PAD_RIGHT_PRESS_SOUTH, Gaze_InputTypes.PAD_RIGHT_RELEASE_SOUTH },
        };

        /// <summary>
        /// Get the release input for an input type
        /// </summary>
        /// <param name="_input"> The checking type </param>
        /// <returns> The "Release Input" if any or NONE </returns>
        public static Gaze_InputTypes GetReleaseInputFor(Gaze_InputTypes _input)
        {
            if (!PressToRelease.ContainsKey(_input))
                return Gaze_InputTypes.NONE;

            return PressToRelease[_input];
        }

        /// <summary>
        /// Checks if the testing input is a "Release" of the base input
        /// </summary>
        /// <param name="_testingInput">The input type that can potentially be a release input of _baseInput</param>
        /// <param name="_baseInput"> The input the can be a potential press event of the _testingInput </param>
        /// <returns></returns>
        public static bool IsReleaseInputtOf(Gaze_InputTypes _testingInput, Gaze_InputTypes _baseInput)
        {
            // If the input doesn't have any release input (probably because is a release input) return false
            if (!pressToReleaseMap.ContainsKey(_baseInput))
                return false;

            // If the base input has any release input test if the event is one of them
            return pressToReleaseMap[_baseInput].Contains(_testingInput);
        }
    }
}
