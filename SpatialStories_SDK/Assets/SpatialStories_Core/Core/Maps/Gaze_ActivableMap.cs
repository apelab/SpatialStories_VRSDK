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
using System.Collections.Generic;
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
		public List<Gaze_ActivableEntry> touchEntryList;

        /// <summary>
        /// The index of the hand's state. (LEFT, RIGHT or BOTH)
        /// </summary>
        public int touchHandsIndex;

        /// <summary>
        /// The index of touch state (PROXIMITY, DISTANT, BOTH)
        /// </summary>
        public int touchDistanceModeLeftIndex, touchDistanceModeRightIndex;

        /// <summary>
        /// The index of action (TOUCH, UNTOUCH, BOTH)
        /// </summary>
        public int touchActionLeftIndex, touchActionRightIndex;

		public Gaze_TouchMap()
		{
			touchEntryList = new List<Gaze_ActivableEntry> ();
		}

		public Gaze_ActivableEntry AddActivableEntry ()
		{
            Gaze_ActivableEntry d = new Gaze_ActivableEntry();
			touchEntryList.Add (d);
            d.hand = VRNode.LeftHand;
			return d;
        }

        public Gaze_ActivableEntry AddActivableEntry(GameObject _interactiveObject)
        {
            Gaze_ActivableEntry d = AddActivableEntry();
            d.interactiveObject = _interactiveObject;
            return d;
        }

        public bool DeleteActivableEntry (Gaze_ActivableEntry d)
		{
			return touchEntryList.Remove (d);
		}

        public void ClearActivableEntries()
        {
                touchEntryList.Clear();
        }
	}
}