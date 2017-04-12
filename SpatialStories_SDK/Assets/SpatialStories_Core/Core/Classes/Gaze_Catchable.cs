// <copyright file="Gaze_Catchable.cs" company="apelab sàrl">
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

public class Gaze_Catchable : MonoBehaviour
{
	/// <summary>
	/// Can this object be grabbed.
	/// </summary>
	public bool isCatchable = false;

	/// <summary>
	/// If true, the object being catched will vibrate the controllers while grabbed.
	/// </summary>
	public bool vibrates = false;

	/// <summary>
	/// Is this catchable object using gravity
	/// </summary>
	public bool hasGravity;

	private Rigidbody rigidBody;

	void Awake ()
	{
		if (isCatchable) {
			// rigidibody
			rigidBody = gameObject.GetComponent<Rigidbody> ();
			if (rigidBody == null)
				rigidBody = gameObject.AddComponent<Rigidbody> ();
		}
	}

	public void setGravity (bool _hasGravity)
	{
		hasGravity = _hasGravity;
	}

	public bool getGravity ()
	{
		return hasGravity;
	}
}
