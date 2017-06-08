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
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gaze
{
    [InitializeOnLoad]
    [CustomEditor(typeof(Gaze_InteractiveObject))]
    public class Gaze_InteractiveObjectEditor : Editor
    {
        #region Members
        private Gaze_InteractiveObject gaze_InteractiveObjectScript;

        // logo image
        private Texture logo;
        private Rect logoRect;
        private string[] grabModes;
        private string[] manipulationModes;
        private List<string> dnd_dropTargetsNames;
        private List<Gaze_InteractiveObject> hierarchyIOsScripts;
        public List<GameObject> hierarchyIOs;
        #endregion

        void OnEnable()
        {
            InitMembers();
        }

        private void InitMembers()
        {
            gaze_InteractiveObjectScript = (Gaze_InteractiveObject)target;

            grabModes = Enum.GetNames(typeof(Gaze_GrabMode));
            manipulationModes = Enum.GetNames(typeof(Gaze_ManipulationModes));

            logo = (Texture)Resources.Load("SpatialStorires_Logo_256", typeof(Texture));
            logoRect = new Rect();
            logoRect.x = 10;
            logoRect.y = 10;
            hierarchyIOs = new List<GameObject>();
            hierarchyIOsScripts = new List<Gaze_InteractiveObject>();
            dnd_dropTargetsNames = new List<string>();
        }

        public override void OnInspectorGUI()
        {
            UpdateListsFromHierarchy();
            DisplayLogo();
            DisplayManipulationMode();
            DisplayTouchDistance();
            DisplayGrabDistance();
            DisplayLevitationDistance();
            DisplayDragAndDrop();
        }
        private void UpdateListsFromHierarchy()
        {
            hierarchyIOsScripts.Clear();
            hierarchyIOs.Clear();
            dnd_dropTargetsNames.Clear();

            // rebuild them
            hierarchyIOsScripts = (FindObjectsOfType(typeof(Gaze_InteractiveObject)) as Gaze_InteractiveObject[]).ToList();
            for (int i = 0; i < hierarchyIOsScripts.Count; i++)
            {
                hierarchyIOs.Add(hierarchyIOsScripts[i].gameObject);
                dnd_dropTargetsNames.Add(hierarchyIOsScripts[i].gameObject.name);
            }
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
            gaze_InteractiveObjectScript.ManipulationModeIndex = EditorGUILayout.Popup("Manipulation Modes", gaze_InteractiveObjectScript.ManipulationModeIndex, manipulationModes);
        }

        private void DisplayTouchDistance()
        {
            if (gaze_InteractiveObjectScript.ManipulationMode != Gaze_ManipulationModes.TOUCH)
                return;

            GUILayout.BeginHorizontal();
            gaze_InteractiveObjectScript.TouchDistance = EditorGUILayout.FloatField("Touch Distance", gaze_InteractiveObjectScript.TouchDistance);
            Gaze_Utils.EnsureFieldIsPositiveOrZero(ref gaze_InteractiveObjectScript.TouchDistance);
            GUILayout.EndHorizontal();
        }

        private void DisplayGrabDistance()
        {
            if (gaze_InteractiveObjectScript.ManipulationMode != Gaze_ManipulationModes.GRAB)
                return;

            GUILayout.BeginHorizontal();
            gaze_InteractiveObjectScript.GrabDistance = EditorGUILayout.FloatField("Grab Distance", gaze_InteractiveObjectScript.GrabDistance);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            gaze_InteractiveObjectScript.AttractionSpeed = EditorGUILayout.FloatField("Attraction Speed", gaze_InteractiveObjectScript.AttractionSpeed);
            GUILayout.EndHorizontal();
            Gaze_Utils.EnsureFieldIsPositiveOrZero(ref gaze_InteractiveObjectScript.GrabDistance);
            Gaze_Utils.EnsureFieldIsPositiveOrZero(ref gaze_InteractiveObjectScript.AttractionSpeed);
        }

        private void DisplayLevitationDistance()
        {
            if (gaze_InteractiveObjectScript.ManipulationMode != Gaze_ManipulationModes.LEVITATE)
                return;

            GUILayout.BeginHorizontal();
            gaze_InteractiveObjectScript.GrabDistance = EditorGUILayout.FloatField("Levitation Distance", gaze_InteractiveObjectScript.GrabDistance);
            Gaze_Utils.EnsureFieldIsPositiveOrZero(ref gaze_InteractiveObjectScript.GrabDistance);
            GUILayout.EndHorizontal();
        }

        private void DisplayDragAndDrop()
        {
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            gaze_InteractiveObjectScript.IsDragAndDropEnabled = EditorGUILayout.ToggleLeft("Enable Drag And Drop", gaze_InteractiveObjectScript.IsDragAndDropEnabled);
            EditorGUILayout.EndHorizontal();

            if (gaze_InteractiveObjectScript.IsDragAndDropEnabled)
            {
                #region Targets
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Drop Targets");
                GUILayout.EndHorizontal();
                DisplayTargets();
                #endregion

                #region Axis contraints
                GUILayout.BeginHorizontal();
                gaze_InteractiveObjectScript.DnD_minDistance = EditorGUILayout.FloatField("Min Distance To Validate", gaze_InteractiveObjectScript.DnD_minDistance);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                gaze_InteractiveObjectScript.DnD_respectXAxis = EditorGUILayout.ToggleLeft("Respect X Axis", gaze_InteractiveObjectScript.DnD_respectXAxis);
                if (gaze_InteractiveObjectScript.DnD_respectXAxis)
                {
                    gaze_InteractiveObjectScript.DnD_respectXAxisMirrored = EditorGUILayout.ToggleLeft("Mirrored", gaze_InteractiveObjectScript.DnD_respectXAxisMirrored);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                gaze_InteractiveObjectScript.DnD_respectYAxis = EditorGUILayout.ToggleLeft("Respect Y Axis", gaze_InteractiveObjectScript.DnD_respectYAxis);
                if (gaze_InteractiveObjectScript.DnD_respectYAxis)
                {
                    gaze_InteractiveObjectScript.DnD_respectYAxisMirrored = EditorGUILayout.ToggleLeft("Mirrored", gaze_InteractiveObjectScript.DnD_respectYAxisMirrored);
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                gaze_InteractiveObjectScript.DnD_respectZAxis = EditorGUILayout.ToggleLeft("Respect Z Axis", gaze_InteractiveObjectScript.DnD_respectZAxis);
                if (gaze_InteractiveObjectScript.DnD_respectZAxis)
                {
                    gaze_InteractiveObjectScript.DnD_respectZAxisMirrored = EditorGUILayout.ToggleLeft("Mirrored", gaze_InteractiveObjectScript.DnD_respectZAxisMirrored);
                }
                GUILayout.EndHorizontal();
                #endregion

                #region Threshold angle
                if (gaze_InteractiveObjectScript.DnD_respectXAxis || gaze_InteractiveObjectScript.DnD_respectYAxis || gaze_InteractiveObjectScript.DnD_respectZAxis)
                {
                    gaze_InteractiveObjectScript.DnD_angleThreshold = EditorGUILayout.Slider("Angle Threshold", gaze_InteractiveObjectScript.DnD_angleThreshold, 1, 100);
                }
                #endregion

                #region Snap
                EditorGUILayout.BeginHorizontal();
                gaze_InteractiveObjectScript.DnD_snapBeforeDrop = EditorGUILayout.ToggleLeft("Snap Before Drop", gaze_InteractiveObjectScript.DnD_snapBeforeDrop);
                EditorGUILayout.EndHorizontal();
                #endregion

                #region Time to snap
                EditorGUILayout.BeginHorizontal();
                gaze_InteractiveObjectScript.DnD_TimeToSnap = EditorGUILayout.FloatField("Time To Snap", gaze_InteractiveObjectScript.DnD_TimeToSnap);
                EditorGUILayout.EndHorizontal();
                #endregion
            }
        }

        // TODO @apelab add targets list with plus button
        private void DisplayTargets()
        {
            // help message if no input is specified
            if (gaze_InteractiveObjectScript.DnD_TargetsIndexes.Count < 1)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("Add at least one drop target or deactivate this condition if not needed.", MessageType.Warning);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                // for each DnD target
                for (int i = 0; i < gaze_InteractiveObjectScript.DnD_TargetsIndexes.Count; i++)
                {
                    // display it in a popup
                    EditorGUILayout.BeginHorizontal();
                    gaze_InteractiveObjectScript.DnD_TargetsIndexes[i] = EditorGUILayout.Popup(gaze_InteractiveObjectScript.DnD_TargetsIndexes[i], dnd_dropTargetsNames.ToArray());


                    // and a '-' button to remove it if needed
                    if (GUILayout.Button("-"))
                        gaze_InteractiveObjectScript.DnD_TargetsIndexes.Remove(gaze_InteractiveObjectScript.DnD_TargetsIndexes[i]);

                    EditorGUILayout.EndHorizontal();
                }
            }
            // display 'add' button
            if (GUILayout.Button("+"))
            {
                // TODO @apelab mike : add a new target in the list with a default value from a list of all IOs
                Debug.Log("target to add : " + hierarchyIOs[0]);
                gaze_InteractiveObjectScript.DnD_TargetsIndexes.Add(0);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-"))
                {
                    //targetConditions.InputsMap.Delete(d);
                    gaze_InteractiveObjectScript.DnD_TargetsIndexes.Remove(gaze_InteractiveObjectScript.DnD_TargetsIndexes[0]);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
            #region old stuff
            /*
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
            */
            #endregion
        }
    }
}