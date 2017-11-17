// <copyright file="Gaze_InputsMap.cs" company="apelab sàrl">
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
    public class Gaze_InputsMap
    {
        [SerializeField]
        public List<Gaze_InputsMapEntry> InputsEntries;

        public bool AreDependenciesSatisfied;

        public Gaze_InputsMap()
        {
            InputsEntries = new List<Gaze_InputsMapEntry>();
        }

        public bool Delete(Gaze_InputsMapEntry d)
        {
            if (InputsEntries.Contains(d))
                // Destroy the dependency from the list
                return InputsEntries.Remove(d);

            return false;
        }

        public Gaze_InputsMapEntry Add()
        {
            Gaze_InputsMapEntry d = new Gaze_InputsMapEntry();
            InputsEntries.Add(d);

            return d;
        }

        public bool IsEmpty()
        {
            return InputsEntries.Count == 0;
        }

        public void Reset(bool _reloadDependencies)
        {
            if (_reloadDependencies)
                AreDependenciesSatisfied = false;
        }
    }
}