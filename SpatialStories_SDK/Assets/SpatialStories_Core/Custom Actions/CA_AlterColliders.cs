using Gaze;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This custom actions lets you decide which colliders among all the colliders in this IO you want to enable or disable.
/// It is usefull because for now, the regular Gaze_Actions doesn't allow you to select which collider to alter. It's either all or none.
/// So with this script you can accuretely define which one to alter.
/// </summary>

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
