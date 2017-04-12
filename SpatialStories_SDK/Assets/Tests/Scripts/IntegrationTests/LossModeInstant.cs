using UnityEngine;
using Gaze;

/// <summary>
/// Description:
/// In this test we are going to satisfy the condition of a trigger for 0.21 seconds and then
/// unsatisfy it in order to check if the loss mode is working correctly.
/// 
/// Tests Passes On:
/// If the progress is 0%
/// 
/// Tests Fails On:
/// If not xD
/// 
/// </summary>
public class LossModeInstant : Gaze_AbstractTest
{
    protected enum TEST_PHASE { NOT_EXPOSED, EXPOSED, CHECKING}
    protected TEST_PHASE actualTestPhase = TEST_PHASE.NOT_EXPOSED;

    public TestCustomCondition ConditionToWorkWith;
    public Gaze_Conditions GazeConditions;

    protected float changePhaseTime = 0;
    
    protected float LastLossMode;

    public Camera CameraToGaze;

    protected void ChangePhaseIn(float _time, TEST_PHASE _newPhase)
    {
        changePhaseTime = _time;
        actualTestPhase = _newPhase;
    }

    public override void Gaze_Update()
    {
        // Used for waiting
        changePhaseTime -= Time.deltaTime;
        if (changePhaseTime > 0)
            return;
        
        switch (actualTestPhase)
        {
            case TEST_PHASE.NOT_EXPOSED:
                PutObjectInfrontOfCamera(ConditionToWorkWith.GetComponentInParent<Gaze_InteractiveObject>().gameObject, CameraToGaze);
                ChangePhaseIn(0.1f, TEST_PHASE.EXPOSED);
                break;
            case TEST_PHASE.EXPOSED:
                PutObjectAwayAllProximities(ConditionToWorkWith.GetComponentInParent<Gaze_InteractiveObject>().gameObject);
                ChangePhaseIn(0.1f, TEST_PHASE.CHECKING);
                break;
            case TEST_PHASE.CHECKING:
                if (GazeConditions.FocusTotalTime == 0)
                    PassTest();
                else
                    FailTest("The focus time is more than 0 <" + GazeConditions.FocusTotalTime.ToString());
                break;
        }
    }
}
