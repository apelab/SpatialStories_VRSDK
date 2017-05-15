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
public abstract class TriggerCounterAbstractTest : Gaze_AbstractTest
{
    public TestCustomCondition ConditionToValidate;
    public Gaze_Conditions ConditionsOfTheTrigger;

    protected int numTimesTriggered = 0;

    public abstract void UpdateTriggerCounter();
}
