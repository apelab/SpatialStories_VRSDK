﻿using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;

namespace Gaze
{
	[Serializable]
	public class Gaze_TriggerCustom : Gaze_AbstractBehaviour
	{
		public bool[] activeTriggerStates = new bool[5];
		public MonoBehaviour[] targetScripts = new MonoBehaviour[5];
		public string[] targetMethods = new string[5];

		private void Invoke (int i)
		{
			targetScripts [i].Invoke (targetMethods [i], 0);
		}

		#region implemented abstract members of Gaze_AbstractBehaviour

		protected override void OnTrigger ()
		{
			if (activeTriggerStates [0])
			{
				Invoke (0);
			}
		}
		
		protected override void OnReload ()
		{
			if (activeTriggerStates [1])
			{
				Invoke (1);
			}
		}

		protected override void OnBefore ()
		{
			if (activeTriggerStates [2])
			{
				Invoke (2);
			}
		}

		protected override void OnActive ()
		{
			if (activeTriggerStates [3])
			{
				Invoke (3);
			}
		}

		protected override void OnAfter ()
		{
			if (activeTriggerStates [4])
			{
				Invoke (4);
			}
		}

		#endregion
	}
}