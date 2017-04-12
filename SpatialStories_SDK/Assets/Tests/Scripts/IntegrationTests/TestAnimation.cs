using UnityEngine;
using UnityTest;
using Gaze;
using System;

/// <summary>
/// Description:
/// In this test we are goint to check if a certain animation is playing after
/// the trigger has ben fired.
/// 
/// Tests Passes On:
/// If the current animation is playing
/// 
/// Tests Fails On:
/// If not.
/// </summary>
public class TestAnimation : Gaze_AbstractTest
{
    public Animator TestAnimator;
    public TestCustomCondition ConditionToValidate;
    private enum TEST_PHASE { BEFORE_TRIGGER, CHECKING_ANIMATION }
    private TEST_PHASE actualTestPhase = TEST_PHASE.BEFORE_TRIGGER;
    
    public override void Gaze_Update()
    {
        switch (actualTestPhase)
        {
            case TEST_PHASE.BEFORE_TRIGGER:
                ConditionToValidate.SatisfyCondition();
                SkipUpdates();
                actualTestPhase = TEST_PHASE.CHECKING_ANIMATION;
                break;
            case TEST_PHASE.CHECKING_ANIMATION:
                if (TestAnimator.GetCurrentAnimatorStateInfo(0).IsName("Party"))
                    PassTest();              
                else
                    FailTest();
                break;
        }
    }


}
