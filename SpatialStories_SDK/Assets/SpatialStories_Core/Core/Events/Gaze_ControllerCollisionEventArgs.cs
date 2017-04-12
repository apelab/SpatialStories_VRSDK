//-----------------------------------------------------------------------
// <copyright file="Gaze_ControllerCollisionEventArgs.cs" company="apelab sàrl">
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
// <author>Michaël Martin</author>
// <email>dev@apelab.ch</email>
// <web>https://twitter.com/apelab_ch</web>
// <web>http://www.apelab.ch</web>
// <date>2016-01-25</date>
// </copyright>
//-----------------------------------------------------------------------
using UnityEngine;
using System;

public class Gaze_ControllerCollisionEventArgs : EventArgs
{
	private GameObject sender;

	public GameObject Sender{ get { return sender; } }

	private GameObject other;

	public GameObject Other{ get { return other; } }

	private Gaze_CollisionTypes collisionType;

	public Gaze_CollisionTypes CollisionType{ get { return collisionType; } }

	public Gaze_ControllerCollisionEventArgs (GameObject _sender, GameObject _other, Gaze_CollisionTypes _collisionType)
	{
		sender = _sender;
		other = _other;
		collisionType = _collisionType;
	}
}