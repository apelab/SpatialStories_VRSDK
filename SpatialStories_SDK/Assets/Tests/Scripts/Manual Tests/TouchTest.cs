using UnityEngine;
using Gaze;
using System;

public class TouchTest : Gaze_AbstractBehaviour
{
    protected override void OnActive()
    {
    }

    protected override void OnAfter()
    {
    }

    protected override void OnBefore()
    {
    }

    protected override void OnReload()
    {
    }

    protected override void OnTrigger()
    {
        Debug.Log(GetComponentInParent<Gaze_InteractiveObject>() + " - "+ this + " trigger !");
    }
}
