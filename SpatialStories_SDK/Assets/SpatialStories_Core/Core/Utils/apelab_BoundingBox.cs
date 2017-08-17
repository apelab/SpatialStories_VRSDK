//========= Copyright 2016, Sam Tague, All rights reserved. ===========
//
// Attach to either or both tracked controller objects in SteamVR camera rig
//
//=============================================================================

// <copyright file="apelab_BoundingBox.cs" company="apelab sàrl">
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

namespace Gaze
{
    public class apelab_BoundingBox : MonoBehaviour
    {
        public void Generate(GameObject go)
        {
            // init the bounding box position and size with the first bounding box in the renderers
            Bounds totalBounds = go.GetComponentInChildren<Renderer>().bounds;

            // get all the renderers
            Renderer[] rs = go.GetComponentsInChildren<Renderer>();

            // get the count of renderers
            int count = rs.Length;

            // for each renderer
            for (int i = 0; i < count; i++)
            {

                // encapsulate each BB
                totalBounds.Encapsulate(rs[i].bounds);
            }
        }
    }
}