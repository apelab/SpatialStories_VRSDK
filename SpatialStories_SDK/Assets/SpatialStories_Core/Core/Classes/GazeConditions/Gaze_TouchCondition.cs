using UnityEditor;
using UnityEngine;
using UnityEngine.VR;

namespace Gaze
{
    public class Gaze_TouchCondition : Gaze_AbstractCondition
    {
        public Gaze_TouchDistanceMode distanceMode;
        public GameObject touchedObject = null;

        private bool isTriggerPressed = false;
        public bool touchLeftValid = false;
        public bool touchRightValid = false;
        public bool touchDistanceModeLeftValid = false;
        public bool touchDistanceModeRightValid = false;
        public Collider gazeCollider;
        private GameObject pointedObject;
        private VRNode eventHand;

        /// <summary>
        /// TRUE if a distant touch ray is hitting this object, else FALSE (the controller grabbing from distance)
        /// </summary>
        public bool isLeftPointing = false;
        public bool isRightPointing = false;


        public Gaze_TouchCondition(Gaze_Conditions _gazeConditionsScript, Collider _gazeCollider) : base(_gazeConditionsScript)
        {
            gazeCollider = _gazeCollider;
        }

        public override bool IsValidated()
        {
            IsValid = ValidateTouchConditions();
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

            bool isTouchedObjectValid = IsTouchObjectValid(touchedObject, gazeConditionsScript.touchMap.touchHandsIndex);
            bool isTouchControllerValid = IsTouchControllerValid(eventHand);
            bool isTouchActionValid = IsTouchActionValid(eventHand, IsValid);
            bool isTouchDistanceValid = IsTouchDistanceValid(distanceMode, eventHand);
            bool isTouchInputValid = IsTouchInputValid(isTriggerPressed);

            bool valid = false;

            // if we've configured
            switch (gazeConditionsScript.touchMap.touchHandsIndex)
            {
                // BOTH hands
                case (int)Gaze_HandsEnum.BOTH:

                    valid = isTouchControllerValid && isTouchDistanceValid && isTouchedObjectValid;
                    break;

                //  the LEFT hand
                case (int)Gaze_HandsEnum.LEFT:
                    touchLeftValid = isTouchActionValid && isTouchControllerValid && isTouchDistanceValid && isTouchedObjectValid;

                    valid = touchLeftValid;
                    break;

                //  the RIGHT hand
                case (int)Gaze_HandsEnum.RIGHT:
                    touchRightValid = isTouchActionValid && isTouchControllerValid && isTouchDistanceValid && isTouchedObjectValid;

                    valid = touchRightValid;
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
                    if (gazeConditionsScript.touchMap.touchDistanceModeLeftIndex.Equals((int)Gaze_TouchDistanceMode.BOTH))
                    {
                        touchDistanceModeLeftValid = true;
                    }
                    else
                    {
                        touchDistanceModeLeftValid = ((_eventDistance.Equals(Gaze_TouchDistanceMode.DISTANT) && gazeConditionsScript.touchMap.touchDistanceModeLeftIndex.Equals((int)Gaze_TouchDistanceMode.DISTANT))) || ((_eventDistance.Equals(Gaze_TouchDistanceMode.PROXIMITY) && gazeConditionsScript.touchMap.touchDistanceModeLeftIndex.Equals((int)Gaze_TouchDistanceMode.PROXIMITY))) ? true : false;
                    }
                }

                // check right hand mode
                if (_eventHand.Equals(VRNode.RightHand))
                {
                    // if both modes are allowed, we're sure this is valid
                    if (gazeConditionsScript.touchMap.touchDistanceModeRightIndex.Equals((int)Gaze_TouchDistanceMode.BOTH))
                    {
                        touchDistanceModeRightValid = true;
                    }
                    else
                    {
                        touchDistanceModeRightValid = ((_eventDistance.Equals(Gaze_TouchDistanceMode.DISTANT) && gazeConditionsScript.touchMap.touchDistanceModeRightIndex.Equals((int)Gaze_TouchDistanceMode.DISTANT))) || ((_eventDistance.Equals(Gaze_TouchDistanceMode.PROXIMITY) && gazeConditionsScript.touchMap.touchDistanceModeRightIndex.Equals((int)Gaze_TouchDistanceMode.PROXIMITY))) ? true : false;
                    }
                }

                if (gazeConditionsScript.requireAllTouchables)
                    return touchDistanceModeLeftValid && touchDistanceModeRightValid;
                else
                    return touchDistanceModeLeftValid || touchDistanceModeRightValid ? true : false;
            }
            else
            {
                touchDistanceModeLeftValid = false;
                touchDistanceModeRightValid = false;

                // get the distance mode of the configured hand in the conditions
                int mapDistanceModeIndex = mapHand.Equals(Gaze_HandsEnum.LEFT) ? gazeConditionsScript.touchMap.touchDistanceModeLeftIndex : gazeConditionsScript.touchMap.touchDistanceModeRightIndex;

                if (_eventDistance.Equals(Gaze_TouchDistanceMode.PROXIMITY) && mapDistanceModeIndex.Equals((int)Gaze_TouchDistanceMode.PROXIMITY))
                {
                    if (mapHand.Equals(Gaze_HandsEnum.LEFT))
                    {
                        //Debug.Log("distanceModeIndex.Equals((int)Gaze_TouchDistanceMode.PROXIMITY) = " + distanceModeIndex.Equals((int)Gaze_TouchDistanceMode.PROXIMITY) + " and _eventDistanceMode =" + _eventDistanceMode);
                        touchDistanceModeLeftValid = true;
                        return true;
                    }
                    else if (mapHand.Equals(Gaze_HandsEnum.RIGHT))
                    {
                        touchDistanceModeRightValid = true;
                        return true;
                    }
                }
                else if (_eventDistance.Equals(Gaze_TouchDistanceMode.DISTANT) && mapDistanceModeIndex.Equals((int)Gaze_TouchDistanceMode.DISTANT))
                {
                    if (mapHand.Equals(Gaze_HandsEnum.LEFT))
                    {
                        touchDistanceModeLeftValid = true;
                        return true;
                    }
                    else if (mapHand.Equals(Gaze_HandsEnum.RIGHT))
                    {
                        touchDistanceModeRightValid = true;
                        return true;
                    }
                }
                else if (mapDistanceModeIndex.Equals((int)Gaze_TouchDistanceMode.BOTH))
                {
                    if (mapHand.Equals(Gaze_HandsEnum.LEFT))
                    {
                        touchDistanceModeLeftValid = true;
                        return true;
                    }
                    else if (mapHand.Equals(Gaze_HandsEnum.RIGHT))
                    {
                        touchDistanceModeRightValid = true;
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
            return _touchedObject.Equals(gazeConditionsScript.touchMap.touchEntryList[index].interactiveObject);
        }

        private bool IsTouchControllerValid(VRNode _touchingController)
        {
            for (int i = 0; i < gazeConditionsScript.touchMap.touchEntryList.Count; i++)
            {
                if (gazeConditionsScript.touchMap.touchEntryList[i].hand.Equals(_touchingController))
                    return true;
            }

            return false;
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
            switch (gazeConditionsScript.touchMap.touchHandsIndex)
            {
                //TODO @apelab BOTH condition


                // RIGHT hand
                case (int)Gaze_HandsEnum.RIGHT:
                    eventActionIndex = gazeConditionsScript.touchMap.touchActionRightIndex;
                    break;

                // LEFT hand
                case (int)Gaze_HandsEnum.LEFT:
                    eventActionIndex = gazeConditionsScript.touchMap.touchActionLeftIndex;
                    break;
            }

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

            //TODO @apelab BOTH hands condition

            return false;
        }

        /// <summary>
        /// Get the pointed object in the dico argument
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private GameObject GetPointedObject(Gaze_ControllerPointingEventArgs e)
        {
            GameObject _object = null;
            switch (gazeConditionsScript.touchMap.touchHandsIndex)
            {
                case (int)Gaze_HandsEnum.BOTH:
                    if (e.Dico.ContainsKey(VRNode.LeftHand))
                    {
                        e.Dico.TryGetValue(VRNode.LeftHand, out _object);
                    }

                    else if (e.Dico.ContainsKey(VRNode.RightHand))
                    {
                        e.Dico.TryGetValue(VRNode.RightHand, out _object);
                    }

                    break;

                case (int)Gaze_HandsEnum.LEFT:
                    e.Dico.TryGetValue(VRNode.LeftHand, out _object);
                    break;

                case (int)Gaze_HandsEnum.RIGHT:
                    e.Dico.TryGetValue(VRNode.RightHand, out _object);
                    break;
            }

            return _object;
        }

        private void OnControllerPointingEvent(Gaze_ControllerPointingEventArgs e)
        {
            // if touch OR grab is enabled
            if (gazeConditionsScript.RootIO.touch)
            {
                // get the pointed object
                pointedObject = GetPointedObject(e);

                // if this object is me
                if (pointedObject && pointedObject.Equals(gazeConditionsScript.RootIO.gameObject))
                {
                    // get the event's pointing hand
                    eventHand = e.Dico.ContainsKey(VRNode.LeftHand) ? VRNode.LeftHand : VRNode.RightHand;

                    // update touch state for inspector GUI
                    if (eventHand.Equals(VRNode.LeftHand))
                        isLeftPointing = e.IsPointed;

                    else if (eventHand.Equals(VRNode.RightHand))
                        isRightPointing = e.IsPointed;

                    // check if touch is valid
                    IsValid = ValidateTouchConditions();
                }
            }
        }

        private void OnControllerTouchEvent(Gaze_ControllerTouchEventArgs e)
        {

            // store the touched object
            VRNode eventHand = e.Dico.ContainsKey(VRNode.LeftHand) ? VRNode.LeftHand : VRNode.RightHand;
            e.Dico.TryGetValue(eventHand, out touchedObject);

            // if I'm concerned
            if (touchedObject.Equals(gazeConditionsScript.RootIO.gameObject))
            {
                // update members
                isTriggerPressed = e.IsTriggerPressed;
                IsValid = e.IsTouching;
                distanceMode = e.Mode;

                // check if touch is valid
                IsValid = ValidateTouchConditions();
            }

        }
    }
}
