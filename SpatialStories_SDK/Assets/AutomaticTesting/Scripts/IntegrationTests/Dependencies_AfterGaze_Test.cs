using UnityEngine;

/// <summary>
/// Description:
/// In this test we have 2 game objects, one of them needs to satisfy a custom condition to
/// trigger, but before another object has to be gazed by the camera.
///
/// Tests Passes On:
/// If the trigger is fired when the object has all its dependencies satisfied.
/// 
/// Tests Fails On:
/// After a timeout or if he triggers before all the dependencies has been satified
/// </summary>
public class Dependencies_AfterGaze_Test : Gaze_SucceedOnTriggerTest
{
    private enum TEST_PHASE { INITIAL, CUSTOM_CONDITION_SATISFIED, WAITING_FOR_TRIGGER }
    private TEST_PHASE actualTestPhase = TEST_PHASE.INITIAL;

    public GameObject DependentGameObject, DependencyGameObject;
    public TestCustomCondition CustomCondtitionToTrigger;

    public override void Gaze_Update()
    {
        switch (actualTestPhase)
        {
            case TEST_PHASE.INITIAL:
                CustomCondtitionToTrigger.SatisfyCondition();
                actualTestPhase = TEST_PHASE.CUSTOM_CONDITION_SATISFIED;
                SkipUpdates();
                break;
            case TEST_PHASE.CUSTOM_CONDITION_SATISFIED:
                PutObjectInfrontOfCamera(DependencyGameObject);
                actualTestPhase = TEST_PHASE.WAITING_FOR_TRIGGER;
                isTriggerAllowed = true;
                SkipUpdates();
                break;
        }
    }
}
