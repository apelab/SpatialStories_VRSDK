using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gaze;


[AddComponentMenu("SpatialStories/Custom Actions/Parent")]
public class CA_Parent : Gaze_AbstractBehaviour {

    public Transform Parent;
    public Transform Child;

    [Tooltip("Do we need to teleport to the parent position after parenting is complete?")]
    public bool MoveToParentPosition = false;

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
        Child.SetParent(Parent);

        if (MoveToParentPosition)
        {
            Child.transform.localPosition = Vector3.zero;
        }
    }
}
