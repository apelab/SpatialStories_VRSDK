//-----------------------------------------------------------------------
// <copyright file="Gaze_ControllerTouchEventArgs.cs" company="apelab sàrl">
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
using Gaze;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class Gaze_ControllerTouchEventArgs : EventArgs
{
    private object sender;

    public object Sender { get { return sender; } set { sender = value; } }

    private KeyValuePair<VRNode, GameObject> dico;

    public KeyValuePair<VRNode, GameObject> Dico { get { return dico; } set { dico = value; } }

    private Gaze_TouchDistanceMode mode;

    public Gaze_TouchDistanceMode Mode { get { return mode; } set { mode = value; } }

    private bool isTouching;

    public bool IsTouching { get { return isTouching; } set { isTouching = value; } }

    private bool isTriggerPressed;

    public bool IsTriggerPressed { get { return isTriggerPressed; } set { isTriggerPressed = value; } }

    public Gaze_ControllerTouchEventArgs(object _sender)
    {
        sender = _sender;
    }

    public Gaze_ControllerTouchEventArgs(object _sender, KeyValuePair<VRNode, GameObject> _dico, Gaze_TouchDistanceMode _eventDistanceMode, bool _isTouching, bool _isTriggerPressed)
    {
        sender = _sender;
        dico = _dico;
        mode = _eventDistanceMode;
        isTouching = _isTouching;
        isTriggerPressed = _isTriggerPressed;
    }
}