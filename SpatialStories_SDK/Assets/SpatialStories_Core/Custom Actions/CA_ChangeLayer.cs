using Gaze;
using UnityEngine;


/// <summary>
/// This script will change the layer the Interactive Object is on when executed (on Gaze_Conditions valid).
/// </summary>

[AddComponentMenu("SpatialStories/Custom Actions/CA_ChangeLayer")]
public class CA_ChangeLayer : Gaze_AbstractBehaviour
{
    public GameObject TargetGameObject;
    public int NewLayer;
    public int choosenLayerIndex = 0;

    protected override void OnTrigger()
    {
        TargetGameObject.layer = NewLayer;
    }
}