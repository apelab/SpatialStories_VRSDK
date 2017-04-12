using UnityEngine;
using UnityTest;
using Gaze;
using System;

/// <summary>
/// Description:
/// In this test we wait some time in order to check that nothing happens
/// 
/// Tests Passes On:
/// If nothing happends before the timeout
/// 
/// Tests Fails On:
/// If a trigger happens
/// </summary>
public class AutoTrigger_None : Gaze_SucceedOnTriggerTest
{

    private void Start()
    {
        isTriggerAllowed = false;
        TrySuceedJustBeforeTimeout();
    }

    public override void Gaze_Update()
    {
    }
}
