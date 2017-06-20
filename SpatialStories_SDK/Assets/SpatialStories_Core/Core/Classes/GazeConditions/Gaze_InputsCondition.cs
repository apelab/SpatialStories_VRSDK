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

using UnityEditor;

namespace Gaze
{
    public class Gaze_InputsCondition : Gaze_AbstractCondition
    {
        private int entriesCount;
        private bool requireAllValidated = false;

        public Gaze_InputsCondition(Gaze_Conditions _gazeConditionsScript) : base(_gazeConditionsScript) { }

        protected override void CustomSetup()
        {
            #region inputs subscription
            Gaze_InputManager.OnStartEvent += OnStartEvent;
            Gaze_InputManager.OnButtonADownEvent += OnButtonADownEvent;
            Gaze_InputManager.OnButtonAUpEvent += OnButtonAUpEvent;
            Gaze_InputManager.OnButtonBDownEvent += OnButtonBDownEvent;
            Gaze_InputManager.OnButtonBUpEvent += OnButtonBUpEvent;
            Gaze_InputManager.OnButtonXDownEvent += OnButtonXDownEvent;
            Gaze_InputManager.OnButtonXUpEvent += OnButtonXUpEvent;
            Gaze_InputManager.OnButtonYDownEvent += OnButtonYDownEvent;
            Gaze_InputManager.OnButtonYUpEvent += OnButtonYUpEvent;
            Gaze_InputManager.OnIndexLeftDownEvent += OnIndexLeftDownEvent;
            Gaze_InputManager.OnIndexLeftUpEvent += OnIndexLeftUpEvent;
            Gaze_InputManager.OnIndexRightDownEvent += OnIndexRightDownEvent;
            Gaze_InputManager.OnIndexRightUpEvent += OnIndexRightUpEvent;
            Gaze_InputManager.OnHandLeftDownEvent += OnHandLeftDownEvent;
            Gaze_InputManager.OnHandLeftUpEvent += OnHandLeftUpEvent;
            Gaze_InputManager.OnHandRightDownEvent += OnHandRightDownEvent;
            Gaze_InputManager.OnHandRightUpEvent += OnHandRightUpEvent;
            Gaze_InputManager.OnStickLeftDownEvent += OnStickLeftDownEvent;
            Gaze_InputManager.OnStickLeftUpEvent += OnStickLeftUpEvent;
            Gaze_InputManager.OnStickRightDownEvent += OnStickRightDownEvent;
            Gaze_InputManager.OnStickRightUpEvent += OnStickRightUpEvent;
            Gaze_InputManager.OnLeftTouchpadEvent += OnLeftTouchpadEvent;
            Gaze_InputManager.OnPadLeftTouchLeftEvent += OnPadLeftTouchLeftEvent;
            Gaze_InputManager.OnPadLeftTouchRightEvent += OnPadLeftTouchRightEvent;
            Gaze_InputManager.OnPadLeftTouchUpEvent += OnPadLeftTouchUpEvent;
            Gaze_InputManager.OnPadLeftTouchDownEvent += OnPadLeftTouchDownEvent;
            Gaze_InputManager.OnRightTouchpadEvent += OnRightTouchpadEvent;
            Gaze_InputManager.OnPadRightTouchLeftEvent += OnPadRightTouchLeftEvent;
            Gaze_InputManager.OnPadRightTouchRightEvent += OnPadRightTouchRightEvent;
            Gaze_InputManager.OnPadRightTouchUpEvent += OnPadRightTouchUpEvent;
            Gaze_InputManager.OnPadRightTouchDownEvent += OnPadRightTouchDownEvent;
            #endregion inputs subscription

            entriesCount = gazeConditionsScript.InputsMap.InputsEntries.Count;
            requireAllValidated = false;
        }

        public override bool IsValidated()
        {
            return IsValid;
        }

        protected override void CustomDispose()
        {
            #region inputs subscription
            Gaze_InputManager.OnStartEvent -= OnStartEvent;
            Gaze_InputManager.OnButtonADownEvent -= OnButtonADownEvent;
            Gaze_InputManager.OnButtonAUpEvent -= OnButtonAUpEvent;
            Gaze_InputManager.OnButtonBDownEvent -= OnButtonBDownEvent;
            Gaze_InputManager.OnButtonBUpEvent -= OnButtonBUpEvent;
            Gaze_InputManager.OnButtonXDownEvent -= OnButtonXDownEvent;
            Gaze_InputManager.OnButtonXUpEvent -= OnButtonXUpEvent;
            Gaze_InputManager.OnButtonYDownEvent -= OnButtonYDownEvent;
            Gaze_InputManager.OnButtonYUpEvent -= OnButtonYUpEvent;
            Gaze_InputManager.OnIndexLeftDownEvent -= OnIndexLeftDownEvent;
            Gaze_InputManager.OnIndexLeftUpEvent -= OnIndexLeftUpEvent;
            Gaze_InputManager.OnIndexRightDownEvent -= OnIndexRightDownEvent;
            Gaze_InputManager.OnIndexRightUpEvent -= OnIndexRightUpEvent;
            Gaze_InputManager.OnHandLeftDownEvent -= OnHandLeftDownEvent;
            Gaze_InputManager.OnHandLeftUpEvent -= OnHandLeftUpEvent;
            Gaze_InputManager.OnHandRightDownEvent -= OnHandRightDownEvent;
            Gaze_InputManager.OnHandRightUpEvent -= OnHandRightUpEvent;
            Gaze_InputManager.OnStickLeftDownEvent -= OnStickLeftDownEvent;
            Gaze_InputManager.OnStickLeftUpEvent -= OnStickLeftUpEvent;
            Gaze_InputManager.OnStickRightDownEvent -= OnStickRightDownEvent;
            Gaze_InputManager.OnStickRightUpEvent -= OnStickRightUpEvent;
            Gaze_InputManager.OnLeftTouchpadEvent -= OnLeftTouchpadEvent;
            Gaze_InputManager.OnPadLeftTouchLeftEvent -= OnPadLeftTouchLeftEvent;
            Gaze_InputManager.OnPadLeftTouchRightEvent -= OnPadLeftTouchRightEvent;
            Gaze_InputManager.OnPadLeftTouchUpEvent -= OnPadLeftTouchUpEvent;
            Gaze_InputManager.OnPadLeftTouchDownEvent -= OnPadLeftTouchDownEvent;
            Gaze_InputManager.OnRightTouchpadEvent -= OnRightTouchpadEvent;
            Gaze_InputManager.OnPadRightTouchLeftEvent -= OnPadRightTouchLeftEvent;
            Gaze_InputManager.OnPadRightTouchRightEvent -= OnPadRightTouchRightEvent;
            Gaze_InputManager.OnPadRightTouchUpEvent -= OnPadRightTouchUpEvent;
            Gaze_InputManager.OnPadRightTouchDownEvent -= OnPadRightTouchDownEvent;
            #endregion inputs subscription
        }

        public override void ToEditorGUI()
        {
            for (int i = 0; i < entriesCount; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (gazeConditionsScript.InputsMap.InputsEntries[i].valid)
                {
                    RenderSatisfiedLabel(gazeConditionsScript.InputsMap.InputsEntries[i].inputType.ToString());
                    RenderSatisfiedLabel("True");
                }
                else
                {
                    RenderNonSatisfiedLabel(gazeConditionsScript.InputsMap.InputsEntries[i].inputType.ToString());
                    RenderNonSatisfiedLabel("False");
                }
                EditorGUILayout.EndHorizontal();
            }
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
            // for all input conditions specified (in the map)
            for (int i = 0; i < entriesCount; i++)
            {
                // if the pressed input is in the map
                if (e.InputType.Equals(gazeConditionsScript.InputsMap.InputsEntries[i].inputType))
                {
                    // update its valid flag
                    gazeConditionsScript.InputsMap.InputsEntries[i].valid = true;

                    // check if all conditions are now met
                    ValidateInputs(e);
                    break;
                }
            }
        }

        private void OnButtonADownEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnButtonBDownEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnStartEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnButtonAUpEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnButtonBUpEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnButtonXDownEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnButtonXUpEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnButtonYDownEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnButtonYUpEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnIndexLeftDownEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnIndexLeftUpEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnIndexRightDownEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnIndexRightUpEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnHandLeftDownEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnHandLeftUpEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnHandRightDownEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnHandRightUpEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnStickLeftDownEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnStickLeftUpEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnStickRightDownEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnStickRightUpEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnLeftTouchpadEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnPadLeftTouchLeftEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnPadLeftTouchRightEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnPadLeftTouchDownEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnPadLeftTouchUpEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnRightTouchpadEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnPadRightTouchLeftEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnPadRightTouchRightEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnPadRightTouchUpEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }

        private void OnPadRightTouchDownEvent(Gaze_InputEventArgs e)
        {
            CheckReceivedInputValidity(e);
        }
    }
}