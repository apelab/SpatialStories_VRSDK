//-----------------------------------------------------------------------
// <copyright file="LevitationEventArgs.cs" company="apelab sàrl">
// © apelab. All Rights Reserved.
//
// This source is subject to the apelab license.
// All other rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// <author>Michaël Martin</author>
// <email>dev@apelab.ch</email>
// <web>https://twitter.com/apelab_ch</web>
// <web>http://www.apelab.ch</web>
// <date>2016-01-25</date>
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_EDITOR
using UnityEditor;
#endif
using SpatialStories;

namespace Gaze
{
    public class Gaze_InputsCondition : Gaze_AbstractCondition
    {
        private int entriesCount;

        public Gaze_InputsCondition(Gaze_Conditions _gazeConditionsScript) : base(_gazeConditionsScript) { }

        protected override void CustomSetup()
        {
            #region inputs subscription
            Gaze_InputManager.OnStartEvent += OnInputEvent;
            Gaze_InputManager.OnButtonADownEvent += OnInputEvent;
            Gaze_InputManager.OnButtonAEvent += OnInputEvent;
            Gaze_InputManager.OnButtonAUpEvent += OnInputEvent;
            Gaze_InputManager.OnButtonBDownEvent += OnInputEvent;
            Gaze_InputManager.OnButtonBUpEvent += OnInputEvent;
            Gaze_InputManager.OnButtonXDownEvent += OnInputEvent;
            Gaze_InputManager.OnButtonXUpEvent += OnInputEvent;
            Gaze_InputManager.OnButtonYDownEvent += OnInputEvent;
            Gaze_InputManager.OnButtonYUpEvent += OnInputEvent;
            Gaze_InputManager.OnIndexLeftDownEvent += OnInputEvent;
            Gaze_InputManager.OnIndexLeftUpEvent += OnInputEvent;
            Gaze_InputManager.OnIndexRightDownEvent += OnInputEvent;
            Gaze_InputManager.OnIndexRightUpEvent += OnInputEvent;
            Gaze_InputManager.OnHandLeftDownEvent += OnInputEvent;
            Gaze_InputManager.OnHandLeftUpEvent += OnInputEvent;
            Gaze_InputManager.OnHandRightDownEvent += OnInputEvent;
            Gaze_InputManager.OnHandRightUpEvent += OnInputEvent;
            Gaze_InputManager.OnStickLeftDownEvent += OnInputEvent;
            Gaze_InputManager.OnStickLeftUpEvent += OnInputEvent;
            Gaze_InputManager.OnStickRightDownEvent += OnInputEvent;
            Gaze_InputManager.OnStickRightUpEvent += OnInputEvent;

            Gaze_InputManager.OnLeftTouchpadEvent += OnInputEvent;

            Gaze_InputManager.OnPadLeftTouchWestEvent += OnInputEvent;
            Gaze_InputManager.OnPadLeftTouchEastEvent += OnInputEvent;
            Gaze_InputManager.OnPadLeftTouchNorthEvent += OnInputEvent;
            Gaze_InputManager.OnPadLeftTouchSouthEvent += OnInputEvent;

            Gaze_InputManager.OnPadLeftPressWestEvent += OnInputEvent;
            Gaze_InputManager.OnPadLeftPressEastEvent += OnInputEvent;
            Gaze_InputManager.OnPadLeftPressNorthEvent += OnInputEvent;
            Gaze_InputManager.OnPadLeftPressDownEvent += OnInputEvent;

            Gaze_InputManager.OnRightTouchpadEvent += OnInputEvent;

            Gaze_InputManager.OnPadRightTouchWestEvent += OnInputEvent;
            Gaze_InputManager.OnPadRightTouchEastEvent += OnInputEvent;
            Gaze_InputManager.OnPadRightTouchNorthEvent += OnInputEvent;
            Gaze_InputManager.OnPadRightTouchSouthEvent += OnInputEvent;

            Gaze_InputManager.OnPadRightPressWestEvent += OnInputEvent;
            Gaze_InputManager.OnPadRightPressEastEvent += OnInputEvent;
            Gaze_InputManager.OnPadRightPressNorthEvent += OnInputEvent;
            Gaze_InputManager.OnPadRightPressSouthEvent += OnInputEvent;

            Gaze_InputManager.OnReleaseEvent += OnReleaseEvent;

            // Testing touch events for oculus rift
            Gaze_InputManager.OnButtonATouch += OnInputEvent;
            Gaze_InputManager.OnButtonBTouch += OnInputEvent;
            Gaze_InputManager.OnButtonXTouch += OnInputEvent;
            Gaze_InputManager.OnButtonYTouch += OnInputEvent;

            Gaze_InputManager.OnButtonLeftIndexTouch += OnInputEvent;
            Gaze_InputManager.OnButtonLeftThumbrestTouch += OnInputEvent;
            Gaze_InputManager.OnButtonLeftThumbstickTouch += OnInputEvent;
            Gaze_InputManager.OnButtonRightIndexTouch += OnInputEvent;
            Gaze_InputManager.OnButtonRightThumbrestTouch += OnInputEvent;
            Gaze_InputManager.OnButtonRightThumbstickTouch += OnInputEvent;

            Gaze_InputManager.OnButtonBUntouch += OnInputEvent;


#if UNITY_ANDROID
            Gaze_GearVR_InputLogic.OnHomeButtonUp += OnInputEvent;
            Gaze_GearVR_InputLogic.OnHomeButtonDown += OnInputEvent;
#endif
            #endregion inputs subscription

            entriesCount = gazeConditionsScript.InputsMap.InputsEntries.Count;
            for (int i = 0; i < entriesCount; i++)
                gazeConditionsScript.InputsMap.InputsEntries[i].CheckIfIsRelease();
        }

        public override bool IsValidated()
        {
            return IsValid;
        }

        protected override void CustomDispose()
        {
            #region inputs subscription
            Gaze_InputManager.OnStartEvent -= OnInputEvent;
            Gaze_InputManager.OnButtonADownEvent -= OnInputEvent;
            Gaze_InputManager.OnButtonAUpEvent -= OnInputEvent;
            Gaze_InputManager.OnButtonBDownEvent -= OnInputEvent;
            Gaze_InputManager.OnButtonBUpEvent -= OnInputEvent;
            Gaze_InputManager.OnButtonXDownEvent -= OnInputEvent;
            Gaze_InputManager.OnButtonXUpEvent -= OnInputEvent;
            Gaze_InputManager.OnButtonYDownEvent -= OnInputEvent;
            Gaze_InputManager.OnButtonYUpEvent -= OnInputEvent;
            Gaze_InputManager.OnIndexLeftDownEvent -= OnInputEvent;
            Gaze_InputManager.OnIndexLeftUpEvent -= OnInputEvent;
            Gaze_InputManager.OnIndexRightDownEvent -= OnInputEvent;
            Gaze_InputManager.OnIndexRightUpEvent -= OnInputEvent;
            Gaze_InputManager.OnHandLeftDownEvent -= OnInputEvent;
            Gaze_InputManager.OnHandLeftUpEvent -= OnInputEvent;
            Gaze_InputManager.OnHandRightDownEvent -= OnInputEvent;
            Gaze_InputManager.OnHandRightUpEvent -= OnInputEvent;
            Gaze_InputManager.OnStickLeftDownEvent -= OnInputEvent;
            Gaze_InputManager.OnStickLeftUpEvent -= OnInputEvent;
            Gaze_InputManager.OnStickRightDownEvent -= OnInputEvent;
            Gaze_InputManager.OnStickRightUpEvent -= OnInputEvent;

            Gaze_InputManager.OnLeftTouchpadEvent -= OnInputEvent;

            Gaze_InputManager.OnPadLeftTouchWestEvent -= OnInputEvent;
            Gaze_InputManager.OnPadLeftTouchEastEvent -= OnInputEvent;
            Gaze_InputManager.OnPadLeftTouchNorthEvent -= OnInputEvent;
            Gaze_InputManager.OnPadLeftTouchSouthEvent -= OnInputEvent;

            Gaze_InputManager.OnPadLeftPressWestEvent -= OnInputEvent;
            Gaze_InputManager.OnPadLeftPressEastEvent -= OnInputEvent;
            Gaze_InputManager.OnPadLeftPressNorthEvent -= OnInputEvent;
            Gaze_InputManager.OnPadLeftPressDownEvent -= OnInputEvent;

            Gaze_InputManager.OnRightTouchpadEvent -= OnInputEvent;

            Gaze_InputManager.OnPadRightTouchWestEvent -= OnInputEvent;
            Gaze_InputManager.OnPadRightTouchEastEvent -= OnInputEvent;
            Gaze_InputManager.OnPadRightTouchNorthEvent -= OnInputEvent;
            Gaze_InputManager.OnPadRightTouchSouthEvent -= OnInputEvent;

            Gaze_InputManager.OnPadRightPressWestEvent -= OnInputEvent;
            Gaze_InputManager.OnPadRightPressEastEvent -= OnInputEvent;
            Gaze_InputManager.OnPadRightPressNorthEvent -= OnInputEvent;
            Gaze_InputManager.OnPadRightPressSouthEvent -= OnInputEvent;

            Gaze_InputManager.OnReleaseEvent -= OnReleaseEvent;

            // Testing touch events for oculus rift
            Gaze_InputManager.OnButtonATouch -= OnInputEvent;
            Gaze_InputManager.OnButtonBTouch -= OnInputEvent;
            Gaze_InputManager.OnButtonXTouch -= OnInputEvent;
            Gaze_InputManager.OnButtonYTouch -= OnInputEvent;

            Gaze_InputManager.OnButtonLeftIndexTouch -= OnInputEvent;
            Gaze_InputManager.OnButtonLeftThumbrestTouch -= OnInputEvent;
            Gaze_InputManager.OnButtonLeftThumbstickTouch -= OnInputEvent;
            Gaze_InputManager.OnButtonRightIndexTouch -= OnInputEvent;
            Gaze_InputManager.OnButtonRightThumbrestTouch -= OnInputEvent;
            Gaze_InputManager.OnButtonRightThumbstickTouch -= OnInputEvent;

            Gaze_InputManager.OnButtonBUntouch -= OnInputEvent;


#if UNITY_ANDROID
            Gaze_GearVR_InputLogic.OnHomeButtonUp -= OnInputEvent;
            Gaze_GearVR_InputLogic.OnHomeButtonDown -= OnInputEvent;
#endif
            #endregion inputs subscription
        }

        public override void ToEditorGUI()
        {
#if UNITY_EDITOR
            for (int i = 0; i < entriesCount; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (gazeConditionsScript.InputsMap.InputsEntries[i].Valid)
                {
                    RenderSatisfiedLabel(gazeConditionsScript.InputsMap.InputsEntries[i].InputType.ToString());
                    RenderSatisfiedLabel("True");
                }
                else
                {
                    RenderNonSatisfiedLabel(gazeConditionsScript.InputsMap.InputsEntries[i].InputType.ToString());
                    RenderNonSatisfiedLabel("False");
                }
                EditorGUILayout.EndHorizontal();
            }
#endif
        }

        private void ValidateInputs(Gaze_InputEventArgs e)
        {
            int validInputs = 0;

            // Count the number of valid inputs
            for (int i = 0; i < entriesCount; i++)
            {
                if (gazeConditionsScript.InputsMap.InputsEntries[i].Valid)
                {
                    ++validInputs;
                }
            }

            // If we need all the inputs and we don't have them return false
            if (gazeConditionsScript.requireAllInputs && validInputs < gazeConditionsScript.InputsMap.InputsEntries.Count)
            {
                IsValid = false;
                return;
            }

            // If we don't have any valid input return false
            if (validInputs == 0)
            {
                IsValid = false;
                return;
            }

            //Is valid
            IsValid = true;
        }

        private void CheckReceivedInputValidity(Gaze_InputEventArgs _e)
        {
            if (gazeConditionsScript.triggerStateIndex == (int)Gaze_TriggerState.BEFORE)
                return;

            // for all input conditions specified (in the map)
            for (int i = 0; i < entriesCount; i++)
            {
                // if the pressed input is in the map
                if (_e.InputType.Equals(gazeConditionsScript.InputsMap.InputsEntries[i].InputType))
                {
                    // update its valid flag
                    Gaze_InputsMapEntry mapEntry = gazeConditionsScript.InputsMap.InputsEntries[i];
                    mapEntry.Valid = true;
                    if (mapEntry.IsRelease)
                        S_Scheduler.AddTask(0.1f, () => { InvalidateReleaseConditionAtNextFrame(mapEntry); });

                    // check if all conditions are now met
                    ValidateInputs(_e);
                    break;
                }
            }

            CheckIfInputReleased(_e);
        }


        private void InvalidateReleaseConditionAtNextFrame(Gaze_InputsMapEntry _entry)
        {
            _entry.Valid = false;
            ValidateInputs(null);
        }

        private void CheckIfInputReleased(Gaze_InputEventArgs _e)
        {
            for (int i = 0; i < entriesCount; i++)
            {
                if (Gaze_InputReleaseMap.IsReleaseInputtOf(_e.InputType, gazeConditionsScript.InputsMap.InputsEntries[i].InputType))
                {
                    gazeConditionsScript.InputsMap.InputsEntries[i].Valid = false;
                    ValidateInputs(_e);
                }
            }
        }

        private void OnReleaseEvent(Gaze_InputEventArgs _e)
        {
            CheckReceivedInputValidity(_e);
        }

        private void OnInputEvent(Gaze_InputEventArgs _e)
        {
            CheckReceivedInputValidity(_e);
        }
    }
}