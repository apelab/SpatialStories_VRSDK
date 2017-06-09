using UnityEngine;
using UnityTest;
using Gaze;
using System;

/// <summary>
/// Description:
/// In this test we are going to check if the trigger is triggered more than 20 times
/// before a certain ammount of time.
/// 
/// Tests Passes On:
/// If the ammount of times that this trigger has ben triggered 20 or more
/// 
/// Tests Fails On:
/// If not.
/// </summary>
public class TestInfiniteReload : TriggerCounterAbstractTest
{
    private enum TEST_PHASE { WAITING_FOR_TRIGGERS, WATING_FOR_MINIMUM_COUNT, COUNTING_TRIGGERS }
    private TEST_PHASE actualTestPhase;

    private const int MIN_ALLOWED_TRIGGERS = 5;

    public void OnEnable()
    {
        actualTestPhase = TEST_PHASE.WAITING_FOR_TRIGGERS;
    }

    public override void Gaze_Update()
    {
        ConditionToValidate.SatisfyCondition();
        switch (actualTestPhase)
        {
            case TEST_PHASE.WAITING_FOR_TRIGGERS:
                ConditionToValidate.SatisfyCondition();
                actualTestPhase = TEST_PHASE.WATING_FOR_MINIMUM_COUNT;
                break;
            case TEST_PHASE.WATING_FOR_MINIMUM_COUNT:
                if (ConditionsOfTheTrigger.TriggerCount < MIN_ALLOWED_TRIGGERS)
                    ConditionToValidate.SatisfyCondition();
                else
                    actualTestPhase = TEST_PHASE.COUNTING_TRIGGERS;
                break;
            case TEST_PHASE.COUNTING_TRIGGERS:
                if (ConditionsOfTheTrigger.TriggerCount > MIN_ALLOWED_TRIGGERS)
                    PassTest();
                else
                    FailTest();
                break;
        }
    }

    public override void UpdateTriggerCounter(){}
}
