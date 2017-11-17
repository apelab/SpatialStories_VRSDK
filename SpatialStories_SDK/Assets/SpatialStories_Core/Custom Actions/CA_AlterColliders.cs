using System.Collections.Generic;
using Gaze;
using UnityEngine;

[AddComponentMenu("SpatialStories/Custom Actions/CA_AlterColliders")]
public class CA_AlterColliders : Gaze_AbstractBehaviour
{
    public List<Collider> CollidersToAlter;
    public bool Enable;
    
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
        foreach (Collider collider in CollidersToAlter)
            collider.enabled = Enable;
    }
}
