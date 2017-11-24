using Gaze;
using UnityEngine;

public class Gaze_HandHover : MonoBehaviour
{
    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer(Gaze_HashIDs.LAYER_HANDHOVER);
    }
}