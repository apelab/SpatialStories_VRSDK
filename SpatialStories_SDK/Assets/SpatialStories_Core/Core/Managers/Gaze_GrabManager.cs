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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.VR;

/// <summary>
/// Grab manager handles all grab mechanisme.
/// This script is attached, at run time, on the controllers.
[assembly: InternalsVisibleTo("Gaze_HandIODetectorFeedback")]
[assembly: InternalsVisibleTo("Gaze_HandIODetectorKernel")]
public class Gaze_GrabManager : MonoBehaviour
{
    #region members
    public GameObject collidingObject;
    /// <summary>
    /// Stores all the grab managers in order to make operations with them
    /// </summary>
    public static List<Gaze_GrabManager> GrabManagers = new List<Gaze_GrabManager>();
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
    public float updateInterval = .2f;
    public Transform grabPosition;
    public List<GameObject> HitsIos;

    private float currentUpdateInterval;
    private bool isGrabbing = false;
    private Transform controllerSnapTransform;
    private bool isTriggerPressed = false;
    private List<GameObject> objectsInProximity = new List<GameObject>();
    private Gaze_TouchDistanceMode searchDistanceState;
    private Dictionary<string, object> analyticsDico;
    private Gaze_ControllerTouchEventArgs gaze_ControllerTouchEventArgs;

    internal GameObject distantGrabOrigin;
    internal LineRenderer laserPointer;
    internal bool intersectsWithInteractiveIO;
    internal Gaze_GrabManagerState grabState;
    internal Vector3 hitPosition;
    internal float closerDistance = 0;
    internal Gaze_InteractiveObject closerIO;
    internal Gaze_HandIODetectorKernel IoDetectorKernel;
    internal Gaze_HandIODetectorFeedback IoDetectorFeedback;

    // Distant grab feedbacks
    public float DefaultDistantGrabRayLength = 30f;

    public GameObject InteractableDistantGrabFeedback;
    public Color InteractableDistantGrabColor = Color.blue;
    public Color InteractableInRangeDistantGrabColor = Color.green;

    public GameObject NotInteractableDistantGrabFeedback;
    public Color NotInteractableDistantGrabColor = Color.white;


    #endregion

    #region CachedVariables

    private float lastUpdateTime;
    #endregion CachedVariables

    void OnEnable()
    {
        Gaze_InputManager.OnControlerSetup += OnControllerSetup;
        Gaze_EventManager.OnGrabEvent += OnGrabEvent;
        Gaze_InputManager.OnControllerCollisionEvent += OnControllerCollisionEvent;
        Gaze_InputManager.OnControllerGrabEvent += OnControllerGrabEvent;

        Gaze_HandsReplacer.OnHandsReplaced += OnHandsReplaced;
        Gaze_EventManager.OnIODestroyed += OnIODestroyed;

        // get the snap location from the controller
        controllerSnapTransform = gameObject.GetComponentInChildren<Gaze_GrabPositionController>().transform;
        grabState = Gaze_GrabManagerState.EMPTY;

        // Add this grab manager to the list
        GrabManagers.Add(this);
    }

    private bool setupDone = false;
    void OnControllerSetup(Gaze_Controllers _controller)
    {
        if (setupDone)
            return;

        if (_controller != Gaze_Controllers.HTC_VIVE)
        {
            Gaze_InputManager.OnHandRightDownEvent += OnHandRightDownEvent;
            Gaze_InputManager.OnHandRightUpEvent += OnHandRightUpEvent;
            Gaze_InputManager.OnHandLeftDownEvent += OnHandLeftDownEvent;
            Gaze_InputManager.OnHandLeftUpEvent += OnHandLeftUpEvent;
        }
        else
        {
            Gaze_InputManager.OnIndexRightDownEvent += OnHandRightDownEvent;
            Gaze_InputManager.OnIndexRightUpEvent += OnHandRightUpEvent;
            Gaze_InputManager.OnIndexLeftDownEvent += OnHandLeftDownEvent;
            Gaze_InputManager.OnIndexLeftUpEvent += OnHandLeftUpEvent;
        }
        setupDone = true;
    }

    void OnDisable()
    {
        Gaze_InputManager.OnControlerSetup -= OnControllerSetup;
        Gaze_EventManager.OnGrabEvent -= OnGrabEvent;
        Gaze_InputManager.OnControllerCollisionEvent -= OnControllerCollisionEvent;

        Gaze_InputManager.OnControllerGrabEvent -= OnControllerGrabEvent;
        Gaze_EventManager.OnIODestroyed -= OnIODestroyed;
        Gaze_HandsReplacer.OnHandsReplaced -= OnHandsReplaced;

        if (Gaze_InputManager.PluggedControllerType != Gaze_Controllers.HTC_VIVE)
        {
            Gaze_InputManager.OnHandRightDownEvent -= OnHandRightDownEvent;
            Gaze_InputManager.OnHandRightUpEvent -= OnHandRightUpEvent;
            Gaze_InputManager.OnHandLeftDownEvent -= OnHandLeftDownEvent;
            Gaze_InputManager.OnHandLeftUpEvent -= OnHandLeftUpEvent;
        }
        else
        {
            Gaze_InputManager.OnIndexRightEvent -= OnHandRightDownEvent;
            Gaze_InputManager.OnIndexRightUpEvent -= OnHandRightUpEvent;
            Gaze_InputManager.OnIndexLeftEvent -= OnHandLeftDownEvent;
            Gaze_InputManager.OnIndexLeftUpEvent -= OnHandLeftUpEvent;
        }

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


        HitsIos = new List<GameObject>();
        SetupDinstanceGrabFeedbacks();
        analyticsDico = new Dictionary<string, object>();


        gaze_ControllerTouchEventArgs = new Gaze_ControllerTouchEventArgs(this.gameObject);
        laserPointer.enabled = true;
        lastUpdateTime = Time.time;

        IoDetectorFeedback = new Gaze_HandIODetectorFeedback(this);
        IoDetectorKernel = new Gaze_HandIODetectorKernel(this);

        IoDetectorKernel.Setup();
        IoDetectorFeedback.Setup();
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

            // Disable de feedback in oder to hide it.
            InteractableDistantGrabFeedback.SetActive(false);

        }
    }



    private void FixedUpdate()
    {
        // check if previous frame we hit an IO with the pointing controller

        if (Time.time >= lastUpdateTime)
        {
            //Debug.Log(interactableIO +" isTriggerPressed = "+isTriggerPressed);
            if (grabState == Gaze_GrabManagerState.EMPTY || grabState == Gaze_GrabManagerState.SEARCHING)
                IoDetectorKernel.Update();

            // update last time value
            lastUpdateTime = Time.time;
        }

        UpdateGrabState();
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
        Collider myProximity = GetComponentInParent<Gaze_InteractiveObject>().Proximity.GetComponent<Collider>();

        for (int i = objectsInProximity.Count - 1; i >= 0; i--)
        {
            GameObject go = objectsInProximity[i];

            if (go == null)
            {
                objectsInProximity.RemoveAt(i);
                continue;
            }
            Gaze_InteractiveObject io = go.GetComponent<Gaze_InteractiveObject>();

            if (!io.Proximity.GetComponent<Collider>().bounds.Intersects(myProximity.bounds))
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
            case Gaze_GrabManagerState.SEARCHING:
                if (debug)
                    Debug.Log("SEARCHING");
                SearchValidObjects();
                break;

            case Gaze_GrabManagerState.ATTRACTING:
                if (debug)
                    Debug.Log("ATTRACTING");
                AttractObjectToHand();
                break;

            case Gaze_GrabManagerState.GRABBING:
                if (debug)
                    Debug.Log("GRABBING");
                GrabObject();
                break;

            case Gaze_GrabManagerState.GRABBED:
                if (debug)
                    Debug.Log("GRABBED");
                break;

            case Gaze_GrabManagerState.EMPTY:
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
            grabState = Gaze_GrabManagerState.EMPTY;
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
                    if (interactableIO.GrabLogic.IsBeingGrabbed)
                    {
                        interactableIO.GrabLogic.SetManipulationMode(true);
                    }

                    if (interactableIO.GrabModeIndex.Equals((int)Gaze_GrabMode.ATTRACT))
                    {
                        grabState = Gaze_GrabManagerState.ATTRACTING;
                        interactableIO.GrabLogic.GrabbingManager = this;
                    }
                    else if (interactableIO.GrabModeIndex.Equals((int)Gaze_GrabMode.LEVITATE))
                    {
                        grabState = Gaze_GrabManagerState.GRABBED;
                        ClearLaserPointer();

                        KeyValuePair<VRNode, GameObject> dico = new KeyValuePair<VRNode, GameObject>(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, interactableIO.gameObject);
                        Gaze_InputManager.FireControllerGrabEvent(new Gaze_ControllerGrabEventArgs(this, dico, true, hitPosition));
                    }

                    // analytics
                    analyticsDico.Clear();
                    analyticsDico.Add("Grabbed Object", interactableIO.name);
                    analyticsDico.Add("Is Left Hand", isLeftHand);
                    Analytics.CustomEvent("Grab", analyticsDico);
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

            if (interactableIO.IsManipulable && !interactableIO.GrabLogic.IsBeingManipulated)
            {
                interactableIO.GrabLogic.SetManipulationMode(true);
            }

            // if the grabbed object is being manipulated
            if (interactableIO.GrabLogic.GrabbingManager != null)
            {
                interactableIO.GrabLogic.GrabbingManager.grabbedObject = null;
                interactableIO.GrabLogic.GrabbingManager.grabState = Gaze_GrabManagerState.EMPTY;
                interactableIO.GrabLogic.GrabbingManager.TryDetach();

                // Set the grab point where the controller actually is in order to avoid weird jumps
                interactableIO.GrabLogic.AddDynamicGrabPositioner(controllerSnapTransform.position, this);

                // change the state to grabbing.
                grabState = Gaze_GrabManagerState.GRABBING;
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
                        grabState = Gaze_GrabManagerState.GRABBING;
                    }
                    else if (interactableIO.GrabModeIndex.Equals((int)Gaze_GrabMode.LEVITATE))
                    {
                        Vector3 offset = interactableIO.transform.position - interactableIO.transform.GetComponentInChildren<Gaze_Manipulation>().GetComponent<Collider>().bounds.center;
                        interactableIO.transform.position = distantGrabOrigin.transform.position + (0.3f * distantGrabOrigin.transform.forward) + offset;

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
            grabState = Gaze_GrabManagerState.SEARCHING;
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
            grabState = Gaze_GrabManagerState.GRABBING;
            interactableIO.transform.position = controllerSnapTransform.position - interactableIO.GrabLogic.GetGrabPoint();
        }
    }

    /// <summary>
    /// Performs the grab logic
    /// </summary>
    private void GrabObject()
    {
        ClearLaserPointer();
        TryAttach();

        grabState = Gaze_GrabManagerState.GRABBED;
    }

    private void TryAttach()
    {

        if (interactableIO == null)
            return;

        // get the snap location from the controller
        Transform controllerSnapLocation = gameObject.GetComponentInChildren<Gaze_GrabPositionController>().transform;

        // snap in position if needed
        if (interactableIO.GrabPositionnerCollider != null)
        {
            // get the snap location from the IO
            Transform grabbedObjectPositionnerTransform = interactableIO.GrabPositionnerCollider.transform;

            // get the delta vector between object and hand grab location
            Vector3 delta = controllerSnapLocation.position - grabbedObjectPositionnerTransform.position;

            // add the delta to the IO
            interactableIO.transform.position = interactableIO.transform.position + delta;

            // parent grabbed object to the hand
            interactableIO.transform.SetParent(controllerSnapLocation, true);
        }
        else
        {
            // store hand grab positionner rotation
            Quaternion originalHandGrabPositionRotation = controllerSnapLocation.rotation;

            // rotate hand grab positionner like grabbed object's grab location's rotation
            Quaternion rotation = interactableIO.GrabLogic.DefaultHandle.transform.rotation;

            controllerSnapLocation.rotation = rotation;

            // get the delta vector between object and hand grab location
            Vector3 delta = controllerSnapLocation.position - interactableIO.GrabLogic.DefaultHandle.transform.position;

            // add the delta to the IO
            interactableIO.transform.position = interactableIO.transform.position + delta;


            // parent grabbed object to the hand
            interactableIO.transform.SetParent(controllerSnapLocation, true);


            // restore original hand grab positionner's rotation (this will rotate the grabbed object to)
            controllerSnapLocation.rotation = originalHandGrabPositionRotation;
        }


        interactableIO.transform.SetParent(null);

        if (interactableIO == null)
            return;

        grabbedObject = interactableIO.gameObject;

        // notify
        KeyValuePair<VRNode, GameObject> grabbedObjects = new KeyValuePair<VRNode, GameObject>(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, grabbedObject);
        isGrabbing = true;

        Gaze_InputManager.FireControllerGrabEvent(new Gaze_ControllerGrabEventArgs(this, grabbedObjects, isGrabbing));
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
            grabState = Gaze_GrabManagerState.SEARCHING;
            return;
        }
        interactableIO.transform.position = Vector3.MoveTowards(interactableIO.transform.position, controllerSnapTransform.position - interactableIO.GrabLogic.GetGrabPoint(), interactableIO.AttractionSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Checks if we need to attract the object that we are trying to grab.
    /// </summary>
    /// <returns></returns>
    private bool IsAttactionNeeded()
    {
        return Vector3.Distance(interactableIO.transform.position, controllerSnapTransform.position - interactableIO.GrabLogic.GetGrabPoint()) > 0.1f;
    }

    public void TryDetach()
    {
        if (grabbedObject != null)
        {
            if (grabbedObject.GetComponentInParent<Gaze_InteractiveObject>() && grabbedObject.GetComponent<Gaze_InteractiveObject>().IsSticky)
                return;

            GameObject grabbedObj = grabbedObject;
            grabbedObj.transform.SetParent(null);


            if (grabbedObj.GetComponent<Gaze_Catchable>() != null && grabbedObj.GetComponent<Gaze_Catchable>().vibrates)
                Gaze_InputManager.instance.HapticFeedback(false);

            KeyValuePair<VRNode, GameObject> dico = new KeyValuePair<VRNode, GameObject>(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, grabbedObj);
            isGrabbing = false;

            Gaze_InputManager.FireControllerGrabEvent(new Gaze_ControllerGrabEventArgs(this, dico, isGrabbing));
        }

        ClearGrabbingVariables();
        if (laserPointer != null)
            laserPointer.enabled = true;
    }

    private void ClearGrabbingVariables()
    {
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

    private void UpdateProximityList(Gaze_InteractiveObject collidingIO, Gaze_ControllerCollisionEventArgs e)
    {
        //if (e.CollisionType.Equals(Gaze_CollisionTypes.COLLIDER_ENTER) || e.CollisionType.Equals(Gaze_CollisionTypes.COLLIDER_STAY))
        if (e.CollisionType.Equals(Gaze_CollisionTypes.COLLIDER_ENTER))
        {
            if (grabState == Gaze_GrabManagerState.SEARCHING || grabState == Gaze_GrabManagerState.EMPTY || grabState == Gaze_GrabManagerState.ATTRACTING)
            {
                if (!IsObjectInProximityList(collidingIO.gameObject))
                {
                    AddNewObjectInProximity(collidingIO.gameObject);
                }

                if (isTriggerPressed && interactableIO && interactableIO.IsGrabEnabled && interactableIO.GrabModeIndex.Equals((int)Gaze_GrabMode.ATTRACT))
                {
                    interactableIO = objectsInProximity.ElementAt(0).GetComponent<Gaze_InteractiveObject>();
                    grabState = Gaze_GrabManagerState.GRABBING;
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
        grabState = Gaze_GrabManagerState.ATTRACTING;
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
        grabState = Gaze_GrabManagerState.EMPTY;
    }

    /// <summary>
    /// Disables the grab manager until the EnableGrabManger method is
    /// called
    /// </summary>
    public void DisableGrabManager()
    {
        TryDetach();
        grabState = Gaze_GrabManagerState.BLOCKED;
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
            grabState = Gaze_GrabManagerState.SEARCHING;
            isTriggerPressed = true;
        }
    }

    private void OnHandRightUpEvent(Gaze_InputEventArgs e)
    {
        if (e.VrNode.Equals(VRNode.RightHand) && !isLeftHand)
        {
            if (interactableIO != null)
            {
                Gaze_EventManager.FireControllerPointingEvent(new Gaze_ControllerPointingEventArgs(this.gameObject, new KeyValuePair<VRNode, GameObject>(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, interactableIO.gameObject), false));
                TriggerReleased(interactableIO.gameObject);
                ResetGrabStateAfterHandUp();
            }
            grabState = Gaze_GrabManagerState.EMPTY;

        }
    }

    private void OnHandLeftDownEvent(Gaze_InputEventArgs e)
    {
        if (e.VrNode.Equals(VRNode.LeftHand) && isLeftHand)
        {
            grabState = Gaze_GrabManagerState.SEARCHING;
            isTriggerPressed = true;
        }
    }

    private void OnHandLeftUpEvent(Gaze_InputEventArgs e)
    {
        if (e.VrNode.Equals(VRNode.LeftHand) && isLeftHand)
        {
            if (interactableIO != null)
            {
                Dictionary<VRNode, GameObject> dico = new Dictionary<VRNode, GameObject>();
                dico.Add(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, interactableIO.gameObject);
                Gaze_EventManager.FireControllerPointingEvent(new Gaze_ControllerPointingEventArgs(this.gameObject, new KeyValuePair<VRNode, GameObject>(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, interactableIO.gameObject), false));
                TriggerReleased(interactableIO.gameObject);
                ResetGrabStateAfterHandUp();
            }
            grabState = Gaze_GrabManagerState.EMPTY;
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
            dico = new KeyValuePair<VRNode, GameObject>(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, interactableIO.gameObject);
        else
            dico = new KeyValuePair<VRNode, GameObject>(isLeftHand ? VRNode.LeftHand : VRNode.RightHand, null);

        Gaze_InputManager.FireControllerGrabEvent(new Gaze_ControllerGrabEventArgs(this, dico, false));
        Gaze_GravityManager.ChangeGravityState(interactableIO, Gaze_GravityRequestType.RETURN_TO_DEFAULT);

        // If the IO was on our hands we need to trydeatch
        TryDetach();

        // Reset other variables
        isTriggerPressed = false;
        grabState = Gaze_GrabManagerState.EMPTY;
        IoDetectorKernel.ClearRaycasts();
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
    /// <param name="_e">E.</param>
    /// <summary>
    /// Calls on controller collision event.
    /// Grabbed objects have to be Interactive Object ('Gaze objects')
    /// 'Other' is either the Proximity for Interactive Objects or the object itself.
    /// 'Sender' is always the controller triggering the event
    /// </summary>
    /// <param name="_e">E.</param>
    private void OnControllerCollisionEvent(Gaze_ControllerCollisionEventArgs _e)
    {

        // If the grab manager is not this one we dont care
        if (_e.GrabManger != this)
            return;

        GameObject colObject = Gaze_Utils.ConvertIntoGameObject(_e.Other);

        if (colObject.GetComponent<Gaze_Manipulation>() == null)
            return;

        Gaze_InteractiveObject IO = colObject.GetComponentInParent<Gaze_InteractiveObject>();

        if (IO.ManipulationMode == Gaze_ManipulationModes.NONE)
            return;

        if (IO.GrabLogic.GrabbingManager != null && IO.ManipulationMode == Gaze_ManipulationModes.LEVITATE)
            return;

        // Discart hands
        if (IO.GetComponentInChildren<Gaze_HandController>())
            return;

        // Discard Head
        if (IO.GetComponent<Gaze_Head>() != null)
            return;

        // Check if the colliding object has a gaze handle in order to avoid noise.
        colObject = (GameObject)_e.Other;

        // exit if the sender is not this Controller
        GameObject senderController = ((GameObject)_e.Sender).GetComponentInParent<Gaze_InteractiveObject>().GetComponentInChildren<Gaze_HandController>().gameObject;
        if (senderController != this.gameObject)
            return;

        // if colliding object is a Gaze_InteractiveObject
        Gaze_InteractiveObject collidingIo = ((GameObject)_e.Other).GetComponentInParent<Gaze_InteractiveObject>();

        // If its the object that we are already taking its not necessary to process
        if (collidingIo.GrabLogic.IsBeingGrabbed && collidingIo.GrabLogic.GrabbingManager == this)
            return;

        if (collidingIo != null)
        {
            // if it's catchable, touchable, levitable OR if it's manipulated and we're colliding with the manipulable collider
            if (collidingIo.IsGrabEnabled || collidingIo.IsTouchEnabled || (collidingIo.GrabLogic.IsBeingManipulated && collidingIo.ManipulableHandle.Equals(_e.Other.GetComponent<Collider>())))
            {
                UpdateProximityList(collidingIo, _e);
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

        if(IoDetectorFeedback != null)
            IoDetectorKernel.RemoveDestroyedIOFromRaycasts(args.IO.gameObject);

        if (HitsIos != null)
            HitsIos.Remove(args.IO.gameObject);

        if (objectsInProximity != null)
            objectsInProximity.Remove(args.IO.gameObject);

        if (grabbedObject == args.IO.gameObject)
            grabbedObject = null;

        if (collidingObject == args.IO.gameObject)
            collidingObject = null;
    }

    #endregion EventHandlers

    private void LateUpdate()
    {
        IoDetectorFeedback.Update();    
    }
}