using UnityEditor;
using UnityEngine;
using UnityEngine.VR;

namespace Gaze
{
    public class Gaze_TouchCondition : Gaze_AbstractCondition
    {
        public Gaze_TouchDistanceMode DistanceMode;
        public GameObject TouchedObject = null;

        public bool TouchLeftValid = false;
        public bool TouchRightValid = false;
        public bool TouchDistanceModeLeftValid = false;
        public bool TouchDistanceModeRightValid = false;
        public Collider GazeCollider;

        /// <summary>
        /// TRUE if a distant touch ray is hitting this object, else FALSE (the controller grabbing from distance)
        /// </summary>
        public bool IsLeftPointing = false;
        public bool IsRightPointing = false;

        private bool isTriggerPressed = false;
        private GameObject pointedObject;
        private VRNode eventHand;

        public Gaze_TouchCondition(Gaze_Conditions _gazeConditionsScript, Collider _gazeCollider) : base(_gazeConditionsScript)
        {
            GazeCollider = _gazeCollider;
        }

        public override bool IsValidated()
        {
            return IsValid;
        }

        protected override void CustomSetup()
        {
            Gaze_EventManager.OnControllerPointingEvent += OnControllerPointingEvent;
            Gaze_InputManager.OnControllerTouchEvent += OnControllerTouchEvent;
        }

        protected override void CustomDispose()
        {
            Gaze_EventManager.OnControllerPointingEvent -= OnControllerPointingEvent;
            Gaze_InputManager.OnControllerTouchEvent -= OnControllerTouchEvent;
        }

        public override void ToEditorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (IsValid)
            {
                RenderSatisfiedLabel("Touch:");
                RenderSatisfiedLabel("True");
            }
            else
            {
                RenderNonSatisfiedLabel("Touch:");
                RenderNonSatisfiedLabel("False");
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// To validate a touch, multiple conditions need to be met.
        /// - Controller : must match the condition (LEFT, RIGHT, BOTH)
        /// - Object : must match the condition (the one specified in the Touch conditions)
        /// - Action : must match the condition (GRAB, UNGRAB, BOTH)
        /// - Distance : must match the condition (PROXIMITY, DISTANT, BOTH)
        /// </summary>
        /// <param name="_eventHand"></param>
        /// <param name="_touchedObject"></param>
        /// <param name="_eventMode"></param>
        /// <param name="_isTouching"></param>
        /// <returns></returns>
        private bool ValidateTouchConditions()
        {
            bool isTouchedObjectValid = IsTouchObjectValid(TouchedObject, gazeConditionsScript.touchMap.touchHandsIndex);
            bool isTouchControllerValid = IsTouchControllerValid(eventHand);
            bool isTouchActionValid = IsTouchActionValid(eventHand, IsValid);
            bool isTouchDistanceValid = IsTouchDistanceValid(DistanceMode, eventHand);
            bool isTouchInputValid = IsTouchInputValid(isTriggerPressed);

            bool valid = false;

            // if we've configured
            switch (gazeConditionsScript.touchMap.touchHandsIndex)
            {
                // BOTH hands
                case (int)Gaze_HandsEnum.BOTH:
                    TouchLeftValid = isTouchActionValid && isTouchControllerValid && isTouchDistanceValid && isTouchedObjectValid;
                    break;

                //  the LEFT hand
                case (int)Gaze_HandsEnum.LEFT:
                    TouchLeftValid = isTouchActionValid && isTouchControllerValid && isTouchDistanceValid && isTouchedObjectValid;
                    valid = TouchLeftValid;
                    break;

                //  the RIGHT hand
                case (int)Gaze_HandsEnum.RIGHT:
                    TouchRightValid = isTouchActionValid && isTouchControllerValid && isTouchDistanceValid && isTouchedObjectValid;

                    valid = TouchRightValid;
                    break;
            }

            return valid;
        }

        private bool IsTouchInputValid(bool _isTriggerPressed)
        {
            int eventActionIndex = 0;
            switch (gazeConditionsScript.touchMap.touchHandsIndex)
            {
                // RIGHT hand
                case (int)Gaze_HandsEnum.RIGHT:
                    eventActionIndex = gazeConditionsScript.touchMap.touchActionRightIndex;
                    break;

                // LEFT hand
                case (int)Gaze_HandsEnum.LEFT:
                    eventActionIndex = gazeConditionsScript.touchMap.touchActionLeftIndex;
                    break;
            }

            // if trigger is pressed and action is TOUCH return TRUE
            if ((_isTriggerPressed && eventActionIndex.Equals((int)Gaze_TouchAction.TOUCH)) ||
                (!_isTriggerPressed && eventActionIndex.Equals((int)Gaze_TouchAction.UNTOUCH)))
            {
                return true;
            }

            return false;
        }

        private bool IsTouchDistanceValid(Gaze_TouchDistanceMode _eventDistance, VRNode _eventHand)
        {
            Gaze_HandsEnum mapHand = Gaze_HandsEnum.BOTH;

            switch (gazeConditionsScript.touchMap.touchHandsIndex)
            {
                // LEFT hands
                case (int)Gaze_HandsEnum.LEFT:
                    mapHand = Gaze_HandsEnum.LEFT;
                    break;

                // RIGHT hands
                case (int)Gaze_HandsEnum.RIGHT:
                    mapHand = Gaze_HandsEnum.RIGHT;
                    break;
            }

            if (mapHand.Equals(Gaze_HandsEnum.BOTH))
            {
                // check left hand mode 
                if (_eventHand.Equals(VRNode.LeftHand))
                {
                    // if both modes are allowed, we're sure this is valid      
                    TouchDistanceModeLeftValid = true;
                }

                // check right hand mode
                if (_eventHand.Equals(VRNode.RightHand))
                {
                    // if both modes are allowed, we're sure this is valid
                    TouchDistanceModeRightValid = true;
                }

                if (gazeConditionsScript.requireAllTouchables)
                    return TouchDistanceModeLeftValid && TouchDistanceModeRightValid;
                else
                    return TouchDistanceModeLeftValid || TouchDistanceModeRightValid;
            }
            else
            {
                TouchDistanceModeLeftValid = false;
                TouchDistanceModeRightValid = false;

                // get the distance mode of the configured hand in the conditions
                int mapDistanceModeIndex = (int)Gaze_TouchDistanceMode.BOTH;

                if (_eventDistance.Equals(Gaze_TouchDistanceMode.PROXIMITY) && mapDistanceModeIndex.Equals((int)Gaze_TouchDistanceMode.PROXIMITY))
                {
                    if (mapHand.Equals(Gaze_HandsEnum.LEFT))
                    {
                        //Debug.Log("distanceModeIndex.Equals((int)Gaze_TouchDistanceMode.PROXIMITY) = " + distanceModeIndex.Equals((int)Gaze_TouchDistanceMode.PROXIMITY) + " and _eventDistanceMode =" + _eventDistanceMode);
                        TouchDistanceModeLeftValid = true;
                        return true;
                    }
                    else if (mapHand.Equals(Gaze_HandsEnum.RIGHT))
                    {
                        TouchDistanceModeRightValid = true;
                        return true;
                    }
                }
                else if (_eventDistance.Equals(Gaze_TouchDistanceMode.DISTANT) && mapDistanceModeIndex.Equals((int)Gaze_TouchDistanceMode.DISTANT))
                {
                    if (mapHand.Equals(Gaze_HandsEnum.LEFT))
                    {
                        TouchDistanceModeLeftValid = true;
                        return true;
                    }
                    else if (mapHand.Equals(Gaze_HandsEnum.RIGHT))
                    {
                        TouchDistanceModeRightValid = true;
                        return true;
                    }
                }
                else if (mapDistanceModeIndex.Equals((int)Gaze_TouchDistanceMode.BOTH))
                {
                    if (mapHand.Equals(Gaze_HandsEnum.LEFT))
                    {
                        TouchDistanceModeLeftValid = true;
                        return true;
                    }
                    else if (mapHand.Equals(Gaze_HandsEnum.RIGHT))
                    {
                        TouchDistanceModeRightValid = true;
                        return true;
                    }
                }
            }

            return false;
        }


        private bool IsTouchObjectValid(GameObject _touchedObject, int _handIndex)
        {
            if (_touchedObject == null)
                return false;

            int index = _handIndex.Equals((int)Gaze_HandsEnum.BOTH) ? 1 : 0;
            return _touchedObject.Equals(gazeConditionsScript.touchMap.TouchEnitry.interactiveObject);
        }

        private bool IsTouchControllerValid(VRNode _touchingController)
        {
            if (gazeConditionsScript.touchMap.touchHandsIndex != (int)Gaze_HandsEnum.BOTH)
            {
                if (gazeConditionsScript.touchMap.TouchEnitry.hand.Equals(_touchingController))
                    return true;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Return TRUE if the Action is valid.
        /// Action is valid if action in the map equals action received.
        /// Action can be TOUCH, UNTOUCH or BOTH.
        /// </summary>
        /// <param name="_dicoVRNode"></param>
        /// <param name="_eventVRNode"></param>
        /// <returns></returns>
        private bool IsTouchActionValid(VRNode _touchingController, bool _isTouching)
        {
            int eventActionIndex = 0;
            eventActionIndex = gazeConditionsScript.touchMap.touchActionLeftIndex;

            
            if ((_touchingController.Equals(VRNode.LeftHand) && gazeConditionsScript.touchMap.touchActionLeftIndex.Equals(eventActionIndex)) ||
                (_touchingController.Equals(VRNode.RightHand) && gazeConditionsScript.touchMap.touchActionRightIndex.Equals(eventActionIndex)))
            {
                // if I'm touching AND the action is TOUCH AND the trigger is PRESSED
                if (_isTouching && eventActionIndex.Equals((int)Gaze_TouchAction.TOUCH) && isTriggerPressed)
                    return true;

                // if I'm touching AND the action is UNTOUCH AND the trigger is RELEASED
                if (_isTouching && eventActionIndex.Equals((int)Gaze_TouchAction.UNTOUCH) && !isTriggerPressed)
                    return true;

                // if I'm not touching AND action is UNTOUCH and trigger is PRESSED, that means we pointed OUT
                if (!_isTouching && eventActionIndex.Equals((int)Gaze_TouchAction.UNTOUCH) && isTriggerPressed)
                    return true;
            }
            return false;
        }


        private void OnControllerPointingEvent(Gaze_ControllerPointingEventArgs e)
        {
            // if touch OR grab is enabled
            if (gazeConditionsScript.RootIO.IsTouchEnabled)
            {
                // get the pointed object
                pointedObject = e.Dico.Value;

                // if this object is me
                if (pointedObject && pointedObject.Equals(gazeConditionsScript.RootIO.gameObject))
                {
                    // get the event's pointing hand
                    eventHand = e.Dico.Key;

                    // update touch state for inspector GUI
                    if (eventHand.Equals(VRNode.LeftHand))
                        IsLeftPointing = e.IsPointed;

                    else if (eventHand.Equals(VRNode.RightHand))
                        IsRightPointing = e.IsPointed;

                    // check if touch is valid
                    IsValid = ValidateTouchConditions();
                }
            }
        }

        private void OnControllerTouchEvent(Gaze_ControllerTouchEventArgs e)
        {
            // store the touched object
            VRNode eventHand = e.Dico.Key;
            TouchedObject = e.Dico.Value;

            // if I'm concerned
            if (TouchedObject.Equals(gazeConditionsScript.touchMap.TouchEnitry.interactiveObject))
            {
                // update members
                isTriggerPressed = e.IsTriggerPressed;
                IsValid = e.IsTouching;
                DistanceMode = e.Mode;

                // check if touch is valid
                IsValid = ValidateTouchConditions();
            }

        }
    }
}
