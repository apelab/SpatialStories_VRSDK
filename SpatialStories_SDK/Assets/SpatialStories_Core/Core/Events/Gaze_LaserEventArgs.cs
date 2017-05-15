// <copyright file="Gaze_LaserEventArgs.cs" company="apelab sàrl">
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
using UnityEngine;

public class Gaze_LaserEventArgs
{
    private object sender;

    public object Sender { get { return sender; } set { sender = value; } }

    private Vector3 startPosition;

    public Vector3 StartPosition { get { return startPosition; } set { startPosition = value; } }

    private Vector3 endPosition;

    public Vector3 EndPosition { get { return endPosition; } set { endPosition = value; } }

    private RaycastHit[] laserHits;

    public RaycastHit[] LaserHits { get { return laserHits; } set { laserHits = value; } }

    public Gaze_LaserEventArgs()
    {
    }

    public Gaze_LaserEventArgs(object _sender, Vector3 _startPosition, Vector3 _endPosition, RaycastHit[] _laserHits = null)
    {
        this.sender = _sender;
        this.startPosition = _startPosition;
        this.endPosition = _endPosition;
        this.laserHits = _laserHits;
    }
}
