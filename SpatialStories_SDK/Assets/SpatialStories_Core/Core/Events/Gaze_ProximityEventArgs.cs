// <copyright file="Gaze_ProximityEventArgs.cs" company="apelab sàrl">
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
    public class Gaze_ProximityEventArgs : EventArgs
    {
        private object sender;

        public object Sender { get { return sender; } }

        private Gaze_InteractiveObject other;

        public Gaze_InteractiveObject Other { get { return other; } }

        private bool isInProximity;

        public bool IsInProximity { get { return isInProximity; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gaze_ProximityEventArgs"/> class.
        /// </summary>
        /// <param name="_sender">The GameObject that fires the event. The Interactive Object (Root)</param>
        /// <param name="_other">The colliding GameObject</param>
        /// <param name="_isInProximity">true means the camera entered the proximity zone, false means exits the zone</param>
        public Gaze_ProximityEventArgs(object _sender, Gaze_InteractiveObject _other, bool _isInProximity)
        {
            sender = _sender;
            other = _other;
            isInProximity = _isInProximity;
        }
    }
}
