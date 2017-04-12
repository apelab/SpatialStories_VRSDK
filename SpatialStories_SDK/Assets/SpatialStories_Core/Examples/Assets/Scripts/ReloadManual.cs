using UnityEngine;
using System.Collections;
using Gaze;

public class ReloadManual : MonoBehaviour
{
	public GameObject visual;
	private Material material;
	private Gaze_Conditions gazable;

	public void OnEnable ()
	{
		Gaze_EventManager.OnTriggerEvent += onTriggerEvent;
		Gaze_EventManager.OnTriggerStateEvent += onTriggerStateEvent;
	}

	public void OnDisable ()
	{
		Gaze_EventManager.OnTriggerEvent -= onTriggerEvent;
		Gaze_EventManager.OnTriggerStateEvent -= onTriggerStateEvent;
	}

	void Start ()
	{
		material = visual.GetComponent<Renderer> ().material;
		gazable = GetComponent<Gaze_Conditions> ();
	}

	private void onTriggerEvent (Gaze_TriggerEventArgs e)
	{
		// if sender is the this Behaviour gameObject
		if (((GameObject)e.Sender).Equals (gameObject)) {
			if (e.IsTrigger) {
				material.color = new Color (Random.value, Random.value, Random.value);
				gazable.ManualReload (Random.value * 2.9f + 0.1f);
			} else if (e.IsReload) {
				material.color = Color.white;
			}
		}
	}

	private void onTriggerStateEvent (Gaze_TriggerStateEventArgs e)
	{
		if (((GameObject)e.Sender).Equals (gameObject)) {
			if (e.TriggerState.Equals (Gaze_TriggerState.ACTIVE)) {
				material.color = Color.white;
			} else if (e.TriggerState.Equals (Gaze_TriggerState.BEFORE)) {
				material.color = Color.gray;
			} else if (e.TriggerState.Equals (Gaze_TriggerState.AFTER)) {
				material.color = Color.red;
			}
		}
	}
}
