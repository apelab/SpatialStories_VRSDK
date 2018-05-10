using Gaze;
using UnityEngine;

public class CA_DeactivateTeleport : Gaze_AbstractBehaviour
{
    

    protected override void OnTrigger()
    {

        Gaze_Teleporter.IsTeleportAllowed = false;
         }
}
