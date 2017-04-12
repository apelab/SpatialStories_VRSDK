//-----------------------------------------------------------------------
// <copyright file="LevitationEventArgs.cs" company="apelab sàrl">
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
using System.Collections.Generic;
using UnityEngine;
using Gaze;
using UnityEngine.VR;

namespace Gaze
{
    public class Gaze_GrabCondition : Gaze_AbstractConditions
    {

        //	public GrabActionValues grabActionValues = GrabActionValues.GRAB;
        public int grabActionIndex;
        public bool reload;

        void OnEnable()
        {
            Gaze_InputManager.OnControllerGrabEvent += OnControllerGrabEvent;
        }

        void OnDisable()
        {
            Gaze_InputManager.OnControllerGrabEvent -= OnControllerGrabEvent;
        }

        void OnControllerGrabEvent(Gaze_ControllerGrabEventArgs e)
        {
            if (e.ControllerObjectPair.Value == GetComponentInParent<Gaze_InteractiveObject>().gameObject)
            {
                // if grabbing and grab condition is GRAB
                if (e.IsGrabbing && grabActionIndex.Equals((int)Gaze_GrabActionValues.GRAB))
                {
                    ValidateCustomCondition(true);

                    // if ungrabbing and grab condition is UNGRAB
                }
                else if (!e.IsGrabbing && grabActionIndex.Equals((int)Gaze_GrabActionValues.UNGRAB))
                {
                    ValidateCustomCondition(true);
                }
            }
            
        }
    }
}