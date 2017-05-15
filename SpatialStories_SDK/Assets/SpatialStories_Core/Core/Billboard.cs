using UnityEngine;
using System.Collections;

/// <summary>
/// Billboard makes the transform object facing the camera
/// </summary>
public class Billboard : MonoBehaviour
{
    public float damping = 1f;

    private Camera targetCamera;

    void Start()
    {
        targetCamera = Camera.main;
    }

    void Update()
    {
        Vector3 lookPos = targetCamera.transform.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
    }
}