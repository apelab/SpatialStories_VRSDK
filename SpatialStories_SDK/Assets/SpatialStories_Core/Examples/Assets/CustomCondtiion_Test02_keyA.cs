using UnityEngine;
using System.Collections;
using Gaze;

public class CustomCondtiion_Test02_keyA : Gaze_AbstractConditions
{

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.A)) {
			Debug.Log ("key A !");
			ValidateCustomCondition (true);
		}		
	}

}
