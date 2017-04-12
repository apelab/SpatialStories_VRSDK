using UnityEngine;

public class Gaze_PlayerTorso : MonoBehaviour
{
    public float verticalOffset = -.5f;

    private Vector3 tmp;
    private Camera IOCamera = null;
    private GameObject LeftHandFixedPosition, RightHandFixedPosition;

    private void Start()
    {
        IOCamera = transform.GetComponentInParent<Gaze_InputManager>().GetComponentInChildren<Camera>();
        LeftHandFixedPosition = transform.FindChild("LeftHandFixedPosition").gameObject;
        RightHandFixedPosition = transform.FindChild("RightHandFixedPosition").gameObject;
    }

    void Update()
    {
        this.transform.localPosition = new Vector3(0, 0.5f, 0f);
        this.transform.localRotation = Quaternion.Euler(new Vector3(0, Camera.main.transform.localRotation.eulerAngles.y, 0));
    }
}