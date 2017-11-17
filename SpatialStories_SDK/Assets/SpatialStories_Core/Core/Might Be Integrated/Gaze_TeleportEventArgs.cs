//-----------------------------------------------------------------------
// <copyright file="Gaze_TeleportEventArgs.cs" company="apelab sàrl">
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

namespace Gaze
{
    public class Gaze_TeleportEventArgs : EventArgs
    {

        public object Sender;

        private Gaze_TeleportMode mode;

        public Gaze_TeleportMode Mode { get { return mode; } set { mode = value; } }

        public Gaze_TeleportEventArgs(object _sender)
        {
            Sender = _sender;
        }

        public Gaze_TeleportEventArgs(object _sender, Gaze_TeleportMode _mode)
        {
            Sender = _sender;
            mode = _mode;
        }
    }
}