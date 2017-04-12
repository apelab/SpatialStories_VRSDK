using UnityEngine;
using System.Collections;
using Gaze;

public class TriggerCube : Gaze_AbstractBehaviour
{
	public Color delayedColor = Color.gray;
	public Color ungazedColor = Color.white;
	public Color gazedColor = Color.yellow;
	public Color triggeredColor = Color.green;
	public Color expiredColor = Color.red;
	private Material material;
	public bool manualReload = false;
	public float reloadDelay = 1f;
	public float reloadDelayDelta = 2f;

	void Update ()
	{
		if (triggerState.Equals (Gaze_TriggerState.ACTIVE) && gazable.canBeTriggered) {
			material.color = Color.Lerp (ungazedColor, gazedColor, gazable.FocusCompletion);
		}
	}

	#region implemented abstract members of Gaze_AbstractTrigger

	protected override void onTrigger ()
	{
		material.color = triggeredColor;

		if (manualReload) {
			gazable.ManualReload (reloadDelay + Random.value * reloadDelayDelta);
		}
	}

	protected override void onReload ()
	{
		material.color = ungazedColor;
	}

	protected override void onBefore ()
	{
		if (!material) {
			material = gazable.Root.GetComponentInChildren<Renderer> ().material;
		}

		material.color = delayedColor;
	}

	protected override void onActive ()
	{
		material.color = ungazedColor;
	}

	protected override void onAfter ()
	{
		material.color = expiredColor;
	}

	#endregion
}
