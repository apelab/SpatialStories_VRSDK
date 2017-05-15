using UnityEngine;
using UnityTest;
using Gaze;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// Description:
/// In this test we are going to try if we are able to teleport to a non allowed zone
/// 
/// Tests Passes On:
/// If we don't move at all.
/// 
/// Tests Fails On:
/// IF our position or rotation changes.
/// </summary>
public abstract class TeleportTest : Gaze_AbstractTest
{
    protected enum TEST_PHASE { WAIT, BEFORE_TELEPORTING, AFTER_TELEPORTING }
    protected TEST_PHASE actualTestPhase = TEST_PHASE.BEFORE_TELEPORTING;

    protected Vector3 originalPosition;
    protected Quaternion originalRotation;
    protected float cameraHeight;

    public GameObject TeleportPoint;

    public Gaze_Teleporter gazeTeleporter;
    public GameObject WorkingCamera;

    private void Start()
    {
    }

    private void OnEnable()
    {
        StoreActualCameraValues();
    }

    protected void StoreActualCameraValues()
    {
        originalPosition = WorkingCamera.transform.position;
        originalRotation = WorkingCamera.transform.rotation;

        WorkingCamera.GetComponentInChildren<Gaze_Camera>().ReconfigureCamera();

        cameraHeight = GetCameraHeight();
    }

    protected float GetCameraHeight()
    {
        return WorkingCamera.GetComponentInChildren<Gaze_Teleporter>().GetPlayerHeight();
    }

    protected void FailIfPositionChanged()
    {
        if (originalPosition != WorkingCamera.transform.position)
        {
            FailTest("The Camera Rig has moved!");
        }
    }

    protected void FailIfRotationChanged()
    {
        if (originalRotation != WorkingCamera.transform.rotation)
        {
            FailTest("The Camera Rig has performed an ilegal rotation");
        }
    }

    protected void FailIfCameraHeightHasChanged()
    {
        float actualPlayerHeigth = GetCameraHeight();

        if (Mathf.Approximately((float)Math.Round(actualPlayerHeigth, 1), (float)Math.Round(cameraHeight, 1)) == false)
        {
            FailTest("The camera height respect the ground has changed! \n"
             + "Original: " + cameraHeight.ToString() + "\n" +
             "Actual: " + actualPlayerHeigth.ToString());
        }
    }

    protected void FailIfPositionIsNot(Vector3 positionToBe)
    {
        GameObject camera = WorkingCamera.GetComponentInChildren<Camera>().gameObject;
        Vector2 parentCameraPosition = new Vector2(WorkingCamera.transform.position.x, WorkingCamera.transform.position.z);
        if (Vector2.Distance(
                parentCameraPosition,
                new Vector2(positionToBe.x, positionToBe.z)) > 0.1f)
        {
            FailTest("The camera is not on the position that it was intended to be!" +
            "\n Position to be in: " + positionToBe.ToString() +
             "\n Camera Position: " + parentCameraPosition.ToString());
        }
    }
}
