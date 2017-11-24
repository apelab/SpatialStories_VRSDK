// <copyright file="Gaze_GrabMap.cs" company="apelab sàrl">
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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

namespace Gaze
{
    [System.Serializable]
    public class Gaze_GrabEntry
    {
        // hand associated
        public UnityEngine.XR.XRNode hand;

        /// <summary>
        /// The associated proximity collider.
        /// </summary>
        public GameObject interactiveObject;
    }

    [System.Serializable]
    public class Gaze_GrabMap
    {

        [SerializeField]
        public List<Gaze_GrabEntry> grabEntryList;

        /// <summary>
        /// The index of the hand's state. (LEFT, RIGHT or BOTH)
        /// </summary>
        public int grabHandsIndex;

        /// <summary>
        /// The state (GRAB or UNGRAB)
        /// </summary>
        public int grabStateLeftIndex, grabStateRightIndex;

        public Gaze_GrabMap()
        {
            grabEntryList = new List<Gaze_GrabEntry>();
        }

        public Gaze_GrabEntry AddGrabableEntry()
        {
            Gaze_GrabEntry d = new Gaze_GrabEntry();
            grabEntryList.Add(d);
            d.hand = UnityEngine.XR.XRNode.LeftHand;
            return d;
        }

        public Gaze_GrabEntry AddGrabableEntry(GameObject _interactiveObject)
        {
            Gaze_GrabEntry d = AddGrabableEntry();
            d.interactiveObject = _interactiveObject;
            return d;
        }

        public bool DeleteGrabableEntry(Gaze_GrabEntry d)
        {
            return grabEntryList.Remove(d);
        }

        public void ClearGrabEntries()
        {
            grabEntryList.Clear();
        }
    }
}