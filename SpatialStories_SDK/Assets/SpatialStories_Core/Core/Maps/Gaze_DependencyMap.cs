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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gaze
{
	[System.Serializable]
	public class Gaze_Dependency
	{
		/// <summary>
		/// The dependent game object.
		/// </summary>
		public GameObject dependentGameObject;

		/// <summary>
		/// The index of the trigger state.
		/// </summary>
		public int triggerStateIndex;

		/// <summary>
		/// TRUE if dependent on Trigger
		/// </summary>
		public bool onTrigger;

		/// <summary>
		/// TRUE once satisfied
		/// </summary>
		public bool satisfied;
	}

	[System.Serializable]
	public class Gaze_DependencyMap
	{
	
		[SerializeField]
		public List<Gaze_Dependency> dependencies;

		public Gaze_DependencyMap ()
		{
			dependencies = new List<Gaze_Dependency> ();
		}

		public Gaze_Dependency Get (GameObject o)
		{
			foreach (Gaze_Dependency d in dependencies) {
				if (d.dependentGameObject.Equals (o)) {
					return d;
				}
			}
			return null;
		}

		public bool Delete (Gaze_Dependency d)
		{
			if (dependencies.Contains (d)) {
				return dependencies.Remove (d);
			}
			return false;
		}

		public Gaze_Dependency Add ()
		{
			Gaze_Dependency d = new Gaze_Dependency ();
			dependencies.Add (d);
			return d;
		}

		public bool isEmpty ()
		{
			return dependencies.Count == 0;
		}
	}
}