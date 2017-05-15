using UnityEngine;
using UnityTest;
using Gaze;
using System;

/// <summary>
/// Description:
/// In this test we have 2 game objects, one of them needs to bee looked by the camera to
/// trigger, but before another object has to be in contact with him.
///
/// Tests Passes On:
/// 
/// 
/// Tests Fails On:
/// After a timeout of 5 seconds
/// </summary>
public class Dependencies_AfterProximity : Gaze_AbstractTest
{

    Gaze_InteractiveObject cube;

	// Use this for initialization
	void Start () {
        cube = GameObject.Find("CubeToInteract").GetComponent<Gaze_InteractiveObject>();
        GazeTest();
	}
	
    private void GazeTest()
    {
        if (cube.GetComponentInChildren<Gaze_Conditions>().IsGazed)
            PassTest();
    }

    public override void Gaze_Update()
    {
        PutObjectInfrontOfCamera(cube.gameObject);
        GazeTest();
    }
}
