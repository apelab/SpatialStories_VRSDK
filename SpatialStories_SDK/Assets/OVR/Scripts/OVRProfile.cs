using UnityEngine;
using System.Collections;
using System.Threading;
using VR = UnityEngine.VR;

/// <summary>
/// (Deprecated) Contains information about the user's preferences and body dimensions.
/// </summary>
public class OVRProfile : Object
{
	[System.Obsolete]
	public enum State
	{
		NOT_TRIGGERED,
		LOADING,
		READY,
		ERROR
	};

	[System.Obsolete]
	public string id { get { return "000abc123def"; } }
	[System.Obsolete]
	public string userName { get { return "Oculus User"; } }
	[System.Obsolete]
	public string locale { get { return "en_US"; } }

	public float ipd { get { return Vector3.Distance (OVRPlugin.GetNodePose (OVRPlugin.Node.EyeLeft, false).ToOVRPose ().position, OVRPlugin.GetNodePose (OVRPlugin.Node.EyeRight, false).ToOVRPose ().position); } }
	public float eyeHeight { get { return OVRPlugin.eyeHeight; } }
	public float eyeDepth { get { return OVRPlugin.eyeDepth; } }
	public float neckHeight { get { return eyeHeight - 0.075f; } }

	[System.Obsolete]
	public State state { get { return State.READY; } }
}
