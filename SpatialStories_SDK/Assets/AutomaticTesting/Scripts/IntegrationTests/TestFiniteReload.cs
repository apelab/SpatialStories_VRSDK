using UnityEngine;
using UnityTest;
using Gaze;
using System;

/// <summary>
/// Description:
/// In this test we are going to check if a trigger is called the correct number of times
/// especified in the gaze_conditions.
/// 
/// Tests Passes On:
/// If the ammount of times that this trigger has ben triggered is correct.
/// 
/// Tests Fails On:
/// If not.
/// </summary>
public class TestFiniteReload : TriggerCounterAbstractTest
{
    private enum TEST_PHASE { WAITING_FOR_TRIGGERS, WATING_FOR_MINIMUM_COUNT, COUNTING_TRIGGERS }
    private TEST_PHASE actualTestPhase;

    private const int ALLOWED_TRIGGERS = 6;

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
                ConditionToValidate.SatisfyCondition();

                if (ConditionsOfTheTrigger.TriggerCount >= ALLOWED_TRIGGERS)
                {
                    actualTestPhase = TEST_PHASE.COUNTING_TRIGGERS;
                    SkipUpdates(20);
                }
                   
                break;
            case TEST_PHASE.COUNTING_TRIGGERS:
                if (ConditionsOfTheTrigger.TriggerCount == ALLOWED_TRIGGERS)
                    PassTest();
                else
                    FailTest();
                break;
        }
    }

    public override void UpdateTriggerCounter() { }
}
