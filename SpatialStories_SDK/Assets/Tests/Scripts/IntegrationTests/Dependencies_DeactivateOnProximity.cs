using Gaze;
using UnityEngine;

/// <summary>
/// Description:
/// In this test we have 2 cubes our main cube has de property that if we look at it it will
/// trigger, but if before the other cube is in proximity with it it has to deactivate and never
/// trigger when gazed.
///
/// Tests Passes On:
/// If the trigger is not been shot after 1 second regarless if he has been triggered.
/// 
/// Tests Fails On:
/// If the cube triggers after the deactivation dependecy has been satisfied.
/// 
/// </summary>
public class Dependencies_DeactivateOnProximity : Gaze_SucceedOnTriggerTest
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
                PutObjectInProximityWithOther(Dependent, Dependency);
                actualTestPhase = TEST_PHASE.TRIGGER_DEACTIVATED;
                isTriggerAllowed = false;
                SkipUpdates();
                break;
            case TEST_PHASE.TRIGGER_DEACTIVATED:
                PutObjectInfrontOfCamera(Dependent);
                actualTestPhase = TEST_PHASE.WAITING_FOR_TRIGGER;
                SkipUpdates();
                break;
            case TEST_PHASE.WAITING_FOR_TRIGGER:
                actualTestPhase = TEST_PHASE.WAITING_FOR_TRIGGER;
                break;

        }
    }
}
