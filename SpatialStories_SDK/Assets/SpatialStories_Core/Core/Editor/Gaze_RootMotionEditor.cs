// <copyright file="Gaze_RootMotionEditor.cs" company="apelab sàrl">
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

namespace Gaze
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Gaze_RootMotion))]
    public class Gaze_RootMotionEditor : Gaze_Editor
    {
        public GameObject rootMotionTarget;
        private Gaze_RootMotion rootMotionScript;


        void OnEnable()
        {
            rootMotionScript = (Gaze_RootMotion)target;
        }

        public override void Gaze_OnInspectorGUI()
        {
            // toggle button
            EditorGUILayout.LabelField("The gazable area will follow");
            rootMotionScript.rootTarget = EditorGUILayout.ObjectField(rootMotionScript.rootTarget, typeof(GameObject), true) as GameObject;

            // save changes
            EditorUtility.SetDirty(rootMotionScript);
        }
    }
}