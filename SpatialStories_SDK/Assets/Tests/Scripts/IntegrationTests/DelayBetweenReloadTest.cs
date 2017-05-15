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
public class DelayBetweenReloadTest : TriggerCounterAbstractTest
{
    private enum TEST_PHASE { SATISFY_CONDITION, FIRST_TRIGGER, SECOND_TRIGGER }
    private TEST_PHASE actualTestPhase;
    private float triggerTime;

    public void OnEnable()
    {
        actualTestPhase = TEST_PHASE.SATISFY_CONDITION;
    }

    public override void Gaze_Update()
    {
        switch (actualTestPhase)
        {
            case TEST_PHASE.SATISFY_CONDITION:
                ConditionToValidate.SatisfyCondition();
                actualTestPhase = TEST_PHASE.FIRST_TRIGGER;
                break;
            case TEST_PHASE.FIRST_TRIGGER:
                if (ConditionsOfTheTrigger.TriggerCount == 1)
                {
                    triggerTime = Time.time;
                    actualTestPhase = TEST_PHASE.SECOND_TRIGGER;
                }
                break;
            case TEST_PHASE.SECOND_TRIGGER:
                if (ConditionsOfTheTrigger.TriggerCount == 2)
                {
                    float timeBetweenTriggers = Time.time - triggerTime;
                    if (timeBetweenTriggers > 0.095f && timeBetweenTriggers < 0.11)
                        PassTest();
                    else
                        FailTest("Incorrect time passed between triggers: " + timeBetweenTriggers.ToString());
                }
                break;                                                      
        }
    }

    public override void UpdateTriggerCounter(){}
}
