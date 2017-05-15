using Gaze;
using UnityEngine;

/// <summary>
/// Description:
/// In this test we have 3 IOS, the dependent one needs to have its dependent condition validated to trigger 
/// but before that One game object has to satisfy its gaze condition AND the other one its proximity condition
/// not before.
///
/// Tests Passes On:
/// If the trigger is fired when the object has all its dependencies satisfied.
/// 
/// Tests Fails On:
/// After a timeout or if he triggers before all the dependencies has been satified
/// </summary>
public class Dependencies_AfterProximityAndGaze : Gaze_SucceedOnTriggerTest
{
    private enum TEST_PHASE { INITIAL, DEPENDENT_CONDITION_SATISFIED, PROXIMITY_SATISFIED, WAITING_FOR_TRIGGER }
    private TEST_PHASE actualTestPhase = TEST_PHASE.INITIAL;

    public GameObject DependentGameObject, DependencyGaze, DependencyProximity;
    public Camera Camera;
    public TestCustomCondition ConditionToValidate;

    public override void Gaze_Update()
    {
        switch (actualTestPhase)
        {
            case TEST_PHASE.INITIAL:
                ConditionToValidate.SatisfyCondition();
                actualTestPhase = TEST_PHASE.DEPENDENT_CONDITION_SATISFIED;
                SkipUpdates();
                break;
            case TEST_PHASE.DEPENDENT_CONDITION_SATISFIED:
                PutObjectInProximityWithOther(DependencyProximity, DependentGameObject);
                actualTestPhase = TEST_PHASE.PROXIMITY_SATISFIED;
                SkipUpdates();
                break;
            case TEST_PHASE.PROXIMITY_SATISFIED:
                PutObjectInfrontOfCamera(DependencyGaze, Camera);
                actualTestPhase = TEST_PHASE.WAITING_FOR_TRIGGER;
                isTriggerAllowed = true;
                break;

        }
    }
}
