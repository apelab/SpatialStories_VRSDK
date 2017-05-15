using UnityEngine;
using Gaze;

/// <summary>
/// Description:
/// In this test we are going to satisfy the condition of a trigger for 0.20 seconds and then
/// unsatisfy it wait 0.10 seconds and look if we are more or less on 10 % progress.
/// 
/// Tests Passes On:
/// If the progress is arround 10%
/// 
/// Tests Fails On:
/// If not xD
/// 
/// </summary>
public class LossModeFade : Gaze_AbstractTest
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
                ChangePhaseIn(0.2f, TEST_PHASE.EXPOSED);
                break;
            case TEST_PHASE.EXPOSED:
                PutObjectAwayAllProximities(ConditionToWorkWith.GetComponentInParent<Gaze_InteractiveObject>().gameObject);
                ChangePhaseIn(0.1f, TEST_PHASE.CHECKING);
                break;
            case TEST_PHASE.CHECKING:
                if (GazeConditions.FocusTotalTime > 0.09f && GazeConditions.FocusTotalTime < 0.11f)
                    PassTest();
                else
                    FailTest("The focus time is diferent than 10%: " + GazeConditions.FocusTotalTime.ToString());
                break;
        }
    }
}
