using UnityEngine;
using UnityTest;
using Gaze;

/// <summary>
/// Description:
/// In this test we move the cube to interact to the position 123, 123, 123 and then
/// we check if the main camera is in proximity with the object or not
/// 
/// Tests Passes On:
/// If the isInProximity flag is not activated after the 90% of the timeout time.
/// 
/// Tests Fails On:
/// If the isInProximity flag is activated before the timeout.
/// </summary>
public class NonProximityTest : Gaze_AbstractTest
{

    private Gaze_InteractiveObject cube;
    private float testSucceedTime;

	// Use this for initialization
	void Start () {
        cube = GameObject.Find("CubeToInteract").GetComponent<Gaze_InteractiveObject>();
        PutObjectAwayAllProximities(cube.gameObject);
    }

    private void OnEnable()
    {
        TrySuceedJustBeforeTimeout();
    }

    private void TestMethod()
    {
        if (cube.GetComponentInChildren<Gaze_Conditions>().isInProximity)
            FailTest();       
    }

    public override void Gaze_Update()
    {
        // Get the cube out of the camera range
        TestMethod();
    }
}
