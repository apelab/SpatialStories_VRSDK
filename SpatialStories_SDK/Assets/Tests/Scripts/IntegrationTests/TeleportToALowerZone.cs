using UnityEngine;
using UnityTest;
using Gaze;
using System;

/// <summary>
/// Description:
/// In this test we are going to try if we are able to teleport to an allowed zone
/// 
/// Tests Passes On:
/// Te camera rig is in the correct spot and the height of the camera hasnt change
/// in relation with the floor.
/// 
/// Tests Fails On:
/// If the camera rig is not where it was intended to be or the height position has changed
/// </summary>
public class TeleportToALowerZone : TeleportTest
{
    bool hasWaited = false;

    private void Start()
    {
        actualTestPhase = TEST_PHASE.BEFORE_TELEPORTING;

    }

    public override void Gaze_Update()
    {
        switch (actualTestPhase)
        {
            case TEST_PHASE.BEFORE_TELEPORTING:
                StoreActualCameraValues();
                gazeTeleporter.Teleport(TeleportPoint.transform.position, true);
                actualTestPhase = TEST_PHASE.AFTER_TELEPORTING;
                SkipUpdates();
                break;
            case TEST_PHASE.AFTER_TELEPORTING:
                FailIfPositionIsNot(TeleportPoint.transform.position);
                FailIfCameraHeightHasChanged();
                PassTest();               
                break;
        }
    }
}
