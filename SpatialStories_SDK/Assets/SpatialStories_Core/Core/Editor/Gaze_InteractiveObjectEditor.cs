// <copyright file="Gaze_InteractiveObjectEditor.cs" company="apelab sàrl">
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
// </copyright>
// <author>Michaël Martin</author>
// <email>dev@apelab.ch</email>
// <web>https://twitter.com/apelab_ch</web>
// <web>http://www.apelab.ch</web>
// <date>2014-06-01</date>
using System;
using UnityEditor;
using UnityEngine;

namespace Gaze
{
    [InitializeOnLoad]
    [CustomEditor(typeof(Gaze_InteractiveObject))]
    public class Gaze_InteractiveObjectEditor : Editor
    {
        #region Members
        private Gaze_InteractiveObject targetIO;

        // logo image
        private Texture logo;
        private Rect logoRect;
        private string[] grabModes;
        private string[] manipulationModes;
        #endregion

        void OnEnable()
        {
            InitMembers();
        }

        private void InitMembers()
        {
            targetIO = (Gaze_InteractiveObject)target;

            grabModes = Enum.GetNames(typeof(Gaze_GrabMode));
            manipulationModes = Enum.GetNames(typeof(Gaze_ManipulationModes));

            logo = (Texture)Resources.Load("SpatialStorires_Logo_256", typeof(Texture));
            logoRect = new Rect();
            logoRect.x = 10;
            logoRect.y = 10;
        }

        public override void OnInspectorGUI()
        {
            DisplayLogo();
            DisplayManipulationMode();
            DisplayTouchDistance();
            DisplayGrabDistance();
            DisplayLevitationDistance();
            DisplayDragAndDrop();
        }

        private void DisplayLogo()
        {
            GUILayout.BeginHorizontal();
            GUI.Label(logoRect, logo);
            GUILayout.Label(logo);
            GUILayout.EndHorizontal();
        }


        private void DisplayManipulationMode()
        {
            targetIO.ManipulationModeIndex = EditorGUILayout.Popup("Manipulation Modes", targetIO.ManipulationModeIndex, manipulationModes);
        }

        private void DisplayTouchDistance()
        {
            if (targetIO.ManipulationMode != Gaze_ManipulationModes.TOUCH)
                return;

            GUILayout.BeginHorizontal();
            targetIO.TouchDistance = EditorGUILayout.FloatField("Touch Distance", targetIO.TouchDistance);
            Gaze_Utils.EnsureFieldIsPositiveOrZero(ref targetIO.TouchDistance);
            GUILayout.EndHorizontal();
        }

        private void DisplayGrabDistance()
        {
            if (targetIO.ManipulationMode != Gaze_ManipulationModes.GRAB)
                return;

            GUILayout.BeginHorizontal();
            targetIO.GrabDistance = EditorGUILayout.FloatField("Grab Distance", targetIO.GrabDistance);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            targetIO.AttractionSpeed = EditorGUILayout.FloatField("Attraction Speed", targetIO.AttractionSpeed);
            GUILayout.EndHorizontal();
            Gaze_Utils.EnsureFieldIsPositiveOrZero(ref targetIO.GrabDistance);
            Gaze_Utils.EnsureFieldIsPositiveOrZero(ref targetIO.AttractionSpeed);
        }

        private void DisplayLevitationDistance()
        {
            if (targetIO.ManipulationMode != Gaze_ManipulationModes.LEVITATE)
                return;

            GUILayout.BeginHorizontal();
            targetIO.GrabDistance = EditorGUILayout.FloatField("Levitation Distance", targetIO.GrabDistance);
            Gaze_Utils.EnsureFieldIsPositiveOrZero(ref targetIO.GrabDistance);
            GUILayout.EndHorizontal();
        }

        private void DisplayDragAndDrop()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            targetIO.IsDragAndDropEnabled = EditorGUILayout.ToggleLeft("Enable Drag And Drop", targetIO.IsDragAndDropEnabled);

            if (targetIO.IsDragAndDropEnabled)
            {
                #region Targets
                DisplayTargets();
                #endregion

                #region Axis contraints
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                targetIO.DnD_minDistance = EditorGUILayout.FloatField("Min Distance To Validate", targetIO.DnD_minDistance);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                targetIO.DnD_respectXAxis = EditorGUILayout.ToggleLeft("Respect X Axis", targetIO.DnD_respectXAxis);
                if (targetIO.DnD_respectXAxis)
                {
                    targetIO.DnD_respectXAxisMirrored = EditorGUILayout.ToggleLeft("Mirrored", targetIO.DnD_respectXAxisMirrored);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                targetIO.DnD_respectYAxis = EditorGUILayout.ToggleLeft("Respect Y Axis", targetIO.DnD_respectYAxis);
                if (targetIO.DnD_respectYAxis)
                {
                    targetIO.DnD_respectYAxisMirrored = EditorGUILayout.ToggleLeft("Mirrored", targetIO.DnD_respectYAxisMirrored);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                targetIO.DnD_respectZAxis = EditorGUILayout.ToggleLeft("Respect Z Axis", targetIO.DnD_respectZAxis);
                if (targetIO.DnD_respectZAxis)
                {
                    targetIO.DnD_respectZAxisMirrored = EditorGUILayout.ToggleLeft("Mirrored", targetIO.DnD_respectZAxisMirrored);
                }
                GUILayout.EndHorizontal();
                #endregion

                #region Threshold angle
                if (targetIO.DnD_respectXAxis || targetIO.DnD_respectYAxis || targetIO.DnD_respectZAxis)
                {
                    targetIO.DnD_angleThreshold = EditorGUILayout.Slider("Angle Threshold", targetIO.DnD_angleThreshold, 1, 100);
                }
                #endregion

                #region Snap
                EditorGUILayout.BeginHorizontal();
                targetIO.DnD_snapBeforeDrop = EditorGUILayout.ToggleLeft("Snap Before Drop", targetIO.DnD_snapBeforeDrop);
                EditorGUILayout.EndHorizontal();
                #endregion

                #region Time to snap
                EditorGUILayout.BeginHorizontal();
                targetIO.DnD_TimeToSnap = EditorGUILayout.FloatField("Time To Snap", targetIO.DnD_TimeToSnap);
                EditorGUILayout.EndHorizontal();
                #endregion
            }
            else
            {
                GUILayout.EndHorizontal();
            }
        }

        // TODO @apelab add targets list with plus button
        private void DisplayTargets()
        {
            EditorGUILayout.BeginHorizontal();
            // help message if no input is specified
            if (targetConditions.InputsMap.InputsEntries.Count < 1)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("Add at least one input or deactivate this condition if not needed.", MessageType.Warning);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                // display require all option
                targetConditions.requireAllInputs = EditorGUILayout.ToggleLeft("Require all", targetConditions.requireAllInputs);
                EditorGUILayout.EndHorizontal();

                // update inputs list in Gaze_Interactions (Gaze_Interactions may have been removed in the hierarchy)
                for (int i = 0; i < targetConditions.InputsMap.InputsEntries.Count; i++)
                {
                    // display the entry
                    EditorGUILayout.BeginHorizontal();
                    targetConditions.InputsMap.InputsEntries[i].inputType = (Gaze_InputTypes)EditorGUILayout.Popup((int)targetConditions.InputsMap.InputsEntries[i].inputType, inputsNames);


                    // TODO @apelab add/remove event subscription with the new popup value

                    // and a '-' button to remove it if needed
                    if (GUILayout.Button("-"))
                        targetConditions.InputsMap.Delete(targetConditions.InputsMap.InputsEntries[i]);

                    EditorGUILayout.EndHorizontal();
                }
            }

            // display 'add' button
            if (GUILayout.Button("+"))
            {
                Gaze_InputsMapEntry d = targetConditions.InputsMap.Add();

                // TODO @apelab add event subscription with a default value
                //Gaze_InputsCondition inputscondition = targetConditions.activeConditions.GetType(typeof(Gaze_InputsCondition));

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("-"))
                {
                    targetConditions.InputsMap.Delete(d);

                    // TODO @apelab remove event subscription


                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
        }
    }
}