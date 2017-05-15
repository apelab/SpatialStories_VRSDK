using UnityEngine;
using UnityTest;
using Gaze;

/// <summary>
/// Description:
/// In this test we have an action which is dependent of another action. In this particular case
/// we are going to satisfy the dependencies when the camera is already watching the game object that
/// we need to gaze.
/// 
/// Tests Passes On:
/// If the trigger that depends on the gaze and the another condition is fired
/// 
/// Tests Fails On:
/// Timeout
/// </summary>
public class GazeAfterADependency_Test : Gaze_AbstractTest
{
    private enum TEST_PHASE { NON_GAZED, GAZED, WAITING_FOR_TRIGGER }
    private TEST_PHASE actualTestPhase;

    public Gaze_InteractiveObject cube;
    private TestCustomCondition dependentCondition;
    public GameObject dependentActionTrigger;

    // Use this for initialization
    private void Start()
    {
        dependentCondition = cube.GetComponentInChildren<TestCustomCondition>();
        actualTestPhase = TEST_PHASE.NON_GAZED;
    }

    int counter = 0;
    private void UpdateTestPhase()
    {
        switch (actualTestPhase)
        {
            case TEST_PHASE.NON_GAZED:
                // position the cube in front of the camera so it's being gazed
                PutObjectInfrontOfCamera(cube.gameObject);
                // update the phase to passe to the next step
                actualTestPhase = TEST_PHASE.GAZED;
                break;

            case TEST_PHASE.GAZED:
                dependentCondition.SatisfyCondition();
                actualTestPhase = TEST_PHASE.WAITING_FOR_TRIGGER;
                break;
            //default:
            //    //This forces the test to pass
            //    if (counter % 2 == 0)
            //        cube.transform.position = Camera.main.transform.position + Camera.main.transform.forward;
            //    else
            //        cube.transform.position = Camera.main.transform.position - 3 * Camera.main.transform.forward;
            //    counter++;
            //    break;
        }
    }

    public override void Gaze_Update()
    {
        UpdateTestPhase();
    }

    private void OnEnable()
    {
        Gaze_EventManager.OnTriggerEvent += OnTrigerEvent;
    }

    private void OnDisable()
    {
        Gaze_EventManager.OnTriggerEvent -= OnTrigerEvent;
    }


    /// <summary>
    /// The test will finnish when the trigger is shot
    /// </summary>
    /// <param name="e"></param>
    private void OnTrigerEvent(Gaze_TriggerEventArgs e)
    {
        if ((GameObject)e.Sender == dependentActionTrigger)
        {
            if (e.IsTrigger)
            {
                PassTest();
            }
        }
    }

}
