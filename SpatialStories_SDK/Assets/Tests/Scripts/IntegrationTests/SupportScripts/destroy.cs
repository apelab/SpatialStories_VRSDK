using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gaze;
using System;

public class destroy : Gaze_AbstractBehaviour
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
        Destroy(GetComponentInParent<Gaze_InteractiveObject>().gameObject);
    }
}
