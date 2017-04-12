using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gaze;
using System;

public class destroy : Gaze_AbstractBehaviour
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
        Destroy(GetComponentInParent<Gaze_InteractiveObject>().gameObject);
    }
}
