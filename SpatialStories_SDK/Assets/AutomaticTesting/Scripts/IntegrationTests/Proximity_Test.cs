using UnityEngine;
using UnityTest;
using Gaze;

/// <summary>
/// Description:
/// We put the cube in the same position than the camera.
///
/// Tests Passes On:
/// When the isInProximity condition of the CubeToInteract/Gaze_Action is flagged to true
/// 
/// Tests Fails On:
/// Timeout of 5 seconds
/// </summary>
public class Proximity_Test : Gaze_AbstractTest
{

    Gaze_InteractiveObject cube;
    public GameObject Camera;

	// Use this for initialization
	void Start () {
        cube = GameObject.Find("ProximityCube").GetComponent<Gaze_InteractiveObject>();
        PutObjectInProximityWithOther(cube.gameObject, Camera);
    }
	
    private void ProximityTest()
    {
        if (cube.GetComponentInChildren<Gaze_Conditions>().isInProximity)
            PassTest();
    }

    public override void Gaze_Update()
    {
        ProximityTest();
    }
}
