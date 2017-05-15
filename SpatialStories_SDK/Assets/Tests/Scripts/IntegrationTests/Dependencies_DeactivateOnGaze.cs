using Gaze;
using UnityEngine;

/// <summary>
/// Description:
/// In this test we have 2 cubes our main cube has de property that if the other cube
/// is in proximity will trigger but if the other has been gazed this trigger should not
/// be activated.
///
/// Tests Passes On:
/// If the trigger is not been shot after 1 second regarless if he has been triggered
/// (Because is no longer active).
/// 
/// Tests Fails On:
/// If the cube triggers after the deactivation dependecy has been satisfied.
/// 
/// </summary>
public class Dependencies_DeactivateOnGaze : Gaze_SucceedOnTriggerTest
{
    private enum TEST_PHASE { INITIAL, TRIGGER_DEACTIVATED, WAITING_FOR_TRIGGER }
    private TEST_PHASE actualTestPhase = TEST_PHASE.INITIAL;

    public GameObject Dependent, Dependency;
    public Camera Camera;

    private void Start()
    {
        TrySuceedJustBeforeTimeout();
    }

    public override void Gaze_Update()
    {
        switch (actualTestPhase)
        {
            case TEST_PHASE.INITIAL:
                PutObjectInfrontOfCamera(Dependency);
                actualTestPhase = TEST_PHASE.TRIGGER_DEACTIVATED;
                isTriggerAllowed = false;
                SkipUpdates();
                break;
            case TEST_PHASE.TRIGGER_DEACTIVATED:
                PutObjectInProximityWithOther(Dependency, Dependent);
                actualTestPhase = TEST_PHASE.WAITING_FOR_TRIGGER;
                SkipUpdates();
                break;
            case TEST_PHASE.WAITING_FOR_TRIGGER:
                actualTestPhase = TEST_PHASE.WAITING_FOR_TRIGGER;
                break;

        }
    }
}
