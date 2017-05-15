using UnityEngine;
using System.Collections;
using Gaze;

public class NoAbstractBehaviourCube : MonoBehaviour
{
	public GameObject visual;
	public GameObject triggerSrc;
	private Material m;

	public void OnEnable ()
	{
		Gaze_EventManager.OnTriggerEvent += onTriggerEvent;
	}

	public void OnDisable ()
	{
		Gaze_EventManager.OnTriggerEvent -= onTriggerEvent;
	}

	void Start ()
	{
		m = visual.GetComponent<Renderer> ().material;
		m.color = Color.white;
	}

	private void onTriggerEvent (Gaze_TriggerEventArgs e)
	{
		// if sender is the this Behaviour gameObject
		if (e.Sender.Equals (triggerSrc))
		{
			if (e.IsTrigger)
			{
				m.color = new Color (Random.value, Random.value, Random.value);
			}

			if (e.IsReload)
			{
				m.color = Color.white;
			}
		}
	}
}
