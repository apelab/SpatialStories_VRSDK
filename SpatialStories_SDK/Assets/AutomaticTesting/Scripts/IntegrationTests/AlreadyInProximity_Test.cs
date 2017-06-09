using UnityEngine;
using UnityTest;
using Gaze;
using System;

/// <summary>
/// Description:
/// We put the cube in the same position than the camera before enabling the game object this way we can test
/// if Gaze SDK is correctly setting the is in proximity variable to true, then we will move the object away and
/// check if the variable is set to false and finally we are going to put back the cube over the camera.
/// 
/// Tests Passes On:
/// If all the isInProximity states are correct in each phase of the test.
/// 
/// Tests Fails On:
/// If the isInProximity states are incorrect in any test phase.
/// </summary>
public class AlreadyInProximity_Test : Gaze_AbstractTest
{

    /// <summary>
    /// Test phases used in the test state machine
    /// </summary>
    private enum TEST_PHASE { ALREADY_IN_PROXIMITY, PROXIMITY_OUT, PROXIMITY_IN, PASSED }
    private TEST_PHASE testPhase = TEST_PHASE.ALREADY_IN_PROXIMITY;

    /// <summary>
    /// The object that we are going to use during the test.
    /// </summary>
    Gaze_InteractiveObject cube;
    Gaze_Conditions cubeConditions;

    public bool ExecuteTest = true;
    
    private void Awake()
    {
        // Get all the test elements
        cube = GameObject.Find("CubeAlreadyInProximity").GetComponent<Gaze_InteractiveObject>();
        cubeConditions = cube.GetComponentInChildren<Gaze_Conditions>();
        cube.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        cube.gameObject.SetActive(true);
    }

    /// <summary>
    /// Make the preparations for the next test phase
    /// </summary>
    /// <param name="_newTestPhase"></param>
    private void ChageTestPhase(TEST_PHASE _newTestPhase)
    {
        SkipUpdates();

        switch (_newTestPhase)
        {
            case TEST_PHASE.ALREADY_IN_PROXIMITY:
                PutObjectInProximityWithOther(cube.gameObject, Camera.main.gameObject);
                break;
            case TEST_PHASE.PROXIMITY_IN:
                PutObjectInProximityWithOther(cube.gameObject, Camera.main.gameObject);
                break;
            case TEST_PHASE.PROXIMITY_OUT:
                PutObjectAwayAllProximities(cube.gameObject);
                break;
            case TEST_PHASE.PASSED:
                PassTest();
                break;
            default:
                break;
        }
        testPhase = _newTestPhase;
    }

    /// <summary>
    /// A machine state that handles the test logic.
    /// </summary>
    private void ProximityTest()
    {
        switch (testPhase)
        {
            case TEST_PHASE.ALREADY_IN_PROXIMITY:
                TestProximityAndMoveToNextPhase(true, TEST_PHASE.PROXIMITY_OUT);
                break;
            case TEST_PHASE.PROXIMITY_OUT:
                TestProximityAndMoveToNextPhase(false, TEST_PHASE.PROXIMITY_IN);
                break;
            case TEST_PHASE.PROXIMITY_IN:
                TestProximityAndMoveToNextPhase(true , TEST_PHASE.PASSED);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// This method checks if the proximity condition especified by parameter is validated and if its true
    /// moves to the next phase.
    /// </summary>
    /// <param name="hasToBeInProximity"></param>
    /// <param name="targetPhase"></param>
    private void TestProximityAndMoveToNextPhase(bool hasToBeInProximity, TEST_PHASE targetPhase)
    {
        if (cubeConditions.isInProximity != hasToBeInProximity)
            FailTest(testPhase.ToString());
        else
            ChageTestPhase(targetPhase);
        
    }

    public override void Gaze_Update()
    {
        ProximityTest();
    }
}
