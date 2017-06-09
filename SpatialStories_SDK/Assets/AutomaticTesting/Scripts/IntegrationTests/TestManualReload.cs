using UnityEngine;
using UnityTest;
using Gaze;
using System;

/// <summary>
/// Description:
/// In this test we are going to try the manual reload funcionality
/// for this purpose we are going to "Manually Reload" 5 times 
/// a certain trigger and we are going to check if at the end
/// of the test the trigger has been triggered the correct ammount of
/// times.
/// 
/// Tests Passes On:
/// If the ammount of times that this trigger has ben triggered is correct
/// 
/// Tests Fails On:
/// If not.
/// </summary>
public class TestManualReload : TriggerCounterAbstractTest
{
    private enum TEST_PHASE { WAITING_FOR_TRIGGERS, SKIPING_SOME_UPDATES, COUNTING_TRIGGERS }
    private TEST_PHASE actualTestPhase = TEST_PHASE.WAITING_FOR_TRIGGERS;

    private const int MAX_ALLOWED_TRIGGERS = 5;

    public override void Gaze_Update()
    {
        switch (actualTestPhase)
        {
            case TEST_PHASE.WAITING_FOR_TRIGGERS:
                ConditionToValidate.SatisfyCondition();
                break;
            case TEST_PHASE.SKIPING_SOME_UPDATES:
                SkipUpdates(100);
                actualTestPhase = TEST_PHASE.COUNTING_TRIGGERS;
                break;
            case TEST_PHASE.COUNTING_TRIGGERS:
                if (ConditionsOfTheTrigger.TriggerCount == MAX_ALLOWED_TRIGGERS)
                    PassTest();
                else
                    FailTest();
                break;
        }
    }

    public override void UpdateTriggerCounter()
    {
        numTimesTriggered++;
        if (numTimesTriggered < MAX_ALLOWED_TRIGGERS)
            ConditionsOfTheTrigger.ManualReload();
        else
            actualTestPhase = TEST_PHASE.SKIPING_SOME_UPDATES;
    }
}
