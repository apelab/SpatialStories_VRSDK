//-----------------------------------------------------------------------
// <copyright file="Gaze_ControllerGrabEventArgs.cs" company="apelab sàrl">
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
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class Gaze_ControllerGrabEventArgs : EventArgs
{
    private object sender;

    public object Sender { get { return sender; } }

    //	private GameObject grabbedObject;
    //
    //	public GameObject GrabbedObject{ get { return grabbedObject; } }

    private bool isGrabbing;

    public bool IsGrabbing { get { return isGrabbing; } }

    private Vector3 hitPosition;

    public Vector3 HitPosition { get { return hitPosition; } }

    private KeyValuePair<UnityEngine.XR.XRNode, GameObject> controllerObjectPair;
    public KeyValuePair<UnityEngine.XR.XRNode, GameObject> ControllerObjectPair { get { return controllerObjectPair; } }

    public Gaze_ControllerGrabEventArgs(object _sender, KeyValuePair<UnityEngine.XR.XRNode, GameObject> _dico, bool _isGrabbing)
    {
        sender = _sender;
        controllerObjectPair = _dico;
        isGrabbing = _isGrabbing;
    }

    public Gaze_ControllerGrabEventArgs(object _sender, KeyValuePair<UnityEngine.XR.XRNode, GameObject> _dico, bool _isGrabbing, Vector3 _hitPosition)
    {
        sender = _sender;
        controllerObjectPair = _dico;
        isGrabbing = _isGrabbing;
        hitPosition = _hitPosition;
    }
}