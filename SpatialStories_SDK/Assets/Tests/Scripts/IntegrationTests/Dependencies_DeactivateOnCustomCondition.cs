using Gaze;
using UnityEngine;

/// <summary>
/// Description:
/// In this test we have 2 cubes our main cube has de property that if we are in proxmity with
/// it it will trigger, but it has a deactivator which is activate when a custom condition in the
/// other cube is satisfied.
///
/// Tests Passes On:
/// If the trigger is not been shot after 1 second regarless if he has been triggered.
/// 
/// Tests Fails On:
/// If the cube triggers after the deactivation dependecy has been satisfied.
/// 
/// </summary>
public class Dependencies_DeactivateOnCustomCondition : Gaze_SucceedOnTriggerTest
{
    private enum TEST_PHASE { INITIAL, TRIGGER_DEACTIVATED, WAITING_FOR_TRIGGER }
    private TEST_PHASE actualTestPhase = TEST_PHASE.INITIAL;

    public GameObject Dependent, Dependency;
    public Camera Camera;
    public TestCustomCondition ConditionToValidate;

    private void Start()
    {
        TrySuceedJustBeforeTimeout();
    }

    public override void Gaze_Update()
    {
        switch (actualTestPhase)
        {
            case TEST_PHASE.INITIAL:
                ConditionToValidate.SatisfyCondition();
                actualTestPhase = TEST_PHASE.TRIGGER_DEACTIVATED;
                isTriggerAllowed = false;
                SkipUpdates();
                break;
            case TEST_PHASE.TRIGGER_DEACTIVATED:
                PutObjectInProximityWithOther(Dependent, Camera.gameObject);
                actualTestPhase = TEST_PHASE.WAITING_FOR_TRIGGER;
                SkipUpdates();
                break;
            case TEST_PHASE.WAITING_FOR_TRIGGER:
                actualTestPhase = TEST_PHASE.WAITING_FOR_TRIGGER;
                break;

        }
    }
}
