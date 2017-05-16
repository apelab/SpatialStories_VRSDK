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

        public bool IsButtonA_Down { get; set; }

        private bool grabLeftValid = false;
        private bool grabRightValid = false;
        private bool grabStateLeftValid = false;
        private bool grabStateRightValid = false;

        private int entriesCount;
        private bool requireAllValidated = false;

        public Gaze_InputsCondition(Gaze_Conditions _gazeConditionsScript) : base(_gazeConditionsScript) { }

        protected override void CustomSetup()
        {
            Gaze_InputManager.OnButtonADownEvent += OnButtonADownEvent;
            Gaze_InputManager.OnButtonBDownEvent += OnButtonBDownEvent;
            entriesCount = gazeConditionsScript.InputsMap.InputsEntries.Count;
            requireAllValidated = false;
        }

        public override bool IsValidated()
        {
            return IsValid;
        }

        protected override void CustomDispose()
        {
            Gaze_InputManager.OnButtonADownEvent -= OnButtonADownEvent;
            Gaze_InputManager.OnButtonBDownEvent -= OnButtonBDownEvent;
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
            bool allValid = false;

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
    }
}