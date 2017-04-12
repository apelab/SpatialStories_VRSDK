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
    [InitializeOnLoad]
    [CustomEditor(typeof(Gaze_Interaction))]
    public class Gaze_InteractionEditor : Gaze_Editor
    {
        #region Members

        private Gaze_Interaction targetConditions;

        // logo image
        Texture logo;
        Rect logoRect;

        private Gaze_Conditions conditionsScript;

        // NEW list
        private List<Gaze_InteractiveObject> interactiveObjectsList;

        #endregion

        void OnEnable()
        {
            InitMembers();
        }

        private void InitMembers()
        {
            targetConditions = (Gaze_Interaction)target;

            interactiveObjectsList = new List<Gaze_InteractiveObject>();

            #region Logo
            logo = (Texture)Resources.Load("SpatialStorires_Logo_256", typeof(Texture));
            logoRect = new Rect();
            logoRect.x = 10;
            logoRect.y = 10;
            #endregion
        }

        public override void Gaze_OnInspectorGUI()
        {
            base.BeginChangeComparision();

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

                #region Actions
                // button to add a Conditions component
                EditorGUILayout.BeginHorizontal();
                targetConditions.HasActions = EditorGUILayout.ToggleLeft("Actions", targetConditions.HasActions);
                EditorGUILayout.EndHorizontal();
                /*
                if (targetConditions.HasActions)
                {
                    // display Activation component
                    if (!targetConditions.gameObject.GetComponent<Gaze_Actions>())
                    {
                        // add the component
                        targetConditions.gameObject.AddComponent<Gaze_Actions>();
                    }

                    // tell the script it's active
                    targetConditions.gameObject.GetComponent<Gaze_Actions>().isActive = true;
                }
                else
                {
                    // remove Activation component
                    if (targetConditions.gameObject.GetComponent<Gaze_Actions>())
                    {
                        // tell the script to deactive itself
                        targetConditions.gameObject.GetComponent<Gaze_Actions>().isActive = false;
                    }
                }
                */
                #endregion

                #region Conditions
                // button to add a Conditions component
                EditorGUILayout.BeginHorizontal();
                targetConditions.HasConditions = EditorGUILayout.ToggleLeft("Conditions", targetConditions.HasConditions);
                EditorGUILayout.EndHorizontal();

                if (targetConditions.HasConditions)
                {
                    // display Conditions component
                    //if (!targetConditions.gameObject.GetComponent<Gaze_Conditions>())
                    //{
                    //    targetConditions.gameObject.AddComponent<Gaze_Conditions>();
                    conditionsScript = targetConditions.gameObject.GetComponent<Gaze_Conditions>();
                    //}

                    // tell the script it's active
                    //targetConditions.gameObject.GetComponent<Gaze_Conditions>().isActive = true;
                }
                /*
                else
                {
                    // remove Conditions component
                    if (targetConditions.gameObject.GetComponent<Gaze_Conditions>())
                    {
                        // tell the script it's active
                        targetConditions.gameObject.GetComponent<Gaze_Conditions>().isActive = false;
                    }
                }
                */
                #endregion
            }
            else
            {
                #region State
                // display Conditions component
                conditionsScript = targetConditions.gameObject.GetComponent<Gaze_Conditions>();
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

            // save changes
            base.EndChangeComparision();
            EditorUtility.SetDirty(targetConditions);
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