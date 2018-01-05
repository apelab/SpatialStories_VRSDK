// <copyright file="Gaze_ControllerCollisionsManager.cs" company="apelab sàrl">
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

public class Gaze_ControllerCollisionsManager : MonoBehaviour
{
    public float colliderSize = 1f;
    public Gaze_GrabManager grabManager;

    private Transform controllerTransform;

    void Start()
    {
        GetComponent<BoxCollider>().size = new Vector3(colliderSize, colliderSize, colliderSize);
        grabManager = Gaze_Utils.GetIOFromGameObject(gameObject).GetComponentInChildren<Gaze_GrabManager>();
    }

    void Update()
    {
        ProximityFollow();
    }

    /// <summary>
    /// Makes the Proximity GameObject follow the Hand Model to keep detecting occuring at Hand's location.
    /// </summary>
    private void ProximityFollow()
    {
        // Find the hand model within the IO children to make the Proximity follow it
        if (GetComponentInParent<Gaze_InteractiveObject>() != null && GetComponentInParent<Gaze_InteractiveObject>().GetComponentInChildren<Gaze_HandController>() != null)
        {

            controllerTransform = GetComponentInParent<Gaze_InteractiveObject>().GetComponentInChildren<Gaze_HandController>().transform;
            if (controllerTransform != null)
            {
                transform.position = controllerTransform.position;
                transform.rotation = controllerTransform.rotation;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // notify manager
        Gaze_InputManager.FireControllerCollisionEvent(new Gaze_ControllerCollisionEventArgs(this.gameObject, other.gameObject, Gaze_CollisionTypes.COLLIDER_ENTER, grabManager));
    }

    void OnTriggerExit(Collider other)
    {
        // notify manager
        Gaze_InputManager.FireControllerCollisionEvent(new Gaze_ControllerCollisionEventArgs(this.gameObject, other.gameObject, Gaze_CollisionTypes.COLLIDER_EXIT, grabManager));
    }
}
