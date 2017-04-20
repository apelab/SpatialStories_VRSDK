// <copyright file="Gaze_GazableEditor.cs" company="apelab sàrl">
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
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gaze
{
    // Custom Editor using SerializedProperties.
    // Automatic handling of multi-object editing, undo, and prefab overrides.
    [CustomEditor(typeof(Gaze_Interaction))]
    public class Gaze_InteractionEditor : Editor
    {
        // logo image
        Texture logo;
        Rect logoRect;

        private Gaze_Conditions conditionsScript;

        // NEW list
        private List<Gaze_InteractiveObject> interactiveObjectsList;

        SerializedProperty HasConditions;
        SerializedProperty HasActions;

        void OnEnable()
        {
            // Setup the SerializedProperties.
            HasConditions = serializedObject.FindProperty("HasActions");
            HasActions = serializedObject.FindProperty("HasConditions");

            InitMembers();
        }

        public override void OnInspectorGUI()
        {

            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();


            if (!Application.isPlaying)
            {
                #region Update InteractiveObjects list
                UpdateInteractiveObjectsList();
                #endregion


                #region Logo
                GUILayout.BeginHorizontal();
                GUI.Label(logoRect, logo);
                GUILayout.Label(logo);
                GUILayout.EndHorizontal();
                #endregion

                bool lastHasActions = HasActions.boolValue;
                bool LastHasConditions = HasConditions.boolValue;

                HasActions.boolValue = EditorGUILayout.ToggleLeft("Actions", HasActions.boolValue);
                HasConditions.boolValue = EditorGUILayout.ToggleLeft("Conditions", HasConditions.boolValue);

                if (lastHasActions != HasActions.boolValue)
                {
                    if (HasActions.boolValue)
                        ((Gaze_Interaction)target).AddActions();
                    else
                    {
                        ((Gaze_Interaction)target).RemoveActions();
                    }
                }

                if (LastHasConditions != HasConditions.boolValue)
                {
                    if (HasConditions.boolValue)
                        ((Gaze_Interaction)target).AddConditions();
                    else
                    {
                        ((Gaze_Interaction)target).RemoveConditions();
                    }
                }
            }
            else
            {
                #region State
                // display Conditions component
                conditionsScript = ((Gaze_Interaction)target).GetComponent<Gaze_Conditions>();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Infos");

                // extra block that can be toggled on and off
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Gazed");
                EditorGUILayout.LabelField(conditionsScript.IsGazed ? "Gazed" : "Ungazed", EditorStyles.whiteLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Proximity");
                EditorGUILayout.LabelField(conditionsScript.isInProximity ? "In" : "Out", EditorStyles.whiteLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Touch Left");

                if (Gaze_InputManager.instance.LeftHandActive)
                    EditorGUILayout.LabelField(conditionsScript.isLeftPointing || conditionsScript.isLeftColliding ? "Touch" : "Untouch", EditorStyles.whiteLabel);
                else
                    EditorGUILayout.LabelField("N/A", EditorStyles.whiteLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Touch Right");
                if (Gaze_InputManager.instance.RightHandActive)
                    EditorGUILayout.LabelField(conditionsScript.isRightPointing || conditionsScript.isRightColliding ? "Touch" : "Untouch", EditorStyles.whiteLabel);
                else
                    EditorGUILayout.LabelField("N/A", EditorStyles.whiteLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Teleport");
                EditorGUILayout.LabelField(conditionsScript.teleportEditorState, EditorStyles.whiteLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("State");
                EditorGUILayout.LabelField(((Gaze_TriggerState)conditionsScript.triggerStateIndex).ToString(), EditorStyles.whiteLabel);
                EditorGUILayout.EndHorizontal();


                if (conditionsScript.dependent)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Dependencies validated");
                    EditorGUILayout.LabelField(conditionsScript.DependenciesValidated.ToString(), EditorStyles.whiteLabel);
                    EditorGUILayout.EndHorizontal();
                }

                if (conditionsScript.focusDuration > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Focus Completion");
                    EditorGUILayout.LabelField(conditionsScript.FocusCompletion.ToString("P"), EditorStyles.whiteLabel);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Trigger Count");
                EditorGUILayout.LabelField(conditionsScript.TriggerCount.ToString(), EditorStyles.whiteLabel);
                EditorGUILayout.EndHorizontal();

                if (conditionsScript.reload)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Reload Count");
                    EditorGUILayout.LabelField(conditionsScript.reloadCount.ToString(), EditorStyles.whiteLabel);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Can Be Triggered");
                EditorGUILayout.LabelField(conditionsScript.canBeTriggered.ToString(), EditorStyles.whiteLabel);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }
            #endregion

            serializedObject.ApplyModifiedProperties();
        }

        private void InitMembers()
        {

            interactiveObjectsList = new List<Gaze_InteractiveObject>();

            #region Logo
            logo = (Texture)Resources.Load("SpatialStorires_Logo_256", typeof(Texture));
            logoRect = new Rect();
            logoRect.x = 10;
            logoRect.y = 10;
            #endregion
        }


        /// <summary>
        /// Get all InteractiveObjects in the scene !
        /// Only executed in Editor Mode (not at runtime)
        /// </summary>
        private void UpdateInteractiveObjectsList()
        {
            interactiveObjectsList = (FindObjectsOfType(typeof(Gaze_InteractiveObject)) as Gaze_InteractiveObject[]).ToList();
        }

    }
}
