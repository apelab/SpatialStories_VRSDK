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
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Gaze
{
    [InitializeOnLoad]
    [CustomEditor(typeof(Gaze_InteractiveObject))]
    public class Gaze_InteractiveObjectEditor : Editor
    {
        #region Members
        private Gaze_InteractiveObject targetIO;
        private GameObject targetIORoot;

        /// <summary>
        /// Can this object be grabbed.
        /// </summary>
        public bool collidersFollowIO = false;

        /// <summary>
        /// It's the collider used to catch the object.
        /// </summary>
        //public GameObject catchableHandle;

        private bool hasGrabPositionnerProperty;
        private GameObject positionnerGameObject;

        /// <summary>
        /// If true, the object being catched will vibrate the controllers while grabbed.
        /// </summary>
        public bool vibratesOnGrab = false;

        /// <summary>
        /// Is this catchable object using gravity
        /// </summary>
        public bool hasGravity;

        /// <summary>
        /// Defines if an object can be grabbed from no matter where once is grabbed
        /// </summary>
        public bool isManupulable;

        // Reflection members
        private List<Collider> touchables;
        private List<string> touchablesNames;

        // Reflection members
        private List<Collider> grabables;
        private List<string> grabablesNames;

        // logo image
        private Texture logo;
        private Rect logoRect;

        private string[] grabModes;
        #endregion

        void OnEnable()
        {
            InitMembers();
        }

        private void InitMembers()
        {
            targetIO = (Gaze_InteractiveObject)target;
            touchables = new List<Collider>();
            touchablesNames = new List<String>();
            grabables = new List<Collider>();
            grabablesNames = new List<String>();
            targetIORoot = ((Gaze_InteractiveObject)target).gameObject;
            grabModes = Enum.GetNames(typeof(Gaze_GrabMode));
            logo = (Texture)Resources.Load("SpatialStorires_Logo_256", typeof(Texture));
            logoRect = new Rect();
            logoRect.x = 10;
            logoRect.y = 10;
        }

        public override void OnInspectorGUI()
        {
            DisplayLogo();
            DisplayTouchDistance();
            DisplayGrabDistance();
            DisplayLevitationDistance();
            DisplayGrabMode();
        }

        private void DisplayLogo()
        {
            GUILayout.BeginHorizontal();
            GUI.Label(logoRect, logo);
            GUILayout.Label(logo);
            GUILayout.EndHorizontal();
        }

        private void DisplayTouchDistance()
        {
            GUILayout.BeginHorizontal();
            targetIO.touchDistance = EditorGUILayout.FloatField("Touch Distance", targetIO.touchDistance);
            GUILayout.EndHorizontal();
        }

        private void DisplayGrabDistance()
        {
            GUILayout.BeginHorizontal();
            targetIO.grabDistance = EditorGUILayout.FloatField("Grab Distance", targetIO.grabDistance);
            GUILayout.EndHorizontal();
        }

        private void DisplayLevitationDistance()
        {
            GUILayout.BeginHorizontal();
            targetIO.levitateDistance = EditorGUILayout.FloatField("Levitation Distance", targetIO.levitateDistance);
            GUILayout.EndHorizontal();
        }

        //TODO @apelab display grab mode (ATTRACT, LEVITATE)
        private void DisplayGrabMode()
        {
            EditorGUILayout.BeginHorizontal();
            targetIO.grabModeIndex = EditorGUILayout.Popup("Grab Mode", targetIO.grabModeIndex, grabModes);
            EditorGUILayout.EndHorizontal();
        }
    }
}