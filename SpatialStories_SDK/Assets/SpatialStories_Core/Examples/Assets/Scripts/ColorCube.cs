using UnityEngine;
using System.Collections;

public class ColorCube : MonoBehaviour {
	public GameObject visual;
	private Material m;

	void Start () {
		m = visual.GetComponent<Renderer>().material;
		m.color = Color.gray;
	}

	public void DisableMe () {
		m.color = Color.gray;
	}
	
	public void ResetMe () {
		m.color = Color.white;
	}

	public void ColorMe () {
		m.color = new Color (Random.value,Random.value,Random.value);
	}
}
