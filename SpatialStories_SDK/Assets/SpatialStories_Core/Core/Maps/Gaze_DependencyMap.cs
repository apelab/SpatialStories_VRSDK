// <copyright file="Gaze_DependencyMap.cs" company="apelab sàrl">
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

    [System.Serializable]
    public class Gaze_DependencyMap
    {
        [SerializeField]
        public List<Gaze_Dependency> dependencies;


        public bool AreDependenciesSatisfied;

        public Gaze_DependencyMap()
        {
            dependencies = new List<Gaze_Dependency>();
        }

        public Gaze_Dependency Get(GameObject o)
        {
            foreach (Gaze_Dependency d in dependencies)
            {
                try
                {
                    if (d.dependentGameObject.Equals(o))
                    {
                        return d;
                    }
                }
                catch (NullReferenceException ex)
                {
                    continue;
                }
            }
            return null;
        }

        public bool Delete(Gaze_Dependency d)
        {
            if (dependencies.Contains(d))
            {
                // Destroy the dependency from the list
                d.Dispose();
                return dependencies.Remove(d);
            }
            return false;
        }

        public Gaze_Dependency Add(Gaze_Conditions conditions)
        {
            Gaze_Dependency d = new Gaze_Dependency(conditions);
            dependencies.Add(d);
            return d;
        }

        public bool isEmpty()
        {
            return dependencies.Count == 0;
        }

        public void OnEnable(Gaze_Conditions conditions)
        {
            conditions.OnReload += Reset;
        }

        public void OnDisable(Gaze_Conditions conditions)
        {
            conditions.OnReload -= Reset;
        }

        public void Reset(bool _reloadDependencies)
        {
            if (_reloadDependencies)
                AreDependenciesSatisfied = false;
        }
    }
}