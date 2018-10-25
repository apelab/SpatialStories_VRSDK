using UnityEngine;
using Gaze;

[ExecuteInEditMode]
public class CA_PreserveInitialCameraRotation : Gaze_AbstractBehaviour
{

    [HideInInspector]
    public Transform frontReference;

    [HideInInspector]
    public Camera cam;

    [HideInInspector]
    public Gaze_InteractiveObject io;

    [HideInInspector]
    public Quaternion ioInitialRotation;

    private void Start()
    {
        if (Application.isEditor)
        {
            io = GetComponentInParent<Gaze_InteractiveObject>();
            frontReference = io.transform.Find("Reference_");

            if (frontReference == null)
                frontReference = new GameObject("Reference_").transform;

            frontReference.hideFlags = HideFlags.HideInHierarchy;
        }
    }

    protected override void OnTrigger()
    {
        Transform parent = io.transform.parent;
        frontReference.SetParent(null);
        io.transform.SetParent(frontReference.transform);
        frontReference.LookAt(cam.transform.position);
        io.transform.rotation = Quaternion.Euler(new Vector3(ioInitialRotation.eulerAngles.x, io.transform.rotation.eulerAngles.y, ioInitialRotation.eulerAngles.z));
        io.transform.rotation = Quaternion.Euler(new Vector3(0, io.transform.rotation.eulerAngles.y, 0));
        frontReference.SetParent(io.transform);
        io.transform.SetParent(parent);
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            frontReference.transform.position = io.transform.position;
            cam = S_ArUtilitiesManager.SelectedCamera;
            if (cam == null || frontReference == null)
                return;

            frontReference.transform.LookAt(cam.transform.position);
            frontReference.transform.SetParent(io.transform);
            ioInitialRotation = io.transform.rotation;
        }
    }

    public override void SetupUsingApi(GameObject _interaction)
    {
        //TODO: Implement that some day if neccesary
    }

}
