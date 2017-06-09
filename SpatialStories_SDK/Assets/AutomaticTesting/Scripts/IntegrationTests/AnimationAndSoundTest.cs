using UnityEngine;

/// <summary>
/// Description:
/// In this test we are goint to check if a certain sound and animation is playing after
/// the trigger has ben fired.
/// 
/// Tests Passes On:
/// If the correct sound and animation is playing
/// 
/// Tests Fails On:
/// If not.
/// </summary>
public class AnimationAndSoundTest : Gaze_AbstractTest
{
    public AudioSource Source;
    public Animator TestAnimator;
    public TestCustomCondition ConditionToValidate;
    private enum TEST_PHASE { BEFORE_TRIGGER, CHECKING_SOUND_AND_ANIM }
    private TEST_PHASE actualTestPhase = TEST_PHASE.BEFORE_TRIGGER;
    
    public override void Gaze_Update()
    {
        switch (actualTestPhase)
        {
            case TEST_PHASE.BEFORE_TRIGGER:
                ConditionToValidate.SatisfyCondition();
                SkipUpdates();
                actualTestPhase = TEST_PHASE.CHECKING_SOUND_AND_ANIM;
                break;
            case TEST_PHASE.CHECKING_SOUND_AND_ANIM:
                if (!Source.isPlaying)
                {
                    FailTest("The source is not playing");
                }
                else if (!Source.clip.name.Equals("Sound"))
                {
                    FailTest("The current clip name is not correct!");
                }
                else if (TestAnimator.GetCurrentAnimatorStateInfo(0).IsName("Bouncing"))
                {
                    FailTest("The current animation is not correct!");
                }
                else
                    PassTest();
                break;
        }
    }
}
