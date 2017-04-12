// <copyright file="Gaze_HandController.cs" company="apelab sàrl">
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
using UnityEngine.VR;
using System.Collections;
using Gaze;

public class Gaze_HandController : MonoBehaviour
{
	public bool leftHand;
	private Animator animator;
	private float triggerValue;

	void OnEnable ()
	{
		Gaze_InputManager.OnHandRightDownEvent += OnHandRightDownEvent;
		Gaze_InputManager.OnHandRightUpEvent += OnHandRightUpEvent;
		Gaze_InputManager.OnHandLeftDownEvent += OnHandLeftDownEvent;
		Gaze_InputManager.OnHandLeftUpEvent += OnHandLeftUpEvent;
	}

	void OnDisable ()
	{
		Gaze_InputManager.OnHandRightDownEvent -= OnHandRightDownEvent;
		Gaze_InputManager.OnHandRightUpEvent -= OnHandRightUpEvent;
		Gaze_InputManager.OnHandLeftDownEvent -= OnHandLeftDownEvent;
		Gaze_InputManager.OnHandLeftUpEvent -= OnHandLeftUpEvent;
	}

	void Start ()
	{
		animator = GetComponentInChildren<Animator> ();
	}

	private void OnHandRightDownEvent (Gaze_InputEventArgs e)
	{
		if (e.VrNode.Equals (VRNode.RightHand) && !leftHand) {
			if (e.InputType.Equals (Gaze_InputTypes.HAND_RIGHT_DOWN) && animator != null)
				animator.SetBool (Gaze_HashIDs.ANIMATOR_PARAMETER_HANDCLOSED, true);				
		}
	}

	private void OnHandRightUpEvent (Gaze_InputEventArgs e)
	{
		if (e.VrNode.Equals (VRNode.RightHand) && !leftHand) {		
			if (e.InputType.Equals (Gaze_InputTypes.HAND_RIGHT_UP) && animator != null)
				animator.SetBool (Gaze_HashIDs.ANIMATOR_PARAMETER_HANDCLOSED, false);				
		}
	}

	private void OnHandLeftDownEvent (Gaze_InputEventArgs e)
	{
		if (e.VrNode.Equals (VRNode.LeftHand) && leftHand) {		
			if (e.InputType.Equals (Gaze_InputTypes.HAND_LEFT_DOWN) && animator != null)
				animator.SetBool (Gaze_HashIDs.ANIMATOR_PARAMETER_HANDCLOSED, true);				
		}
	}

	private void OnHandLeftUpEvent (Gaze_InputEventArgs e)
	{
		if (e.VrNode.Equals (VRNode.LeftHand) && leftHand) {		
			if (e.InputType.Equals (Gaze_InputTypes.HAND_LEFT_UP) && animator != null)
				animator.SetBool (Gaze_HashIDs.ANIMATOR_PARAMETER_HANDCLOSED, false);				
		}
	}
}