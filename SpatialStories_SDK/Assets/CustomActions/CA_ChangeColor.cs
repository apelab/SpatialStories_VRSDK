using Gaze;
using System;
using UnityEngine;


[AddComponentMenu("SpatialStories/Custom Actions/CA_ChangeColor")]

public class CA_ChangeColor : Gaze_AbstractBehaviour
{
    protected override void OnTrigger()
    {
        GetComponentInParent<Gaze_InteractiveObject>().GetComponentInChildren<Renderer>().material.color = UnityEngine.Random.ColorHSV();
    }
}
