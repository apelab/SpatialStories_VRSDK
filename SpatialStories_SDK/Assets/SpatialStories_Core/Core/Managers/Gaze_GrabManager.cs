// <copyright file="Gaze_GrabManager.cs" company="apelab sàrl">
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.VR;

/// <summary>
/// Grab manager handles all grab mechanisme.
/// This script is attached, at run time, on the controllers.
/// </summary>
public class Gaze_GrabManager : MonoBehaviour
{
    #region members
    public GameObject collidingObject;
    /// <summary>
    /// Stores all the grab managers in order to make operations with them
    /// </summary>
    public static List<Gaze_GrabManager> GrabManagers = new List<Gaze_GrabManager>();
    //	public SteamController steamController;
    public GameObject grabbedObject;
    /// <summary>
    /// true if this is the left hand controller
    /// </summary>
    public bool isLeftHand;
    public bool displayGrabPointer = false;
    public float laserStartWidth, laserEndWidth, laserLength = 5f;
    public Material laserMaterial;
    public Transform ControllerSnapTransform { get { return controllerSnapTransform; } }
    public bool debug = false;
    public Gaze_InteractiveObject interactableIO;
    public bool IsTriggerPressed { get { return isTriggerPressed; } }

    private Gaze_InteractiveObject closerIO;
    public Transform grabPosition;
    private Vector3 lastPosition;
    private Vector3 lastRotation;
    private short positionsIndex = 0;
    private short samples = 10;
    private Vector3[] positions;
    private Vector3 velocity;
    private Vector3 angularVelocity;
    private bool isGrabbing = false;
    private Transform controllerSnapTransform;
    private LineRenderer laserPointer;
    private RaycastHit[] hits;
    public List<GameObject> hitsIOs;
    private bool isTriggerPressed = false;
    private List<GameObject> objectsInProximity = new List<GameObject>();
    private GameObject distantGrabOrigin;
    private List<GameObject> raycastIOs;
    private Gaze_TouchDistanceMode searchDistanceState;
    private const float MAX_THROW_VELOCITY = 5;
    private const float MAX_DELTA_X = 0.1f;
    private float closerDistance = 0;
    private Vector3 hitPosition;
    private bool intersectsWithInteractiveIO;
    private Dictionary<string, object> analyticsDico;
    private Gaze_ControllerPointingEventArgs gaze_ControllerPointingEventArgs = new Gaze_ControllerPointingEventArgs();
    private Gaze_LaserEventArgs gaze_LaserEventArgs = new Gaze_LaserEventArgs();
    private Gaze_ControllerTouchEventArgs gaze_ControllerTouchEventArgs;

    /// <summary>
    /// Grab states needed to track the grab state process:
    /// SEARCHING = Player has the trigger down and we are raycasting to seach an object.
    /// ATTRACTING = We have an object to take and we are attracting it to us.
    /// GRABBING = We are going to grab the object with tryatach.
    /// GRABBED = The object is on our hand.
    /// EMPTY = We don't have any objects and the user isn't pressing the button.
    /// BLOCKED = Used to diable the grab logic.
    /// </summary>
    private enum GRAB_STATE
    {
        SEARCHING,
        ATTRACTING,
        GRABBING,
        GRABBED,
        EMPTY,
        BLOCKED
    }


    private GRAB_STATE grabState;

    // Distant grab feedbacks

    public float DefaultDistantGrabRayLength = 30f;

    public GameObject InteractableDistantGrabFeedback;
    public Color InteractableDistantGrabColor = Color.blue;
    public Color InteractableInRangeDistantGrabColor = Color.green;

    public GameObject NotInteractableDistantGrabFeedback;
    public Color NotInteractableDistantGrabColor = Color.white;
    private KeyValuePair<VRNode, GameObject> keyValue;
    #endregion


    #region CachedVariables

    // This is used on the function ShowDistantGrabFeedbacks
    private SpriteRenderer intrctvDstntGrbFdbckSprRndrr;

    #endregion CachedVariables

    void OnEnable()
    {
        Gaze_EventManager.OnGrabEvent += OnGrabEvent;
        Gaze_InputManager.OnControllerCollisionEvent += OnControllerCollisionEvent;
        Gaze_InputManager.OnControllerGrabEvent += OnControllerGrabEvent;
        Gaze_InputManager.OnHandRightDownEvent += OnHandRightDownEvent;
        Gaze_InputManager.OnHandRightUpEvent += OnHandRightUpEvent;
        Gaze_InputManager.OnHandLeftDownEvent += OnHandLeftDownEvent;
        Gaze_InputManager.OnHandLeftUpEvent += OnHandLeftUpEvent;
        Gaze_HandsReplacer.OnHandsReplaced += OnHandsReplaced;
        Gaze_EventManager.OnIODestroyed += OnIODestroyed;

        // get the snap location from the controller
        controllerSnapTransform = gameObject.GetComponentInChildren<Gaze_GrabPositionController>().transform;
        grabState = GRAB_STATE.EMPTY;

        // Add this grab manager to the list
        GrabManagers.Add(this);
    }



    void OnDisable()
    {
        Gaze_EventManager.OnGrabEvent -= OnGrabEvent;
        Gaze_InputManager.OnControllerCollisionEvent -= OnControllerCollisionEvent;
        Gaze_InputManager.OnControllerGrabEvent -= OnControllerGrabEvent;
        Gaze_InputManager.OnHandRightDownEvent -= OnHandRightDownEvent;
        Gaze_InputManager.OnHandRightUpEvent -= OnHandRightUpEvent;
        Gaze_InputManager.OnHandLeftDownEvent -= OnHandLeftDownEvent;
        Gaze_InputManager.OnHandLeftUpEvent -= OnHandLeftUpEvent;
        Gaze_HandsReplacer.OnHandsReplaced -= OnHandsReplaced;
        Gaze_EventManager.OnIODestroyed -= OnIODestroyed;

        // Remove this grab manager from the list
        GrabManagers.Remove(this);
    }

    /// <summary>
    /// If this game object gets destroyed we need to remove this reference from the list
    /// </summary>
    private void OnDestroy()
    {
        GrabManagers.Remove(this);
    }

    void Start()
    {
        distantGrabOrigin = GetComponentInChildren<Gaze_DistantGrabPointer>().gameObject;
        positions = new Vector3[samples];
        if (displayGrabPointer)
        {
            if (GetComponent<LineRenderer>() == null)
                laserPointer = gameObject.AddComponent<LineRenderer>();
            else
                laserPointer = GetComponent<LineRenderer>();
            laserPointer.enabled = false;
            laserPointer.startWidth = laserStartWidth;
            laserPointer.endWidth = laserEndWidth;
            laserPointer.positionCount = 2;
            laserPointer.material = laserMaterial;
        }

        raycastIOs = new List<GameObject>();
        hitsIOs = new List<GameObject>();
        SetupDinstanceGrabFeedbacks();
        analyticsDico = new Dictionary<string, object>();

        gaze_ControllerPointingEventArgs.Sender = this.gameObject;
        gaze_LaserEventArgs = new Gaze_LaserEventArgs()
        {
            Sender = this
        };

        keyValue = new KeyValuePair<VRNode, GameObject>();
        gaze_ControllerTouchEventArgs = new Gaze_ControllerTouchEventArgs(this.gameObject);
        gaze_ControllerPointingEventArgs = new Gaze_ControllerPointingEventArgs(this.gameObject, keyValue, true);
        laserPointer.enabled = true;
    }

    void SetupDinstanceGrabFeedbacks()
    {

        if (NotInteractableDistantGrabFeedback != null)
        {
            // First of all let's check if the user has added a prefab or an instance
            if (NotInteractableDistantGrabFeedback.gameObject.scene.name == null) // If it is a prefab we need to instanciate it
            {
                GameObject temp = GameObject.Instantiate(NotInteractableDistantGrabFeedback);
                NotInteractableDistantGrabFeedback = temp;
            }

            // Disable de feedback in oder to hide it.
            NotInteractableDistantGrabFeedback.SetActive(false);
        }

        if (InteractableDistantGrabFeedback != null)
        {
            // First of all let's check if the user has added a prefab or an instance
            if (InteractableDistantGrabFeedback.gameObject.scene.name == null) // If it is a prefab we need to instanciate it
            {
                GameObject temp = GameObject.Instantiate(InteractableDistantGrabFeedback);
                InteractableDistantGrabFeedback = temp;
            }

            intrctvDstntGrbFdbckSprRndrr = InteractableDistantGrabFeedback.GetComponent<SpriteRenderer>();

            // Disable de feedback in oder to hide it.
            InteractableDistantGrabFeedback.SetActive(false);

        }
    }

    void Update()
    {
        UpdateGrabState();
    }

    private void FixedUpdate()
    {
        //Debug.Log(interactableIO +" isTriggerPressed = "+isTriggerPressed);
        if (grabState == GRAB_STATE.EMPTY || grabState == GRAB_STATE.SEARCHING)
            FindValidDistantObject();
        //else
        //    ClearLaserPointer();
    }

    /// <summary>
    /// This list contains objects which are either Grabable, Touchable or Levitable
    /// and colliding with a controller (in proximity).
    /// If an object has been dropped maybe is still on the proximity list but
    /// he wont be removed from it because the colliders has been deactivated
    /// by calling this method all the objects that are in the proximity list
    /// without being catchable anymore will be removed from it.
    /// </summary>
    public void UpdateProximityList()
    {
        Collider myProximity = GetComponentInParent<Gaze_InteractiveObject>().GetComponentInChildren<Gaze_Proximity>().GetComponent<Collider>();

        for (int i = objectsInProximity.Count - 1; i >= 0; i--)
        {
            GameObject go = objectsInProximity[i];

            if (go == null)
            {
                objectsInProximity.RemoveAt(i);
                continue;
            }
            Gaze_InteractiveObject io = go.GetComponent<Gaze_InteractiveObject>();

            if (!io.GetComponentInChildren<Gaze_Proximity>().GetComponent<Collider>().bounds.Intersects(myProximity.bounds))
            {
                objectsInProximity.RemoveAt(i);
                continue;
            }

            // if it's not an Interactive Object OR it's neither grabbable nor touchable, remove it !
            if (io == null || (!io.IsGrabEnabled && !io.IsTouchEnabled))
                objectsInProximity.RemoveAt(i);
        }
    }

    /// <summary>
    /// This is an state machine thats handles all the logic of grabbing an
    /// object.
    /// </summary>
    private void UpdateGrabState()
    {
        switch (grabState)
        {
            case GRAB_STATE.SEARCHING:
                if (debug)
                    Debug.Log("SEARCHING");
                SearchValidObjects();
                break;

            case GRAB_STATE.ATTRACTING:
                if (debug)
                    Debug.Log("ATTRACTING");
                AttractObjectToHand();
                break;

            case GRAB_STATE.GRABBING:
                if (debug)
                    Debug.Log("GRABBING");
                GrabObject();
                break;

            case GRAB_STATE.GRABBED:
                if (debug)
                    Debug.Log("GRABBED");
                UpdateVelocities();
                break;

            case GRAB_STATE.EMPTY:
                if (debug)
                    Debug.Log("EMPTY");
                break;
        }
    }

    private void Touch(Gaze_InteractiveObject _interactableIO)
    {
        if (_interactableIO.IsTouchEnabled)
        {
            // set the dico members
            VRNode vrNode = isLeftHand ? VRNode.LeftHand : VRNode.RightHand;

            // fire the touch event
            gaze_ControllerTouchEventArgs.Dico = new KeyValuePair<VRNode, GameObject>(vrNode, _interactableIO.gameObject);
            gaze_ControllerTouchEventArgs.Mode = searchDistanceState;
            gaze_ControllerTouchEventArgs.IsTouching = true;
            gaze_ControllerTouchEventArgs.IsTriggerPressed = isTriggerPressed;
            Gaze_InputManager.FireControllerTouchEvent(gaze_ControllerTouchEventArgs);

            // analytics
            analyticsDico.Clear();
            analyticsDico.Add("Grabbed Object", _interactableIO.name);
            analyticsDico.Add("Is Left Hand", isLeftHand);
            Analytics.CustomEvent("Grab", analyticsDico);

            // set state to EMPTY
            grabState = GRAB_STATE.EMPTY;
        }
    }

    /// <summary>
    /// Used in the new grab logic in order to search an object to take one important thing about this method is
    /// that it priorizes the objects in manipulation over the distant grab.
    /// Update : this method now search for any valid object which are : Grabable, Touchable OR Levitable
    /// </summary>
    private void SearchValidObjects()
    {
        // update list of objects in proximity with the controller
        UpdateProximityList();

        // if there are no object in proximity, Distant Grab
        if (objectsInProximity.Count == 0)
        {
            // If we've found something to grab, update the grab state
            if (closerIO != null)
            {
                interactableIO = closerIO;

                if (interactableIO.IsGrabEnabled)
                {
                    if (interactableIO.IsBeingGrabbed)
                    {
                        interactableIO.SetManipulationMode(true);
                    }

                    if (interactableIO.GrabModeIndex.Equals((int)Gaze_GrabMode.ATTRACT))
                    {
                        Gaze_GravityManager.ChangeGravityState(interactableIO, Gaze_GravityRequestType.DEACTIVATE_AND_ATTACH);
                        grabState = GRAB_STATE.ATTRACTING;
                    }
                    else if (interactableIO.GrabModeIndex.Equals((int)Gaze_GrabMode.LEVITATE))
                    {
                        grabState = GRAB_STATE.GRABBED;
                        ClearLaserPointer();

                        KeyValuePair<VRNode, GameObject> dico = new KeyValuePair<VRNode, GameObject>(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, interactableIO.gameObject);
                        Gaze_InputManager.FireControllerGrabEvent(new Gaze_ControllerGrabEventArgs(this, dico, true, hitPosition));

                        // analytics
                        analyticsDico.Clear();
                        analyticsDico.Add("Grabbed Object", interactableIO.name);
                        analyticsDico.Add("Is Left Hand", isLeftHand);
                        Analytics.CustomEvent("Grab", analyticsDico);
                    }
                }

                if (interactableIO.IsTouchEnabled)
                {
                    searchDistanceState = Gaze_TouchDistanceMode.DISTANT;
                    Touch(interactableIO);
                }
            }
        }

        // the controller is in collision with an IO
        else
        {
            // get the IO being collided with
            interactableIO = objectsInProximity.ElementAt(0).GetComponent<Gaze_InteractiveObject>();

            if (interactableIO.IsManipulable && !interactableIO.IsBeingManipulated)
            {
                interactableIO.SetManipulationMode(true);
            }

            // if the grabbed object is being manipulated
            if (interactableIO.IsBeingManipulated)
            {
                // get the hand manipulating
                Gaze_GrabManager grManager = GetGrabbingHand();

                // If the other hand is taking this object we need to remove it from this hand 
                if (grManager != null && grManager.grabbedObject == grabbedObject)
                {
                    grManager.grabbedObject = null;
                    grManager.grabState = GRAB_STATE.EMPTY;
                }

                // Set the grab point where the controller actually is in order to avoid weird jumps
                interactableIO.SetGrabPoint(controllerSnapTransform.position);

                // change the state to grabbing.
                grabState = GRAB_STATE.GRABBING;
            }
            else
            {
                // If the object is not being manipulated we just try to Find an object by the distance method
                if (interactableIO.IsTouchEnabled)
                {
                    searchDistanceState = Gaze_TouchDistanceMode.PROXIMITY;
                    Touch(interactableIO);
                }
                else if (interactableIO.IsGrabEnabled)
                {
                    if (interactableIO.GrabModeIndex.Equals((int)Gaze_GrabMode.ATTRACT))
                    {
                        grabState = GRAB_STATE.GRABBING;
                    }
                    else if (interactableIO.GrabModeIndex.Equals((int)Gaze_GrabMode.LEVITATE))
                    {
                        grabState = GRAB_STATE.GRABBED;
                        KeyValuePair<VRNode, GameObject> dico = new KeyValuePair<VRNode, GameObject>(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, interactableIO.gameObject);
                        Gaze_InputManager.FireControllerGrabEvent(new Gaze_ControllerGrabEventArgs(this, dico, true));

                        // analytics
                        analyticsDico.Clear();
                        analyticsDico.Add("Grabbed Object", interactableIO.name);
                        analyticsDico.Add("Is Left Hand", isLeftHand);
                        Analytics.CustomEvent("Grab", analyticsDico);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Attracts the object that we are taking to the grab manager and then changes the state
    /// </summary>
    private void AttractObjectToHand()
    {
        if (interactableIO == null)
        {
            grabState = GRAB_STATE.SEARCHING;
            return;
        }

        // Check if we need to attract more the object and atract it if needed.
        if (IsAttactionNeeded())
        {
            AttractObject();
        }
        else
        {
            // If the object doesn't need to be attracted we can change the state to grabbing.
            grabState = GRAB_STATE.GRABBING;
            interactableIO.transform.position = controllerSnapTransform.position - interactableIO.GetGrabPoint();
        }
    }

    /// <summary>
    /// Performs the grab logic
    /// </summary>
    private void GrabObject()
    {
        ClearLaserPointer();
        TryAttach();
        grabState = GRAB_STATE.GRABBED;
    }

    private void ClearLaserPointer()
    {
        if (laserPointer != null)
            laserPointer.enabled = false;

        if (NotInteractableDistantGrabFeedback != null)
            NotInteractableDistantGrabFeedback.SetActive(false);

        if (InteractableDistantGrabFeedback != null)
            InteractableDistantGrabFeedback.SetActive(false);
    }

    /// <summary>
    /// Moves an object to the hand at a certain speed.
    /// </summary>
    private void AttractObject()
    {
        if (interactableIO == null)
        {
            grabState = GRAB_STATE.SEARCHING;
            return;
        }
        interactableIO.transform.position = Vector3.MoveTowards(interactableIO.transform.position, controllerSnapTransform.position - interactableIO.GetGrabPoint(), interactableIO.AttractionSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Checks if we need to attract the object that we are trying to grab.
    /// </summary>
    /// <returns></returns>
    private bool IsAttactionNeeded()
    {
        return Vector3.Distance(interactableIO.transform.position, controllerSnapTransform.position - interactableIO.GetGrabPoint()) > 0.1f;
    }

    /// <summary>
    /// Returns the position where an object has to be in order to consider itself as 
    /// grabbed
    /// </summary>
    /// <returns></returns>
    private Vector3 CalcObjectGrabPosition()
    {
        return controllerSnapTransform.position - interactableIO.GetGrabPoint();
    }

    /// <summary>
    /// Trows a raycast forward in order to find an object to grab
    /// </summary>
    private void FindValidDistantObject()
    {
        if (distantGrabOrigin == null)
            return;

        hits = Physics.RaycastAll(distantGrabOrigin.transform.position, distantGrabOrigin.transform.forward);
        hitsIOs.Clear();

        closerIO = null;
        float visualRayLength = float.MaxValue;

        // if the raycast hits nothing
        if (hits.Length < 1)
        {
            // notify every previously pointed object they are no longer pointed
            for (int i = 0; i < raycastIOs.Count; i++)
            {
                gaze_ControllerPointingEventArgs.IsPointed = false;
                Gaze_EventManager.FireControllerPointingEvent(gaze_ControllerPointingEventArgs);

                // analytics
                analyticsDico["GrabbedObject"] = raycastIOs[i].name;
                analyticsDico["IsLeftHand"] = isLeftHand;
                Analytics.CustomEvent("ControllerPointing", analyticsDico);
            }

            // clear the list
            raycastIOs.Clear();
        }
        else
        {
            // 1° notify new raycasted objects in hits
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.GetComponent<Gaze_HandHover>() != null)
                {
                    // get the pointed object
                    Gaze_InteractiveObject interactiveObject = hits[i].collider.transform.GetComponentInParent<Gaze_InteractiveObject>();

                    // If the other hand is levitating the object dont consider this as a valid target.
                    if (Gaze_LevitationManager.IsIOBeingLevitatedByHand(interactiveObject, !isLeftHand))
                    {
                        continue;
                    }

                    // populate the list of IOs hit
                    hitsIOs.Add(interactiveObject.gameObject);

                    // if pointed object is not in the list
                    if (!raycastIOs.Contains(interactiveObject.gameObject))
                    {
                        // notify the new pointed object
                        raycastIOs.Add(interactiveObject.gameObject);
                        gaze_ControllerPointingEventArgs.Dico = new KeyValuePair<VRNode, GameObject>(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, interactiveObject.gameObject);
                        gaze_ControllerPointingEventArgs.IsPointed = true;
                        Gaze_EventManager.FireControllerPointingEvent(gaze_ControllerPointingEventArgs);
                    }

                    if (interactiveObject != null)
                    {
                        if (!interactiveObject.IsBeingGrabbed && interactiveObject.IsGrabEnabled && interactiveObject.GrabModeIndex.Equals((int)Gaze_GrabMode.ATTRACT) && hits[i].distance < interactiveObject.GrabDistance)
                        {
                            closerIO = interactiveObject;
                            closerDistance = hits[i].distance;
                            break;
                        }
                        if (interactiveObject.IsTouchEnabled && hits[i].distance < interactiveObject.TouchDistance)
                        {
                            closerIO = interactiveObject;
                            closerDistance = hits[i].distance;
                            break;
                        }
                        if (interactiveObject.IsGrabEnabled && interactiveObject.GrabModeIndex.Equals((int)Gaze_GrabMode.LEVITATE) && hits[i].distance < interactiveObject.GrabDistance)
                        {
                            // update the hit position until we grab something
                            if (grabState != GRAB_STATE.GRABBED)
                                hitPosition = hits[i].point;

                            closerIO = interactiveObject;
                            closerDistance = hits[i].distance;
                            break;
                        }

                        // Get the visual ray length
                        visualRayLength = interactiveObject.IsGrabEnabled && interactiveObject.GrabModeIndex.Equals((int)Gaze_GrabMode.ATTRACT) && visualRayLength > hits[i].distance ? hits[i].distance : visualRayLength;
                    }
                }
            }

            // 2 : notify no longer raycasted objects in raycastIOs
            for (int i = 0; i < raycastIOs.Count; i++)
            {
                if (!hitsIOs.Contains(raycastIOs[i]))
                {
                    // notify
                    gaze_ControllerPointingEventArgs.Dico = new KeyValuePair<VRNode, GameObject>(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, raycastIOs[i]);
                    gaze_ControllerPointingEventArgs.IsPointed = false;
                    Gaze_EventManager.FireControllerPointingEvent(gaze_ControllerPointingEventArgs);

                    // remove it
                    raycastIOs.RemoveAt(i);
                }
            }

        }
        ShowDistantGrabFeedbacks(distantGrabOrigin.transform.position, distantGrabOrigin.transform.forward, visualRayLength, closerIO != null);
    }

    void ShowDistantGrabFeedbacks(Vector3 targetPosition, Vector3 direction, float length, bool inRange)
    {
        if (closerIO != null)
            length = closerDistance;

        intersectsWithInteractiveIO = true;

        if (laserPointer == null)
            return;

        // This will check if the raycast intersecs with a valid grabbable object
        if (length == float.MaxValue)
        {
            length = DefaultDistantGrabRayLength;
            intersectsWithInteractiveIO = false;
        }

        Vector3 endPosition = targetPosition + (length * direction);

        ShowDistantGrabLaser(targetPosition, endPosition, direction, length, intersectsWithInteractiveIO, inRange);
        ShowDistantGrabPointer(intersectsWithInteractiveIO, endPosition, inRange);
    }

    private void ShowDistantGrabLaser(Vector3 targetPosition, Vector3 endPosition, Vector3 direction, float length, bool intersectsWithIO, bool iOInRange)
    {
        if (!displayGrabPointer)
            return;


        //laserPointer.enabled = true;
        laserPointer.SetPosition(0, targetPosition);
        laserPointer.SetPosition(1, endPosition);
        Color actualColor;

        if (iOInRange)
            actualColor = InteractableInRangeDistantGrabColor;
        else if (intersectsWithIO)
            actualColor = InteractableDistantGrabColor;
        else
            actualColor = NotInteractableDistantGrabColor;

        laserPointer.startColor = actualColor;
        laserPointer.endColor = actualColor;
        gaze_LaserEventArgs.StartPosition = targetPosition;
        gaze_LaserEventArgs.EndPosition = endPosition;
        gaze_LaserEventArgs.LaserHits = hits;
        Gaze_EventManager.FireLaserEvent(gaze_LaserEventArgs);
    }

    private void ShowDistantGrabPointer(bool intersectsWithGrabbableIO, Vector3 endPosition, bool IOInRange)
    {
        // Add the object at the end of the ray if needed
        if (NotInteractableDistantGrabFeedback != null)
        {
            // Disable or enable the object
            NotInteractableDistantGrabFeedback.SetActive(!intersectsWithGrabbableIO);

            // Move the feedback object to the end of the ray
            if (!intersectsWithGrabbableIO)
                NotInteractableDistantGrabFeedback.transform.position = endPosition;
        }

        // Add the object at the end of the ray if needed
        if (InteractableDistantGrabFeedback != null)
        {
            // Disable or enable the object
            InteractableDistantGrabFeedback.SetActive(intersectsWithGrabbableIO);

            // Change the pointer color in order to distinguish if the object is in range or not.
            if (intrctvDstntGrbFdbckSprRndrr)
                intrctvDstntGrbFdbckSprRndrr.color = IOInRange ? InteractableInRangeDistantGrabColor : InteractableDistantGrabColor;

            // Move the feedback object to the end of the ray
            if (intersectsWithGrabbableIO)
                InteractableDistantGrabFeedback.transform.position = endPosition;

        }

    }

    private void TryAttach()
    {
        if (interactableIO == null)
            return;

        // snap in position if needed
        if (interactableIO.SnapOnGrab && interactableIO.GetComponentInChildren<Gaze_SnapPosition>() != null && !interactableIO.IsBeingManipulated)
        {
            // get the snap location from the IO
            Transform grabbedObjectPositionnerTransform = isLeftHand ? interactableIO.LeftHandSnapPoint : interactableIO.RightHandSnapPoint;

            // get the snap location from the controller
            Transform controllerSnapLocation = gameObject.GetComponentInChildren<Gaze_GrabPositionController>().transform;

            // store hand grab positionner rotation
            Quaternion originalHandGrabPositionRotation = controllerSnapLocation.rotation;


            controllerSnapLocation.rotation = grabbedObjectPositionnerTransform.rotation;

            // get the delta vector between object and hand grab location
            Vector3 delta = controllerSnapLocation.position - grabbedObjectPositionnerTransform.position;

            // add the delta to the IO
            interactableIO.transform.position = interactableIO.transform.position + delta;


            // parent grabbed object to the hand
            interactableIO.transform.SetParent(controllerSnapLocation, true);


            // restore original hand grab positionner's rotation (this will rotate the grabbed object to)
            controllerSnapLocation.rotation = originalHandGrabPositionRotation;

        }
        else
        {
            interactableIO.transform.SetParent(this.gameObject.transform);
        }

        grabbedObject = interactableIO.gameObject;

        interactableIO.RootMotion = grabPosition;

        // notify
        KeyValuePair<VRNode, GameObject> grabbedObjects = new KeyValuePair<VRNode, GameObject>(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, grabbedObject);
        isGrabbing = true;

        // tell the Proximity script of this grabbed object it's being grabbed (for collisions testing on TryDetach)
        //grabbedObject.GetComponentInChildren<Gaze_Proximity>().IsGrabbed = true;

        Gaze_GravityManager.ChangeGravityState(interactableIO, Gaze_GravityRequestType.DEACTIVATE_AND_ATTACH);

        Gaze_InputManager.FireControllerGrabEvent(new Gaze_ControllerGrabEventArgs(this, grabbedObjects, isGrabbing));
    }

    public void TryDetach(bool removeParent = true)
    {
        if (grabbedObject != null)
        {
            if (grabbedObject.GetComponentInParent<Gaze_InteractiveObject>() && grabbedObject.GetComponent<Gaze_InteractiveObject>().IsSticky)
                return;

            GameObject grabbedObj = grabbedObject;

            if (removeParent)
            {
                grabbedObj.transform.SetParent(null);

                Gaze_GravityManager.ChangeGravityState(interactableIO, Gaze_GravityRequestType.RETURN_TO_DEFAULT);

                if (grabState != GRAB_STATE.ATTRACTING)
                    ThrowObject(grabbedObj);

                if (grabbedObj.GetComponent<Gaze_Catchable>() != null && grabbedObj.GetComponent<Gaze_Catchable>().vibrates)
                    Gaze_InputManager.instance.HapticFeedback(false);

                KeyValuePair<VRNode, GameObject> dico = new KeyValuePair<VRNode, GameObject>(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, grabbedObj);
                isGrabbing = false;

                if (grabbedObject.GetComponentInParent<Gaze_InteractiveObject>())
                    grabbedObject.GetComponentInParent<Gaze_InteractiveObject>().RootMotion = null;

                Gaze_InputManager.FireControllerGrabEvent(new Gaze_ControllerGrabEventArgs(this, dico, isGrabbing));
            }

        }

        ClearGrabbingVariables();
        if (laserPointer != null)
            laserPointer.enabled = true;
    }

    private void ClearGrabbingVariables()
    {
        // tell the Proximity script of this grabbed object it's being grabbed (for collisions testing on TryDetach)
        //if (grabbedObject)
        //    grabbedObject.GetComponentInChildren<Gaze_Proximity>().IsGrabbed = false;

        grabbedObject = null;
        interactableIO = null;
        isGrabbing = false;
    }

    public bool IsObjectInProximityList(GameObject _gameObject)
    {
        return objectsInProximity.Contains(_gameObject);
    }

    public void RemoveObjectInProximity(GameObject _gameObject)
    {
        objectsInProximity.Remove(_gameObject);
    }

    public void AddNewObjectInProximity(GameObject _gameObject)
    {
        objectsInProximity.Insert(0, _gameObject);
    }

    private void UpdateVelocities()
    {
        positions[positionsIndex % samples] = transform.position;
        positionsIndex++;
    }



    private static Vector3[] SortCicularList(Vector3[] _circularList, int currentIndex)
    {
        Vector3[] sortedList = new Vector3[_circularList.Length];
        _circularList.SubArray(currentIndex, _circularList.Length - currentIndex).CopyTo(sortedList, 0);
        _circularList.SubArray(0, currentIndex).CopyTo(sortedList, _circularList.Length - currentIndex);
        return sortedList;
    }


    private float GetMeanAcceleration()
    {

        float timeBetweenFrames = Time.deltaTime;

        // Sort the circular list in order to easilly work with it
        Vector3[] sortedList = SortCicularList(positions, positionsIndex % samples);
        Array.Reverse(sortedList);

        List<float> velocities = new List<float>();
        for (int i = 0; i < positions.Length - 1; i++)
        {
            if (positions[i + 1] == Vector3.zero)
                continue;
            velocities.Add(Vector3.Distance(sortedList[i], sortedList[i + 1]) / Mathf.Pow(timeBetweenFrames, 2));
        }

        try
        {
            if (velocities.Count > 0)
            {
                return velocities.Average();
            }

            return 0;
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return 0;
        }
    }

    /// <summary>
    /// Clears the grabbed object positions array
    /// </summary>
    private void ClearPositionsArray()
    {
        positionsIndex = 0;
        for (int i = 0; i < positions.Length; i++)
            positions[i] = Vector3.zero;
    }


    private const float MIKES_MAGIC_NUMBER_FOR_GEARVR = 3f;

    private void ThrowObject(GameObject _object)
    {
        Rigidbody rb = _object.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Gaze_GravityManager.ChangeGravityState(interactableIO, Gaze_GravityRequestType.RETURN_TO_DEFAULT);

            // get the current index (beware it's been incremented in the updated)
            short currentIndex = (short)(positionsIndex == 0 ? samples - 1 : (positionsIndex - 1) % samples);

            // get next value in the array (which is the oldest sample)
            short oldestIndex = (short)((currentIndex + 1) % samples);


            float v = GetMeanAcceleration();

            // This works fine with GearVr controllers
            Vector3 direction = transform.forward;

            // Calc the direction if needed
            if (Gaze_InputManager.instance.trackPosition)
                direction = (positions[currentIndex] - positions[oldestIndex]).normalized;
            else
            {
                direction = transform.forward;
                v *= MIKES_MAGIC_NUMBER_FOR_GEARVR;

                // Limit the velocity of the trow
                if (v > MAX_THROW_VELOCITY)
                    v = MAX_THROW_VELOCITY;
            }


            // Add a velocity to the object in the current directio
            Vector3 Vv = direction * v;

            // Add an inpulse based on velocity
            rb.AddForce(Vv.x, Vv.y, Vv.z, ForceMode.Acceleration);

            ClearPositionsArray();
        }
    }

    private void UpdateProximityList(Gaze_InteractiveObject collidingIO, Gaze_ControllerCollisionEventArgs e)
    {
        //if (e.CollisionType.Equals(Gaze_CollisionTypes.COLLIDER_ENTER) || e.CollisionType.Equals(Gaze_CollisionTypes.COLLIDER_STAY))
        if (e.CollisionType.Equals(Gaze_CollisionTypes.COLLIDER_ENTER))
        {
            if (grabState == GRAB_STATE.SEARCHING || grabState == GRAB_STATE.EMPTY || grabState == GRAB_STATE.ATTRACTING)
            {
                if (!IsObjectInProximityList(collidingIO.gameObject))
                {
                    AddNewObjectInProximity(collidingIO.gameObject);
                }

                if (isTriggerPressed && interactableIO && interactableIO.IsGrabEnabled && interactableIO.GrabModeIndex.Equals((int)Gaze_GrabMode.ATTRACT))
                {
                    interactableIO = objectsInProximity.ElementAt(0).GetComponent<Gaze_InteractiveObject>();
                    grabState = GRAB_STATE.GRABBING;
                }
            }
        }
        else if (e.CollisionType == Gaze_CollisionTypes.COLLIDER_EXIT)
        {
            RemoveObjectInProximity(collidingIO.gameObject);
        }
    }

    private IEnumerator GrabInCertainTime(float _time, Gaze_InteractiveObject _objectToTake)
    {
        yield return new WaitForSeconds(_time);
        TryDetach();
        interactableIO = _objectToTake;
        Gaze_GravityManager.ChangeGravityState(_objectToTake, Gaze_GravityRequestType.DEACTIVATE_AND_ATTACH);
        grabState = GRAB_STATE.ATTRACTING;
    }

    private void TriggerReleased(GameObject _object)
    {
        // notify the trigger is released
        isTriggerPressed = false;
        if (_object)
            Touch(interactableIO);
    }

    #region StaticMethods
    /// <summary>
    /// Enables the grab manager
    /// </summary>
    public void EnableGrabManager()
    {
        grabState = GRAB_STATE.EMPTY;
    }

    /// <summary>
    /// Disables the grab manager until the EnableGrabManger method is
    /// called
    /// </summary>
    public void DisableGrabManager()
    {
        Debug.Log("1");
        TryDetach();
        grabState = GRAB_STATE.BLOCKED;
    }

    /// <summary>
    /// Disables the user ability to take objects for a certain ammount of time
    /// </summary>
    /// <param name="seconds"></param>
    public void DisableGrabbingForAmmountOfSeconds(float seconds)
    {
        DisableGrabManager();
        StartCoroutine(UnBlockGrabbingInTime(seconds));
    }

    /// <summary>
    /// Enables the grab manager is a certain ammoount of time (in seconds)
    /// </summary>
    /// <param name="time">Time in Seconds</param>
    /// <returns></returns>
    IEnumerator UnBlockGrabbingInTime(float time)
    {
        yield return new WaitForSeconds(time);
        EnableGrabManager();
    }

    /// <summary>
    /// Enables all the grab managers
    /// </summary>
    public static void EnableAllGrabManagers()
    {
        foreach (Gaze_GrabManager grabManager in GrabManagers)
            grabManager.EnableGrabManager();
    }

    /// <summary>
    /// Disables all the grab managers until the enalbe grab manager method is called
    /// </summary>
    public static void DisableAllGrabManagers()
    {
        foreach (Gaze_GrabManager grabManager in GrabManagers)
            grabManager.DisableGrabManager();
    }

    /// <summary>
    /// Gets the a grabbing hand (Usefull to know if we are already taking an object if the other hand)
    /// </summary>
    /// <returns></returns>
    public static Gaze_GrabManager GetGrabbingHand()
    {
        return GrabManagers.FirstOrDefault(gm => gm.isGrabbing && gm.interactableIO != null);
    }

    public static List<Gaze_GrabManager> GetGrabbingHands(Gaze_InteractiveObject io)
    {
        return GrabManagers.Where(gm => gm.interactableIO == io).ToList();
    }

    /// <summary>
    /// Used to disable all the grab for a certain ammount of time
    /// </summary>
    /// <param name="_time"></param>
    public static void DisableGrabForTime(float _time)
    {
        foreach (Gaze_GrabManager grabManager in GrabManagers)
            grabManager.DisableGrabbingForAmmountOfSeconds(_time);
    }
    #endregion StaticMethods

    #region EventHandlers
    // if the grabbed object is already grabbed by the other controller, let the object go
    private void OnControllerGrabEvent(Gaze_ControllerGrabEventArgs e)
    {
        if (e.IsGrabbing)
        {
            // if the other controller has grabbed an object (! before the isLeftHand)
            if ((e.ControllerObjectPair.Key.Equals(VRNode.LeftHand) && !isLeftHand) || (e.ControllerObjectPair.Key.Equals(VRNode.RightHand) && isLeftHand))
            {
                // and this object is the one I'm currently grabbing
                if (e.ControllerObjectPair.Value == grabbedObject)
                {
                    // drop it to let the other controller grab it
                    grabbedObject = null;
                }
            }
        }
    }

    private void OnHandRightDownEvent(Gaze_InputEventArgs e)
    {
        if (e.VrNode.Equals(VRNode.RightHand) && !isLeftHand)
        {
            if (e.InputType.Equals(Gaze_InputTypes.HAND_RIGHT_DOWN))
            {
                grabState = GRAB_STATE.SEARCHING;
                isTriggerPressed = true;
            }
        }
    }

    private void OnHandRightUpEvent(Gaze_InputEventArgs e)
    {
        if (e.VrNode.Equals(VRNode.RightHand) && !isLeftHand)
        {
            if (e.InputType.Equals(Gaze_InputTypes.HAND_RIGHT_UP))
            {
                if (interactableIO)
                {
                    Gaze_EventManager.FireControllerPointingEvent(new Gaze_ControllerPointingEventArgs(this.gameObject, new KeyValuePair<VRNode, GameObject>(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, interactableIO.gameObject), false));
                    TriggerReleased(interactableIO.gameObject);
                    ResetGrabStateAfterHandUp();
                }
                grabState = GRAB_STATE.EMPTY;
            }
        }
    }

    private void OnHandLeftDownEvent(Gaze_InputEventArgs e)
    {
        if (e.VrNode.Equals(VRNode.LeftHand) && isLeftHand)
        {
            if (e.InputType.Equals(Gaze_InputTypes.HAND_LEFT_DOWN))
            {
                grabState = GRAB_STATE.SEARCHING;
                isTriggerPressed = true;
            }
        }
    }

    private void OnHandLeftUpEvent(Gaze_InputEventArgs e)
    {
        if (e.VrNode.Equals(VRNode.LeftHand) && isLeftHand)
        {
            if (e.InputType.Equals(Gaze_InputTypes.HAND_LEFT_UP))
            {
                if (interactableIO)
                {
                    Dictionary<VRNode, GameObject> dico = new Dictionary<VRNode, GameObject>();
                    dico.Add(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, interactableIO.gameObject);
                    Gaze_EventManager.FireControllerPointingEvent(new Gaze_ControllerPointingEventArgs(this.gameObject, new KeyValuePair<VRNode, GameObject>(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, interactableIO.gameObject), false));
                    TriggerReleased(interactableIO.gameObject);
                    ResetGrabStateAfterHandUp();
                }
                grabState = GRAB_STATE.EMPTY;
            }
        }
    }

    /// <summary>
    /// Change the state of the grabmanager to the correct one after releasing the hand
    /// button.
    /// </summary>
    private void ResetGrabStateAfterHandUp()
    {
        KeyValuePair<VRNode, GameObject> dico;
        //Gaze_GravityManager.ChangeGravityState(interactableIO, Gaze_GravityRequestType.ACTIVATE_AND_DETACH);

        // If we where attracting we need to to add graviy again to the IO
        if (interactableIO != null)
        {
            dico = new KeyValuePair<VRNode, GameObject>(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, interactableIO.gameObject);
        }
        else
        {
            dico = new KeyValuePair<VRNode, GameObject>(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, null);
        }

        Gaze_InputManager.FireControllerGrabEvent(new Gaze_ControllerGrabEventArgs(this, dico, false));
        Gaze_GravityManager.ChangeGravityState(interactableIO, Gaze_GravityRequestType.RETURN_TO_DEFAULT);

        // If the IO was on our hands we need to trydeatch
        TryDetach();

        // Reset other variables
        isTriggerPressed = false;
        grabState = GRAB_STATE.EMPTY;
        raycastIOs.Clear();
    }

    private void OnGrabEvent(Gaze_GrabEventArgs e)
    {
        if (e.GrabManager.GetInstanceID() != GetInstanceID())
            return;

        StartCoroutine(GrabInCertainTime(e.TimeToGrab, e.InteractiveObject));
    }

    private void OnHandsReplaced(Gaze_GrabManager grabManager, Transform newGrabPosition, GameObject newDistantGrabOrigin)
    {
        if (grabManager != this)
            return;

        grabPosition = newGrabPosition;
        distantGrabOrigin = newDistantGrabOrigin;
        controllerSnapTransform = newGrabPosition;
        interactableIO = null;
    }

    /// <summary>
    /// Calls on controller collision event.
    /// Grabbed objects have to be Interactive Object ('Gaze objects')
    /// 'Other' is either the Proximity for Interactive Objects or the object itself.
    /// 'Sender' is always the controller triggering the event
    /// </summary>
    /// <param name="e">E.</param>
    private void OnControllerCollisionEvent(Gaze_ControllerCollisionEventArgs e)
    {
        GameObject collidingObject = Gaze_Utils.ConvertIntoGameObject(e.Other);

        if (collidingObject.GetComponent<Gaze_Manipulation>() == null)
            return;

        Gaze_InteractiveObject IO = collidingObject.GetComponentInParent<Gaze_InteractiveObject>();

        // Discart hands
        if (IO.GetComponentInChildren<Gaze_HandController>())
            return;

        // Discard Head
        if (IO.GetComponent<Gaze_Head>() != null)
            return;

        if (IO.ManipulationMode != Gaze_ManipulationModes.GRAB)
            return;

        // Check if the colliding object has a gaze handle in order to avoid noise.
        collidingObject = (GameObject)e.Other;

        // exit if the sender is not this Controller
        GameObject senderController = ((GameObject)e.Sender).GetComponentInParent<Gaze_InteractiveObject>().GetComponentInChildren<Gaze_HandController>().gameObject;
        if (senderController != this.gameObject)
            return;

        // if colliding object is a Gaze_InteractiveObject
        Gaze_InteractiveObject collidingIO = ((GameObject)e.Other).GetComponentInParent<Gaze_InteractiveObject>();
        if (collidingIO != null)
        {
            // if it's catchable, touchable, levitable OR if it's manipulated and we're colliding with the manipulable collider
            if (collidingIO.IsGrabEnabled || collidingIO.IsTouchEnabled || (collidingIO.IsBeingManipulated && collidingIO.ManipulableHandle.Equals(e.Other.GetComponent<Collider>())))
            {
                UpdateProximityList(collidingIO, e);
            }
        }
    }

    /// <summary>
    /// Clear all the lists where the destroyed IO was in order to 
    /// prevent null reference exceptions.
    /// </summary>
    /// <param name="args"></param>
    private void OnIODestroyed(Gaze_IODestroyEventArgs args)
    {
        raycastIOs.Remove(args.IO.gameObject);
        hitsIOs.Remove(args.IO.gameObject);
        objectsInProximity.Remove(args.IO.gameObject);

        if (grabbedObject == args.IO.gameObject)
            grabbedObject = null;

        if (collidingObject == args.IO.gameObject)
            collidingObject = null;
    }

    #endregion EventHandlers
}