// <copyright file="Gaze_ControllerPointingEventArgs.cs" company="apelab sàrl">
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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

namespace Gaze
{
    public class Gaze_ControllerPointingEventArgs : EventArgs
    {
        private object sender;

        public object Sender { get { return sender; } set { sender = value; } }

        private KeyValuePair<VRNode, GameObject> keyValue;

        public KeyValuePair<VRNode, GameObject> KeyValue { get { return keyValue; } set { keyValue = value; } }

        private Dictionary<VRNode, GameObject> dico;

        public Dictionary<VRNode, GameObject> Dico { get { return dico; } set { dico = value; } }

        private bool isPointed;

        public bool IsPointed { get { return isPointed; } set { isPointed = value; } }

        /// <summary>
        /// Arguments for animation events related.     
        /// </summary>
        /// <param name="_sender">Is the sender's object.</param>
        public Gaze_ControllerPointingEventArgs()
        {
        }

        /// <summary>
        /// Arguments for animation events related.     
        /// </summary>
        /// <param name="_sender">Is the sender's object.</param>
        public Gaze_ControllerPointingEventArgs(object _sender, Dictionary<VRNode, GameObject> _dico, bool _isPointed)
        {
            sender = _sender;
            dico = _dico;
            isPointed = _isPointed;
        }

        /// <summary>
        /// Arguments for animation events related.     
        /// </summary>
        /// <param name="_sender">Is the sender's object.</param>
        public Gaze_ControllerPointingEventArgs(object _sender, KeyValuePair<VRNode, GameObject> _keyValue, bool _isPointed)
        {
            sender = _sender;
            keyValue = _keyValue;
            isPointed = _isPointed;
        }
    }
}
