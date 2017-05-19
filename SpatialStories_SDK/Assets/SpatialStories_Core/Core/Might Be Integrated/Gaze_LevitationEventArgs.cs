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
using System;
using UnityEngine;

namespace Gaze
{
    public class Gaze_LevitationEventArgs : EventArgs
    {
        private object sender;

        public object Sender { get { return sender; } }

        private GameObject objectToLevitate;

        public GameObject ObjectToLevitate { get { return objectToLevitate; } }

        private Gaze_LevitationTypes type;

        public Gaze_LevitationTypes Type { get { return type; } }

        public Gaze_HandsEnum Hand { get; private set; }

        public Gaze_LevitationEventArgs(object _sender, GameObject _objectToLevitate, Gaze_LevitationTypes _type, Gaze_HandsEnum actualHand)
        {
            sender = _sender;
            objectToLevitate = _objectToLevitate;
            type = _type;
            Hand = actualHand;
        }
    }
}