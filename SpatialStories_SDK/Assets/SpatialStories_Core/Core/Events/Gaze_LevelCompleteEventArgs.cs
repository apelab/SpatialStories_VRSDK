// <copyright file="Gaze_LevelCompleteEventArgs.cs" company="apelab sàrl">
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
    public class Gaze_LevelCompleteEventArgs : EventArgs
    {
        private object sender;
        public object Sender { get { return sender; } }

        private int levelID;
        public int LevelID { get { return levelID; } }

        /// <summary>
        /// Arguments for animation events related.     
        /// </summary>
        /// <param name="_sender">Is the sender's object.</param>
        public Gaze_LevelCompleteEventArgs(object _sender, int _levelID)
        {
            sender = _sender;
            levelID = _levelID;
        }
    }
}
