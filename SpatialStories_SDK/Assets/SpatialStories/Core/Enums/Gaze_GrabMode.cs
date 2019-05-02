// <copyright file="Gaze_GrabMode.cs" company="apelab sàrl">
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

namespace Gaze
{
    /// <summary>
    /// Gaze_GrabStates :<br>
    /// GRAB = the collider has been grabbed
    /// UNGRAB = the collider has been released
    /// </summary>
    public enum Gaze_GrabMode
    {
        GRAB,
        LEVITATE,
        TOUCH
    }
}