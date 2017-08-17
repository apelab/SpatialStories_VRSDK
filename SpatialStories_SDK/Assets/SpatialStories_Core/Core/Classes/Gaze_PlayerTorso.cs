using UnityEngine;

public class Gaze_PlayerTorso : MonoBehaviour
{
    public float verticalOffset = -.5f;

    private Vector3 tmp;

    void Update()
    {
        this.transform.localPosition = new Vector3(0, 0.5f, 0f);
        this.transform.localRotation = Quaternion.Euler(new Vector3(0, Camera.main.transform.localRotation.eulerAngles.y, 0));
    }
}