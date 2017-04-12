using UnityEngine;
using Gaze;
using System;

public class TouchTest : Gaze_AbstractBehaviour
{
    protected override void onActive()
    {
    }

    protected override void onAfter()
    {
    }

    protected override void onBefore()
    {
    }

    protected override void onReload()
    {
    }

    protected override void onTrigger()
    {
        Debug.Log(GetComponentInParent<Gaze_InteractiveObject>() + " - "+ this + " trigger !");
    }
}
