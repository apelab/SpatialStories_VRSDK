﻿//-----------------------------------------------------------------------
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

namespace Gaze
{
    public class Gaze_DragAndDropEventArgs : EventArgs
    {
        /// <summary>
        /// The Drop Object
        /// </summary>
        private object sender;

        public object Sender { get { return sender; } }

        /// <summary>
        /// The object dropped on a target
        /// </summary>
        private object dropObject;

        public object DropObject { get { return dropObject; } }

        /// <summary>
        /// The target an object is dropped on
        /// </summary>
        private object dropTarget;

        public object DropTarget { get { return dropTarget; } }

        private Gaze_DragAndDropStates state;

        public Gaze_DragAndDropStates State { get { return state; } }

        //public Gaze_DragAndDropEventArgs(object _sender, object _targetObject, Gaze_DragAndDropStates _state)
        //{
        //    sender = _sender;
        //    dropTarget = _targetObject;
        //    state = _state;
        //}

        public Gaze_DragAndDropEventArgs(object _sender, object _dropObject, object _dropTarget, Gaze_DragAndDropStates _state)
        {
            sender = _sender;
            dropObject = _dropObject;
            dropTarget = _dropTarget;
            state = _state;
        }
    }
}