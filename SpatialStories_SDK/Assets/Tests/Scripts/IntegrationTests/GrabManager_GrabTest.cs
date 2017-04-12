using UnityEngine;
using UnityTest;
using Gaze;
using System;

/// <summary>
/// Description:
/// In this test we are going to put an object near to the left hand and then 
/// trigger the OnIndexLeftDownEvent and then listen if the grab event has been
/// called.
/// 
/// Tests Passes On:
/// If the grab event has ben called.
/// 
/// Tests Fails On:
/// Timeout of 1 seconds.
///
/// </summary>
public class GrabManager_GrabTest : Gaze_AbstractTest
{
    private enum TEST_PHASE { NON_IN_HAND, IN_HAND, TRIGGER_DOWN, WAITING_FOR_EVENT }
    private TEST_PHASE actualTestPhase;

    public override void Gaze_Update()
    {
        switch (actualTestPhase)
        {
            case TEST_PHASE.NON_IN_HAND:
                break;
            case TEST_PHASE.IN_HAND:
                break;
            case TEST_PHASE.TRIGGER_DOWN:
                break;
            case TEST_PHASE.WAITING_FOR_EVENT:
                break;
        }
    }
}
