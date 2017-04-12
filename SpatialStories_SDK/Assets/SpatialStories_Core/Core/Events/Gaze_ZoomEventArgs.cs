// <copyright file="Gaze_ZoomEventArgs.cs" company="apelab sàrl">
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
using System;

namespace Gaze
{
	public class Gaze_ZoomEventArgs : EventArgs
	{
		public object Sender { get; private set; }

		public Collider Collider { get; private set; }

		public float FovFactor { get; private set; }

		public float ZoomSpeedFactor { get; private set; }

		public Gaze_DezoomMode DezoomMode { get; private set; }

		public float DezoomSpeedFactor { get; private set; }

		public AnimationCurve ZoomCurve { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Gaze_ZoomEventArgs"/> class.
		/// </summary>
		/// <param name="_sender">The GameObject that fires the event.</param>
		public Gaze_ZoomEventArgs (object _sender, Collider _collider, float _fovFactor = 1, float _zoomSpeedFactor = 1, Gaze_DezoomMode _dezoomMode = Gaze_DezoomMode.SAME, float _dezoomSpeedFactor = 1, AnimationCurve _zoomCurve = null)
		{
			Sender = _sender;
			Collider = _collider;
			FovFactor = _fovFactor;
			ZoomSpeedFactor = _zoomSpeedFactor;
			DezoomMode = _dezoomMode;
			DezoomSpeedFactor = _dezoomSpeedFactor;
			ZoomCurve = _zoomCurve;
		}
	}
}
