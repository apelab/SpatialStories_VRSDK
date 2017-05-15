using UnityEngine;
using System.Collections;

public class CameraKeyboardController : MonoBehaviour {
	public float speed;

	void Start () {
		if(speed <= 0) speed = 1f;
	}

	void FixedUpdate () {
		transform.Translate(speed * Input.GetAxis("Horizontal"), 0, speed * Input.GetAxis("Vertical"));
	}
}
