using UnityEngine;
using System.Collections;

namespace Gaze
{
	public class Gaze_Zoom : MonoBehaviour
    {
		public Gaze_Conditions gazable;
		
		/// <summary>
		///  If TRUE, the camera zooms when gazed.
		/// </summary>
		public bool zoom;

		public bool[] zoomOnTriggerState = new bool [3];
		
		/// <summary>
		/// Zoom amount multiplier if zoom is TRUE
		/// </summary>
		public float zoomFovFactor;
		
		/// <summary>
		/// Zoom speed multiplier if zoom is TRUE
		/// </summary>
		public float zoomSpeedFactor;
		
		/// <summary>
		/// Only active if zoom is TRUE.
		/// When ungazed in the active window, determines the speed of dezoom
		/// </summary>
		public int dezoomModeIndex;
		
		/// <summary>
		/// De-zoom speed multiplier if zoom is TRUE
		/// </summary>
		public float dezoomSpeedFactor;

		[SerializeField]
		public AnimationCurve zoomCurve;
		
		public virtual void OnEnable ()
		{
			Gaze_EventManager.OnGazeEvent += onGazeEvent;
		}
		
		public virtual void OnDisable ()
		{
			Gaze_EventManager.OnGazeEvent -= onGazeEvent;
		}

		public virtual void Awake ()
		{
			gazable = GetComponent<Gaze_Conditions> ();
		}
		
		private void onGazeEvent (Gaze_GazeEventArgs e)
		{
			// if sender is the gazable collider GameObject
			if (e.Sender != null && (GameObject)e.Sender == gazable.gazeCollider.gameObject)
			{
				// if zoom is enabled
				if (zoom)
				{
					// if we are in a zoom status
					if (zoomOnTriggerState [gazable.triggerStateIndex])
					{
						// notify manager
						Gaze_EventManager.FireZoomEvent (new Gaze_ZoomEventArgs (gameObject, gazable.gazeCollider, zoomFovFactor, zoomSpeedFactor, (Gaze_DezoomMode)dezoomModeIndex, dezoomSpeedFactor, zoomCurve));
					}
				}
			}
		}
	}
}
