// <copyright file="Gaze_ActivableMap.cs" company="apelab sàrl">
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
using UnityEngine.VR;

namespace Gaze
{
    [System.Serializable]
    public class Gaze_ActivableEntry
    {
        // hand associated
        public VRNode hand;

        /// <summary>
        /// The associated proximity collider.
        /// </summary>
        public GameObject interactiveObject;
    }

    [System.Serializable]
    public class Gaze_TouchMap
    {

        [SerializeField]
        public Gaze_ActivableEntry TouchEnitry;

        /// <summary>
        /// The index of the hand's state. (LEFT, RIGHT or BOTH)
        /// </summary>
        public int touchHandsIndex;

        /// <summary>
        /// The index of action (TOUCH, UNTOUCH, BOTH)
        /// </summary>
        public int touchActionIndex;

        public Gaze_TouchMap()
        {
        }

        public Gaze_ActivableEntry AddActivableEntry()
        {
            TouchEnitry = new Gaze_ActivableEntry();
            TouchEnitry.hand = VRNode.LeftHand;
            return TouchEnitry;
        }

        public Gaze_ActivableEntry AddActivableEntry(GameObject _interactiveObject)
        {
            Gaze_ActivableEntry d = AddActivableEntry();
            d.interactiveObject = _interactiveObject;
            return d;
        }

        public bool DeleteActivableEntry(Gaze_ActivableEntry d)
        {
            if (TouchEnitry == null)
                return false;
            else
            {
                TouchEnitry = null;
            }
            return true;
        }

        public void ClearActivableEntries()
        {
            TouchEnitry = null;
        }
    }
}