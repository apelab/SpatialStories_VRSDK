using UnityEngine;
using Gaze;

[AddComponentMenu("SpatialStories/Custom Actions/CA_ChangeLayer")]
public class CA_ChangeLayer : Gaze_AbstractBehaviour{

    public GameObject TargetGameObject;
    public int NewLayer;
	public int choosenLayerIndex = 0;

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
        TargetGameObject.layer = NewLayer;
    }
}
