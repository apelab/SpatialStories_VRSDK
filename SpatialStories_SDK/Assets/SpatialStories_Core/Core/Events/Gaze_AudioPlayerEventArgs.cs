// <copyright file="Gaze_AudioPlayerEventArgs.cs" company="apelab sàrl">
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
using System;

namespace Gaze
{
	public class Gaze_AudioPlayerEventArgs : EventArgs
	{
		private object sender;
		public object Sender { get { return sender; } }

		private bool playNext;
		public bool PlayNext { get { return playNext; } }
    
		/// <summary>
		/// Arguments for animation events related.     
		/// </summary>
		/// <param name="_sender">Is the sender's object.</param>
		public Gaze_AudioPlayerEventArgs (object _sender, bool _playNext)
		{
			sender = _sender;
			playNext = _playNext;
		}
	}
}
