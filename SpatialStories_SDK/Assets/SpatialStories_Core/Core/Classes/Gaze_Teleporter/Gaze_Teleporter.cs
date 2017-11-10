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
    private static bool isteleportAllowed = true;
    public static bool IsTeleportAllowed
    {
        get
        {
            return isteleportAllowed;
        }
        set
        {
            isteleportAllowed = value;
        }
    }


    public float HoldTimeToAppear = 0.2f;
    public GameObject GyroPrefab;
    public bool OrientOnTeleport = true;
    public float InptuThreshold = .5f;
    public LayerMask AllowedLayers;
    public float MaxTeleportDistance = 5f;
    public float MaxSlope = 10f;
    public float Cooldown = 1.0f;
    public float MatScale = 5;
    public Color GoodDestinationColor = new Color(0, 0.6f, 1f, 0.2f);
    public Color BadDestinationColor = new Color(0.8f, 0, 0, 0.2f);
    public GameObject GyroUI;
    public float LineWidth = 0.05f;
    public Material LineMaterial;

    public List<Transform> HotSpots;
    public float MinHotspotDistance = 1f;

    private Vector3 finalHitLocation = new Vector3();
    private Vector3 finalHitNormal = new Vector3();
    internal GameObject gyroInstance;
    internal Vector3 gyroUIOriginalAngles;
    internal float angle;
    internal Transform lastParent;
    internal bool transitioning;
    internal LineRenderer _lineRenderer;
    internal Quaternion _roomRotation;
    internal Vector3 _roomPosition;
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
    internal Transform teleportOrigin;
    internal Gaze_TeleportLogic actualTeleportLogic = null;
    internal Collider[] CameraColliders;

    public new bool enabled
    {
        set
        {
            base.enabled = value;
        }
        get
        {
            return base.enabled;
        }
    }

    private float timeHoldingButton = 0;

    private Gaze_TeleportEventArgs gaze_TeleportEventArgs;

    // stores the current TeleportMode while teleport is enables
    private Gaze_TeleportMode lastTeleportMode;
    #endregion

    void Awake()
    {
        // NOTE Hotfix to prevent BaL_PlatformMenu to disable the teleport
        IsTeleportAllowed = true;
    }

    void OnEnable()
    {

        Gaze_InputManager.OnControlerSetup += OnControlerSetup;
        Gaze_HandsReplacer.OnHandsReplaced += Gaze_HandsReplacer_OnHandsReplaced;

        if (actualTeleportLogic != null)
        {
            actualTeleportLogic.Dispose();
            actualTeleportLogic.Setup();
        }

        lastParent = transform.parent;
    }

    private bool CheckIfControllerEnabled()
    {
        // get the active controller
        bool leftHandActive = Gaze_InputManager.instance.LeftHandActive ? true : false;
        bool rightHandActive = Gaze_InputManager.instance.RightHandActive ? true : false;

        if (debug)
            Debug.Log(this + " is active = " + ((leftHandActive && GetComponentInChildren<Gaze_GrabManager>().isLeftHand) || (!leftHandActive && !GetComponentInChildren<Gaze_GrabManager>().isLeftHand)));

        // if the concerned hand is not activated in the camera input manager, exit
        if ((!rightHandActive && !GetComponentInChildren<Gaze_GrabManager>().isLeftHand) ||
            (!leftHandActive && GetComponentInChildren<Gaze_GrabManager>().isLeftHand))
            return false;

        return true;
    }

    void OnDisable()
    {
        Gaze_InputManager.OnControlerSetup -= OnControlerSetup;
        Gaze_HandsReplacer.OnHandsReplaced -= Gaze_HandsReplacer_OnHandsReplaced;
        if (actualTeleportLogic != null)
            actualTeleportLogic.Dispose();
    }

    void Start()
    {
        if (!CheckIfControllerEnabled())
        {
            enabled = false;
            return;
        }

        InstanciateGyroPrefab();

        gyroUIOriginalAngles = GyroPrefab.transform.eulerAngles;

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
        GyroUI = gyroInstance.GetComponentInChildren<Gaze_GyroTarget>().gameObject;

        lastTeleportTime = -Cooldown;

        GameObject arcParentObject = new GameObject("ArcTeleporter");
        arcParentObject.transform.localScale = cameraRigIO.transform.localScale;
        GameObject arcLine1 = new GameObject("ArcLine1");

        arcLine1.transform.SetParent(arcParentObject.transform);
        _lineRenderer = arcLine1.AddComponent<LineRenderer>();
        GameObject arcLine2 = new GameObject("ArcLine2");
        arcLine2.transform.SetParent(arcParentObject.transform);
        _lineRenderer.startWidth = LineWidth * cameraRigIO.transform.localScale.magnitude;
        _lineRenderer.endWidth = LineWidth * cameraRigIO.transform.localScale.magnitude;
        _lineRenderer.material = LineMaterial;
        _lineRenderer.SetPosition(0, Vector3.zero);
        _lineRenderer.SetPosition(1, Vector3.zero);

        if (playerHeightBeforeTeleport == 0)
            playerHeightBeforeTeleport = GetPlayerHeight();

        CameraColliders = cameraRigIO.GetComponentsInChildren<Collider>();
        gaze_TeleportEventArgs = new Gaze_TeleportEventArgs(this);

        teleportOrigin = GetComponentInChildren<Gaze_GrabPositionController>().transform;
    }

    void Update()
    {
        if (teleportActive || actualTeleportLogic is Gaze_ViveTeleport)
        {
            actualTeleportLogic.Update();
        }
    }

    private void OnControlerSetup(Gaze_Controllers actualController)
    {
        if (actualTeleportLogic != null)
            return;

        SetupActualTeleportLogic(actualController);
    }

    public void SetupActualTeleportLogic(Gaze_Controllers _actualController)
    {
        switch (_actualController)
        {
            case Gaze_Controllers.HTC_VIVE:
                actualTeleportLogic = new Gaze_ViveTeleport(this);
                break;
            case Gaze_Controllers.OCULUS_RIFT:
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

    void Gaze_HandsReplacer_OnHandsReplaced(Gaze_GrabManager grabManager, Transform GrabTarget, GameObject DistantGrabObject)
    {
        teleportOrigin = GrabTarget;
    }

    public void InstanciateGyroPrefab()
    {
        if (gyroInstance != null)
            Destroy(gyroInstance);

        gyroInstance = Instantiate(GyroPrefab);

        if (OrientOnTeleport)
            gyroInstance.transform.Find("GyroSprite").GetComponent<SpriteRenderer>().enabled = true;
        else
            gyroInstance.transform.Find("GyroSpriteNoRotation").GetComponent<SpriteRenderer>().enabled = true;
    }

    /// <summary>
    /// Position the gyro UI at the tip of the teleport arc.
    /// </summary>
    /// <param name="hit">Hit.</param>
    private void Gyro(Vector3 _pos, Vector3 _normal)
    {
        gyroInstance.transform.position = _pos;
        gyroInstance.transform.localEulerAngles = new Vector3(_normal.x, angle, _normal.z);
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
    /// <param name="enable"></param>\
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
        hits = Physics.RaycastAll(ray, 4f, AllowedLayers.value);
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
            if (go.layer == AllowedLayers)
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
        timeHoldingButton += Time.deltaTime;

        // Ensure that we don't show teleport until the button hold time has passed
        if (timeHoldingButton < HoldTimeToAppear)
        {
            if (_lineRenderer.enabled)
                _lineRenderer.enabled = false;
            if (gyroInstance.activeSelf)
                gyroInstance.SetActive(false);
            return;
        }
        else
        {
            if (!_lineRenderer.enabled)
                _lineRenderer.enabled = true;

            if (!gyroInstance.activeSelf)
                gyroInstance.SetActive(true);
        }

        //	Line renderer position storage (two because line renderer texture will stretch if one is used)
        List<Vector3> positions1 = new List<Vector3>();

        //	first Vector3 positions array will be used for the curve and the second line renderer is used for the straight down after the curve
        float totalDistance1 = 0;

        //	Variables need for curve
        Quaternion currentRotation = transform.rotation;
        Vector3 currentPosition = teleportOrigin.transform.position;
        Vector3 lastPostion;
        positions1.Add(currentPosition);

        lastPostion = transform.position - transform.forward;
        Vector3 currentDirection = transform.forward;
        Vector3 downForward = new Vector3(transform.forward.x * 0.01f, -1, transform.forward.z * 0.01f);
        RaycastHit hit = new RaycastHit();
        finalHitLocation = new Vector3(float.MinValue, float.MinValue, float.MinValue);

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
            //newRay.origin = currentPosition;
            //newRay.direction = currentPosition - lastPostion;

            float length = (MaxTeleportDistance * 0.01f) * cameraRigIO.transform.localScale.magnitude;
            if (currentRotation == downQuat)
            {
                //We have finished the arc and are facing down
                //So were going to use the second line renderer and extend the normal length as a last effort to hit something
                length = (MaxTeleportDistance * MatScale) * cameraRigIO.transform.localScale.magnitude;
                positions1.Add(currentPosition);
            }

            float raycastLength = length * 1.1f;

            //	Check if we hit something
            bool hitSomething = Physics.Raycast(newRay, out hit, raycastLength, AllowedLayers);

            // don't allow to teleport to negative normals (we don't want to be stuck under floors)
            if (hit.normal.y > 0)
            {
                if (!CloseToHotSpot(hit.point) && hitSomething)
                {
                    finalHitLocation = hit.point;
                    finalHitNormal = hit.normal;
                }

                totalDistance1 += (currentPosition - finalHitLocation).magnitude;
                positions1.Add(finalHitLocation);

                _destinationNormal = finalHitNormal;

                // add rotation at the tip of arc to orient camera when teleport occurs
                Gyro(finalHitLocation, finalHitNormal);

                break;
            }

            //	Convert the rotation to a forward vector and apply to our current position
            currentDirection = currentRotation * Vector3.forward;
            lastPostion = currentPosition;
            currentPosition += currentDirection * length;

            totalDistance1 += length;
            positions1.Add(currentPosition);

            //	If we're pointing down then we did the whole arc and down without hitting anything so we're done
            if (currentRotation == downQuat)
                break;
        }

        //	Decide using the current teleport rule whether this is a good teleporting spot or not
        _goodSpot = IsGoodSpot(finalHitLocation);

        //	Update line, teleport highlight and room highlight based on it being a good spot or bad
        if (_goodSpot)
        {
            _lineRenderer.enabled = true;
            gyroInstance.SetActive(true);

            _lineRenderer.startColor = GoodDestinationColor;
            _lineRenderer.endColor = GoodDestinationColor;


            // If we need to redirect the line
            if (CloseToHotSpot(hit.point))
            {
                // Remove the 30 percent of the points
                int pointsToRemove = Mathf.FloorToInt(positions1.Count * 0.80f);
                positions1.RemoveRange(pointsToRemove, positions1.Count - pointsToRemove);

                positions1.Add(finalHitLocation);

                // Create the second curve by using the points array
                MakeSmoothCurve(positions1, 1f);

                // Assing the new curve to the positions
                positions1 = curvedPoints;
            }

            _lineRenderer.positionCount = positions1.Count;
            _lineRenderer.SetPositions(positions1.ToArray());
        }
        else
        {
            gyroInstance.SetActive(false);

            _lineRenderer.startColor = BadDestinationColor;
            _lineRenderer.endColor = BadDestinationColor;

            _lineRenderer.positionCount = positions1.Count;
            _lineRenderer.SetPositions(positions1.ToArray());
        }
    }

    private bool CloseToHotSpot(Vector3 _hit)
    {
        for (int i = 0; i < HotSpots.Count; i++)
        {
            if (Vector3.Distance(HotSpots[i].position, _hit) < MinHotspotDistance)
            {
                finalHitLocation = HotSpots[i].position;
                return true;
            }
        }

        return false;
    }

    private void CheckSpotChange()
    {
        if (lastTeleportMode.Equals(gaze_TeleportEventArgs.Mode))
            return;

        lastTeleportMode = gaze_TeleportEventArgs.Mode;
        Gaze_EventManager.FireTeleportEvent(gaze_TeleportEventArgs);
    }

    //	Overide and change to expand on what is a good landing spot
    virtual public bool IsGoodSpot(Vector3 _pos)
    {
        if (_pos == null)
        {
            gaze_TeleportEventArgs.Mode = Gaze_TeleportMode.BAD_DESTINATION;
            CheckSpotChange();
            return false;
        }

        //check if height delta is ok
        if (Mathf.Abs(GetComponentInParent<Gaze_InputManager>().transform.position.y - _pos.y) > MaxSlope)
        {
            gaze_TeleportEventArgs.Mode = Gaze_TeleportMode.BAD_DESTINATION;
            CheckSpotChange();
            return false;
        }

        gaze_TeleportEventArgs.Mode = Gaze_TeleportMode.GOOD_DESTINATION;
        CheckSpotChange();

        return true;
    }

    virtual public void EnableTeleport()
    {
        if (gyroInstance == null)
            return;

        teleportActive = true;

        // fire event
        gaze_TeleportEventArgs.Mode = Gaze_TeleportMode.ACTIVATED;
        Gaze_EventManager.FireTeleportEvent(gaze_TeleportEventArgs);
    }

    virtual public void DisableTeleport()
    {
        timeHoldingButton = 0;
        if (gyroInstance == null)
            return;

        gyroInstance.SetActive(false);
        teleportActive = false;
        if (_lineRenderer != null)
            _lineRenderer.enabled = false;
    }

    virtual public void Teleport()
    {
        Teleport(gyroInstance.transform.position);
    }

    public void SetTeleportRotation(Quaternion _rotation)
    {
        gyroInstance.transform.rotation = _rotation;
    }

    virtual public void Teleport(Vector3 _position, bool _checkIfGoodSpot = false, bool _byScript = false)
    {
        if (!_byScript && timeHoldingButton < HoldTimeToAppear)
            return;

        if (transitioning || (Time.time - lastTeleportTime) < Cooldown)
            return;

        bool isGoodSpoot = _goodSpot;

        // This is used in integration tests
        if (_checkIfGoodSpot)
        {
            Ray newRay = new Ray(_position + Vector3.up, Vector3.down);
            RaycastHit hit;
            teleportActive = true;
            bool hitSomething = Physics.Raycast(newRay, out hit, 2f, AllowedLayers);
            if (hitSomething)
            {
                isGoodSpoot = IsGoodSpot(_position);
            }
            else
                isGoodSpoot = false;
        }

        if (teleportActive && isGoodSpoot || _byScript)
        {
            if (OrientOnTeleport)
                RotateCamera();

            // If the user haven't specified a position we are going to choose the gyro position
            MoveToTarget(_position);
            lastTeleportTime = Time.time;

            // fire event
            gaze_TeleportEventArgs.Mode = Gaze_TeleportMode.TELEPORT;
            Gaze_EventManager.FireTeleportEvent(gaze_TeleportEventArgs);
        }
    }

    public void MoveToGyro()
    {
        if (OrientOnTeleport)
            RotateCamera();

        MoveToTarget(gyroInstance.transform.position);
    }

    internal bool IsInputValid()
    {
        return axisValue >= InptuThreshold;
    }

    private void OnLevelWasLoaded(int level)
    {
        // Destroy the actual teleport logic
        if (actualTeleportLogic != null)
            actualTeleportLogic.Dispose();

        actualTeleportLogic = null;
        SetupActualTeleportLogic(Gaze_InputManager.instance.CurrentController);
        InstanciateGyroPrefab();

        if (actualTeleportLogic != null)
            actualTeleportLogic.Dispose();
    }

    static List<Vector3> curvedPoints = new List<Vector3>();
    static Vector3 lastPointInCurve = Vector3.zero;
    public bool debug = false;
    private bool isScriptEnabled;
    static List<Vector3> points;

    public static Vector3[] MakeSmoothCurve(List<Vector3> _arrayToCurve, float _smoothness)
    {

        if (Vector3.Distance(_arrayToCurve[0], lastPointInCurve) > 0.001f)
        {
            curvedPoints.Clear();
            lastPointInCurve = _arrayToCurve[0];
            int pointsLength = 0;
            int curvedLength = 0;

            if (_smoothness < 1.0f) _smoothness = 1.0f;

            pointsLength = _arrayToCurve.Count;

            curvedLength = (pointsLength * Mathf.RoundToInt(_smoothness)) - 1;

            // Don't create the array all the time
            if (curvedPoints == null)
                curvedPoints = new List<Vector3>(curvedLength);

            float t = 0.0f;
            for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
            {
                t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

                points = new List<Vector3>(_arrayToCurve);

                for (int j = pointsLength - 1; j > 0; j--)
                {
                    for (int i = 0; i < j; i++)
                    {
                        points[i] = (1 - t) * points[i] + t * points[i + 1];
                    }
                }

                curvedPoints.Add(points[0]);
            }
            return (curvedPoints.ToArray());
        }
        else
        {
            return curvedPoints.ToArray();
        }


    }
}