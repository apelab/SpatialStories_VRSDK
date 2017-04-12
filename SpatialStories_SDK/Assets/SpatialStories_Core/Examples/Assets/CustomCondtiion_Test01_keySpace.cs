using UnityEngine;
using System.Collections;
using Gaze;

public class CustomCondtiion_Test01_keySpace : Gaze_AbstractConditions
{

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			Debug.Log ("key space !");
			ValidateCustomCondition (true);
		}		
	}

}
