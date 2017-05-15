// <copyright file="Gaze_Collision.cs" company="apelab sàrl">
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
	public class Gaze_Collision : MonoBehaviour
	{
		public bool detectCollision;

		private Gaze_CollisionEventArgs gaze_CollisionEventArgs;

		void Start ()
		{
			gaze_CollisionEventArgs = new Gaze_CollisionEventArgs ();
		}

		void OnCollisionEnter (Collision collision)
		{
			if (collision.gameObject.GetComponent<Gaze_Collision> () != null &&
			    collision.gameObject.GetComponent<Gaze_Collision> ().detectCollision) {
				gaze_CollisionEventArgs.Sender = this.gameObject;
				gaze_CollisionEventArgs.CollisionInfo = collision;
				gaze_CollisionEventArgs.CollisionState = 1;
				Gaze_EventManager.FireCollisionEvent (gaze_CollisionEventArgs);
				//Gaze_EventManager.FireCollisionEvent (new Gaze_CollisionEventArgs (this.gameObject, collision, 1));
			}
		}
	}
}
