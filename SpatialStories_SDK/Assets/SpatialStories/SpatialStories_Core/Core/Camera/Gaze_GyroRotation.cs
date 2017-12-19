// <copyright file="Gaze_GyroRotation.cs" company="apelab sàrl">
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
	/// <summary>
	/// this script rotates the object according to the gyroscope and respects the Unity world Y
	/// axis when the app launches: the object look direction is -x if the app is in landscape
	/// mode and +z if the app is in portrait mode.
	/// </summary>
	public class Gaze_GyroRotation : MonoBehaviour
	{
		private float startingYRotation;
		private bool sober = false;
		private float drunkenRotAmount = 0.0f;

		void Awake ()
		{
			#if UNITY_EDITOR
			this.enabled = false;
			#else
			this.enabled = true;
			#endif

			Input.gyro.enabled = true;
		}

		void Start ()
		{
			startingYRotation = transform.localEulerAngles.y;

			// we need a pivot object, parent to the rotated object to compensate the x axis rotation by 90 degrees
			// to respect the Unity world Y axis
			/*GameObject pivot = new GameObject();
	        pivot.name = "Pivot";
	        pivot.transform.position = transform.position;
	        pivot.transform.rotation = transform.rotation;
	        pivot.transform.Rotate(new Vector3(90, 0, 0));
	        transform.parent = pivot.transform;*/

			transform.parent.Rotate (new Vector3 (90, 0, 0));
		}

		void Update ()
		{
			// rotate according to the gyroscope attitude
			Quaternion rot = Input.gyro.attitude;
			rot.x = -rot.x;			// invert the x and y rotation axis
			rot.y = -rot.y;
			transform.localRotation = rot;

			// the first gyro position depends on the starting position of the device
			// resulting in varations on the starting rotation.
			// the following gets it right so that we can choose the beginning rotation
			if (!sober && transform.localEulerAngles != Vector3.zero) {
				// we check the drunkness amount of the gyro
				drunkenRotAmount = transform.eulerAngles.y;
				// we compensate the rotation on the parent object and we add the starting rotation we set in the inspector
				transform.Rotate (Vector3.up, startingYRotation - drunkenRotAmount, Space.World);
				sober = true;
			}
		}
	}
}
