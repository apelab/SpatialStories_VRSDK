// <copyright file="Gaze_ProximityEditor.cs" company="apelab sàrl">
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
using UnityEditor.AnimatedValues;
using System;

namespace Gaze
{
	[CustomEditor (typeof(Gaze_Proximity))]
	[CanEditMultipleObjects]
	public class Gaze_ProximityEditor : Gaze_Editor
    {
		private Gaze_Proximity proximityScript;

		void OnEnable ()
		{
			proximityScript = (Gaze_Proximity)target;
		}

        public override void Gaze_OnInspectorGUI()
        {
            // debug option
            DrawDefaultInspector();

            // save changes
            EditorUtility.SetDirty(proximityScript);
        }
    }
}