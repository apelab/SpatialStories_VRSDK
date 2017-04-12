using Gaze;
using UnityEngine;

/// <summary>
/// Description:
/// In this test we have 2 game objects, one of them needs to bee looked by the camera to
/// trigger, but before another object has to be in contact with him.
///
/// Tests Passes On:
/// If the trigger comes when the object has all its dependencies satisfied.
/// 
/// Tests Fails On:
/// After a timeout or if he triggers before all the dependencies has been satified
/// </summary>
public class Dependencies_AfterProximity_Test : Gaze_SucceedOnTriggerTest
{
    private enum TEST_PHASE { INITIAL, GAZED_BUT_NOT_DEPENDENCIES, WAITING_FOR_TRIGGER }
    private TEST_PHASE actualTestPhase = TEST_PHASE.INITIAL;

    public GameObject DependentGameObject, DependencyGameObject, ObjectToBeInProx;
    public Gaze_InteractiveObject Camera;

    public override void Gaze_Update()
    {
        switch (actualTestPhase)
        {
            case TEST_PHASE.INITIAL:
                PutObjectInfrontOfCamera(DependentGameObject);
                actualTestPhase = TEST_PHASE.GAZED_BUT_NOT_DEPENDENCIES;
                SkipUpdates();
                break;
            case TEST_PHASE.GAZED_BUT_NOT_DEPENDENCIES:
                PutObjectInProximityWithOther(DependencyGameObject, ObjectToBeInProx);
                isTriggerAllowed = true;
                actualTestPhase = TEST_PHASE.WAITING_FOR_TRIGGER;
                SkipUpdates();
                break;
            case TEST_PHASE.WAITING_FOR_TRIGGER:
                PutObjectAwayAllProximities(DependencyGameObject);
                break;
        }
    }
}
