using UnityEngine;
using UnityTest;
using Gaze;
using System;

/// <summary>
/// Description:
/// In this test we are going to try if we are able to teleport to a non allowed zone
/// 
/// Tests Passes On:
/// If we don't move at all.
/// 
/// Tests Fails On:
/// IF our position or rotation changes.
/// </summary>
public class TeleportToNotAllowedZone : TeleportTest
{
    public override void Gaze_Update()
    {
        switch (actualTestPhase)
        {
            case TEST_PHASE.BEFORE_TELEPORTING:
                gazeTeleporter.Teleport(TeleportPoint.transform.position, true);
                actualTestPhase = TEST_PHASE.AFTER_TELEPORTING;
                break;
            case TEST_PHASE.AFTER_TELEPORTING:
                FailIfPositionChanged();
                FailIfRotationChanged();
                PassTest();
                break;
        }
    }
}
