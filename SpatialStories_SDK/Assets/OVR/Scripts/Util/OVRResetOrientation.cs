using UnityEngine;

/// <summary>
/// Allows you to reset VR input tracking with a gamepad button press.
/// </summary>
public class OVRResetOrientation : MonoBehaviour
{
	/// <summary>
	/// The gamepad button that will reset VR input tracking.
	/// </summary>
	public OVRInput.RawButton resetButton = OVRInput.RawButton.Y;

	/// <summary>
	/// Check input and reset orientation if necessary
	/// See the input mapping setup in the Unity Integration guide
	/// </summary>
	void Update()
	{
		// NOTE: some of the buttons defined in OVRInput.RawButton are not available on the Android game pad controller
		if (OVRInput.GetDown(resetButton))
		{
			//*************************
			// reset orientation
			//*************************
			OVRManager.display.RecenterPose();
		}
	}
}
