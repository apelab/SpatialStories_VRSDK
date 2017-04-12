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
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    [System.Serializable]
    public class Gaze_ProximityEntry
    {

        /// <summary>
        /// The associated proximity collider.
        /// </summary>
        public GameObject dependentGameObject;

        // list of all colliding objects
        private List<GameObject> collidingObjects;

        public List<GameObject> CollidingObjects { get { return collidingObjects; } }

        public Gaze_ProximityEntry()
        {
            collidingObjects = new List<GameObject>();
        }

        public void AddCollidingObject(GameObject g)
        {
            if (!collidingObjects.Contains(g))
                collidingObjects.Add(g);
        }

        public void RemoveCollidingObject(GameObject g)
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
            Debug.Log("Entry " + dependentGameObject.transform.parent.name + " is colliding with :");
            foreach (GameObject item in collidingObjects)
            {
                Debug.Log(item.name);
            }
        }
    }




    [System.Serializable]
    public class Gaze_ProximityMap
    {

        [SerializeField]
        public List<Gaze_ProximityEntry> proximityEntryList;

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
        }

        public Gaze_ProximityEntry AddProximityEntry()
        {
            Gaze_ProximityEntry d = new Gaze_ProximityEntry();
            proximityEntryList.Add(d);
            return d;
        }

        public bool DeleteProximityEntry(Gaze_ProximityEntry d)
        {
            return proximityEntryList.Remove(d);
        }

        public void AddCollidingObjectToEntry(Gaze_ProximityEntry _entry, GameObject _collidingObject, bool displayCollidingObjects = false)
        {
            _entry.AddCollidingObject(_collidingObject);
            if (displayCollidingObjects)
                _entry.DisplayCollidingObjects();
        }

        public void RemoveCollidingObjectToEntry(Gaze_ProximityEntry _entry, GameObject _collidingObject, bool displayCollidingObjects = false)
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
            isEveryoneColliding = true;
        }

        public void ResetEveryoneColliding()
        {
            isEveryoneColliding = false;
        }

        public void UpdateEveryoneColliding_old()
        {
            isEveryoneColliding = false;
            foreach (Gaze_ProximityEntry p in proximityEntryList)
            {
                if (p.CollidingObjects.Count < 1)
                {
                    return;
                }
            }
            isEveryoneColliding = true;
        }

        /// <summary>
        /// Determines whether all the proximity entries are valid.
        /// </summary>
        /// <returns><c>true</c> if this instance is valid; otherwise, <c>false</c>.</returns>
        public bool IsProximityListValid()
        {
            foreach (Gaze_ProximityEntry p in proximityEntryList)
            {
                if (!p.IsValid(proximityStateIndex))
                    return false;
            }

            return true;
        }

        public void DisplayAllCollidingObjects()
        {
            foreach (Gaze_ProximityEntry item in proximityEntryList)
            {
                item.DisplayCollidingObjects();
            }
        }

        public bool ContainsEntry(GameObject _entryGameObject)
        {
            for (int i = 0; i < proximityEntryList.Count; i++)
            {
                if (proximityEntryList[i].dependentGameObject.Equals(_entryGameObject))
                {
                    return true;
                }
            }
            return false;
        }
    }
}