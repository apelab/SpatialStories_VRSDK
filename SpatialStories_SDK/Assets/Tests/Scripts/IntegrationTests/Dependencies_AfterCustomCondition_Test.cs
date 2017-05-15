using Gaze;
using UnityEngine;

/// <summary>
/// Description:
/// In this test we have 2 game objects, one of them needs to satisfy a the proximity condition with
/// another to trigger, but before the other object has to be satisfy a custom condition.
///
/// Tests Passes On:
/// If the trigger is fired when the object has all its dependencies satisfied.
/// 
/// Tests Fails On:
/// After a timeout or if he triggers before all the dependencies has been satified
/// </summary>
public class Dependencies_AfterCustomCondition_Test : Gaze_SucceedOnTriggerTest
{
    private enum TEST_PHASE { INITIAL, PROXIMITY_SATISFIED, WAITING_FOR_TRIGGER }
    private TEST_PHASE actualTestPhase = TEST_PHASE.INITIAL;

    public GameObject DependentGameObject, DependencyGameObject;
    public TestCustomCondition CustomCondtitionToTrigger;

    public override void Gaze_Update()
    {
        switch (actualTestPhase)
        {
            case TEST_PHASE.INITIAL:
                PutObjectInProximityWithOther(DependencyGameObject, DependentGameObject);
                actualTestPhase = TEST_PHASE.PROXIMITY_SATISFIED;
                SkipUpdates();
                break;
            case TEST_PHASE.PROXIMITY_SATISFIED:
                CustomCondtitionToTrigger.SatisfyCondition();
                actualTestPhase = TEST_PHASE.WAITING_FOR_TRIGGER;
                isTriggerAllowed = true;
                SkipUpdates();
                break;
        }
    }
}
