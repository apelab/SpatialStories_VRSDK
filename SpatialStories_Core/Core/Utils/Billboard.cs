using UnityEngine;
using System.Collections;
using Gaze;
using SpatialStories;

/// <summary>
/// Billboard makes the transform object facing the camera
/// </summary>
public class Billboard : MonoBehaviour
{
    public float Damping = 1f;
    public bool Invert = false;

    void Update()
    {
        // If for some reason the billboard loses the camera just recover it
        Vector3 lookPos = S_ArUtilitiesManager.SelectedCamera.transform.position - transform.position;
        
        if (Invert)
            lookPos = -lookPos;
        
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * Damping);
    }
}