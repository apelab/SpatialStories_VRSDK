using UnityEngine;
using System;

namespace Gaze
{
	public class Gaze_TriggerEventArgs : EventArgs
	{
		public object Sender { get; private set; }

		public float Time { get; private set; }

		public bool IsTrigger { get; private set; }

		public bool IsReload { get; private set; }
		
		public int Count { get; private set; }

		public Gaze_AutoTriggerMode AutoTriggerMode { get; private set; }

		public Gaze_ReloadMode ReloadMode { get; private set; }

		public int ReloadMaxRepetitions { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Gaze.Gaze_TriggerEventArgs"/> class.
		/// </summary>
		/// <param name="sender">The sender of the Trigger Event.</param>
		/// <param name="time">The time at which the Trigger Event happened.</param>
		/// <param name="isTrigger">Set to <c>true</c> when the trigger has been triggered.</param>
		/// <param name="isReload">Set to <c>true</c> when the trigger has been reloaded.</param>
		/// <param name="count">the number of times this trigger has been triggered/reloaded.</param>
		/// <param name="autoTriggerMode">The auto trigger mode.</param>
		/// <param name="reloadMode">The reload mode.</param>
		/// <param name="reloadMaxRepetitions">The maximum number of trigger repetitions.</param>
		public Gaze_TriggerEventArgs (object sender, float time, bool isTrigger, bool isReload, int count, Gaze_AutoTriggerMode autoTriggerMode, Gaze_ReloadMode reloadMode, int reloadMaxRepetitions)
		{
			this.Sender = sender;
			this.Time = time;
			this.IsTrigger = isTrigger;
			this.IsReload = isReload;
			this.Count = count;
			this.AutoTriggerMode = autoTriggerMode;
			this.ReloadMode = reloadMode;
			this.ReloadMaxRepetitions = reloadMaxRepetitions;
		}
	}
}
