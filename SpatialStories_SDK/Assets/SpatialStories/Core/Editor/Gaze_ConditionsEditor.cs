// <copyright file="Gaze_ConditionsEditor.cs" company="apelab sàrl">
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
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
namespace Gaze
{
    [CanEditMultipleObjects]
    [InitializeOnLoad]
    [CustomEditor(typeof(Gaze_Conditions))]
    public class Gaze_ConditionsEditor : Gaze_Editor
    {
        #region Members
        private Gaze_Conditions targetConditions;
        private string[] reloadModes;
        private string[] autoTriggerModes;
        private string[] focusLossModes;

        // hierarchy lists
        private List<Gaze_InteractiveObject> hierarchyIOsScripts;
        private List<string> hierarchyIOsNames;
        public List<GameObject> hierarchyIOs;
        private List<GameObject> hierarchyInteractions;
        private List<string> hierarchyInteractionsNames;

        // Input Condition
        private string[] platformsNames;
        private string[] viveInputNames;
        private string[] oculusInputNames;
        private string[] inputsNames;
        private string[] gearVrInputNames;

        private List<Gaze_InteractiveObject> hierarchyProximities;

        // Reflection members
        private List<Collider> hierarchyGazeColliders;
        private List<Collider> hierarchyHandHoverColliders;
        private Dictionary<Collider, string> interactiveObjectsDico;
        private List<Gaze_AbstractConditions> hierarchyCustomConditions;
        private List<string> hierarchyCustomConditionsNames;
        private string[] dndEventValidatorEnum;
        private string[] dndTargetsModes;
        private List<string> dndTargetsNames;
        private Gaze_SceneInventory sceneInventory;
        #endregion

        void OnEnable()
        {
            InitMembers();
        }

        private void InitMembers()
        {
            targetConditions = (Gaze_Conditions)target;

            focusLossModes = Enum.GetNames(typeof(Gaze_FocusLossMode));
            hierarchyIOsScripts = new List<Gaze_InteractiveObject>();
            hierarchyIOsNames = new List<string>();
            hierarchyIOs = new List<GameObject>();
            hierarchyInteractions = new List<GameObject>();
            hierarchyInteractionsNames = new List<string>();
            hierarchyGazeColliders = new List<Collider>();
            hierarchyProximities = new List<Gaze_InteractiveObject>();
            hierarchyHandHoverColliders = new List<Collider>();
            dndTargetsNames = new List<string>();
            reloadModes = Enum.GetNames(typeof(Gaze_ReloadMode));
            autoTriggerModes = Enum.GetNames(typeof(Gaze_AutoTriggerMode));
            hierarchyCustomConditions = new List<Gaze_AbstractConditions>();
            hierarchyCustomConditionsNames = new List<string>();
            dndEventValidatorEnum = Enum.GetNames(typeof(Gaze_DragAndDropStates));
            dndTargetsModes = Enum.GetNames(typeof(apelab_DnDTargetsModes));
            FetchDnDTargets();
            FetchInputsLists();
            sceneInventory = UnityEngine.Object.FindObjectOfType<Gaze_SceneInventory>();
        }

        private void FetchDnDTargets()
        {
            dndTargetsNames.Clear();

            // get names of targets IOs in the Gaze_InteractiveObject's editor list
            int targetsCount = targetConditions.RootIO.DnD_Targets.Count();
            if (targetsCount > 0)
            {
                for (int i = 0; i < targetsCount; i++)
                {
                    if (targetConditions.RootIO.DnD_Targets[i] == null)
                        targetConditions.RootIO.DnD_Targets.RemoveAt(i);
                    else
                        dndTargetsNames.Add(targetConditions.RootIO.DnD_Targets[i].ToString());
                }
            }
        }

        /// <summary>
        /// Get all the input related lists
        /// </summary>
        private void FetchInputsLists()
        {
            inputsNames = Enum.GetNames(typeof(Gaze_InputTypes));
            platformsNames = Enum.GetNames(typeof(Gaze_Controllers));
            viveInputNames = Enum.GetNames(typeof(Gaze_HTCViveInputTypes));
            oculusInputNames = Enum.GetNames(typeof(Gaze_OculusInputTypes));
            gearVrInputNames = Enum.GetNames(typeof(Gaze_GearVRInputTypes));

            SplitWord(oculusInputNames);
            SplitWord(viveInputNames);
            SplitWord(gearVrInputNames);
        }

        // Split the word at every capital letter to make it more readable
        private void SplitWord(string[] _string)
        {
            for (int i = 0; i < _string.Count(); i++)
            {
                _string[i] = Regex.Replace(_string[i], "(\\B[A-Z])", " $1");
            }

        }

        public override void Gaze_OnInspectorGUI()
        {
            base.BeginChangeComparision();

            // display conditions
            if (targetConditions.isActive)
            {
                if (!Application.isPlaying)
                {
                    Gaze_EditorUtils.DrawEditorHint("This script allows you to set when the interaction will happen");
                    EditorGUILayout.Space();
                    // update InteractiveObjects list
                    UpdateInteractiveObjectsList();

                    Gaze_EditorUtils.DrawSectionTitle("DEPENDENCIES");

                    Gaze_EditorUtils.DrawEditorHint("Use this to create chain reactions with any other interactions in the scene.");
                    #region Dependency
                    // set boolean value accordingly in Trigger settings
                    targetConditions.dependent = EditorGUILayout.ToggleLeft("Dependent", targetConditions.dependent);
                    if (targetConditions.dependent)
                    {
                        DisplayDependencyBlock();
                    }
                    EditorGUILayout.Space();
                    #endregion

                    #region Conditions
                    EditorGUILayout.Space();


                    Gaze_EditorUtils.DrawSectionTitle("CONDITIONS");
                    DisplayConditionsBlock();
                    DisplayProximityList();


                    DisplayHandHoverBlock();
                    DisplayTouchCondition();
                    DisplayGrabCondition();
                    DisplayInputsCondition();
                    DisplayTeleportCondition();
                    DisplayDragAndDropCondition();
                    EditorGUILayout.Space();
                    #endregion

                    #region Custom Conditions
                    //Gaze_EditorUtils.DrawSectionTitle("CUSTOM CONDITIONS");
                 
                    targetConditions.customConditionsEnabled = EditorGUILayout.ToggleLeft("Custom Conditions", targetConditions.customConditionsEnabled);
                    Gaze_EditorUtils.DrawEditorHint("Check this option if you added a custom condition as a separate component.", false);
                    Gaze_EditorUtils.DrawEditorHint("See user manual for detailed structure.");
                    if (targetConditions.customConditionsEnabled)
                    {
                        DisplayCustomConditionsList();
                    }
                    EditorGUILayout.Space();
                    #endregion

                    #region Warning
                    //DisplayWarning();
                    #endregion

                    DisplayDuration();

                    #region Delayed ActiveWindow
                    Gaze_EditorUtils.DrawSectionTitle("TIMEFRAME");

                    // extra block that can be toggled on and off
                    GUILayout.BeginHorizontal();
                    // set boolean value accordingly in Trigger settings
                    targetConditions.delayed = EditorGUILayout.ToggleLeft(new GUIContent("Delayed", "Amount of time (seconds) this interaction will have to wait before allowing the user to validate the conditions."), targetConditions.delayed);
                    if (targetConditions.delayed)
                    {
                        DisplayDelayBlock();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    // set boolean value accordingly in Trigger settings
                    targetConditions.expires = EditorGUILayout.ToggleLeft(new GUIContent("Expires", "Conditions can be met only during this amount of time. The interaction will then be deactivated."), targetConditions.expires);
                    if (targetConditions.expires)
                    {
                        DisplayExpiresBlock();
                    }
                    GUILayout.EndHorizontal();

                    // set boolean value accordingly in Trigger settings
                    targetConditions.autoTriggerModeIndex = Gaze_EditorUtils.Gaze_HintPopup("Auto Trigger Mode", targetConditions.autoTriggerModeIndex, autoTriggerModes, "START: This option will be removed as actions automatically fire on start\nEND: The action will automatically fire after its expiring window.", 130);

                    if (((Gaze_AutoTriggerMode)targetConditions.autoTriggerModeIndex).Equals(Gaze_AutoTriggerMode.END) && !targetConditions.expires)
                    {
                        EditorGUILayout.HelpBox("This interaction does not expire, it will never auto-trigger.", MessageType.Warning);

                    }
                    else if (targetConditions.autoTriggerModeIndex.Equals((int)Gaze_AutoTriggerMode.NONE) && targetConditions.expires && targetConditions.activeDuration == 0)
                    {
                        if (!(targetConditions.isExpireRandom && targetConditions.expireRange[1] > 0))
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox("Will never trigger !\n(Expires immediately AND has no Auto-Trigger).", MessageType.Warning);
                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    EditorGUILayout.Space();
                    #endregion

                    #region Reload
                    // toggle button
                    Gaze_EditorUtils.DrawSectionTitle("RELOAD");

                    // set boolean value accordingly in Trigger settings
                    targetConditions.reload = EditorGUILayout.ToggleLeft(new GUIContent("Reload", "Allows you to reload the conditions of an interaction (does not reload dependencies)."), targetConditions.reload);

                    if (targetConditions.reload)
                    {
                        DisplayReloadBlock();
                    }

                    EditorGUILayout.Space();
                    #endregion
                }
            }

            // save changes
            base.EndChangeComparision();
            EditorUtility.SetDirty(targetConditions);
        }

        private void DisplayDuration()
        {
            // DURATION
            Gaze_EditorUtils.DrawSectionTitle("Duration");
            Gaze_EditorUtils.DrawEditorHint("use this to specify a given amount of time while conditions need to remain valid.");

            EditorGUI.indentLevel++;
            GUILayout.BeginHorizontal();
            targetConditions.focusDuration = EditorGUILayout.FloatField("Duration [s]", targetConditions.focusDuration);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            targetConditions.focusLossModeIndex = EditorGUILayout.Popup("Loss Mode", targetConditions.focusLossModeIndex, focusLossModes);
            if (targetConditions.focusLossModeIndex.Equals((int)Gaze_FocusLossMode.FADE))
            {
                targetConditions.FocusLossSpeed = EditorGUILayout.FloatField("Speed Factor", targetConditions.FocusLossSpeed);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
            // END OF DURATION
        }

        /// <summary>
        /// Get all InteractiveObjects in the scene !
        /// Only executed in Editor Mode (not at runtime)
        /// </summary>
        private void UpdateInteractiveObjectsList()
        {
            // clear lists
            hierarchyIOsScripts.Clear();
            hierarchyIOsNames.Clear();
            hierarchyIOs.Clear();
            hierarchyInteractions.Clear();
            hierarchyGazeColliders.Clear();
            hierarchyProximities.Clear();
            hierarchyCustomConditions.Clear();
            hierarchyCustomConditionsNames.Clear();
            targetConditions.customConditions.Clear();
            hierarchyHandHoverColliders.Clear();

            hierarchyIOsScripts = (FindObjectsOfType(typeof(Gaze_InteractiveObject)) as Gaze_InteractiveObject[]).ToList();
            for (int i = 0; i < hierarchyIOsScripts.Count; i++)
            {
                hierarchyIOsNames.Add(hierarchyIOsScripts[i].name);
                hierarchyIOs.Add(hierarchyIOsScripts[i].gameObject);

                UpdateConditionsLists(hierarchyIOsScripts[i].gameObject);
                UpdateGazeCollidersList(hierarchyIOsScripts[i].gameObject);
                UpdateProximitiesList(hierarchyIOsScripts[i].gameObject);
                UpdateHandHoverCollidersList(hierarchyIOsScripts[i].gameObject);
            }
            UpdateCustomConditionsList();
        }

        private void UpdateConditionsLists(GameObject g)
        {
            Gaze_Interaction[] interactions = g.GetComponentsInChildren<Gaze_Interaction>() as Gaze_Interaction[];

            // Sort the hierarchy list
            List<Gaze_Interaction> SortedList = interactions.OrderBy(o => o.name).ToList();
            interactions = SortedList.ToArray();

            for (int i = 0; i < interactions.Length; i++)
            {
                // build contitions
                hierarchyInteractions.Add(interactions[i].gameObject);

                // build conditions' names
                hierarchyInteractionsNames.Add(interactions[i].name);
            }
        }

        private void UpdateHandHoverCollidersList(GameObject g)
        {
            if (g && g.GetComponentInChildren<Gaze_HandHover>())
                hierarchyHandHoverColliders.Add(g.GetComponentInChildren<Gaze_HandHover>().GetComponent<Collider>());
        }

        private void UpdateGazeCollidersList(GameObject g)
        {
            if (g && g.GetComponentInChildren<Gaze_Gaze>())
                hierarchyGazeColliders.Add(g.GetComponentInChildren<Gaze_Gaze>().GetComponent<Collider>());
        }

        private void UpdateProximitiesList(GameObject g)
        {
            Gaze_Proximity prox = g.GetComponentInChildrenBFS<Gaze_Proximity>();
            if (prox != null)
            {
                Gaze_InteractiveObject proximity = Gaze_Utils.GetIOFromGameObject(prox.gameObject);
                hierarchyProximities.Add(proximity);
            }
        }

        private void UpdateCustomConditionsList()
        {
            // find all AbstractCondtiions scripts in the component
            hierarchyCustomConditions.AddRange(targetConditions.GetComponents<Gaze_AbstractConditions>());

            // get list names of scripts
            for (int i = 0; i < hierarchyCustomConditions.Count; i++)
            {
                hierarchyCustomConditionsNames.Add(hierarchyCustomConditions[i].GetType().Name);
                targetConditions.customConditions.Add(hierarchyCustomConditions[i]);
            }
        }

        private void DisplayCustomConditionsList()
        {
            if (targetConditions.customConditionsEnabled)
            {
                if (hierarchyCustomConditions.Count < 1)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("No valid custom condition found on this component !\nInherit your custom script from Gaze_AbstractConditions.", MessageType.Warning);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    // display all the custom scripts
                    for (int i = 0; i < hierarchyCustomConditions.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("    - " + hierarchyCustomConditionsNames[i], EditorStyles.whiteMiniLabel);
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }

        private void DisplayProximityList()
        {
            EditorGUILayout.BeginHorizontal();
            targetConditions.proximityEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Proximity", "Define which IO is in collision with another. Uses the Proximity Collider Zone. Add Rig Group: Add this if you want to use the head or the hands of the user as the proximity zone."), targetConditions.proximityEnabled);
            EditorGUILayout.EndHorizontal();

            if (targetConditions.proximityEnabled)
            {
                // display the OnEnter OR OnExit condition for the list
                EditorGUILayout.BeginHorizontal();
                targetConditions.proximityMap.proximityStateIndex = EditorGUILayout.Popup(targetConditions.proximityMap.proximityStateIndex, Enum.GetNames(typeof(ProximityEventsAndStates)));

                if (hierarchyProximities.Count < 2)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("No other Interactive Object found.", MessageType.Warning);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    targetConditions.requireAllProximities = EditorGUILayout.ToggleLeft("Require all", targetConditions.requireAllProximities);

                    if (targetConditions.proximityMap.proximityEntryList.Count > 0)
                    {
                        // NOTE don't use foreach to avoid InvalidOperationException
                        for (int i = 0; i < targetConditions.proximityMap.proximityEntryList.Count; i++)
                        {
                            // delete from the list the gazables no more in the hierarchy
                            if (!hierarchyProximities.Contains(targetConditions.proximityMap.proximityEntryList[i].dependentGameObject))
                            {
                                targetConditions.proximityMap.DeleteProximityEntry(targetConditions.proximityMap.proximityEntryList[i]);
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    // update the list of all possible rig groups
                    targetConditions.UpdateRigSets(Gaze_Proximity.HierarchyRigProximities);

                    if (targetConditions.proximityMap.proximityEntryGroupList.Count < 1 && Gaze_Proximity.HierarchyRigProximities.Count > 1)
                    {
                        // display 'add rig group' button
                        if (GUILayout.Button("Add Rig Group"))
                        {
                            targetConditions.proximityMap.AddProximityEntryGroup(targetConditions);
                        }
                    }

                    // This is a HOTFIX for solving editor problems that appear when user deletes every element of rig except one and there is a proximity rig group set up somewhere
                    // will need to be changed if custom proximity groups (other than rig group) are implemented
                    if (Gaze_Proximity.HierarchyRigProximities.Count < 2)
                    {
                        if (targetConditions.proximityMap.proximityEntryGroupList.Count > 0)
                            targetConditions.proximityMap.proximityEntryGroupList.Clear();
                    }
                    else
                    {
                        for (int i = 0; i < targetConditions.proximityMap.proximityEntryGroupList.Count; i++)
                        {
                            EditorGUILayout.BeginHorizontal();

                            // Display the popup with all possible combinations
                            targetConditions.proximityGroupIndex = EditorGUILayout.Popup(targetConditions.proximityGroupIndex, targetConditions.rigCombinations.ToArray());

                            targetConditions.proximityMap.proximityEntryGroupList[i].proximityEntries.Clear();
                            for (int j = 0; j < targetConditions.proximityRigGroups[targetConditions.proximityGroupIndex].Count; j++)
                            {
                                targetConditions.proximityMap.proximityEntryGroupList[i].AddProximityEntryToGroup(targetConditions.proximityRigGroups[targetConditions.proximityGroupIndex][j]);
                            }

                            if (GUILayout.Button("-"))
                            {
                                targetConditions.proximityMap.DeleteProximityEntryGroup(targetConditions.proximityMap.proximityEntryGroupList[i]);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    for (int i = 0; i < targetConditions.proximityMap.proximityEntryList.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        // Set the default collider for the gaze
                        if (targetConditions.proximityMap.proximityEntryList[i].dependentGameObject == null)
                        {
                            targetConditions.proximityMap.proximityEntryList[i].dependentGameObject = targetConditions.GetComponentInParent<Gaze_InteractiveObject>();
                        }

                        var proximityObject = EditorGUILayout.ObjectField(targetConditions.proximityMap.proximityEntryList[i].dependentGameObject, typeof(Gaze_InteractiveObject), true);

                        if (proximityObject != null)
                            targetConditions.proximityMap.proximityEntryList[i].dependentGameObject = (Gaze_InteractiveObject)proximityObject;

                        if (GUILayout.Button("-"))
                        {
                            targetConditions.proximityMap.DeleteProximityEntry(targetConditions.proximityMap.proximityEntryList[i]);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    if (targetConditions.proximityMap.proximityEntryList.Count + targetConditions.proximityMap.proximityEntryGroupList.Count < 2)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.HelpBox("You need at least two proximities.", MessageType.Warning);
                        EditorGUILayout.EndHorizontal();
                    }

                    // display 'add' button
                    if (GUILayout.Button("+"))
                    {
                        Gaze_ProximityEntry d = targetConditions.proximityMap.AddProximityEntry(targetConditions);
                        EditorGUILayout.BeginHorizontal();

                        // assign the first interactive object in the hierarchy list by default (0)
                        if (hierarchyProximities != null && hierarchyProximities.Count > 0)
                        {
                            if (d.dependentGameObject == null)
                            {
                                d.dependentGameObject = targetConditions.GetComponentInParent<Gaze_InteractiveObject>();
                            }

                            var proximityObject = EditorGUILayout.ObjectField(d.dependentGameObject, typeof(Gaze_InteractiveObject), true);

                            if (proximityObject != null)
                                d.dependentGameObject = (Gaze_InteractiveObject)proximityObject;

                            if (GUILayout.Button("-"))
                            {
                                targetConditions.proximityMap.DeleteProximityEntry(d);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.Space();
        }

        private void DisplayTouchCondition()
        {
            EditorGUILayout.BeginHorizontal();
            targetConditions.touchEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Touch", "Point and trigger at the selected IO to fire an action. Uses the Hand Hover Collider."), targetConditions.touchEnabled);
            EditorGUILayout.EndHorizontal();

            if (targetConditions.touchEnabled)
            {
                // if there are no grabbables in the scene, exit
                if (hierarchyIOs.Count < 1)
                {
                    EditorGUILayout.HelpBox("You need at least one Interactive Object with Touch enabled in the scene !", MessageType.Warning);
                    return;
                }

                // if there are no entry yet, create a default one
                if (targetConditions.touchMap.TouchEnitry == null ||
                    (targetConditions.touchMap.TouchEnitry != null &&
                    targetConditions.touchMap.TouchEnitry.interactiveObject == null))
                    targetConditions.touchMap.AddActivableEntry(targetConditions.RootIO.gameObject);

                // chose which hand to use1
                EditorGUILayout.BeginHorizontal();
                targetConditions.touchMap.touchHandsIndex = EditorGUILayout.Popup(targetConditions.touchMap.touchHandsIndex, Enum.GetNames(typeof(Gaze_HandsEnum)));

                if (targetConditions.touchMap.touchHandsIndex.Equals((int)Gaze_HandsEnum.LEFT))
                {
                    targetConditions.touchMap.TouchEnitry.hand = UnityEngine.XR.XRNode.LeftHand;
                }
                else if (targetConditions.touchMap.touchHandsIndex.Equals((int)Gaze_HandsEnum.RIGHT))
                {
                    targetConditions.touchMap.TouchEnitry.hand = UnityEngine.XR.XRNode.RightHand;
                }
                else // We store both in left
                {
                    targetConditions.touchMap.TouchEnitry.hand = UnityEngine.XR.XRNode.LeftHand;
                }
                targetConditions.touchMap.touchActionIndex = EditorGUILayout.Popup(targetConditions.touchMap.touchActionIndex, Enum.GetNames(typeof(Gaze_TouchAction)));

                // Add the searchable objec dialog
                var objectToTouch = EditorGUILayout.ObjectField(targetConditions.touchMap.TouchEnitry.interactiveObject, typeof(Gaze_InteractiveObject), true);
                if (objectToTouch != null)
                {
                    if (objectToTouch is GameObject)
                        targetConditions.touchMap.TouchEnitry.interactiveObject = (GameObject)objectToTouch;
                    else
                        targetConditions.touchMap.TouchEnitry.interactiveObject = ((Gaze_InteractiveObject)objectToTouch).gameObject;
                }
                else
                    targetConditions.touchMap.TouchEnitry.interactiveObject = null;

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
        }

        private void DisplayGrabCondition()
        {
            EditorGUILayout.BeginHorizontal();
            targetConditions.grabEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Grab", "Checks if an IO is grabbed or released. Uses the Manipulation Collider zone."), targetConditions.grabEnabled);
            EditorGUILayout.EndHorizontal();

            if (targetConditions.grabEnabled)
            {
                // if there are no grabbables in the scene, exit
                if (hierarchyIOs.Count < 1)
                {
                    EditorGUILayout.HelpBox("You need at least one Interactive Object in the scene !", MessageType.Warning);
                    return;
                }

                // if there are no entry yet, create a default one
                if (targetConditions.grabMap.grabEntryList.Count < 1)
                    targetConditions.grabMap.AddGrabableEntry(targetConditions.RootIO.gameObject);

                // chose which hand to use
                EditorGUILayout.BeginHorizontal();
                targetConditions.grabMap.grabHandsIndex = EditorGUILayout.Popup(targetConditions.grabMap.grabHandsIndex, Enum.GetNames(typeof(Gaze_HandsEnum)));

                if (targetConditions.grabMap.grabHandsIndex.Equals((int)Gaze_HandsEnum.LEFT))
                {
                    targetConditions.grabMap.grabStateLeftIndex = EditorGUILayout.Popup(targetConditions.grabMap.grabStateLeftIndex, Enum.GetNames(typeof(Gaze_GrabStates)));
                    targetConditions.grabMap.grabEntryList[0].hand = UnityEngine.XR.XRNode.LeftHand;
                }
                else if (targetConditions.grabMap.grabHandsIndex.Equals((int)Gaze_HandsEnum.RIGHT))
                {
                    targetConditions.grabMap.grabStateRightIndex = EditorGUILayout.Popup(targetConditions.grabMap.grabStateRightIndex, Enum.GetNames(typeof(Gaze_GrabStates)));
                    targetConditions.grabMap.grabEntryList[0].hand = UnityEngine.XR.XRNode.RightHand;
                }
                else
                {
                    targetConditions.grabMap.grabStateLeftIndex = EditorGUILayout.Popup(targetConditions.grabMap.grabStateLeftIndex, Enum.GetNames(typeof(Gaze_GrabStates)));
                    targetConditions.grabMap.grabStateRightIndex = targetConditions.grabMap.grabStateLeftIndex;
                    targetConditions.grabMap.grabEntryList[0].hand = UnityEngine.XR.XRNode.LeftHand;
                }

                var grabObject = EditorGUILayout.ObjectField(targetConditions.grabMap.grabEntryList[0].interactiveObject, typeof(Gaze_InteractiveObject), true);
                if (grabObject != null)
                {
                    if (grabObject is Gaze_InteractiveObject)
                        targetConditions.grabMap.grabEntryList[0].interactiveObject = ((Gaze_InteractiveObject)grabObject).gameObject;
                    else
                        targetConditions.grabMap.grabEntryList[0].interactiveObject = (GameObject)grabObject;
                }
                EditorGUILayout.EndHorizontal();
            }


            EditorGUILayout.Space();
        }

        private void DisplayInputsCondition()
        {
            EditorGUILayout.BeginHorizontal();
            targetConditions.inputsEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Inputs", "Choose any controller input available to trigger an action. See unity doc for button mapping."), targetConditions.inputsEnabled);

            if (targetConditions.inputsEnabled)
            {
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
                        DisplayInputEntry(targetConditions.InputsMap.InputsEntries[i]);
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
            }
            else
                EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space();
        }

        private void DisplayInputEntry(Gaze_InputsMapEntry _inputEntry)
        {
            Gaze_Controllers lastSelectedPlatform = _inputEntry.UISelectedPlatform;
            int lastSelectedSpecificInput = _inputEntry.UIControllerSpecificInput;
            bool somethingHasChanged = false;

            // display the entry
            EditorGUILayout.BeginHorizontal();

            // Display the platform
            _inputEntry.UISelectedPlatform = (Gaze_Controllers)EditorGUILayout.Popup((int)_inputEntry.UISelectedPlatform, platformsNames);

            // If the user changes the platform restart the input index to avoid index out of bound exeptions
            if (lastSelectedPlatform != _inputEntry.UISelectedPlatform)
            {
                _inputEntry.UIControllerSpecificInput = 0;
                somethingHasChanged = true;
            }

            // Display the correct input list for the selected platform
            switch (_inputEntry.UISelectedPlatform)
            {
                case Gaze_Controllers.HTC_VIVE:
                    _inputEntry.UIControllerSpecificInput = EditorGUILayout.Popup(_inputEntry.UIControllerSpecificInput, viveInputNames);
                    break;
                case Gaze_Controllers.OCULUS_RIFT:
                    _inputEntry.UIControllerSpecificInput = EditorGUILayout.Popup(_inputEntry.UIControllerSpecificInput, oculusInputNames);
                    break;
                case Gaze_Controllers.GEARVR_CONTROLLER:
                    _inputEntry.UIControllerSpecificInput = EditorGUILayout.Popup(_inputEntry.UIControllerSpecificInput, gearVrInputNames);
                    break;
            }


            somethingHasChanged = lastSelectedSpecificInput != _inputEntry.UIControllerSpecificInput || somethingHasChanged;

            if (somethingHasChanged)
            {
                _inputEntry.InputType = Gaze_PlatformSpecificToGenericInputMapper.ToGenericInput(_inputEntry.UISelectedPlatform, _inputEntry.UIControllerSpecificInput);

            }

            if (GUILayout.Button("-"))
                targetConditions.InputsMap.Delete(_inputEntry);

            EditorGUILayout.EndHorizontal();
            //EditorGUILayout.LabelField(_inputEntry.InputType.ToString());
        }

        private void DisplayTeleportCondition()
        {
            EditorGUILayout.BeginHorizontal();
            targetConditions.teleportEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Teleport", "Fire an action when Teleport is Good, Bad, activated or deactivated."), targetConditions.teleportEnabled);

            // chose teleport's action mode as a condition
            if (targetConditions.teleportEnabled)
                targetConditions.teleportIndex = EditorGUILayout.Popup(targetConditions.teleportIndex, Enum.GetNames(typeof(Gaze_TeleportMode)));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        private void DisplayWarning()
        {
            if (!targetConditions.gazeEnabled &&
                !targetConditions.proximityEnabled &&
                !targetConditions.dependent &&
                !targetConditions.touchEnabled &&
                !targetConditions.grabEnabled &&
                !targetConditions.customConditionsEnabled &&
                !targetConditions.handHoverEnabled)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("No condition selected !\nThis Interactive Object will trigger immediately !", MessageType.Warning);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DisplayDependencyBlock()
        {
            // NOTE : should check if there are null ref in gazable.dependencies still
            if (hierarchyInteractions.Count == 0)
            {
                EditorGUILayout.HelpBox("No triggers found.", MessageType.Warning);
                return;
            }

            // check if there are conditions specified
            if (targetConditions.ActivateOnDependencyMap.dependencies.Count < 1 && targetConditions.DeactivateOnDependencyMap.dependencies.Count < 1)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("Add at least one dependency or deactivate it if not needed.", MessageType.Warning);
                EditorGUILayout.EndHorizontal();
            }

            // check if there are there are deactivate without activate because it wont work
            if (targetConditions.ActivateOnDependencyMap.dependencies.Count < 1 && targetConditions.DeactivateOnDependencyMap.dependencies.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("Add at least one dependency to activate in order to make the deactivate dependencies work.", MessageType.Warning);
                EditorGUILayout.EndHorizontal();
            }

            // On Activation button
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Activate on");
            targetConditions.requireAllActivators = EditorGUILayout.ToggleLeft("Require all", targetConditions.requireAllActivators);
            EditorGUILayout.EndHorizontal();

            // update dependencies list in gazable (gazables may have been removed in the hierarchy)
            if (targetConditions.ActivateOnDependencyMap.dependencies.Count > 0)
            {
                // NOTE don't use foreach to avoid InvalidOperationException
                for (int i = 0; i < targetConditions.ActivateOnDependencyMap.dependencies.Count; i++)
                {
                    if (!hierarchyInteractions.Contains(targetConditions.ActivateOnDependencyMap.dependencies[i].dependentGameObject))
                    {
                        targetConditions.ActivateOnDependencyMap.Delete(targetConditions.ActivateOnDependencyMap.dependencies[i]);
                    }
                }
            }

            // display all existing dependencies in the Gazable
            for (int i = 0; i < targetConditions.ActivateOnDependencyMap.dependencies.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                var activateDependency = EditorGUILayout.ObjectField(targetConditions.ActivateOnDependencyMap.dependencies[i].dependentGameObject, typeof(Gaze_Conditions), true);

                if (activateDependency != null)
                {
                    if (activateDependency is Gaze_Conditions)
                        targetConditions.ActivateOnDependencyMap.dependencies[i].dependentGameObject = ((Gaze_Conditions)activateDependency).gameObject;
                    else
                        targetConditions.ActivateOnDependencyMap.dependencies[i].dependentGameObject = (GameObject)activateDependency;
                }

                targetConditions.ActivateOnDependencyMap.dependencies[i].triggerStateIndex = EditorGUILayout.Popup(targetConditions.ActivateOnDependencyMap.dependencies[i].triggerStateIndex, Enum.GetNames(typeof(DependencyTriggerEventsAndStates)));

                // if dependent on Trigger event
                targetConditions.ActivateOnDependencyMap.dependencies[i].onTrigger = targetConditions.ActivateOnDependencyMap.dependencies[i].triggerStateIndex == (int)DependencyTriggerEventsAndStates.Triggered;

                if (GUILayout.Button("-"))
                {
                    targetConditions.ActivateOnDependencyMap.Delete(targetConditions.ActivateOnDependencyMap.dependencies[i]);
                }

                EditorGUILayout.EndHorizontal();
            }

            // display 'add' button
            if (GUILayout.Button("+"))
            {
                Gaze_Dependency d = targetConditions.ActivateOnDependencyMap.Add(targetConditions);
                EditorGUILayout.BeginHorizontal();
                d.dependentGameObject = hierarchyInteractions[EditorGUILayout.Popup(0, hierarchyInteractionsNames.ToArray())];

                d.triggerStateIndex = (int)DependencyTriggerEventsAndStates.Triggered;
                if (GUILayout.Button("-"))
                {
                    targetConditions.ActivateOnDependencyMap.Delete(d);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();

            // Deactivate
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Deactivate on");
            targetConditions.requireAllDeactivators = EditorGUILayout.ToggleLeft("Require all", targetConditions.requireAllDeactivators);
            EditorGUILayout.EndHorizontal();

            // update dependencies list in gazable (gazables may have been removed in the hierarchy)
            if (targetConditions.DeactivateOnDependencyMap.dependencies.Count > 0)
            {
                // NOTE don't use foreach to avoid InvalidOperationException
                for (int i = 0; i < targetConditions.DeactivateOnDependencyMap.dependencies.Count; i++)
                {
                    if (!hierarchyInteractions.Contains(targetConditions.DeactivateOnDependencyMap.dependencies[i].dependentGameObject))
                    {
                        targetConditions.DeactivateOnDependencyMap.Delete(targetConditions.DeactivateOnDependencyMap.dependencies[i]);
                    }
                }
            }

            // display all existing dependencies in the Gazable
            for (int i = 0; i < targetConditions.DeactivateOnDependencyMap.dependencies.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                var activateDependency = EditorGUILayout.ObjectField(targetConditions.DeactivateOnDependencyMap.dependencies[i].dependentGameObject, typeof(Gaze_Conditions), true);

                if (activateDependency != null)
                {
                    if (activateDependency is Gaze_Conditions)
                        targetConditions.DeactivateOnDependencyMap.dependencies[i].dependentGameObject = ((Gaze_Conditions)activateDependency).gameObject;
                    else
                        targetConditions.DeactivateOnDependencyMap.dependencies[i].dependentGameObject = (GameObject)activateDependency;
                }

                targetConditions.DeactivateOnDependencyMap.dependencies[i].triggerStateIndex = EditorGUILayout.Popup(targetConditions.DeactivateOnDependencyMap.dependencies[i].triggerStateIndex, Enum.GetNames(typeof(DependencyTriggerEventsAndStates)));

                // if dependent on Trigger event
                targetConditions.DeactivateOnDependencyMap.dependencies[i].onTrigger = targetConditions.DeactivateOnDependencyMap.dependencies[i].triggerStateIndex == (int)DependencyTriggerEventsAndStates.Triggered;

                if (GUILayout.Button("-"))
                {
                    targetConditions.DeactivateOnDependencyMap.Delete(targetConditions.DeactivateOnDependencyMap.dependencies[i]);
                }
                EditorGUILayout.EndHorizontal();
            }

            // display 'add' button
            if (GUILayout.Button("+"))
            {
                Gaze_Dependency d = targetConditions.DeactivateOnDependencyMap.Add(targetConditions);
                EditorGUILayout.BeginHorizontal();
                d.dependentGameObject = hierarchyInteractions[EditorGUILayout.Popup(0, hierarchyInteractionsNames.ToArray())];
                d.triggerStateIndex = (int)DependencyTriggerEventsAndStates.Triggered;
                if (GUILayout.Button("-"))
                {
                    targetConditions.DeactivateOnDependencyMap.Delete(d);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DisplayDelayBlock()
        {
            GUILayout.BeginVertical();

            if (!targetConditions.isDelayRandom)
            {
                GUILayout.BeginHorizontal();
                targetConditions.delayDuration = EditorGUILayout.FloatField(targetConditions.delayDuration);
                Gaze_Utils.EnsureFieldIsPositiveOrZero(ref targetConditions.delayDuration);
                EditorGUILayout.LabelField("[s]");
                GUILayout.EndHorizontal();
            }

            else
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Min", GUILayout.MaxWidth(50));
                targetConditions.delayRange[0] = EditorGUILayout.FloatField(targetConditions.delayRange[0]);
                Gaze_Utils.EnsureFieldIsPositiveOrZero(ref targetConditions.delayRange[0]);
                EditorGUILayout.LabelField("[s]");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Max", GUILayout.MaxWidth(50));
                targetConditions.delayRange[1] = EditorGUILayout.FloatField(targetConditions.delayRange[1]);
                Gaze_Utils.EnsureFieldIsPositiveOrZero(ref targetConditions.delayRange[1]);
                EditorGUILayout.LabelField("[s]");
                GUILayout.EndHorizontal();

                if (targetConditions.delayRange[1] < targetConditions.delayRange[0])
                    targetConditions.delayRange[1] = targetConditions.delayRange[0] + 1.0f;
            }

            GUILayout.BeginHorizontal();
            targetConditions.isDelayRandom = EditorGUILayout.ToggleLeft("Random", targetConditions.isDelayRandom);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private void DisplayExpiresBlock()
        {
            GUILayout.BeginVertical();

            if (!targetConditions.isExpireRandom)
            {
                GUILayout.BeginHorizontal();
                targetConditions.activeDuration = EditorGUILayout.FloatField(targetConditions.activeDuration);
                Gaze_Utils.EnsureFieldIsPositiveOrZero(ref targetConditions.activeDuration);
                EditorGUILayout.LabelField("[s]");
                GUILayout.EndHorizontal();
            }

            else
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Min", GUILayout.MaxWidth(50));
                targetConditions.expireRange[0] = EditorGUILayout.FloatField(targetConditions.expireRange[0]);
                Gaze_Utils.EnsureFieldIsPositiveOrZero(ref targetConditions.expireRange[0]);
                EditorGUILayout.LabelField("[s]");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Max", GUILayout.MaxWidth(50));
                targetConditions.expireRange[1] = EditorGUILayout.FloatField(targetConditions.expireRange[1]);
                Gaze_Utils.EnsureFieldIsPositiveOrZero(ref targetConditions.expireRange[1]);
                EditorGUILayout.LabelField("[s]");
                GUILayout.EndHorizontal();

                if (targetConditions.expireRange[1] < targetConditions.expireRange[0])
                    targetConditions.expireRange[1] = targetConditions.expireRange[0] + 1.0f;
            }

            GUILayout.BeginHorizontal();
            targetConditions.isExpireRandom = EditorGUILayout.ToggleLeft("Random", targetConditions.isExpireRandom);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private void DisplayReloadBlock()
        {
            targetConditions.reloadModeIndex = Gaze_EditorUtils.Gaze_HintPopup("Mode", targetConditions.reloadModeIndex, reloadModes, "Reload the trigger Infinitely, for a specific number of times or after a specific script.", 130);
            if (targetConditions.reloadModeIndex.Equals((int)Gaze_ReloadMode.FINITE))
            {
                targetConditions.reloadMaxRepetitions = EditorGUILayout.IntField("Repetitions", targetConditions.reloadMaxRepetitions);
                if (targetConditions.reloadMaxRepetitions < 1)
                {
                    targetConditions.reloadMaxRepetitions = 1;
                }
            }

            if (targetConditions.reloadModeIndex.Equals((int)Gaze_ReloadMode.FINITE) || targetConditions.reloadModeIndex.Equals((int)Gaze_ReloadMode.INFINITE))
            {
                GUILayout.BeginHorizontal();
                DisplayReloadDelayBlock();
                GUILayout.EndHorizontal();
            }

            // TODO(4nc3str4l): Put this on a better place
            EditorGUILayout.Space();
            targetConditions.ReloadDependencies = EditorGUILayout.ToggleLeft("Reload Dependencies", targetConditions.ReloadDependencies);
        }

        private void DisplayReloadDelayBlock()
        {
            EditorGUILayout.LabelField("Delay");

            GUILayout.BeginVertical();

            if (!targetConditions.isReloadRandom)
            {
                GUILayout.BeginHorizontal();
                targetConditions.reloadDelay = EditorGUILayout.FloatField(targetConditions.reloadDelay);
                Gaze_Utils.EnsureFieldIsPositiveOrZero(ref targetConditions.reloadDelay);
                EditorGUILayout.LabelField("[s]");
                GUILayout.EndHorizontal();
            }

            else
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Min", GUILayout.MaxWidth(50));
                targetConditions.reloadRange[0] = EditorGUILayout.FloatField(targetConditions.reloadRange[0]);
                Gaze_Utils.EnsureFieldIsPositiveOrZero(ref targetConditions.reloadRange[0]);
                EditorGUILayout.LabelField("[s]");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Max", GUILayout.MaxWidth(50));
                targetConditions.reloadRange[1] = EditorGUILayout.FloatField(targetConditions.reloadRange[1]);
                Gaze_Utils.EnsureFieldIsPositiveOrZero(ref targetConditions.reloadRange[1]);
                EditorGUILayout.LabelField("[s]");
                GUILayout.EndHorizontal();

                if (targetConditions.reloadRange[1] < targetConditions.reloadRange[0])
                    targetConditions.reloadRange[1] = targetConditions.reloadRange[0] + 1.0f;
            }

            GUILayout.BeginHorizontal();
            targetConditions.isReloadRandom = EditorGUILayout.ToggleLeft("Random", targetConditions.isReloadRandom);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private void DisplayConditionsBlock()
        {
            targetConditions.gazeEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Gaze", "Defines which IO to look at to fire an action. Uses the Gaze Collider zone"), targetConditions.gazeEnabled);

            if (targetConditions.gazeEnabled)
            {
                if (hierarchyGazeColliders.Count > 0)
                {
                    EditorGUILayout.BeginHorizontal();

                    // Set the state (in or out) 
                    targetConditions.gazeStateIndex = EditorGUILayout.Popup(targetConditions.gazeStateIndex, Enum.GetNames(typeof(Gaze_HoverStates)));
                    if (targetConditions.gazeStateIndex == (int)Gaze_HoverStates.IN)
                        targetConditions.gazeIn = true;
                    if (targetConditions.gazeStateIndex == (int)Gaze_HoverStates.OUT)
                        targetConditions.gazeIn = false;

                    // Set the default collider for the gaze
                    if (targetConditions.gazeColliderIO == null)
                    {
                        targetConditions.gazeColliderIO = targetConditions.GetComponentInParent<Gaze_InteractiveObject>();
                    }
                    // Enable choosing another one
                    var gazeObject = EditorGUILayout.ObjectField(targetConditions.gazeColliderIO, typeof(Gaze_InteractiveObject), true);

                    if (gazeObject != null)
                    {
                        targetConditions.gazeColliderIO = (Gaze_InteractiveObject)gazeObject;
                    }

                    EditorGUILayout.EndHorizontal();

                }
            }
            EditorGUILayout.Space();
        }

        private void DisplayHandHoverBlock()
        {
            targetConditions.handHoverEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Hand Hover", " Define which IO is hovered with the hand to fire an action. Uses the Hand Hover Collider zone."), targetConditions.handHoverEnabled);

            if (targetConditions.handHoverEnabled)
            {
                if (hierarchyHandHoverColliders.Count > 0)
                {
                    GUILayout.BeginHorizontal();

                    // Set the chosen hand
                    targetConditions.hoverHandIndex = EditorGUILayout.Popup(targetConditions.hoverHandIndex, Enum.GetNames(typeof(Gaze_HandsEnum)));
                    if (targetConditions.hoverHandIndex == ((int)Gaze_HandsEnum.LEFT))
                        targetConditions.hoverHand = UnityEngine.XR.XRNode.LeftHand;
                    if (targetConditions.hoverHandIndex == ((int)Gaze_HandsEnum.RIGHT))
                        targetConditions.hoverHand = UnityEngine.XR.XRNode.RightHand;


                    // Set the state of the hover (In or Out)
                    targetConditions.hoverStateIndex = EditorGUILayout.Popup(targetConditions.hoverStateIndex, Enum.GetNames(typeof(Gaze_HoverStates)));
                    if (targetConditions.hoverStateIndex == (int)Gaze_HoverStates.IN)
                        targetConditions.hoverIn = true;
                    if (targetConditions.hoverStateIndex == (int)Gaze_HoverStates.OUT)
                        targetConditions.hoverIn = false;

                    // Set the default IO for the Hand Hover
                    if (targetConditions.handHoverIO == null)
                        targetConditions.handHoverIO = targetConditions.GetComponentInParent<Gaze_InteractiveObject>();
                    // Enable choosing another IO
                    var handHoverObject = EditorGUILayout.ObjectField(targetConditions.handHoverIO, typeof(Gaze_InteractiveObject), true);
                    if (handHoverObject != null)
                    {
                        targetConditions.handHoverIO = (Gaze_InteractiveObject)handHoverObject;
                    }

                    GUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.Space();
        }

        private void DisplayDragAndDropCondition()
        {
            EditorGUILayout.BeginHorizontal();
            targetConditions.dragAndDropEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Drag And Drop", "Fire an action when the IO is using Drag and Drop (condition can be set either on the source object or the target)."), targetConditions.dragAndDropEnabled);
            EditorGUILayout.EndHorizontal();
            FetchDnDTargets();
            if (targetConditions.dragAndDropEnabled)
            {
                if (!targetConditions.RootIO.IsDragAndDropEnabled || targetConditions.RootIO.DnD_Targets == null || targetConditions.RootIO.DnD_Targets.Count < 1)
                {
                    EditorGUILayout.BeginHorizontal();
                    targetConditions.dndEventValidator = EditorGUILayout.Popup("Valid When", targetConditions.dndEventValidator, dndEventValidatorEnum);
                    EditorGUILayout.EndHorizontal();
                    if (!targetConditions.RootIO.IsDragAndDropEnabled)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox("Enable Drag and Drop in the Root of your (IO) for this condition to work.", MessageType.Warning);
                            EditorGUILayout.EndHorizontal();
                        }

                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    targetConditions.dndEventValidator = EditorGUILayout.Popup("Valid When", targetConditions.dndEventValidator, dndEventValidatorEnum);
                    EditorGUILayout.EndHorizontal();
                    targetConditions.dndTargetModesIndex = EditorGUILayout.Popup("On Target(s)", targetConditions.dndTargetModesIndex, dndTargetsModes);

                    if (targetConditions.dndTargetModesIndex.Equals((int)apelab_DnDTargetsModes.CUSTOM))
                        DisplayDnDTargetsChoices();
                }
            }
        }

        private void DisplayDnDTargetsChoices()
        {
            // get count of targets
            int targetsCount = targetConditions.dndTargets.Count;

            // help message if no target is specified
            if (targetsCount < 1)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("Add and choose the desired target(s)", MessageType.Warning);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                // update inputs list in Gaze_Interactions (Gaze_Interactions may have been removed in the hierarchy)
                for (int i = 0; i < targetsCount; i++)
                {
                    // display the entry
                    EditorGUILayout.BeginHorizontal();
                    int index = targetConditions.RootIO.DnD_Targets.IndexOf(targetConditions.dndTargets[i]);
                    index = Math.Max(index, 0);
                    targetConditions.dndTargets[i] = targetConditions.RootIO.DnD_Targets[EditorGUILayout.Popup(index, dndTargetsNames.ToArray())];

                    // and a '-' button to remove it if needed
                    if (GUILayout.Button("-"))
                        targetConditions.dndTargets.Remove(targetConditions.dndTargets[i]);

                    EditorGUILayout.EndHorizontal();
                }
            }

            // display 'add' button
            if (GUILayout.Button("+"))
            {
                // by default, add the first IO in hierarchy
                targetConditions.dndTargets.Add(sceneInventory.InteractiveObjects[0]);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("-"))
                {
                    targetConditions.dndTargets.Remove(sceneInventory.InteractiveObjects[0]);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
        }
    }
}