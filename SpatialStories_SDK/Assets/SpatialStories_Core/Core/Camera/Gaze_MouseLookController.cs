// <copyright file="Gaze_MouseLookController.cs" company="apelab sàrl">
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

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaze
{
	public class Gaze_MouseLookController : MonoBehaviour
	{
		public float minimumX = -360F;
		public float maximumX = 360F;
		public float minimumY = -60F;
		public float maximumY = 60F;
		public float sensibility = 15F;
		private float rotationX;
		private float rotationY;

		void Awake ()
		{
			#if UNITY_EDITOR
			if (PlayerSettings.virtualRealitySupported)
				this.enabled = false;
			#else
			this.enabled = false;
			#endif
		}

		void Start ()
		{
			/*Debug.Log(transform.localEulerAngles);
			rotationX = transform.localEulerAngles.x;
			rotationY = transform.localEulerAngles.y;*/
		}

		void Update ()
		{
			// Read the mouse input axis
			rotationX += Input.GetAxis ("Mouse X") * sensibility;
			rotationY += Input.GetAxis ("Mouse Y") * sensibility;

			rotationX = ClampAngle (rotationX, minimumX, maximumX);
			rotationY = ClampAngle (rotationY, minimumY, maximumY);

			Quaternion xQuaternion = Quaternion.AngleAxis (rotationX, Vector3.up);
			Quaternion yQuaternion = Quaternion.AngleAxis (rotationY, -Vector3.right);

			transform.localRotation = xQuaternion * yQuaternion;
		}

		public static float ClampAngle (float angle, float min, float max)
		{
			if (angle < -360F)
				angle += 360F;
			if (angle > 360F)
				angle -= 360F;
			return Mathf.Clamp (angle, min, max);
		}
	}
}
