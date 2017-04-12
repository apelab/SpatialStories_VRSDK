using UnityEngine;
using UnityTest;
using Gaze;
using System;

/// <summary>
/// Description:
/// In this test we are going to check if a custom action is successfully fired
/// 
/// Tests Passes On:
/// If the action is fired.
/// 
/// Tests Fails On:
/// On timeout if the action is not fired.
/// </summary>
public class TestCustomAction : Gaze_AbstractTest
{
    public TestCustomCondition ConditionToValidate;

    bool skipedSomeUpdates = false;
    public override void Gaze_Update()
    {
        if (!skipedSomeUpdates)
        {
            SkipUpdates();
            skipedSomeUpdates = true;
        }
        ConditionToValidate.SatisfyCondition();
    }

    public void Pass()
    {
        PassTest();
    }
}
