// <copyright file="Gaze_ProximityMap.cs" company="apelab sàrl">
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
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    [Serializable]
    public class Gaze_ProximityEntry
    {
        /// <summary>
        /// The associated proximity collider.
        /// </summary>
        public Gaze_InteractiveObject dependentGameObject;

        // list of all colliding objects
        private List<Gaze_InteractiveObject> collidingObjects;

        public List<Gaze_InteractiveObject> CollidingObjects
        {
            get
            {
                if (collidingObjects == null)
                {
                    collidingObjects = new List<Gaze_InteractiveObject>();
                }
                return collidingObjects;
            }
        }

        public Gaze_ProximityEntry()
        {
            collidingObjects = new List<Gaze_InteractiveObject>();
        }

        public void AddCollidingObject(Gaze_InteractiveObject g)
        {
            if (!collidingObjects.Contains(g))
            {
                collidingObjects.Add(g);
            }
        }

        public void RemoveCollidingObject(Gaze_InteractiveObject g)
        {
            collidingObjects.Remove(g);
        }

        private bool IsColliding()
        {
            return collidingObjects.Count > 0 ? true : false;
        }

        public bool IsValid(int _colliderStateIndex)
        {

            if (_colliderStateIndex.Equals((int)Gaze_ProximityStates.ENTER) && IsColliding())
                return true;
            else if (_colliderStateIndex.Equals((int)Gaze_ProximityStates.EXIT) && !IsColliding())
                return true;

            return false;
        }

        public void DisplayCollidingObjects()
        {
            if (collidingObjects == null)
                return;
            Debug.Log("Entry " + dependentGameObject + " is colliding with :");
            foreach (Gaze_InteractiveObject item in collidingObjects)
            {
                Debug.Log(item.name);
            }
        }
    }

    [System.Serializable]
    public class Gaze_ProximityEntryGroup
    {
        [SerializeField]
        public List<Gaze_ProximityEntry> proximityEntries = new List<Gaze_ProximityEntry>();

        public void AddProximityEntryToGroup(Gaze_InteractiveObject dependentObject)
        {
            Gaze_ProximityEntry p = new Gaze_ProximityEntry();
            p.dependentGameObject = dependentObject;
            proximityEntries.Add(p);
        }


    }

    [System.Serializable]
    public class Gaze_ProximityMap
    {
        public List<Gaze_ProximityEntry> proximityEntryList;
        public List<Gaze_ProximityEntryGroup> proximityEntryGroupList;

        public int NumValidCollidingObjects { get { return GetValidatedEntriesCount(); } }

        /// <summary>
        /// The index of the collider state. (OnEnter or OnExit)
        /// </summary>
        public int proximityStateIndex;

        /// <summary>
        /// Are all the interactive objects inside the proximity ?
        /// Used if OnExit & RequireAll specified because we need to know first that they all were inside at the same time.
        /// </summary>
        private bool isEveryoneColliding = false;

        public bool IsEveryoneColliding { get { return isEveryoneColliding; } }

        public Gaze_ProximityMap()
        {
            proximityEntryList = new List<Gaze_ProximityEntry>();
            proximityEntryGroupList = new List<Gaze_ProximityEntryGroup>();
        }

        public Gaze_ProximityEntry AddProximityEntry(Gaze_Conditions _conditions)
        {
            Gaze_ProximityEntry d = new Gaze_ProximityEntry();
            proximityEntryList.Add(d);
            return d;
        }

        public Gaze_ProximityEntryGroup AddProximityEntryGroup(Gaze_Conditions _conditions)
        {
            Gaze_ProximityEntryGroup d = new Gaze_ProximityEntryGroup();
            proximityEntryGroupList.Add(d);
            return d;
        }

        public bool DeleteProximityEntry(Gaze_ProximityEntry d)
        {
            return proximityEntryList.Remove(d);
        }

        public bool DeleteProximityEntryGroup(Gaze_ProximityEntryGroup d)
        {
            return proximityEntryGroupList.Remove(d);
        }

        public void AddCollidingObjectToEntry(Gaze_ProximityEntry _entry, Gaze_InteractiveObject _collidingObject, bool displayCollidingObjects = false)
        {
            _entry.AddCollidingObject(_collidingObject);
            if (displayCollidingObjects)
                _entry.DisplayCollidingObjects();
        }

        public void RemoveCollidingObjectToEntry(Gaze_ProximityEntry _entry, Gaze_InteractiveObject _collidingObject, bool displayCollidingObjects = false)
        {
            _entry.RemoveCollidingObject(_collidingObject);
            if (displayCollidingObjects)
                _entry.DisplayCollidingObjects();
        }

        /// <summary>
        /// Returns the number of validated entries.
        /// </summary>
        /// <returns>The validated entries count.</returns>
        public int GetValidatedEntriesCount()
        {
            int count = 0;
            foreach (Gaze_ProximityEntry p in proximityEntryList)
            {
                //				Debug.Log (p.dependentGameObject.transform.parent + " IsValid (" + proximityStateIndex + ") = " + p.IsValid (proximityStateIndex));
                if (p.IsValid(proximityStateIndex))
                    count++;
            }

            foreach (Gaze_ProximityEntryGroup g in proximityEntryGroupList)
            {
                foreach (Gaze_ProximityEntry p in g.proximityEntries)
                {
                    if (p.IsValid(proximityStateIndex))
                    {
                        count++;
                        break;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Determines whether every Interactive Object in the list is colliding
        /// </summary>
        /// <returns><c>true</c> if everyone is colliding; otherwise, <c>false</c>.</returns>
        public void UpdateEveryoneColliding()
        {
            foreach (Gaze_ProximityEntry p in proximityEntryList)
            {
                if (p.CollidingObjects.Count < 1)
                {
                    return;
                }
            }
            foreach (Gaze_ProximityEntryGroup g in proximityEntryGroupList)
            {
                foreach (Gaze_ProximityEntry p in g.proximityEntries)
                {
                    if (p.CollidingObjects.Count < 1)
                    {
                        return;
                    }
                }
            }
            isEveryoneColliding = true;
        }

        public void ResetEveryoneColliding()
        {
            isEveryoneColliding = false;
        }
    }
}