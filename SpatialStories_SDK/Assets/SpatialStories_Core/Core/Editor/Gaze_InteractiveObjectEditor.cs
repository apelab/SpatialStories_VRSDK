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
using UnityEditor;
using UnityEngine;

namespace Gaze
{
    [InitializeOnLoad]
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Gaze_InteractiveObject))]
    public class Gaze_InteractiveObjectEditor : Gaze_Editor
    {
        #region Members
        private Gaze_InteractiveObject gaze_InteractiveObjectScript;

        // logo image
        private Texture logo;
        private Rect logoRect;
        private string[] manipulationModes;
        public List<string> Dnd_DropTargetsNames { get { return dnd_dropTargetsNames; } private set { } }
        private List<string> dnd_dropTargetsNames;
        public int dnd_targetsToGenerate;
        private Material dnd_targetMaterial;
        #endregion

        void OnEnable()
        {
            InitMembers();
        }

        private void InitMembers()
        {
            gaze_InteractiveObjectScript = (Gaze_InteractiveObject)target;

            manipulationModes = Enum.GetNames(typeof(Gaze_ManipulationModes));

            logo = (Texture)Resources.Load("SpatialStorires_Logo_256", typeof(Texture));
            logoRect = new Rect();
            logoRect.x = 4;
            logoRect.y = 4;
            logoRect.width = 200;
            dnd_dropTargetsNames = new List<string>();
            dnd_targetMaterial = Resources.Load("DnD_TargetMaterial", typeof(Material)) as Material;

        }

        public override void Gaze_OnInspectorGUI()
        {
            base.BeginChangeComparision();

            UpdateDropTargetsNames();
            EditorGUILayout.Space();
            DisplayLogo();
            EditorGUILayout.Space();
            Gaze_EditorUtils.DrawSectionTitle("Interactive Object Setup");
            EditorGUILayout.Space();
            DisplayManipulationMode();
            DisplayTouchDistance();
            DisplayGrabDistance();
            DisplayLevitationDistance();
            DisplayDragAndDrop();

            base.EndChangeComparision();
            EditorUtility.SetDirty(gaze_InteractiveObjectScript);
        }

        private void UpdateDropTargetsNames()
        {
            dnd_dropTargetsNames.Clear();

            // rebuild them
            if (Gaze_SceneInventory.Instance != null)
            {
                for (int i = 0; i < Gaze_SceneInventory.Instance.InteractiveObjectsCount; i++)
                {
                    if (Gaze_SceneInventory.Instance.InteractiveObjects[i] != null)
                        dnd_dropTargetsNames.Add(Gaze_SceneInventory.Instance.InteractiveObjects[i].gameObject.name);
                }
            }
        }

        private void DisplayLogo()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(logo);
            GUILayout.EndHorizontal();
            GUILayout.Label("V.0.2b6", EditorStyles.boldLabel);
        }
        
        private void DisplayManipulationMode()
        {
            gaze_InteractiveObjectScript.ManipulationModeIndex = Gaze_EditorUtils.Gaze_HintPopup("Manipulation Modes", gaze_InteractiveObjectScript.ManipulationModeIndex, manipulationModes, "Define if your IO should be grabbed or levitable, or touched", 116);
        }

        private void DisplayTouchDistance()
        {
            if (gaze_InteractiveObjectScript.ManipulationMode != Gaze_ManipulationModes.TOUCH)
                return;

            GUILayout.BeginHorizontal();
            gaze_InteractiveObjectScript.TouchDistance = EditorGUILayout.FloatField(new GUIContent("Touch Distance"), gaze_InteractiveObjectScript.TouchDistance);
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
            GUILayout.BeginHorizontal();
            gaze_InteractiveObjectScript.SnapOnGrab = EditorGUILayout.Toggle(new GUIContent("Snap On Grab", "Make the IO snap into a specific position. Uses the Snap Left or Snap Right positions"), gaze_InteractiveObjectScript.SnapOnGrab);
            if (gaze_InteractiveObjectScript.SnapOnGrab)
            {
                gaze_InteractiveObjectScript.IsManipulable = EditorGUILayout.Toggle("Is Manipulable", gaze_InteractiveObjectScript.IsManipulable);
            }
            else
            {
                gaze_InteractiveObjectScript.IsManipulable = true;
            }
            GUILayout.EndHorizontal();
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
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Drag And Drop", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            gaze_InteractiveObjectScript.IsDragAndDropEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Enable Drag And Drop", "Enable Drag and Drop if you want your IO to be snapped in a particular target."), gaze_InteractiveObjectScript.IsDragAndDropEnabled);
            EditorGUILayout.EndHorizontal();

            if (gaze_InteractiveObjectScript.IsDragAndDropEnabled)
            {
                DisplayTargets();

                DisplayAxisConstraints();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                gaze_InteractiveObjectScript.DnD_minDistance = EditorGUILayout.FloatField("Min Distance To Validate", gaze_InteractiveObjectScript.DnD_minDistance);
                GUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal();
                gaze_InteractiveObjectScript.DnD_attached = EditorGUILayout.ToggleLeft(new GUIContent("Attached", "The IO can not be taken off its target after being released"), gaze_InteractiveObjectScript.DnD_attached);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                gaze_InteractiveObjectScript.DnD_SnapOnDrop = EditorGUILayout.ToggleLeft(new GUIContent("Snap On Drop", "The IO will snap in place when released"), gaze_InteractiveObjectScript.DnD_SnapOnDrop);
                EditorGUILayout.EndHorizontal();

                if (gaze_InteractiveObjectScript.DnD_SnapOnDrop)
                {
                    EditorGUILayout.BeginHorizontal();
                    gaze_InteractiveObjectScript.DnD_snapBeforeDrop = EditorGUILayout.ToggleLeft(new GUIContent("Snap Before Drop", "The IO will snap in place if already in the right position (before releasing it)"), gaze_InteractiveObjectScript.DnD_snapBeforeDrop);
                    EditorGUILayout.EndHorizontal();
                }
                else
                    gaze_InteractiveObjectScript.DnD_snapBeforeDrop = false;



                EditorGUILayout.BeginHorizontal();
                gaze_InteractiveObjectScript.DnD_TimeToSnap = EditorGUILayout.FloatField(new GUIContent("Time To Snap", "The time it takes for the object to snap"), gaze_InteractiveObjectScript.DnD_TimeToSnap);
                EditorGUILayout.EndHorizontal();

                DisplayTargetGenerator();
            }
        }

        private void DisplayTargetGenerator()
        {
            EditorGUILayout.BeginHorizontal();
            dnd_targetsToGenerate = EditorGUILayout.IntField(new GUIContent("Generate Targets", "Generates a pre-configured target with a blue transparent visual."), dnd_targetsToGenerate);
            if (GUILayout.Button("GO"))
            {
                GenerateDnDTargets();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DisplayAxisConstraints()
        {
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

            if (gaze_InteractiveObjectScript.DnD_respectXAxis || gaze_InteractiveObjectScript.DnD_respectYAxis || gaze_InteractiveObjectScript.DnD_respectZAxis)
            {
                gaze_InteractiveObjectScript.DnD_angleThreshold = EditorGUILayout.Slider("Angle Threshold", gaze_InteractiveObjectScript.DnD_angleThreshold, 1, 100);
            }
        }

        private void DisplayTargets()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Drop Targets", " Use existing IO’s in the scene or create an automatic target below for your IO."));
            EditorGUILayout.EndHorizontal();

            // help message if no target is specified
            if (gaze_InteractiveObjectScript.DnD_Targets == null || gaze_InteractiveObjectScript.DnD_Targets.Count < 1)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("Add at least one drop target or deactivate this condition if not needed.", MessageType.Warning);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                // for each DnD target
                for (int i = 0; i < gaze_InteractiveObjectScript.DnD_Targets.Count; i++)
                {
                    if (gaze_InteractiveObjectScript.DnD_Targets[i] == null)
                    {
                        gaze_InteractiveObjectScript.DnD_Targets.RemoveAt(i);
                    }
                    else
                    {
                        // refresh DnD_Targets IOs list modification (an IO target may has been destroyed)
                        if (Gaze_SceneInventory.Instance.InteractiveObjects.Contains(gaze_InteractiveObjectScript.DnD_Targets[i]))
                        {
                            // display it in a popup
                            EditorGUILayout.BeginHorizontal();

                            gaze_InteractiveObjectScript.DnD_Targets[i] = Gaze_SceneInventory.Instance.InteractiveObjects[EditorGUILayout.Popup(Gaze_SceneInventory.Instance.InteractiveObjects.IndexOf(gaze_InteractiveObjectScript.DnD_Targets[i]), dnd_dropTargetsNames.ToArray())];

                            // and a '-' button to remove it if needed
                            if (GUILayout.Button("-"))
                                gaze_InteractiveObjectScript.DnD_Targets.Remove(gaze_InteractiveObjectScript.DnD_Targets[i]);

                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }
            }

            // display 'add' button
            if (GUILayout.Button("+"))
            {
                // exit if there are no Interactive Object in the scene
                if (Gaze_SceneInventory.Instance.InteractiveObjectsCount < 1)
                    return;

                // add the first Interactive Object by default
                // TODO @mike add only if doesn't exist already !
                gaze_InteractiveObjectScript.DnD_Targets.Add(Gaze_SceneInventory.Instance.InteractiveObjects[0]);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("-"))
                {
                    //targetConditions.InputsMap.Delete(d);
                    gaze_InteractiveObjectScript.DnD_Targets.Remove(Gaze_SceneInventory.Instance.InteractiveObjects[0]);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
        }

        private void GenerateDnDTargets()
        {
            GameObject instance;
            Gaze_InteractiveObject gaze_InteractiveObject;
            for (int i = 0; i < dnd_targetsToGenerate; i++)
            {
                // instantiate a target
                instance = Instantiate(gaze_InteractiveObjectScript.gameObject);

                // add suffix to its name
                instance.name = gaze_InteractiveObjectScript.gameObject.name + " (DnD Target " + i + ")";

                // get the visuals of Drop object
                Gaze_InteractiveObjectVisuals visualsRoot = instance.GetComponentInChildren<Gaze_InteractiveObjectVisuals>();
                Renderer[] visualsChildren = visualsRoot.gameObject.GetComponentsInChildren<Renderer>();

                // for every visual
                for (int k = 0; k < visualsChildren.Length; k++)
                {
                    // assign ghost material for each generated target
                    visualsChildren[k].material = dnd_targetMaterial;

                    // set all colliders in this visual gameobject to isTrigger
                    Collider[] collider = visualsChildren[k].gameObject.GetComponents<Collider>();
                    if (collider != null && collider.Length > 0)
                    {
                        for (int l = 0; l < collider.Length; l++)
                        {
                            collider[l].isTrigger = true;
                        }
                    }
                }

                // get the InteractiveObject script
                gaze_InteractiveObject = instance.GetComponent<Gaze_InteractiveObject>();

                //gaze_InteractiveObject.DnD_Targets.Clear();

                // change manipulation mode to NONE
                gaze_InteractiveObject.ManipulationModeIndex = (int)Gaze_ManipulationModes.NONE;

                // deactivate DnD
                gaze_InteractiveObject.IsDragAndDropEnabled = false;

                // change gravity to none and kinematic to true
                instance.GetComponent<Rigidbody>().useGravity = false;
                instance.GetComponent<Rigidbody>().isKinematic = true;

                // add the generated targets in the list of targets for this drop object
                gaze_InteractiveObjectScript.DnD_Targets.Add(instance);

                // add an interaction for hiding the ghost when drop object is snapped
                instance.GetComponent<Gaze_InteractiveObject>().AddInteraction();
            }
        }
    }
}