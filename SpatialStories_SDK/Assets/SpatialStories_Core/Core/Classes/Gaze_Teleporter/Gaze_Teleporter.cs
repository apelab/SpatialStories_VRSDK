//========= Copyright 2016, Sam Tague, All rights reserved. ===========
//
// Attach to either or both tracked controller objects in SteamVR camera rig
//
//=============================================================================

// <copyright file="Gaze_Teleporter.cs" company="apelab sàrl">
// © apelab. All Rights Reserved.
//
// This source is subject to the apelab license.
// All other rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// </copyright>
// <author>Michaël Martin</author>
// <email>dev@apelab.ch</email>
// <web>https://twitter.com/apelab_ch</web>
// <web>http://www.apelab.ch</web>
// <date>2014-06-01</date>
using Gaze;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Gaze_GearVrTeleport")]
[assembly: InternalsVisibleTo("Gaze_GenericTeleport")]
[assembly: InternalsVisibleTo("Gaze_TeleportLogic")]
public class Gaze_Teleporter : MonoBehaviour
{
    #region Members declaration

    /// <summary>
    /// If this flag is set to false the user won't be able to teleport
    /// </summary>
    public static bool IsTeleportAllowed = true;

    public enum Transition
    {
        INSTANT,
        FADE,
        DASH
    }

    public GameObject gyroPrefab;
    public bool OrientOnTeleport = true;
    public float inputThreshold = .5f;
    public LayerMask teleportLayer;
    public Transition transition;
    public Material fadeMat;
    public float fadeDuration = 0.5f;
    public float gravity = 9f;
    public float initialVelMagnitude = 10f;
    public float timeStep = 0.1f;
    public float maxDistance = 5f;
    public float dashSpeed = 20f;
    public float teleportCooldown = 1.0f;
    public float matScale = 5;
    public Vector2 texMovementSpeed = new Vector2(-0.1f, 0);
    public Color goodSpotCol = new Color(0, 0.6f, 1f, 0.2f);
    public Color badSpotCol = new Color(0.8f, 0, 0, 0.2f);
    public Color gyroColor = new Color(0, 0.6f, 1f, 0.2f);
    public GameObject gyroUI;
    public float arcLineWidth = 0.05f;
    public Material arcMaterial;

    internal GameObject gyroInstance;
    internal Vector3 gyroUIOriginalAngles;
    internal float angle;
    internal Transform lastParent;
    internal MeshRenderer fadeQuad;
    internal Color fadeColour;
    internal bool transitioning;
    internal LineRenderer _lineRenderer;
    internal LineRenderer _lineRenderer2;
    internal Quaternion _roomRotation;
    internal Vector3 _roomPosition;
    internal Vector3 _destination;
    internal Vector3 _destinationNormal;
    internal Vector2 oldTrackpadAxis = Vector2.zero;
    internal float lastTeleportTime;
    internal bool _goodSpot;
    internal bool teleportActive;
    internal Ray ray;
    internal RaycastHit[] hits;
    internal static float playerHeightBeforeTeleport;
    internal GameObject cameraRigIO, cam;
    internal Vector3 gyroLocalEulerAngles;
    internal float axisValue;

    internal Gaze_TeleportLogic actualTeleportLogic = null;

    internal Collider[] CameraColliders;

    private Gaze_TeleportEventArgs gaze_TeleportEventArgs;

    #endregion


    private void OnControlerSetup(Gaze_Controllers actualController)
    {
        if (actualTeleportLogic != null)
            return;
        switch (actualController)
        {
            case Gaze_Controllers.GENERIC:
                //Hack
                actualTeleportLogic = new Gaze_GenericTeleport(this);
                break;
            case Gaze_Controllers.GEARVR_CONTROLLER:
                actualTeleportLogic = new Gaze_GearVrTeleport(this);
                break;
            default:
                break;
        }

        if (actualTeleportLogic != null)
            actualTeleportLogic.Setup();
    }

    void OnEnable()
    {
        Gaze_InputManager.OnControlerSetup += OnControlerSetup;

        if (actualTeleportLogic != null)
            actualTeleportLogic.Setup();

        lastParent = transform.parent;
    }

    void OnDisable()
    {
        Gaze_InputManager.OnControlerSetup -= OnControlerSetup;
        if (actualTeleportLogic != null)
            actualTeleportLogic.Dispose();
    }

    void Start()
    {

        gyroInstance = Instantiate(gyroPrefab);

        if (OrientOnTeleport)
            gyroInstance.transform.Find("GyroSprite").GetComponent<SpriteRenderer>().enabled = true;
        else
            gyroInstance.transform.Find("GyroSpriteNoRotation").GetComponent<SpriteRenderer>().enabled = true;


        gyroUIOriginalAngles = gyroPrefab.transform.eulerAngles;

        if (Camera.main == null)
        {
            Debug.LogError("No camera found !");
            enabled = false;
            return;
        }

        Gaze_Camera gazeCamera = Camera.main.transform.gameObject.GetComponentInChildren<Gaze_Camera>();
        if (gazeCamera.IsReconfiguiringNeeded)
            gazeCamera.ReconfigureCamera();

        cam = gazeCamera.gameObject;
        cameraRigIO = gazeCamera.GetComponentInParent<Gaze_InputManager>().gameObject;
        gyroUI = gyroInstance.GetComponentInChildren<Gaze_GyroTarget>().gameObject;

        lastTeleportTime = -teleportCooldown;

        GameObject arcParentObject = new GameObject("ArcTeleporter");
        arcParentObject.transform.localScale = cameraRigIO.transform.localScale;
        GameObject arcLine1 = new GameObject("ArcLine1");
        arcLine1.transform.SetParent(arcParentObject.transform);
        _lineRenderer = arcLine1.AddComponent<LineRenderer>();
        GameObject arcLine2 = new GameObject("ArcLine2");
        arcLine2.transform.SetParent(arcParentObject.transform);
        _lineRenderer2 = arcLine2.AddComponent<LineRenderer>();
        _lineRenderer.startWidth = arcLineWidth * cameraRigIO.transform.localScale.magnitude;
        _lineRenderer.endWidth = arcLineWidth * cameraRigIO.transform.localScale.magnitude;
        _lineRenderer.material = arcMaterial;
        _lineRenderer2.material = arcMaterial;

        _lineRenderer2.startWidth = arcLineWidth * cameraRigIO.transform.localScale.magnitude;
        _lineRenderer2.endWidth = arcLineWidth * cameraRigIO.transform.localScale.magnitude;
        _lineRenderer.enabled = false;
        _lineRenderer2.enabled = false;

        playerHeightBeforeTeleport = GetPlayerHeight();

        CameraColliders = cameraRigIO.GetComponentsInChildren<Collider>();
        gaze_TeleportEventArgs = new Gaze_TeleportEventArgs(this);
    }

    void Update()
    {
        //AlterColliders(false);
        if (teleportActive)
        {
            actualTeleportLogic.Update();
        }
        //AlterColliders(true);
    }


    /// <summary>
    /// Position the gyro UI at the tip of the teleport arc.
    /// </summary>
    /// <param name="hit">Hit.</param>
    private void Gyro(RaycastHit _hit)
    {
        gyroInstance.transform.position = _hit.point;
        //		gyroInstance.transform.up = hit.normal;
        gyroInstance.transform.localEulerAngles = new Vector3(_hit.normal.x, angle, _hit.normal.z);
    }

    private void MoveToTarget(Vector3 _position)
    {
        //AlterColliders(false);
        // Get the correct offset between the camera position and the origin of the "user space"
        Vector3 offset = cam.transform.position - cameraRigIO.transform.position;

        // Then teleport the player to the destination having in account this offset
        cameraRigIO.transform.position = new Vector3(_position.x - offset.x, _position.y + playerHeightBeforeTeleport, _position.z - offset.z);

        //AlterColliders(true);


    }

    /// <summary>
    /// This is a hack that allows us to detect that a GO has been teleported
    /// </summary>
    /// <param name="enable"></param>
    public void AlterColliders(bool enable)
    {
        for (int i = 0; i < CameraColliders.Length; i++)
            CameraColliders[i].enabled = enable;
    }

    public float GetPlayerHeight()
    {
        cameraRigIO = GetComponentInParent<Gaze_InputManager>().gameObject;
        ray = new Ray(cameraRigIO.transform.position, Vector3.down);

        // raycast on chosen layers only
        hits = Physics.RaycastAll(ray, 4f, teleportLayer.value);
        if (hits.Length <= 0)
            return GetDifferenceBetweenClosestPlaneAndPlayer();
        hits = SortArray(hits);
        return hits[0].distance;
    }


    private float GetDifferenceBetweenClosestPlaneAndPlayer()
    {
        float heightToreturn = 1.6f;

        GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();
        List<GameObject> allTeleportZones = new List<GameObject>();

        foreach (GameObject go in allGameObjects)
        {
            if (go.layer == teleportLayer)
                allTeleportZones.Add(go);
        }

        if (allTeleportZones.Count == 0)
            return heightToreturn;

        float possibleHeight = float.MaxValue;
        foreach (GameObject spot in allTeleportZones)
        {
            float distanceBetweenGroundAndCamera = cameraRigIO.transform.position.y - spot.transform.position.y;
            if (distanceBetweenGroundAndCamera < possibleHeight && distanceBetweenGroundAndCamera > 0)
                possibleHeight = distanceBetweenGroundAndCamera;
        }
        return possibleHeight;
    }

    private static RaycastHit[] SortArray(RaycastHit[] array)
    {
        int length = array.Length;

        RaycastHit temp = array[0];

        for (int i = 0; i < length; i++)
        {
            for (int j = i + 1; j < length; j++)
            {
                if (array[i].distance < array[j].distance)
                {
                    temp = array[i];

                    array[i] = array[j];

                    array[j] = temp;
                }
            }
        }

        return array;
    }

    /// <summary>
    /// Rotates the camera aligned with the Gyro UI arrow's direction when teleport occurs.
    /// </summary>
    private void RotateCamera()
    {
        // Set the rotation of the camera rig correctly
        cameraRigIO.transform.forward = gyroInstance.transform.forward;
        cameraRigIO.transform.rotation = Quaternion.Euler(cameraRigIO.transform.rotation.eulerAngles - new Vector3(0, cam.transform.localRotation.eulerAngles.y, 0));
    }

    internal void CalculateArc()
    {
        //	Line renderer position storage (two because line renderer texture will stretch if one is used)
        List<Vector3> positions1 = new List<Vector3>();
        List<Vector3> positions2 = new List<Vector3>();

        //	first Vector3 positions array will be used for the curve and the second line renderer is used for the straight down after the curve
        bool useFirstArray = true;
        RaycastHit hit = new RaycastHit();
        float totalDistance1 = 0;
        float totalDistance2 = 0;

        //	Variables need for curve
        Quaternion currentRotation = transform.rotation;
        Vector3 currentPosition;
        currentPosition = transform.position;
        Vector3 lastPostion;
        positions1.Add(currentPosition);

        lastPostion = transform.position - transform.forward;
        Vector3 currentDirection = transform.forward;
        Vector3 downForward = new Vector3(transform.forward.x * 0.01f, -1, transform.forward.z * 0.01f);

        //	Advance arc each iteration looking for a surface or until pointed staight down
        //	Should never come close to 500 iterations but just as safety to avoid indefinite looping
        int i = 0;
        while (i < 500)
        {
            i++;

            //	Rotate the current rotation toward the downForward rotation
            Quaternion downQuat = Quaternion.LookRotation(downForward);
            currentRotation = Quaternion.RotateTowards(currentRotation, downQuat, 1f);

            //	Make ray for new direction
            Ray newRay = new Ray(currentPosition, currentPosition - lastPostion);
            float length = (maxDistance * 0.01f) * cameraRigIO.transform.localScale.magnitude;
            if (currentRotation == downQuat)
            {
                //We have finished the arc and are facing down
                //So were going to use the second line renderer and extend the normal length as a last effort to hit something
                useFirstArray = false;
                length = (maxDistance * matScale) * cameraRigIO.transform.localScale.magnitude;
                positions2.Add(currentPosition);
            }
            float raycastLength = length * 1.1f;

            //	Check if we hit something
            bool hitSomething = Physics.Raycast(newRay, out hit, raycastLength, teleportLayer);

            // @ Apelab don't allow to teleport to negative normals
            if (hitSomething && hit.normal.y > 0)
            {

                //	Depending on whether we had switched to the first or second line renderer
                //	add the point and finish calculating the total distance
                if (useFirstArray)
                {
                    totalDistance1 += (currentPosition - hit.point).magnitude;
                    positions1.Add(hit.point);
                }
                else
                {
                    totalDistance2 += (currentPosition - hit.point).magnitude;
                    positions2.Add(hit.point);
                }
                _destinationNormal = hit.normal;
                //	And we're done

                // @apelab add rotation at the tip of arc to orient camera when teleport occurs
                Gyro(hit);

                break;
            }

            //	Convert the rotation to a forward vector and apply to our current position
            currentDirection = currentRotation * Vector3.forward;
            lastPostion = currentPosition;
            currentPosition += currentDirection * length;

            //	Depending on whether we have switched to the second line renderer add this point and update total distance
            if (useFirstArray)
            {
                totalDistance1 += length;
                positions1.Add(currentPosition);
            }
            else
            {
                totalDistance2 += length;
                positions2.Add(currentPosition);
            }

            //	If we're pointing down then we did the whole arc and down without hitting anything so we're done
            if (currentRotation == downQuat)
                break;
        }

        if (useFirstArray)
        {
            _lineRenderer2.enabled = false;
            _destination = positions1[positions1.Count - 1];
        }
        else
        {
            _lineRenderer2.enabled = true;
            _destination = positions2[positions2.Count - 1];
        }

        //	Decide using the current teleport rule whether this is a good teleporting spot or not
        _goodSpot = IsGoodSpot(hit);

        //	Update line, teleport highlight and room highlight based on it being a good spot or bad
        if (_goodSpot)
        {
            gyroInstance.SetActive(true);

            _lineRenderer.startColor = goodSpotCol;
            _lineRenderer.endColor = goodSpotCol;
            _lineRenderer2.startColor = goodSpotCol;
            _lineRenderer2.endColor = goodSpotCol;
        }
        else
        {
            gyroInstance.SetActive(false);

            _lineRenderer.startColor = badSpotCol;
            _lineRenderer.endColor = badSpotCol;
            _lineRenderer2.startColor = badSpotCol;
            _lineRenderer2.endColor = badSpotCol;
        }

        _lineRenderer.positionCount = positions1.Count;
        _lineRenderer.SetPositions(positions1.ToArray());

        if (_lineRenderer2.enabled)
        {
            _lineRenderer2.positionCount = positions2.Count;
            _lineRenderer2.SetPositions(positions2.ToArray());
        }
    }

    //	Overide and change to expand on what is a good landing spot
    virtual public bool IsGoodSpot(RaycastHit hit)
    {
        if (hit.transform == null)
            return false;

        return true;
    }

    virtual public void EnableTeleport()
    {
        if (transitioning)
            return;

        if (gyroInstance == null)
            return;

        gyroInstance.SetActive(true);

        teleportActive = true;
        _lineRenderer.enabled = true;
        _lineRenderer2.enabled = true;
    }

    virtual public void DisableTeleport()
    {
        if (gyroInstance == null)
            return;

        gyroInstance.SetActive(false);

        teleportActive = false;
        _lineRenderer.enabled = false;
        _lineRenderer2.enabled = false;
    }

    virtual public void Teleport()
    {
        Teleport(gyroInstance.transform.position);
    }

    virtual public void Teleport(Vector3 _position, bool _checkIfGoodSpot = false)
    {
        if (transitioning || (Time.time - lastTeleportTime) < teleportCooldown)
            return;

        bool isGoodSpoot = _goodSpot;

        // This is used in integration tests
        if (_checkIfGoodSpot)
        {
            Ray newRay = new Ray(_position + Vector3.up, Vector3.down);
            RaycastHit hit;
            transition = Transition.INSTANT;
            teleportActive = true;
            bool hitSomething = Physics.Raycast(newRay, out hit, 2f, teleportLayer);
            if (hitSomething)
            {
                isGoodSpoot = IsGoodSpot(hit);
            }
            else
                isGoodSpoot = false;
        }

        if (teleportActive && isGoodSpoot)
        {
            if (OrientOnTeleport)
                RotateCamera();

            switch (transition)
            {
                case Transition.INSTANT:
                    // If the user haven't specified a position we are going to choose the gyro position
                    MoveToTarget(_position);
                    lastTeleportTime = Time.time;
                    break;
            }

            // Recenter space on the camera position
            RecenterSpaceOnCameraPosition();

            // fire event
            Gaze_EventManager.FireTeleportEvent(gaze_TeleportEventArgs);
        }
    }

    private void RecenterSpaceOnCameraPosition()
    {
        Transform[] cameraRigIOChilds = cameraRigIO.transform.GetComponentsInChildren<Transform>();
        List<Transform> objectsToReparent = new List<Transform>();
        foreach (Transform trans in cameraRigIOChilds)
        {
            if (trans.parent == cameraRigIO.transform)
            {
                trans.parent = null;
                objectsToReparent.Add(trans);
            }
        }

        cameraRigIO.transform.position = new Vector3(cam.transform.position.x, cameraRigIO.transform.position.y, cam.transform.position.z);

        foreach (Transform trans in objectsToReparent)
            trans.parent = cameraRigIO.transform;
    }

    internal bool IsInputValid()
    {
        return axisValue >= inputThreshold;
    }

}
