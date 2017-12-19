// <copyright file="Gaze_TriggerDependencyMode.cs" company="apelab sàrl">
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
// <date>2015-07-02</date>

namespace Gaze
{    
    /// <summary>
    /// Gaze_ trigger dependency mode.
    ///     Active = the dependent object is in its active window but not yet triggered or finished
    ///     Triggered = the dependent object has been triggered
    ///     Finished = the dependent object is out of its activity window
    /// </summary>
    public enum Gaze_TriggerDependencyMode
    {
        Active,
        Triggered,
        Finished
    }
}