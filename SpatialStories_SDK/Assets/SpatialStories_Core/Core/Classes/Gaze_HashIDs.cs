// <copyright file="Gaze_HashIDs.cs" company="apelab sàrl">
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
using UnityEngine;

namespace Gaze
{
    public static class Gaze_HashIDs
    {
        public static float CONDITIONS_UPDATE_INTERVAL = .05f;

        public static string[] DependencyTriggerEventsAndStates = {
            "Before",
            "Active",
            "After",
            "Triggered"
        };

        public static string[] TriggerEventsAndStates = {
            "OnTrigger",
            "OnReload",
            "OnBefore",
            "OnActive",
            "OnAfter"
        };

        public static string[] ProximityEventsAndStates = {
            "OnEnter",
            "OnExit"
        };

        public static int ANIMATOR_PARAMETER_HANDCLOSED = Animator.StringToHash("handClosed");
        public static float THROW_SPEED = 2f;
        public static string LAYER_SOLID = "Custom Collision";
    }
}