using UnityEngine;
using UnityTest;
using Gaze;
using System;

/// <summary>
/// Description:
/// In this test we are goint to check if a certain sound is playing after
/// the trigger has ben fired.
/// 
/// Tests Passes On:
/// If the current sound is playing
/// 
/// Tests Fails On:
/// If not.
/// </summary>
public class TestSound : Gaze_AbstractTest
{
    public AudioSource Source;
    public TestCustomCondition ConditionToValidate;
    private enum TEST_PHASE { BEFORE_TRIGGER, CHECKING_SOUND }
    private TEST_PHASE actualTestPhase = TEST_PHASE.BEFORE_TRIGGER;
    
    public override void Gaze_Update()
    {
        switch (actualTestPhase)
        {
            case TEST_PHASE.BEFORE_TRIGGER:
                ConditionToValidate.SatisfyCondition();
                SkipUpdates();
                actualTestPhase = TEST_PHASE.CHECKING_SOUND;
                break;
            case TEST_PHASE.CHECKING_SOUND:
                if (Source.isPlaying && Source.clip.name == "Sound")
                    PassTest();              
                else
                    FailTest();
                break;
        }
    }


}
