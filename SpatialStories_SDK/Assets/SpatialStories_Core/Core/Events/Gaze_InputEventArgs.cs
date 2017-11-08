// <copyright file="Gaze_InputEventArgs.cs" company="apelab sàrl">
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

public class Gaze_InputEventArgs
{
    private object sender;

    public object Sender { get { return sender; } set { sender = value; } }

    private UnityEngine.XR.XRNode? vrNode;

    public UnityEngine.XR.XRNode? VrNode { get { return vrNode; } set { vrNode = value; } }

    private Gaze_InputTypes inputType;

    public Gaze_InputTypes InputType { get { return inputType; } set { inputType = value; } }

    private float inputValue;

    public float InputValue { get { return inputValue; } set { inputValue = value; } }

    private Vector2 axisValue;

    public Vector2 AxisValue { get { return axisValue; } set { axisValue = value; } }

    public Gaze_InputEventArgs(object _sender)
    {
        this.sender = _sender;
    }

    public Gaze_InputEventArgs(object _sender, Gaze_InputTypes _inputType)
    {
        this.sender = _sender;
        this.inputType = _inputType;
    }

    public Gaze_InputEventArgs(object _sender, UnityEngine.XR.XRNode _vrNode, Gaze_InputTypes _inputType) : this(_sender, _inputType)
    {
        this.vrNode = _vrNode;
    }

    public Gaze_InputEventArgs(object _sender, UnityEngine.XR.XRNode _vrNode, Gaze_InputTypes _inputType, float _inputValue) : this(_sender, _vrNode, _inputType)
    {
        this.inputValue = _inputValue;
    }

    public Gaze_InputEventArgs(object _sender, UnityEngine.XR.XRNode _vrNode, Gaze_InputTypes _inputType, Vector2 _axisValue) : this(_sender, _vrNode, _inputType)
    {
        this.axisValue = _axisValue;
    }
}
