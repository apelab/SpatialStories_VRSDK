// <copyright file="Gaze_GazeEventArgs.cs" company="apelab sàrl">
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
    public class Gaze_GazeEventArgs : EventArgs
    {
        private object sender;

        public object Sender { get { return sender; } set { sender = value; } }

        private bool isGazed;

        public bool IsGazed { get { return isGazed; } set { isGazed = value; } }

        public Gaze_GazeEventArgs()
        {
        }

        /// <summary>
        /// Arguments for animation events related.     
        /// </summary>
        /// <param name="_sender">Is the sender's object.</param>
        public Gaze_GazeEventArgs(object _sender, bool _isGazed)
        {
            sender = _sender;
            isGazed = _isGazed;
        }
    }
}