using UnityEngine;
using Gaze;

[AddComponentMenu("SpatialStories/Custom Actions/CA_Drop Object")]
public class CA_DropObject : Gaze_AbstractBehaviour
{
    public Gaze_InteractiveObject ObjectToDrop;
    public Transform DropTarget;

    private Gaze_DragAndDropManager dragAndDropManager;
    
    private void Start()
    {
        dragAndDropManager = ObjectToDrop.GetComponent<Gaze_DragAndDropManager>();
    }

    protected override void OnTrigger()
    {
        dragAndDropManager.AutoDrop(DropTarget);
    }
}
