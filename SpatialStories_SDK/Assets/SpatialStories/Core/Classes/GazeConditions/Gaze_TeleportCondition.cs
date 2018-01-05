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

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaze
{
    public class Gaze_TeleportCondition : Gaze_AbstractCondition
    {
        public Gaze_TeleportCondition(Gaze_Conditions _gazeConditionsScript) : base(_gazeConditionsScript) { }

        protected override void CustomSetup()
        {
            Gaze_EventManager.OnTeleportEvent += OnTeleportEvent;
        }

        protected override void CustomDispose()
        {
            Gaze_EventManager.OnTeleportEvent -= OnTeleportEvent;
        }

        public override bool IsValidated()
        {
            return IsValid;
        }

        public override void ToEditorGUI()
        {
#if UNITY_EDITOR
            EditorGUILayout.BeginHorizontal();
            if (IsValid)
            {
                RenderSatisfiedLabel("Teleport:");
                RenderSatisfiedLabel("True");
            }
            else
            {
                RenderNonSatisfiedLabel("Teleport:");
                RenderNonSatisfiedLabel("False");
            }
            EditorGUILayout.EndHorizontal();
#endif
        }

        private void ValidateGrab(Gaze_TeleportEventArgs e)
        {
            if (((int)e.Mode).Equals(gazeConditionsScript.teleportIndex))
                IsValid = true;
        }

        private void OnTeleportEvent(Gaze_TeleportEventArgs e)
        {
            ValidateGrab(e);
        }
    }
}