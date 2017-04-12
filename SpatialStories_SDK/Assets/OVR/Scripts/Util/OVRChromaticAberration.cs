using UnityEngine;

/// <summary>
/// Allows you to toggle chromatic aberration correction with a gamepad button press.
/// </summary>
public class OVRChromaticAberration : MonoBehaviour
{
	/// <summary>
	/// The button that will toggle chromatic aberration correction.
	/// </summary>
	public OVRInput.RawButton			toggleButton = OVRInput.RawButton.X;	

	private bool								chromatic = false;

	void Start ()
	{
		// Enable/Disable Chromatic Aberration Correction.
		// NOTE: Enabling Chromatic Aberration for mobile has a large performance cost.
		OVRManager.instance.chromatic = chromatic;
	}

	void Update()
	{
		// NOTE: some of the buttons defined in OVRInput.RawButton are not available on the Android game pad controller
		if (OVRInput.GetDown(toggleButton))
		{
			//*************************
			// toggle chromatic aberration correction
			//*************************
			chromatic = !chromatic;
			OVRManager.instance.chromatic = chromatic;
		}
	}

}
