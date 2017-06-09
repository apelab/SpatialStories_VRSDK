using Gaze;
using UnityEngine;

/// <summary>
/// Description:
/// In this test we have 3 IOs the first one (The dependent) has to activate his action once it is
/// in proximity with the main camera, but if the 2 other cubes triggers before this shouldn't happen
/// because the dependent cube action will be deactivated.
///
/// Tests Passes On:
/// If the trigger is not been shot after 1 second regarless if he has been triggered.
/// 
/// Tests Fails On:
/// If the cube triggers after the deactivation depdendencies has been satisfied.
/// 
/// </summary>
public class Dependencies_DeactivateOnProximityAndCustomCondition : Gaze_SucceedOnTriggerTest
{
    private enum TEST_PHASE { INITIAL, FIST_DEPENDENCY_VALIDATED, TRIGGER_DEACTIVATED, WAITING_FOR_TRIGGER }
    private TEST_PHASE actualTestPhase = TEST_PHASE.INITIAL;

    public GameObject Dependent, DependencyCustom, DependencyProximity;
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
                actualTestPhase = TEST_PHASE.FIST_DEPENDENCY_VALIDATED;
                isTriggerAllowed = false;
                SkipUpdates();
                break;
            case TEST_PHASE.FIST_DEPENDENCY_VALIDATED:
                PutObjectInProximityWithOther(DependencyCustom, DependencyProximity);
                actualTestPhase = TEST_PHASE.TRIGGER_DEACTIVATED;
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
