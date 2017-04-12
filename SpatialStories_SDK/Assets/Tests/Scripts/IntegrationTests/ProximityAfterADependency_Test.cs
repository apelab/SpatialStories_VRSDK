using UnityEngine;
using UnityTest;
using Gaze;

/// <summary>
/// Description:
/// In this test we have an action which is dependent of another action. In this particular case
/// we are going to satisfy the dependencies when the camera is already in proximity with the game object that
/// we need to be.
/// 
/// Tests Passes On:
/// If the trigger that depends on the proximity and the another condition is fired
/// 
/// Tests Fails On:
/// Timeout
/// </summary>
public class ProximityAfterADependency_Test : Gaze_AbstractTest {

    private enum TEST_PHASE { NOT_IN_PROXIMITY, IN_PROXIMITY, WAITING_FOR_TRIGGER }
    private TEST_PHASE actualTestPhase;

    public Gaze_InteractiveObject cube;
    public TestCustomCondition dependentCondition;
    public GameObject CameraObject;
    public GameObject dependentActionTrigger;

    int counter = 0;
    private void UpdateTestPhase()
    {
        switch (actualTestPhase)
        {
            case TEST_PHASE.NOT_IN_PROXIMITY:
                cube.transform.position = CameraObject.transform.position;
                actualTestPhase = TEST_PHASE.IN_PROXIMITY;
                break;
            case TEST_PHASE.IN_PROXIMITY:
                dependentCondition.SatisfyCondition();
                actualTestPhase = TEST_PHASE.WAITING_FOR_TRIGGER;
                break;
            default:
                break;
        }
    }

    public override void Gaze_Update()
    {
        UpdateTestPhase();
    }

    private void OnEnable()
    {
        Gaze_EventManager.OnTriggerEvent += OnTriigerEvent;
    }

    private void OnDisable()
    {
        Gaze_EventManager.OnTriggerEvent -= OnTriigerEvent;
    }

    /// <summary>
    /// The test will finnish when the trigger is shot
    /// </summary>
    /// <param name="e"></param>
    private void OnTriigerEvent(Gaze_TriggerEventArgs e)
    {
        GameObject sender = (GameObject)e.Sender;
        if ((GameObject)e.Sender == dependentActionTrigger)
        {
            if (e.IsTrigger && cube.GetComponentInChildren<Gaze_Conditions>().isInProximity)
            {
                PassTest();
            }
        }
    }

}
