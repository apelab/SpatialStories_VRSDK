// <copyright file="Gaze_AbstractGazeListener.cs" company="apelab sàrl">
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

namespace Gaze
{
	public abstract class Gaze_AbstractGazeListener : MonoBehaviour
	{
		void OnEnable ()
		{
			Gaze_EventManager.OnGazeEvent += onGazeEvent;
			Gaze_EventManager.OnTriggerStateEvent += onTriggerStateEvent;
			Gaze_EventManager.OnTriggerEvent += onTriggerEvent;
			Gaze_EventManager.OnProximityEvent += onProximityEvent;
		}

		void OnDisable ()
		{
			Gaze_EventManager.OnGazeEvent -= onGazeEvent;
			Gaze_EventManager.OnTriggerStateEvent -= onTriggerStateEvent;
			Gaze_EventManager.OnTriggerEvent -= onTriggerEvent;
			Gaze_EventManager.OnProximityEvent -= onProximityEvent;
		}

		/// <summary>
		/// Method called when a gaze events occur.
		/// </summary>
		/// <param name="e">event arguments</param>
		protected abstract void onGazeEvent (Gaze_GazeEventArgs e);

		/// <summary>
		/// Method called when a trigger state events occur.
		/// </summary>
		/// <param name="e">event arguments</param>
		protected abstract void onTriggerStateEvent (Gaze_TriggerStateEventArgs e);

		/// <summary>
		/// Method called when a trigger events occur.
		/// </summary>
		/// <param name="e">event arguments</param>
		protected abstract void onTriggerEvent (Gaze_TriggerEventArgs e);

		/// <summary>
		/// Method called when a proximity events occur.
		/// </summary>
		/// <param name="e">event arguments</param>
		protected abstract void onProximityEvent (Gaze_ProximityEventArgs e);
	}
}
