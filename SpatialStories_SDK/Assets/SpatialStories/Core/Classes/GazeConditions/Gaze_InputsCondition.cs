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
using System;
using UnityEditor;
using UnityEngine;
#endif

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

            #endregion inputs subscription

            entriesCount = gazeConditionsScript.InputsMap.InputsEntries.Count;
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
            #endregion inputs subscription
        }

        public override void ToEditorGUI()
        {
#if UNITY_EDITOR
            for (int i = 0; i < entriesCount; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (gazeConditionsScript.InputsMap.InputsEntries[i].valid)
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
            // for all input conditions specified (in the map)
            for (int i = 0; i < entriesCount; i++)
            {
                // if the current input is valid
                if (gazeConditionsScript.InputsMap.InputsEntries[i].valid)
                {
                    // if NOT require all
                    if (!gazeConditionsScript.requireAllInputs)
                    {
                        IsValid = true;
                        break;
                    }
                }
                else
                {
                    // if require all
                    if (gazeConditionsScript.requireAllInputs)
                    {
                        IsValid = false;
                        break;
                    }
                }
                IsValid = true;
            }
        }

        private void CheckReceivedInputValidity(Gaze_InputEventArgs e)
        {
            if (gazeConditionsScript.triggerStateIndex == (int)Gaze_TriggerState.BEFORE)
                return;

            // for all input conditions specified (in the map)
            for (int i = 0; i < entriesCount; i++)
            {
                // if the pressed input is in the map
                if (e.InputType.Equals(gazeConditionsScript.InputsMap.InputsEntries[i].InputType))
                {
                    // update its valid flag
                    gazeConditionsScript.InputsMap.InputsEntries[i].valid = true;

                    // check if all conditions are now met
                    ValidateInputs(e);
                    break;
                }
            }
        }

        private void OnReleaseEvent(Gaze_InputEventArgs _e)
        {
            Debug.Log(_e.InputType);
            CheckReceivedInputValidity(_e);
        }

        private void OnInputEvent(Gaze_InputEventArgs _e)
        {
            Debug.Log(_e.InputType);
            CheckReceivedInputValidity(_e);
        }
    }
}