using UnityEngine;
using System.Collections;

public class ListenerCubeExample : Gaze.Gaze_AbstractGazeListener
{
	#region implemented abstract members of Gaze_AbstractGazeListener
	protected override void onGazeEvent (Gaze.Gaze_GazeEventArgs e)
	{
		Debug.Log ("onGazeEvent " + e.Sender);
	}

	protected override void onTriggerStateEvent (Gaze.Gaze_TriggerStateEventArgs e)
	{
		Debug.Log ("onTriggerStateEvent " + e.Sender);
	}

	protected override void onTriggerEvent (Gaze.Gaze_TriggerEventArgs e)
	{
		Debug.Log ("onTriggerEvent " + e.Sender);
	}

	protected override void onProximityEvent (Gaze.Gaze_ProximityEventArgs e)
	{
		Debug.Log ("onProximityEvent " + e.Sender);
	}
	#endregion
}
