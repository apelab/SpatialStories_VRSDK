//-----------------------------------------------------------------------
// <copyright file="LevitationEventArgs.cs" company="apelab sàrl">
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
// <author>Michaël Martin</author>
// <email>dev@apelab.ch</email>
// <web>https://twitter.com/apelab_ch</web>
// <web>http://www.apelab.ch</web>
// <date>2016-01-25</date>
// </copyright>
//-----------------------------------------------------------------------
using UnityEditor;
using System;

namespace Gaze
{
    [InitializeOnLoad]
    [CustomEditor(typeof(Gaze_GrabCondition))]
    public class Gaze_GrabConditionsEditor : Editor
    {
        private Gaze_GrabCondition targetScript;

        void OnEnable()
        {
            targetScript = (Gaze_GrabCondition)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Activate on");
            targetScript.grabActionIndex = EditorGUILayout.Popup(targetScript.grabActionIndex, Enum.GetNames(typeof(Gaze_GrabActionValues)));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            targetScript.reload = EditorGUILayout.ToggleLeft("Reload", targetScript.reload);
            EditorGUILayout.EndHorizontal();
        }
    }
}