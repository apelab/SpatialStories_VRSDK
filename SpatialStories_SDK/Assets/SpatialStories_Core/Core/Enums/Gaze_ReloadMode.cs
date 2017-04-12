// <copyright file="Gaze_ReloadMode.cs" company="apelab sàrl">
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
// <date>2015-07-07</date>

namespace Gaze
{
	/// <summary>
	/// Number of times the trigger can be reloaded<br>
	/// 	MANUAL_ONLY = exclusivley manually defined in code
	///     INFINITE = can be reloaded infinitely while not expired
	///     FINITE = the number of reload is limited to the specified number
	/// </summary>
	public enum Gaze_ReloadMode
	{
		MANUAL_ONLY,
		INFINITE,
		FINITE
	}
}