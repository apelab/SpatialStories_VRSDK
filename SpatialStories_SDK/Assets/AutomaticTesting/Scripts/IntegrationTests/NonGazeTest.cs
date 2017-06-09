using UnityEngine;
using UnityTest;
using Gaze;

/// <summary>
/// Description:
/// In this test we put the CubeToInteract far away from the camera and we verify that
/// the object is not being gazed.
/// 
/// Tests Passes On:
/// If the isBeingGazed flag is not activated after the 90% of the timeout time.
/// 
/// Tests Fails On:
/// If the isBeingGazed flag is set to true before the timeout.
/// </summary>
public class NonGazeTest : Gaze_AbstractTest
{
    Gaze_InteractiveObject cube;

	// Use this for initialization
	void Start () {
        cube = GameObject.Find("CubeToInteract").GetComponent<Gaze_InteractiveObject>();

        // Get the cube out of the camera range
        PutObjectAwayAllProximities(cube.gameObject);
    }

    private void OnEnable()
    {
        // Ask for the test to be succeded just before the timeout happens
        TrySuceedJustBeforeTimeout();
    }

    private void TestMethod()
    {
        if (cube.GetComponentInChildren<Gaze_Conditions>().IsGazed)
            FailTest();       
    }

    public override void Gaze_Update()
    {
        TestMethod();
    }
}
