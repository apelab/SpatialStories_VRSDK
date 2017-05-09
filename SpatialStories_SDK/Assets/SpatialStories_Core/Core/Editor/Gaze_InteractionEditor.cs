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
        private static bool showStateBlock = true;

        private Gaze_Conditions conditionsScript;

        // NEW list
        private List<Gaze_InteractiveObject> interactiveObjectsList;

        SerializedProperty HasConditions;
        SerializedProperty HasActions;

        private void Awake()
        {
            conditionsScript = ((Gaze_Interaction)target).GetComponent<Gaze_Conditions>();
        }

        void OnEnable()
        {
            // Setup the SerializedProperties.
            HasConditions = serializedObject.FindProperty("HasConditions");
            HasActions = serializedObject.FindProperty("HasActions");

            InitMembers();
        }

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();

            #region Logo
            GUILayout.BeginHorizontal();
            GUI.Label(logoRect, logo);
            GUILayout.Label(logo);
            GUILayout.EndHorizontal();
            #endregion

            if (!Application.isPlaying)
            {
                #region Update InteractiveObjects list
                UpdateInteractiveObjectsList();
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

                EditorGUILayout.Space();
                showStateBlock = EditorGUILayout.Foldout(showStateBlock, "Infos");

                // extra block that can be toggled on and off
                if (showStateBlock)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("State");
                    EditorGUILayout.LabelField(((Gaze_TriggerState)conditionsScript.triggerStateIndex).ToString(), EditorStyles.whiteLabel);
                    EditorGUILayout.EndHorizontal();


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

                #region State
                ShowConditionsState();
                #endregion
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Displays the state of all the conditions, dependencies, and so on 
        /// in runtime.
        /// </summary>
        private void ShowConditionsState()
        {
            EditorGUILayout.LabelField("Conditions: ", EditorStyles.boldLabel);

            if (conditionsScript.activeConditions.Count() > 0)
            {
                foreach (Gaze_AbstractCondition condition in conditionsScript.activeConditions)
                    condition.ToEditorGUI();
            }
            else
            {
                EditorGUILayout.LabelField("No condition is used");
            }


            EditorGUILayout.LabelField("Dependencies (Activate)", EditorStyles.boldLabel);

            if (conditionsScript.dependent && conditionsScript.ActivateOnDependencyMap.dependencies.Count() > 0)
            {
                foreach (Gaze_Dependency dep in conditionsScript.ActivateOnDependencyMap.dependencies)
                {
                    dep.ToEditorGUI();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Not used");
            }

            EditorGUILayout.LabelField("Dependencies (Deactivate)", EditorStyles.boldLabel);

            if (conditionsScript.dependent && conditionsScript.DeactivateOnDependencyMap.dependencies.Count() > 0)
            {
                foreach (Gaze_Dependency dep in conditionsScript.DeactivateOnDependencyMap.dependencies)
                {
                    dep.ToEditorGUI();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Not used");
            }

            if (conditionsScript.customConditionsEnabled && conditionsScript.customConditions.Count > 0)
            {
                EditorGUILayout.LabelField("Custom Conditions:", EditorStyles.boldLabel);
                foreach (Gaze_AbstractConditions conditon in conditionsScript.customConditions)
                {
                    conditon.ToGUI();
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
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
