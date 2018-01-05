// <copyright file="Gaze_CameraCollider.cs" company="apelab sàrl">
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
	public class Gaze_CameraCollider : MonoBehaviour
	{
		public Vector3 colliderSize = new Vector3 (.2f, .2f, .2f);
		private Camera cam;

		void Start ()
		{
			initMembers ();
			addComponents ();
		}

		private void initMembers ()
		{
			cam = Camera.main;

        }

		private void addComponents ()
		{
			#region Rigibody
			cam.transform.gameObject.AddComponent<Rigidbody> ();
			cam.transform.gameObject.GetComponent<Rigidbody> ().isKinematic = true;
			cam.transform.gameObject.GetComponent<Rigidbody> ().useGravity = false;
			#endregion

			#region Collider
			cam.transform.gameObject.AddComponent<BoxCollider> ();
			cam.transform.gameObject.GetComponent<BoxCollider> ().isTrigger = true;
			cam.transform.gameObject.GetComponent<BoxCollider> ().size = colliderSize;
			#endregion
		}
	}
}
