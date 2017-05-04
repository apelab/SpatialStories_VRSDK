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
            targetIO.touchDistance = EditorGUILayout.FloatField("Touch Distance", targetIO.touchDistance);
            Gaze_Utils.EnsureFieldIsPositiveOrZero(ref targetIO.touchDistance);
            GUILayout.EndHorizontal();
        }

        private void DisplayGrabDistance()
        {
            if (targetIO.ManipulationMode != Gaze_ManipulationModes.GRAB)
                return;

            GUILayout.BeginHorizontal();
            targetIO.grabDistance = EditorGUILayout.FloatField("Grab Distance", targetIO.grabDistance);
            Gaze_Utils.EnsureFieldIsPositiveOrZero(ref targetIO.grabDistance);
            GUILayout.EndHorizontal();
        }

        private void DisplayLevitationDistance()
        {
            if (targetIO.ManipulationMode != Gaze_ManipulationModes.LEVITATE)
                return;

            GUILayout.BeginHorizontal();
            targetIO.grabDistance = EditorGUILayout.FloatField("Levitation Distance", targetIO.grabDistance);
            Gaze_Utils.EnsureFieldIsPositiveOrZero(ref targetIO.grabDistance);
            GUILayout.EndHorizontal();
        }
    }
}