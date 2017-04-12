using UnityEngine;
using Gaze;

/// <summary>
/// Description:
/// In this test we are going to satisfy the condition of a trigger for 0.1 seconds and then
/// unsatisfy it in order to check if the loss mode is working correctly.
/// 
/// Tests Passes On:
/// If the progress does not decrease
/// 
/// Tests Fails On:
/// If not xD
/// 
/// </summary>
public class LossModeNone : Gaze_AbstractTest
{
    protected enum TEST_PHASE { NOT_EXPOSED, EXPOSED, CHECKING}
    protected TEST_PHASE actualTestPhase = TEST_PHASE.NOT_EXPOSED;

    public TestCustomCondition ConditionToWorkWith;
    public Gaze_Conditions GazeConditions;

    protected float changePhaseTime = 0;
    
    protected float LastLossMode;

    protected void ChangePhaseIn(float _time, TEST_PHASE _newPhase)
    {
        changePhaseTime = _time;
        actualTestPhase = _newPhase;
    }

    public override void Gaze_Update()
    {
        // In the whole test we need to ensure that we are not loosing any focus
        if (LastLossMode > GazeConditions.FocusTotalTime)
            FailTest("Some focus total time has been lost!");
        else
            LastLossMode = GazeConditions.FocusTotalTime;

        // Used for waiting
        changePhaseTime -= Time.deltaTime;
        if (changePhaseTime > 0)
            return;
        
        switch (actualTestPhase)
        {
            case TEST_PHASE.NOT_EXPOSED:
                ConditionToWorkWith.SatisfyCondition();
                ChangePhaseIn(0.1f, TEST_PHASE.EXPOSED);
                break;
            case TEST_PHASE.EXPOSED:
                ConditionToWorkWith.UnSatisfyCondition();
                ChangePhaseIn(0.1f, TEST_PHASE.CHECKING);
                break;
            case TEST_PHASE.CHECKING:
                if (GazeConditions.FocusTotalTime > 0)
                    PassTest();
                else
                    FailTest(GazeConditions.FocusTotalTime.ToString());
                break;
        }
    }
}
