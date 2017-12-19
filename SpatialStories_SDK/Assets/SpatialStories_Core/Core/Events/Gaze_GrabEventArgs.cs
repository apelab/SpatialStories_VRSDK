// <copyright file="Gaze_GrabEventArgs.cs" company="apelab sàrl">
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
using System;

namespace Gaze
{
    public class Gaze_GrabEventArgs : EventArgs
    {
        private object sender;

        public object Sender { get { return sender; } }

        private Gaze_GrabManager grabManager;

        public Gaze_GrabManager GrabManager { get { return grabManager; } }

        private Gaze_InteractiveObject interactiveObject;

        public Gaze_InteractiveObject InteractiveObject { get { return interactiveObject; } }

        private float timeToGrab;

        public float TimeToGrab { get { return timeToGrab; } }

        /// <summary>
        /// Arguments for animation events related.     
        /// </summary>
        /// <param name="_sender">Is the sender's object.</param>
        public Gaze_GrabEventArgs(object _sender, Gaze_GrabManager _grabManager, Gaze_InteractiveObject _interactiveObject, float _timeToGrab = 0, bool _forceEnable = false)
        {
            // If the grab manaber is disabled we can force the enable of it
            if (_forceEnable)
                _grabManager.enabled = true;
            sender = _sender;
            grabManager = _grabManager;
            interactiveObject = _interactiveObject;
            timeToGrab = _timeToGrab;
        }
    }
}
