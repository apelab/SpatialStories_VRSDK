using Gaze;
using UnityEngine;

/// <summary>
/// This action parents a transform to another transform.
/// </summary>

[AddComponentMenu("SpatialStories/Custom Actions/CA_Parent")]
public class CA_Parent : Gaze_AbstractBehaviour
{

    /// <summary>
    /// The parent the child will be parented to.
    /// </summary>
    public Transform Parent;

    /// <summary>
    /// The child to parent.
    /// </summary>
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
