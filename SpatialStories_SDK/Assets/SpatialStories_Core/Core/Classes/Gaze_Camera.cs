// <copyright file="Gaze_InputManager.cs" company="apelab sàrl">
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
using Gaze;
using UnityEngine;

/// <summary>
/// Gaze camera.
/// Changes the parenting structure to allow Proximity and other children to follow the Camera.
/// </summary>
public class Gaze_Camera : MonoBehaviour
{

    private bool isReconfigurationNeeded = true;
    public bool IsReconfiguiringNeeded { get { return isReconfigurationNeeded; } }

    private void Awake()
    {
        //OVRManager.instance.trackingOriginType = OVRManager.TrackingOrigin.FloorLevel;
    }

    void Start()
    {
        if (isReconfigurationNeeded)
            ReconfigureCamera();
    }

    public void ReconfigureCamera()
    {
        if (isReconfigurationNeeded == false)
            return;

        Transform cameraIO = GetComponentInParent<Gaze_InteractiveObject>().transform;
        Transform rootIO = GetComponentInParent<Gaze_InputManager>().transform;


        transform.localPosition = rootIO.localPosition;
        transform.localRotation = rootIO.localRotation;
        transform.parent = cameraIO.parent.transform;
        cameraIO.parent = transform;
        rootIO.localPosition = Vector3.zero;
        rootIO.localRotation = Quaternion.identity;
        isReconfigurationNeeded = false;
    }
}
