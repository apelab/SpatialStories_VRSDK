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

            logo = (Texture)Resources.Load("SpatialStorires_Logo_256", typeof(Texture));
            logoRect = new Rect();
            logoRect.x = 10;
            logoRect.y = 10;
            dnd_dropTargetsNames = new List<string>();
            dnd_targetMaterial = Resources.Load("DnD_TargetMaterial", typeof(Material)) as Material;
        }

        public override void Gaze_OnInspectorGUI()
        {
            base.BeginChangeComparision();

            UpdateDropTargetsNames();
            DisplayLogo();

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
            GUI.Label(logoRect, logo);
            GUILayout.Label(logo);
            GUILayout.EndHorizontal();
        }
    }
}