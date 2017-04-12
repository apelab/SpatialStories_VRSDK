using UnityEngine;
using Gaze;

/// <summary>
/// Description:
/// In this test we are going to test that a certain IO passes for all the 
/// phases that an IO has DEFORE - ACTIVE - AFTER.
/// 
/// Tests Passes On:
/// IF we are able to check that the IO is passing for all the timeframe
/// 
/// Tests Fails On:
/// Timeout
/// 
/// </summary>
public class TimeFrameTest : Gaze_AbstractTest
{
    private enum TEST_PHASE { NONE, BEFORE, ACTIVE, AFTER }
    private TEST_PHASE actualTestPhase = TEST_PHASE.NONE;

    public Gaze_Conditions TimeFrameConditions;

    public override void Gaze_Update()
    {
        switch(actualTestPhase)
        {
            case TEST_PHASE.NONE:
                TimeFrameConditions.gameObject.SetActive(true);
                actualTestPhase = TEST_PHASE.BEFORE;
                break;
            case TEST_PHASE.BEFORE:
                if (TimeFrameConditions.triggerStateIndex == (int)Gaze_TriggerState.BEFORE)
                    actualTestPhase = TEST_PHASE.ACTIVE;
                break;
            case TEST_PHASE.ACTIVE:
                if (TimeFrameConditions.triggerStateIndex == (int)Gaze_TriggerState.ACTIVE)
                    actualTestPhase = TEST_PHASE.AFTER;
                break;
            case TEST_PHASE.AFTER:
                if (TimeFrameConditions.triggerStateIndex == (int)Gaze_TriggerState.AFTER)
                {
                    PassTest();
                    TimeFrameConditions.gameObject.SetActive(false);
                }
                break;
        }
    }
}
