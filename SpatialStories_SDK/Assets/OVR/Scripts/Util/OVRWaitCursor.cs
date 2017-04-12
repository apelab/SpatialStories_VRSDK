using UnityEngine;

/// <summary>
/// Rotates this GameObject at a given speed.
/// </summary>
public class OVRWaitCursor : MonoBehaviour
{
	public Vector3 rotateSpeeds = new Vector3(0.0f, 0.0f, -60.0f);

	/// <summary>
	/// Auto rotates the attached cursor.
	/// </summary>
	void Update()
	{
		transform.Rotate(rotateSpeeds * Time.smoothDeltaTime);
	}
}
