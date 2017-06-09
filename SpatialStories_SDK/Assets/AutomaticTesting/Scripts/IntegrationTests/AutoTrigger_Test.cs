using UnityEngine;
using UnityTest;
using Gaze;
using System;

/// <summary>
/// Description:
/// In the CubeToInteract/AutoTrigger we set the condition "expires" to true and we add 1 second, also
/// we set the Auto Trigger Mode to END.After that we subscribe on the OnTrigger event.
/// 
/// Tests Passes On:
/// If the OnTriggerEvent is called before the timeout and the sender is correct.
/// 
/// Tests Fails On:
/// Timeout of 5 seconds.
/// </summary>
public class AutoTrigger_Test : Gaze_SucceedOnTriggerTest
{

    private void Start()
    {
        isTriggerAllowed = true;
    }

    public override void Gaze_Update()
    {
        isTriggerAllowed = true;
    }
}
