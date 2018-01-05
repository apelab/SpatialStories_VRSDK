// <copyright file="Gaze_RootMotion.cs" company="apelab sàrl">
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
using System.Collections;

namespace Gaze
{
	public class Gaze_RootMotion : MonoBehaviour
	{

		/// <summary>
		/// The target defines the gaze object to follow for the gaze area (a visual usually).
		/// </summary>
		public GameObject rootTarget;
	
		void Update ()
		{
			this.transform.position = rootTarget.transform.position;
			this.transform.rotation = rootTarget.transform.rotation;
		}
	}
}
