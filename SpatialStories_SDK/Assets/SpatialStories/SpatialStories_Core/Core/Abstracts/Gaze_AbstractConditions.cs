// <copyright file="Gaze_AbstractBehaviour.cs" company="apelab sàrl">
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
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Gaze
{
    public abstract class Gaze_AbstractConditions : MonoBehaviour
    {
#if UNITY_EDITOR
        [HideInInspector]
#endif
        public bool IsValid = false;

        public virtual void ValidateCustomCondition(bool _conditionValidated)
        {
            IsValid = _conditionValidated;
            Gaze_EventManager.FireCustomConditionEvent(new Gaze_CustomConditionEventArgs(this.GetInstanceID(), _conditionValidated));
        }

#if UNITY_EDITOR
        public void ToGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (IsValid)
            {
                Gaze_AbstractCondition.RenderSatisfiedLabel("Custom Condition:");
                Gaze_AbstractCondition.RenderSatisfiedLabel("Valid");
            }
            else
            {
                Gaze_AbstractCondition.RenderNonSatisfiedLabel("Custom Condition:");
                Gaze_AbstractCondition.RenderNonSatisfiedLabel("Not Valid");
            }
            EditorGUILayout.EndHorizontal();
        }
#endif
    }
}