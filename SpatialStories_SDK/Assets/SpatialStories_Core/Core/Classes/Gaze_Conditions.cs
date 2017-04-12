// <copyright file="Gaze_Gazable.cs" company="apelab sàrl">
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
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

namespace Gaze
{
    /// <summary>
    /// Gazable
    /// This component should be attached to any GameObject who may be gazed.
    /// </summary>
    [Serializable]
    [ExecuteInEditMode]
    public class Gaze_Conditions : MonoBehaviour
    {

        #region members
        // flag to know if this script is enabled in the Gaze_InteractiveObjectsEditor checkbox
        public bool isActive = false;

        /// <summary>
        /// True means the trigger is only time dependant.
        /// False means the trigger's start time frame depends on the specified dependancy.
        /// If False, user should call StartTimeFrame() to set the frame starting time.
        /// </summary>
        public bool dependent;

        /// <summary>
        /// if FALSE, require only one
        /// </summary>
        public bool requireAllActivators;
        public bool requireAllDeactivators;

        /// <summary>
        /// If specified, these are the game objects this trigger is dependant upon.
        /// </summary>
        public Gaze_DependencyMap ActivateOnDependencyMap = new Gaze_DependencyMap();
        public Gaze_DependencyMap DeactivateOnDependencyMap = new Gaze_DependencyMap();
        public static bool ShowDependencies;
        public bool DependenciesValidated;
        public static Mesh cubeMesh;

        public Gaze_ProximityMap proximityMap = new Gaze_ProximityMap();
        public Gaze_GrabMap grabMap = new Gaze_GrabMap();
        public Gaze_TouchMap touchMap = new Gaze_TouchMap();

        // change Component to interface when ready
        public List<Gaze_AbstractConditions> customConditions = new List<Gaze_AbstractConditions>();
        public Dictionary<int, bool> customConditionsDico = new Dictionary<int, bool>();

        /// <summary>
        /// index of this gazable's current trigger status
        /// </summary>
        public int triggerStateIndex;

        /// <summary>
        /// This trigger must wait 'waitTime' before being in active mode.
        /// This wait time is dependant on triggerObject start if any otherwise
        /// its dependant on the load level scene time.
        /// </summary>
        /// <value>The duration in seconds</value>
        public float delayDuration;

        /// <summary>
        /// The toggle to display delay duration in the Editor
        /// Used in Gaze_TriggerSettingsEditor.cs
        /// </summary>
        public bool delayed;

        /// <summary>
        /// Window of time during which the object is active (gazable).
        /// </summary>
        /// <value>The duration in seconds</value>
        public float activeDuration;

        /// <summary>
        /// True if the active window has no end. (active forever, always gazable)
        /// </summary>
        public bool expires;

        /// <summary>
        /// The index of the auto trigger mode as defined in Gaze_AutoTriggerMode.
        /// </summary>
        public int autoTriggerModeIndex;

        /// <summary>
        /// True if the trigger will reload after being triggered
        /// </summary>
        public bool reload;

        /// <summary>
        /// True if the trigger can be triggered (is loaded), false if it has been triggered and has not be reloaded
        /// </summary>
        public bool canBeTriggered;

        /// <summary>
        /// The number of times the trigger has been triggered
        /// </summary>
        public int TriggerCount = 0;

        /// <summary>
        /// The index of the reload mode.
        /// There are 3 types of reload modes : MANUAL, FINITE and INFINITE
        /// </summary>
        public int reloadModeIndex;

        /// <summary>
        /// Maximum number of times the trigger can be reloaded during the ACTIVE window
        /// </summary>
        public int reloadMaxRepetitions;

        /// <summary>
        /// The number of times the trigger has been reloaded
        /// </summary>
        public int reloadCount;

        /// <summary>
        /// Time it takes to reload the trigger
        /// </summary>
        /// <value>The duration in seconds</value>
        public float reloadDelay;

        /// <summary>
        /// Time at which to reload the trigger
        /// </summary>
        private float nextReloadTime;

        /// <summary>
        /// TRUE when a reload has been scheduled
        /// </summary>
        private bool reloadScheduled;

        /// <summary>
        /// The focus time needed to trigger.
        /// The object will not trigger until it has been focused this duration of time.
        /// </summary>
        public float focusDuration;

        /// <summary>
        /// Only active if focus is TRUE.
        /// When ungazed in the active window, determines if and how the amount of gaze
        /// time is lost.
        /// </summary>
        public int focusLossModeIndex;

        /// <summary>
        /// Only active if focus is TRUE and focusLossMode is FADE
        /// Speed factor for the time it takes to lose gaze time once ungazed.
        /// </summary>
        public float FocusLossSpeed;

        /// <summary>
        /// Is the Trigger being focused ?
        /// </summary>
        private bool focusInProgress;

        public bool FocusInProgress { get { return focusInProgress; } }

        /// <summary>
        /// True if focus time is reached, else FALSE
        /// </summary>
        private bool focusComplete;

        public bool FocusComplete { get { return focusComplete; } }

        /// <summary>
        /// The amount of time the object has been focused.
        /// </summary>
        private float focusTotalTime;

        public float FocusTotalTime { get { return focusTotalTime; } }

        /// <summary>
        /// The focus completion amount normalized between 0 and 1
        /// </summary>
        public float FocusCompletion { get { return Mathf.Clamp01(focusTotalTime / focusDuration); } }

        /// <summary>
        /// Frame starting time (works only if time driven is ON)
        /// </summary>
        /// <value>The start time.</value>
        private float startTime;

        /// <summary>
        /// Is the Trigger being gazed at ?
        /// </summary>
        private bool isGazed;

        public bool IsGazed { get { return isGazed; } }

        /// <summary>
        /// Is Gaze condition enabled.
        /// Corresponds to the editor checkbox as a trigger condition.
        /// </summary>
        public bool gazeEnabled;

        /// <summary>
        /// True if gaze is either disabled or enabled and being gazed.
        /// </summary>
        private bool gazeFlag;

        /// <summary>
        /// Is Proximity condition enabled.
        /// Corresponds to the editor checkbox as a trigger condition
        /// </summary>
        public bool proximityEnabled;

        /// <summary>
        /// Is Grab condition enabled.
        /// Corresponds to the editor checkbox as a trigger condition
        /// </summary>
        public bool grabEnabled;
        private bool grabValidated = false;
        private bool grabLeftValid = false, grabRightValid = false;
        private bool grabStateLeftValid = false, grabStateRightValid = false;

        /// <summary>
        /// All the grabable Interactive Objects in the list are required in order to be validated.
        /// </summary>
        public bool requireAllGrabables;

        public bool touchEnabled;
        public bool touchValidated = false;
        public bool touchLeftValid = false, touchRightValid = false;
        public bool touchDistanceModeLeftValid = false, touchDistanceModeRightValid = false;

        /// <summary>
        /// All the grabable Interactive Objects in the list are required in order to be validated.
        /// </summary>
        public bool requireAllTouchables;

        /// <summary>
        /// Are Custom conditions enabled.
        /// Corresponds to the editor checkbox as a trigger condition
        /// </summary>
        public bool customConditionsEnabled;

        /// <summary>
        /// Are all the specified proximities in the list validated ?
        /// </summary>
        private bool proximitiesValidated;

        public bool ProximitiesValidated { get { return proximitiesValidated; } }

        /// <summary>
        /// All the proximities in the list are required in order to be validated.
        /// </summary>
        public bool requireAllProximities;

        /// <summary>
        /// Whether this Interactive Object's Proximity is colliding.
        /// </summary>
        public bool isInProximity;

        private List<GameObject> collidingProximities;

        public List<GameObject> CollidingProximities { get { return collidingProximities; } }

        private GameObject root;

        public GameObject Root { get { return root; } }

        private Gaze_InteractiveObject rootIO;

        public Gaze_InteractiveObject RootIO { get { return rootIO; } }

        public Collider gazeCollider;

        /// <summary>
        /// Is the current time in the defined time frame ?
        /// </summary>
        private bool withinTimeFrameFlag;

        /// <summary>
        /// Is the current time after the defined time frame ?
        /// </summary>
        private bool afterTimeFrameFlag;

        private GameObject pointedObject;

        /// <summary>
        /// TRUE if a distant touch ray is hitting this object, else FALSE (the controller grabbing from distance)
        /// </summary>
        public bool isLeftPointing = false, isRightPointing = false;


        /// <summary>
        /// TRUE if a controller directly touches this object, else FALSE
        /// </summary>
        public bool isLeftColliding = false, isRightColliding = false;

        private bool isTouched = false;
        public Gaze_TouchDistanceMode distanceMode;
        public GameObject touchedObject = null;
        private bool isTriggerPressed = false;
        private VRNode eventHand;
        private float lastUpdateTime;

        #endregion

        void OnEnable()
        {
            rootIO = GetComponentInParent<Gaze_InteractiveObject>();
            root = rootIO.gameObject;

            if (Application.isPlaying)
            {
                Gaze_EventManager.OnGazeEvent += OnGazeEvent;
                Gaze_EventManager.OnTriggerStateEvent += OnTriggerStateEvent;
                Gaze_EventManager.OnTriggerEvent += OnTriggerEvent;
                Gaze_EventManager.OnProximityEvent += OnProximityEvent;
                Gaze_EventManager.OnCustomConditionEvent += OnCustomConditionEvent;
                Gaze_EventManager.OnControllerPointingEvent += OnControllerPointingEvent;

                Gaze_InputManager.OnControllerGrabEvent += OnControllerGrabEvent;
                Gaze_InputManager.OnControllerTouchEvent += OnControllerTouchEvent;
                Gaze_InputManager.OnControllerCollisionEvent += OnControllerCollisionEvent;

                if (customConditionsDico.Count != customConditions.Count)
                {
                    foreach (Gaze_AbstractConditions condition in customConditions)
                    {
                        if (!customConditionsDico.ContainsKey(condition.GetInstanceID()))
                            customConditionsDico.Add(condition.GetInstanceID(), false);
                    }
                }

                // enable proximity detection accordingly
                proximitiesValidated = proximityEnabled ? false : true;
            }
        }

        void OnDisable()
        {
            if (Application.isPlaying)
            {
                Gaze_EventManager.OnGazeEvent -= OnGazeEvent;
                Gaze_EventManager.OnTriggerStateEvent -= OnTriggerStateEvent;
                Gaze_EventManager.OnTriggerEvent -= OnTriggerEvent;
                Gaze_EventManager.OnProximityEvent -= OnProximityEvent;
                Gaze_EventManager.OnCustomConditionEvent -= OnCustomConditionEvent;
                Gaze_EventManager.OnControllerPointingEvent -= OnControllerPointingEvent;

                Gaze_InputManager.OnControllerGrabEvent -= OnControllerGrabEvent;
                Gaze_InputManager.OnControllerTouchEvent -= OnControllerTouchEvent;
                Gaze_InputManager.OnControllerCollisionEvent -= OnControllerCollisionEvent;
            }
        }

        void Start()
        {
            focusInProgress = false;
            focusComplete = focusDuration <= 0 ? true : false;
            withinTimeFrameFlag = false;
            afterTimeFrameFlag = false;
            DependenciesValidated = dependent ? false : true;
            startTime = Time.time;
            reloadCount = 0;

            // notify we're before the activity window time
            SetTriggerState(Gaze_TriggerState.BEFORE, true);

            Gaze_EventManager.FireTriggerEvent(new Gaze_TriggerEventArgs(gameObject, Time.time, false, false, -1, GetAutoTriggerMode(), GetReloadMode(), reloadMaxRepetitions));

            // check loss speed is valid if used
            FocusLossSpeed = (GetFocusLossMode() == Gaze_FocusLossMode.FADE && FocusLossSpeed > 0) ? FocusLossSpeed : 1f;

            CheckReloadParams();

            collidingProximities = new List<GameObject>();
            TriggerCount = 0;
            lastUpdateTime = Time.time;
        }

        void Update()
        {

            if (Time.time > lastUpdateTime + Gaze_HashIDs.CONDITIONS_UPDATE_INTERVAL)
            {
                UpdateTimeFrameStatus();

                // if in the appropriate time frame (ACTIVE)
                if (withinTimeFrameFlag)
                {

                    // check if we need to reload
                    HandleReload();

                    // check if a trigger occurs
                    HandleTrigger();
                }
            }
        }

        private void HandleTrigger()
        {
            // stop function if custom conditions are not fulfilled
            if (customConditionsEnabled && !ValidateCustomConditions())
                return;

            // if grab condition is not met
            if (grabEnabled && !grabValidated)
                return;

            // if touch condition is not met
            if (touchEnabled && !touchValidated)
                return;

            // if all trigger conditions are met
            if (canBeTriggered && Time.time > nextReloadTime)
            {
                gazeFlag = (gazeEnabled && isGazed) || !gazeEnabled;

                if (gazeFlag && proximitiesValidated)
                    UpdateFocus();
                else
                    ResetFocus();
            }
        }

        private void HandleReload()
        {
            if (reload)
            {
                // reload if necessary
                if (Time.time > nextReloadTime && reloadScheduled)
                {
                    Reload();

                    // notify manager
                    Gaze_EventManager.FireTriggerEvent(new Gaze_TriggerEventArgs(gameObject, Time.time, false, true, reloadCount, GetAutoTriggerMode(), GetReloadMode(), reloadMaxRepetitions));
                }
            }
        }

        private void ResetProximitiesCondition()
        {
            Debug.Log("ResetProximitiesCondition()");
            proximitiesValidated = false;
            proximityMap.ResetEveryoneColliding();
        }

        private void UpdateFocus()
        {
            // if focus is not complete
            if (!focusComplete || focusDuration <= 0f)
            {
                // set the flag
                focusInProgress = true;

                // update focus time
                focusTotalTime += Time.deltaTime;

                // set focused flag if focus time overpassed
                if (focusTotalTime >= focusDuration)
                {
                    focusComplete = true;
                    focusInProgress = false;
                    canBeTriggered = false;
                    TriggerCount++;

                    // notify manager
                    Gaze_EventManager.FireTriggerEvent(new Gaze_TriggerEventArgs(gameObject, Time.time, true, false, TriggerCount, GetAutoTriggerMode(), GetReloadMode(), reloadMaxRepetitions));

                    // check if reload needed
                    if (reload)
                    {
                        ScheduleAutoReload();
                    }

                    return;
                }
            }
        }

        private void ResetFocus()
        {
            // set the flag
            focusInProgress = false;

            if (GetFocusLossMode().Equals(Gaze_FocusLossMode.INSTANT))
            {
                // reset if INSTANT loss mode
                focusTotalTime = 0f;
            }
            else if (GetFocusLossMode().Equals(Gaze_FocusLossMode.FADE))
            {
                // decrease if FADE loss mode
                focusTotalTime = Mathf.Max(0, focusTotalTime - Time.deltaTime * FocusLossSpeed);
            }
        }

        private void UpdateTimeFrameStatus()
        {
            // while we were not in the time frame
            if (!withinTimeFrameFlag)
            {
                // check if we're now in the time frame
                IsWithinTimeFrame();
            }
            // while we were in the time frame but not after
            else if (!afterTimeFrameFlag)
            {
                // check if we're now after the time frame
                IsAfterTimeFrame();
            }
        }

        /// <summary>
        /// Is current time within the defined time frame
        /// </summary>
        /// <returns><c>true</c>, if current time is within absolute defined time, <c>false</c> otherwise.</returns>
        private bool IsWithinTimeFrame()
        {
            if (DependenciesValidated)
            {
                // if time is beyond the specified wait time
                if (!delayed || delayed && Time.time >= startTime + delayDuration)
                {
                    // if it never expires OR expires and below ending time frame
                    if (!expires || (expires && Time.time < startTime + delayDuration + activeDuration))
                    {
                        // update flag
                        withinTimeFrameFlag = true;

                        // notify manager
                        SetTriggerState(Gaze_TriggerState.ACTIVE);
                        canBeTriggered = true;

                        // trigger if auto-trigger is set to START
                        if (GetAutoTriggerMode().Equals(Gaze_AutoTriggerMode.START))
                        {
                            TriggerCount++;
                            canBeTriggered = false;
                            Gaze_EventManager.FireTriggerEvent(new Gaze_TriggerEventArgs(gameObject, Time.time, true, false, TriggerCount, GetAutoTriggerMode(), GetReloadMode(), reloadMaxRepetitions));

                            // check if reload needed
                            if (reload)
                            {
                                ScheduleAutoReload();
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Is Current time after the time frame.
        /// </summary>
        /// <returns><c>true</c>, if current time is after defined time frame, <c>false</c> otherwise.</returns>
        private bool IsAfterTimeFrame()
        {
            // if it expires
            if (expires)
            {
                // set delay duration
                delayDuration = delayed ? delayDuration : 0;

                // check if we're over the time limit
                if (Time.time >= startTime + delayDuration + activeDuration)
                {
                    // update flags
                    withinTimeFrameFlag = false;
                    afterTimeFrameFlag = true;

                    // trigger if auto-trigger is set to END
                    // FIX GAZE-193 GazeSDK_v0.5.6 : and if canBeTriggered
                    if (GetAutoTriggerMode().Equals(Gaze_AutoTriggerMode.END) && canBeTriggered)
                    {
                        TriggerCount++;
                        Gaze_EventManager.FireTriggerEvent(new Gaze_TriggerEventArgs(gameObject, Time.time, true, false, TriggerCount, GetAutoTriggerMode(), GetReloadMode(), reloadMaxRepetitions));
                    }

                    canBeTriggered = false;

                    // notify manager
                    SetTriggerState(Gaze_TriggerState.AFTER);

                    return true;
                }

            }

            // if deactivated by dependencies
            if (!DependenciesValidated)
            {
                canBeTriggered = false;

                // notify manager
                SetTriggerState(Gaze_TriggerState.AFTER);

                return true;
            }

            return false;
        }

        private bool CheckReloadParams()
        {
            // check delay parameters validity
            if (reloadDelay < 0)
            {
                // handle invalid delay value
                throw new ArgumentException("Invalid argument, Delay must be nonnegative.");
            }
            return true;
        }

        private void ScheduleAutoReload()
        {
            if (GetReloadMode().Equals(Gaze_ReloadMode.INFINITE) || (GetReloadMode().Equals(Gaze_ReloadMode.FINITE) && reloadCount < reloadMaxRepetitions))
            {
                ScheduleReload();
            }
        }

        private void ScheduleReload()
        {
            // update next reload time
            nextReloadTime = Time.time + reloadDelay;
            reloadScheduled = true;
        }

        private void Reload()
        {
            // increase reload count
            reloadCount++;

            // update focus values
            if (focusDuration > 0f)
            {
                focusTotalTime = 0;
                focusComplete = false;
            }

            // update reload pending flag
            reloadScheduled = false;
            canBeTriggered = true;
            grabValidated = false;
            touchValidated = false;
        }

        /// <summary>
        /// Starts reloading when reloadMode is manually conrolled in code.
        /// </summary>
        /// <param name="delay">Delay in secods.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="delay"/> must be nonnegative.
        /// </exception>
        public void ManualReload(float _delay = 0f)
        {
            if (!reload)
            {
                // handle invalid use of Reload function
                throw new InvalidOperationException("Invalid function call, Reload must be enabled.");
            }

            reloadDelay = _delay;
            CheckReloadParams();
            ScheduleReload();
        }

        private Gaze_FocusLossMode GetFocusLossMode()
        {
            return (Gaze_FocusLossMode)focusLossModeIndex;
        }

        private Gaze_ReloadMode GetReloadMode()
        {
            return (Gaze_ReloadMode)reloadModeIndex;
        }

        private Gaze_AutoTriggerMode GetAutoTriggerMode()
        {
            return (Gaze_AutoTriggerMode)autoTriggerModeIndex;
        }

        private Gaze_TriggerState GetTriggerState()
        {
            return (Gaze_TriggerState)triggerStateIndex;
        }

        private void SetTriggerState(Gaze_TriggerState state, bool forceNotify = false)
        {
            if (forceNotify || !GetTriggerState().Equals(state))
            {
                // update trigger status index
                triggerStateIndex = (int)state;

                // notify manager
                Gaze_EventManager.FireTriggerStateEvent(new Gaze_TriggerStateEventArgs(gameObject, state));
            }
        }

        private void ValidateDependencies(GameObject sender, int triggerStateIndex)
        {
            if (!DependenciesValidated && !ActivateOnDependencyMap.isEmpty())
            {
                Gaze_Dependency activator = ActivateOnDependencyMap.Get(sender);

                if (activator != null)
                {
                    ValidateDependency(activator, triggerStateIndex);

                    if (ValidateDependencyMap(ActivateOnDependencyMap, requireAllActivators))
                    {
                        DependenciesValidated = true;

                        // reset start time reference for next delay computation
                        Gaze_EventManager.FireOnDependenciesValidated(new Gaze_DependenciesValidatedEventArgs(this));
                        startTime = Time.time;
                    }
                }
            }
            else if (!DeactivateOnDependencyMap.isEmpty())
            {
                Gaze_Dependency deactivator = DeactivateOnDependencyMap.Get(sender);

                if (deactivator != null)
                {
                    ValidateDependency(deactivator, triggerStateIndex);

                    if (ValidateDependencyMap(DeactivateOnDependencyMap, requireAllDeactivators))
                    {
                        DependenciesValidated = false;
                    }
                }
            }
        }

        private void ValidateDependency(Gaze_Dependency d, int triggerStateIndex)
        {
            if (triggerStateIndex == -1)
            {
                // dependend on trigger
                d.satisfied = d.onTrigger;
            }
            else
            {
                // dependent on state
                d.satisfied = triggerStateIndex.Equals(d.triggerStateIndex);
            }
        }

        private bool ValidateDependencyMap(Gaze_DependencyMap dependencies, bool requireAll)
        {
            bool validated = requireAll;

            foreach (Gaze_Dependency d in dependencies.dependencies)
            {
                if (requireAll)
                {
                    validated &= d.satisfied;
                }
                else
                {
                    validated |= d.satisfied;
                }
            }

            return validated;
        }


        private int collisionsOccuringCount = 0;

        /// <summary>
        /// Checks the proximities conditions validity.
        /// </summary>
        /// <returns><c>true</c>, if proximities was checked, <c>false</c> otherwise.</returns>
        /// <param name="e">E.</param>
        private bool ValidateProximity(Gaze_ProximityEventArgs e)
        {


            if (proximityEnabled)
            {

                // get colliding objects
                GameObject sender = ((GameObject)e.Sender).GetComponentInChildren<Gaze_Proximity>().gameObject;
                GameObject other = ((GameObject)e.Other).GetComponentInChildren<Gaze_Proximity>().gameObject;

                // make sure the collision concerns two objects in the list of proximities (-1 if NOT)

                int otherIndex = IsCollidingObjectsInList(other, sender);
                //				Debug.Log ("otherIndex = " + otherIndex);
                if (otherIndex != -1)
                {

                    // OnEnter
                    if (e.IsInProximity)
                    {

                        // update number of collision in the list occuring
                        collisionsOccuringCount++;
                        proximityMap.AddCollidingObjectToEntry(proximityMap.proximityEntryList[otherIndex], sender.GetComponentInChildren<Gaze_Proximity>().IOGameObject);

                        if (proximityMap.proximityStateIndex.Equals((int)Gaze_ProximityStates.ENTER))
                        {
                            // get number of valid entries
                            int validatedEntriesCount = proximityMap.GetValidatedEntriesCount();

                            // OnEnter + RequireAll
                            if (requireAllProximities)
                            {
                                return validatedEntriesCount == proximityMap.proximityEntryList.Count;
                            }

                            // OnEnter + NOT RequireAll
                            if (!requireAllProximities)
                            {
                                return validatedEntriesCount >= 2;
                            }
                        }

                        // OnExit
                    }
                    else if (!e.IsInProximity)
                    {

                        // update everyoneIsColliding tag before removing an element
                        proximityMap.UpdateEveryoneColliding();

                        // remove colliding object
                        proximityMap.RemoveCollidingObjectToEntry(proximityMap.proximityEntryList[otherIndex], sender.GetComponentInChildren<Gaze_Proximity>().IOGameObject);
                        // if proximity condition is EXIT

                        if (proximityMap.proximityStateIndex.Equals((int)Gaze_ProximityStates.EXIT))
                        {

                            if (requireAllProximities)
                            {

                                // every entry was colliding before the exit
                                if (proximityMap.IsEveryoneColliding)
                                {
                                    //									proximityMap.ResetEveryoneColliding ();
                                    return true;
                                }

                                // OnExit + NOT RequireAll
                            }
                            else
                            {
                                proximityMap.ResetEveryoneColliding();

                                // if sender & other are in the list => true
                                return (proximityMap.ContainsEntry(sender) && proximityMap.ContainsEntry(other));
                            }
                        }
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Check if both colliding objects are in the list.
        /// </summary>
        /// <returns>The colliding's (_other) index within list.
        /// If one of the object is not in the list, return -1</returns>
        /// <param name="_other">Other.</param>
        /// <param name="_sender">Sender.</param>
        private int IsCollidingObjectsInList(GameObject _other, GameObject _sender)
        {
            //			Debug.Log ("other = " + _other.GetComponentInParent<Gaze_InteractiveObject> ().name + " and sender = " + _sender.GetComponentInParent<Gaze_InteractiveObject> ().name);
            int found = 0;
            int otherIndex = -1;
            int tmpIndex = -1;
            for (int i = 0; i < proximityMap.proximityEntryList.Count; i++)
            {
                //				Debug.Log ("proximityMap.proximityEntryList [i].dependentGameObject = " + proximityMap.proximityEntryList [i].dependentGameObject.GetComponentInParent<Gaze_InteractiveObject> ().name);
                if (proximityMap.proximityEntryList[i].dependentGameObject.Equals(_other))
                {
                    tmpIndex = i;
                    found++;
                }
                if (proximityMap.proximityEntryList[i].dependentGameObject.Equals(_sender))
                {
                    found++;
                }
                if (found == 2)
                {
                    otherIndex = tmpIndex;
                    break;
                }
            }
            //			Debug.Log (GetComponentInParent<Gaze_InteractiveObject> ().name + " found = " + found + " with otherIndex = " + otherIndex);
            return otherIndex;
        }
        /// <summary>
        /// Check if both colliding objects are in the list.
        /// </summary>
        /// <returns>The colliding's (_other) index within list.
        /// If one of the object is not in the list, return -1</returns>
        /// <param name="_other">Other.</param>
        /// <param name="_sender">Sender.</param>
        private int IsCollidingObjectsWithinList(GameObject _other, GameObject _sender)
        {
            int found = 0;
            int otherIndex = -1;
            int tmpIndex = -1;
            for (int i = 0; i < proximityMap.proximityEntryList.Count; i++)
            {
                if (proximityMap.proximityEntryList[i].dependentGameObject.Equals(_other))
                {
                    tmpIndex = i;
                    found++;
                }
                if (proximityMap.proximityEntryList[i].dependentGameObject.Equals(_sender))
                {
                    found++;
                }
                if (found == 2)
                {
                    otherIndex = tmpIndex;
                    break;
                }
            }
            return otherIndex;
        }

        private bool ValidateCustomConditions()
        {
            foreach (KeyValuePair<int, bool> condition in customConditionsDico)
            {
                if (condition.Value == false)
                    return false;
            }
            return true;
        }



        private void ValidateGrab(Gaze_ControllerGrabEventArgs e)
        {
            // TODO(4nc3str4l): Validate if this lines need to be uncommented. 
            //if (!DependenciesValidated)
            //    grabValidated = false;
            //else
            grabValidated = ValidateGrabController(e);
        }

        private bool IsGrabbingControllerInMap(VRNode grabbingController)
        {
            for (int i = 0; i < grabMap.grabEntryList.Count; i++)
            {
                if (grabMap.grabEntryList[i].hand.Equals(grabbingController))
                    return true;
            }

            return false;
        }

        private bool IsGrabbingControllerStateValid(bool _isGrabbing, Gaze_HandsEnum _mapHand, VRNode _dicoHand)
        {
            if (_mapHand.Equals(Gaze_HandsEnum.BOTH))
            {
                // check left hand state is ok
                if (_dicoHand.Equals(VRNode.LeftHand))
                    grabStateLeftValid = (_isGrabbing && grabMap.grabStateLeftIndex.Equals((int)Gaze_GrabStates.GRAB)) || (!_isGrabbing && grabMap.grabStateLeftIndex.Equals((int)Gaze_GrabStates.UNGRAB));

                // check right hand state is ok
                if (_dicoHand.Equals(VRNode.RightHand))
                    grabStateRightValid = (_isGrabbing && grabMap.grabStateRightIndex.Equals((int)Gaze_GrabStates.GRAB)) || (!_isGrabbing && grabMap.grabStateRightIndex.Equals((int)Gaze_GrabStates.UNGRAB));

                return grabStateLeftValid && grabStateRightValid;
            }
            else
            {
                int state = _mapHand.Equals(Gaze_HandsEnum.LEFT) ? grabMap.grabStateLeftIndex : grabMap.grabStateRightIndex;

                if (_isGrabbing && state.Equals((int)Gaze_GrabStates.GRAB))
                    return true;

                if (!_isGrabbing && state.Equals((int)Gaze_GrabStates.UNGRAB))
                    return true;
            }

            return false;
        }

        private bool IsGrabbingObjectValid(GameObject _grabbedObject, int _handIndex)
        {
            int index = _handIndex.Equals((int)Gaze_HandsEnum.BOTH) ? 1 : 0;
            return _grabbedObject.Equals(grabMap.grabEntryList[index].interactiveObject);
        }


        private bool ValidateGrabController(Gaze_ControllerGrabEventArgs e)
        {
            VRNode dicoVRNode = e.ControllerObjectPair.Key;
            GameObject grabbedObject = e.ControllerObjectPair.Value;

            // return if there's no object in the Dico, otherwise, store it !
            if (grabbedObject == null)
                return false;

            // get the hand VRNode from the event
            bool isGrabbingControllerLeft = e.ControllerObjectPair.Key == VRNode.LeftHand;
            VRNode eventVRNode = isGrabbingControllerLeft ? VRNode.LeftHand : VRNode.RightHand;

            bool grabbedObjectValid = IsGrabbingObjectValid(grabbedObject, grabMap.grabHandsIndex);
            touchLeftValid = IsGrabbingControllerInMap(dicoVRNode) && IsGrabbingControllerStateValid(e.IsGrabbing, Gaze_HandsEnum.LEFT, eventVRNode) && grabbedObjectValid;


            // if we've configured
            switch (grabMap.grabHandsIndex)
            {
                case (int)Gaze_HandsEnum.BOTH:
                    bool isGrabbingControllerStateValid = IsGrabbingControllerStateValid(e.IsGrabbing, Gaze_HandsEnum.BOTH, eventVRNode);
                    bool isGrabbingControllerInMap = IsGrabbingControllerInMap(dicoVRNode);

                    return isGrabbingControllerInMap && isGrabbingControllerStateValid && grabbedObjectValid;
                    break;

                //  the LEFT hand
                case (int)Gaze_HandsEnum.LEFT:
                    touchLeftValid = IsGrabbingControllerInMap(dicoVRNode) && IsGrabbingControllerStateValid(e.IsGrabbing, Gaze_HandsEnum.LEFT, eventVRNode) && grabbedObjectValid;
                    return touchLeftValid;
                    break;

                //  the RIGHT hand
                case (int)Gaze_HandsEnum.RIGHT:
                    grabRightValid = IsGrabbingControllerInMap(dicoVRNode) && IsGrabbingControllerStateValid(e.IsGrabbing, Gaze_HandsEnum.RIGHT, eventVRNode) && grabbedObjectValid;
                    return grabRightValid;
                    break;
            }

            return false;
        }

        private bool IsTouchObjectValid(GameObject _touchedObject, int _handIndex)
        {
            if (_touchedObject == null)
                return false;

            int index = _handIndex.Equals((int)Gaze_HandsEnum.BOTH) ? 1 : 0;
            return _touchedObject.Equals(touchMap.touchEntryList[index].interactiveObject);
        }

        private bool IsTouchControllerValid(VRNode _touchingController)
        {
            for (int i = 0; i < touchMap.touchEntryList.Count; i++)
            {
                if (touchMap.touchEntryList[i].hand.Equals(_touchingController))
                    return true;
            }

            return false;
        }

        private bool IsTouchDistanceValid(Gaze_TouchDistanceMode _eventDistance, VRNode _eventHand)
        {
            Gaze_HandsEnum mapHand = Gaze_HandsEnum.BOTH;

            switch (touchMap.touchHandsIndex)
            {
                // LEFT hands
                case (int)Gaze_HandsEnum.LEFT:
                    mapHand = Gaze_HandsEnum.LEFT;
                    break;

                // RIGHT hands
                case (int)Gaze_HandsEnum.RIGHT:
                    mapHand = Gaze_HandsEnum.RIGHT;
                    break;
            }

            if (mapHand.Equals(Gaze_HandsEnum.BOTH))
            {
                // check left hand mode 
                if (_eventHand.Equals(VRNode.LeftHand))
                {
                    // if both modes are allowed, we're sure this is valid
                    if (touchMap.touchDistanceModeLeftIndex.Equals((int)Gaze_TouchDistanceMode.BOTH))
                    {
                        touchDistanceModeLeftValid = true;
                    }
                    else
                    {
                        touchDistanceModeLeftValid = ((_eventDistance.Equals(Gaze_TouchDistanceMode.DISTANT) && touchMap.touchDistanceModeLeftIndex.Equals((int)Gaze_TouchDistanceMode.DISTANT))) || ((_eventDistance.Equals(Gaze_TouchDistanceMode.PROXIMITY) && touchMap.touchDistanceModeLeftIndex.Equals((int)Gaze_TouchDistanceMode.PROXIMITY))) ? true : false;
                    }
                }

                // check right hand mode
                if (_eventHand.Equals(VRNode.RightHand))
                {
                    // if both modes are allowed, we're sure this is valid
                    if (touchMap.touchDistanceModeRightIndex.Equals((int)Gaze_TouchDistanceMode.BOTH))
                    {
                        touchDistanceModeRightValid = true;
                    }
                    else
                    {
                        touchDistanceModeRightValid = ((_eventDistance.Equals(Gaze_TouchDistanceMode.DISTANT) && touchMap.touchDistanceModeRightIndex.Equals((int)Gaze_TouchDistanceMode.DISTANT))) || ((_eventDistance.Equals(Gaze_TouchDistanceMode.PROXIMITY) && touchMap.touchDistanceModeRightIndex.Equals((int)Gaze_TouchDistanceMode.PROXIMITY))) ? true : false;
                    }
                }

                if (requireAllTouchables)
                    return touchDistanceModeLeftValid && touchDistanceModeRightValid;
                else
                    return touchDistanceModeLeftValid || touchDistanceModeRightValid ? true : false;
            }
            else
            {
                touchDistanceModeLeftValid = false;
                touchDistanceModeRightValid = false;

                // get the distance mode of the configured hand in the conditions
                int mapDistanceModeIndex = mapHand.Equals(Gaze_HandsEnum.LEFT) ? touchMap.touchDistanceModeLeftIndex : touchMap.touchDistanceModeRightIndex;

                if (_eventDistance.Equals(Gaze_TouchDistanceMode.PROXIMITY) && mapDistanceModeIndex.Equals((int)Gaze_TouchDistanceMode.PROXIMITY))
                {
                    if (mapHand.Equals(Gaze_HandsEnum.LEFT))
                    {
                        //Debug.Log("distanceModeIndex.Equals((int)Gaze_TouchDistanceMode.PROXIMITY) = " + distanceModeIndex.Equals((int)Gaze_TouchDistanceMode.PROXIMITY) + " and _eventDistanceMode =" + _eventDistanceMode);
                        touchDistanceModeLeftValid = true;
                        return true;
                    }
                    else if (mapHand.Equals(Gaze_HandsEnum.RIGHT))
                    {
                        touchDistanceModeRightValid = true;
                        return true;
                    }
                }
                else if (_eventDistance.Equals(Gaze_TouchDistanceMode.DISTANT) && mapDistanceModeIndex.Equals((int)Gaze_TouchDistanceMode.DISTANT))
                {
                    if (mapHand.Equals(Gaze_HandsEnum.LEFT))
                    {
                        touchDistanceModeLeftValid = true;
                        return true;
                    }
                    else if (mapHand.Equals(Gaze_HandsEnum.RIGHT))
                    {
                        touchDistanceModeRightValid = true;
                        return true;
                    }
                }
                else if (mapDistanceModeIndex.Equals((int)Gaze_TouchDistanceMode.BOTH))
                {
                    if (mapHand.Equals(Gaze_HandsEnum.LEFT))
                    {
                        touchDistanceModeLeftValid = true;
                        return true;
                    }
                    else if (mapHand.Equals(Gaze_HandsEnum.RIGHT))
                    {
                        touchDistanceModeRightValid = true;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Return TRUE if the Action is valid.
        /// Action is valid if action in the map equals action received.
        /// Action can be TOUCH, UNTOUCH or BOTH.
        /// </summary>
        /// <param name="_dicoVRNode"></param>
        /// <param name="_eventVRNode"></param>
        /// <returns></returns>
        private bool IsTouchActionValid(VRNode _touchingController, bool _isTouching)
        {
            int eventActionIndex = 0;
            switch (touchMap.touchHandsIndex)
            {
                //TODO @apelab BOTH condition


                // RIGHT hand
                case (int)Gaze_HandsEnum.RIGHT:
                    eventActionIndex = touchMap.touchActionRightIndex;
                    break;

                // LEFT hand
                case (int)Gaze_HandsEnum.LEFT:
                    eventActionIndex = touchMap.touchActionLeftIndex;
                    break;
            }

            if ((_touchingController.Equals(VRNode.LeftHand) && touchMap.touchActionLeftIndex.Equals(eventActionIndex)) ||
                (_touchingController.Equals(VRNode.RightHand) && touchMap.touchActionRightIndex.Equals(eventActionIndex)))
            {
                // if I'm touching AND the action is TOUCH AND the trigger is PRESSED
                if (_isTouching && eventActionIndex.Equals((int)Gaze_TouchAction.TOUCH) && isTriggerPressed)
                    return true;

                // if I'm touching AND the action is UNTOUCH AND the trigger is RELEASED
                if (_isTouching && eventActionIndex.Equals((int)Gaze_TouchAction.UNTOUCH) && !isTriggerPressed)
                    return true;

                // if I'm not touching AND action is UNTOUCH and trigger is PRESSED, that means we pointed OUT
                if (!_isTouching && eventActionIndex.Equals((int)Gaze_TouchAction.UNTOUCH) && isTriggerPressed)
                    return true;
            }

            //TODO @apelab BOTH hands condition

            return false;
        }

        private bool IsTouchInputValid(bool _isTriggerPressed)
        {
            int eventActionIndex = 0;
            switch (touchMap.touchHandsIndex)
            {
                // RIGHT hand
                case (int)Gaze_HandsEnum.RIGHT:
                    eventActionIndex = touchMap.touchActionRightIndex;
                    break;

                // LEFT hand
                case (int)Gaze_HandsEnum.LEFT:
                    eventActionIndex = touchMap.touchActionLeftIndex;
                    break;
            }

            // if trigger is pressed and action is TOUCH return TRUE
            if ((_isTriggerPressed && eventActionIndex.Equals((int)Gaze_TouchAction.TOUCH)) ||
                (!_isTriggerPressed && eventActionIndex.Equals((int)Gaze_TouchAction.UNTOUCH)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// To validate a touch, multiple conditions need to be met.
        /// - Controller : must match the condition (LEFT, RIGHT, BOTH)
        /// - Object : must match the condition (the one specified in the Touch conditions)
        /// - Action : must match the condition (GRAB, UNGRAB, BOTH)
        /// - Distance : must match the condition (PROXIMITY, DISTANT, BOTH)
        /// </summary>
        /// <param name="_eventHand"></param>
        /// <param name="_touchedObject"></param>
        /// <param name="_eventMode"></param>
        /// <param name="_isTouching"></param>
        /// <returns></returns>
        private bool ValidateTouchConditions()
        {
            bool isTouchedObjectValid = IsTouchObjectValid(touchedObject, touchMap.touchHandsIndex);
            bool isTouchControllerValid = IsTouchControllerValid(eventHand);
            bool isTouchActionValid = IsTouchActionValid(eventHand, isTouched);
            bool isTouchDistanceValid = IsTouchDistanceValid(distanceMode, eventHand);
            bool valid = false;

            // if we've configured
            switch (touchMap.touchHandsIndex)
            {
                // BOTH hands
                case (int)Gaze_HandsEnum.BOTH:

                    valid = isTouchControllerValid && isTouchDistanceValid && isTouchedObjectValid;
                    break;

                //  the LEFT hand
                case (int)Gaze_HandsEnum.LEFT:
                    touchLeftValid = isTouchActionValid && isTouchControllerValid && isTouchDistanceValid && isTouchedObjectValid;

                    valid = touchLeftValid;
                    break;

                //  the RIGHT hand
                case (int)Gaze_HandsEnum.RIGHT:
                    touchRightValid = isTouchActionValid && isTouchControllerValid && isTouchDistanceValid && isTouchedObjectValid;

                    valid = touchRightValid;
                    break;
            }

            return valid;
        }

        /// <summary>

        /// Get the pointed object in the dico argument
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private GameObject GetPointedObject(Gaze_ControllerPointingEventArgs e)
        {
            GameObject _object = null;
            if (isActive)
            {

                switch (touchMap.touchHandsIndex)
                {

                    case (int)Gaze_HandsEnum.BOTH:
                        if (e.Dico.ContainsKey(VRNode.LeftHand))
                        {
                            e.Dico.TryGetValue(VRNode.LeftHand, out _object);
                        }
                        else if (e.Dico.ContainsKey(VRNode.RightHand))
                        {
                            e.Dico.TryGetValue(VRNode.RightHand, out _object);
                        }

                        break;

                    case (int)Gaze_HandsEnum.LEFT:
                        e.Dico.TryGetValue(VRNode.LeftHand, out _object);
                        break;

                    case (int)Gaze_HandsEnum.RIGHT:
                        e.Dico.TryGetValue(VRNode.RightHand, out _object);
                        break;
                }
            }
            else
            {
                if (e.Dico.ContainsKey(VRNode.LeftHand))
                    e.Dico.TryGetValue(VRNode.LeftHand, out _object);
                else
                    e.Dico.TryGetValue(VRNode.RightHand, out _object);
            }

            return _object;
        }

        private void OnGazeEvent(Gaze_GazeEventArgs e)
        {
            // if sender is the gazable collider GameObject specified in the InteractiveObject Gaze field
            if (e.Sender != null && ((GameObject)e.Sender).GetComponentInParent<Gaze_InteractiveObject>().gameObject.Equals(root))
            {
                isGazed = e.IsGazed;
            }
        }

        private void OnTriggerEvent(Gaze_TriggerEventArgs e)
        {
            if (e.Sender != null)
            {
                // if I'm not the gazed object
                if (!((GameObject)e.Sender).Equals(gameObject) && e.IsTrigger)
                {
                    if (dependent)
                    {
                        // check if the sender is one of my dependencies
                        ValidateDependencies((GameObject)e.Sender, -1);
                    }
                }
            }
        }

        private void OnTriggerStateEvent(Gaze_TriggerStateEventArgs e)
        {
            if (e.Sender != null)
            {
                // if I'm the gazed object
                if (!((GameObject)e.Sender).Equals(gameObject))
                {
                    if (dependent)
                    {
                        // check if the sender is one of my dependencies
                        ValidateDependencies((GameObject)e.Sender, (int)e.TriggerState);
                    }
                }
            }
        }

        private void OnProximityEvent(Gaze_ProximityEventArgs e)
        {
            if (e.Sender.Equals(root))
                isInProximity = e.IsInProximity;

            if (proximityEnabled)
            {
                proximitiesValidated = ValidateProximity(e);
            }
        }

        private void OnCustomConditionEvent(Gaze_CustomConditionEventArgs e)
        {
            customConditionsDico[(int)e.Sender] = e.IsValid;
        }

        private void OnDrawGizmos()
        {
            String gizmo;

            if (gameObject.GetComponent<Gaze_SceneLoader>())
            {
                gizmo = "Gaze SDK/Gaze_SimpleSceneLoader.png";
            }
            else if (!dependent || ActivateOnDependencyMap.isEmpty())
            {
                gizmo = "Gaze SDK/Gaze_Gazable_starter.png";
            }
            else
            {
                gizmo = "Gaze SDK/Gaze_Gazable.png";
            }

            Gizmos.DrawIcon(transform.position, gizmo, true);

            if (ShowDependencies && dependent)
            {
                Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                DrawConnections(ActivateOnDependencyMap);
                DrawConnections(DeactivateOnDependencyMap);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (ShowDependencies && dependent)
            {
                Gizmos.color = Color.green;
                DrawConnections(ActivateOnDependencyMap, 0.03f);

                Gizmos.color = Color.red;
                DrawConnections(DeactivateOnDependencyMap, 0.03f);
            }
        }

        private void OnControllerGrabEvent(Gaze_ControllerGrabEventArgs e)
        {
            if (grabEnabled)
                ValidateGrab(e);
        }

        private void OnControllerCollisionEvent(Gaze_ControllerCollisionEventArgs e)
        {
            if (touchEnabled)
            {
                VRNode eventHand = ((GameObject)e.Sender).GetComponentInParent<Gaze_InteractiveObject>().gameObject.GetComponentInChildren<Gaze_GrabManager>().isLeftHand ? VRNode.LeftHand : VRNode.RightHand;

                // We can onli touch with handle!
                if (e.Other.GetComponent<Gaze_Handle>() == null)
                    return;

                // if I'm concerned
                Gaze_InteractiveObject touchedObject = e.Other.GetComponentInParent<Gaze_InteractiveObject>();
                if (touchedObject.Equals(rootIO))
                {
                    // update touch state
                    bool isColliding = e.CollisionType.Equals(Gaze_CollisionTypes.COLLIDER_EXIT) ? false : true;

                    if (eventHand.Equals(VRNode.LeftHand))
                        isLeftColliding = isColliding;

                    if (eventHand.Equals(VRNode.RightHand))
                        isRightColliding = isColliding;

                    //CheckUntouch(eventHand, isColliding, touchedObject.gameObject, Gaze_TouchDistanceMode.PROXIMITY, isTriggerPressed);
                }
            }
        }

        private void OnControllerPointingEvent(Gaze_ControllerPointingEventArgs e)
        {
            // get the pointed object
            pointedObject = GetPointedObject(e);

            // if this object is me
            if (pointedObject && pointedObject.Equals(GetComponentInParent<Gaze_InteractiveObject>().gameObject))
            {
                Debug.Log(this + " pointedObject  = " + pointedObject + " (" + e.IsPointed + ")");
                // get the event's pointing hand
                eventHand = e.Dico.ContainsKey(VRNode.LeftHand) ? VRNode.LeftHand : VRNode.RightHand;

                // update touch state for inspector GUI
                if (eventHand.Equals(VRNode.LeftHand))
                    isLeftPointing = e.IsPointed;
                else if (eventHand.Equals(VRNode.RightHand))
                    isRightPointing = e.IsPointed;

                // check if touch is valid
                touchValidated = ValidateTouchConditions();
            }
        }

        private void OnControllerTouchEvent(Gaze_ControllerTouchEventArgs e)
        {
            if (touchEnabled)
            {
                // store the touched object
                eventHand = e.Dico.ContainsKey(VRNode.LeftHand) ? VRNode.LeftHand : VRNode.RightHand;
                e.Dico.TryGetValue(eventHand, out touchedObject);

                // if I'm concerned
                if (touchedObject.Equals(root))
                {
                    // update members
                    isTriggerPressed = e.IsTriggerPressed;
                    isTouched = e.IsTouching;
                    distanceMode = e.Mode;

                    // check if touch is valid
                    touchValidated = ValidateTouchConditions();
                }
            }
        }

        private void DrawConnections(Gaze_DependencyMap map, float width = 0.01f)
        {
            if (!cubeMesh)
            {
                cubeMesh = MakeCube();
            }

            Vector3 p0, p1, mid, direction, normal;
            Quaternion rotation;
            float length;

            foreach (Gaze_Dependency d in map.dependencies)
            {
                p0 = transform.position;
                p1 = d.dependentGameObject.transform.position;
                mid = (p0 + p1) * 0.5f;
                direction = (p1 - p0).normalized;
                normal = Vector3.Cross(direction, transform.forward).normalized;
                length = (p1 - p0).magnitude * 0.5f;
                rotation = Quaternion.identity;
                if (normal.magnitude > 0)
                {
                    rotation.SetLookRotation(direction, normal);
                }

                Gizmos.DrawMesh(cubeMesh, mid, rotation, new Vector3(width, width, length));
            }
        }

        private Mesh MakeCube()
        {
            Vector3[] vertexList = {
                new Vector3 (-1, -1, -1),
                new Vector3 (-1, 1, -1),
                new Vector3 (1, 1, -1),
                new Vector3 (1, -1, -1),
                new Vector3 (1, -1, 1),
                new Vector3 (1, 1, 1),
                new Vector3 (-1, 1, 1),
                new Vector3 (-1, -1, 1)
            };

            int[] faceList = {
                0, 1, 3, // back
                0, 2, 3,
                3, 2, 5, // right
                3, 5, 4,
                5, 2, 1, // top
                5, 1, 6,
                3, 4, 7, // bottom
                3, 7, 0,
                0, 7, 6, // left
                0, 6, 1,
                4, 5, 6, // front
                4, 6, 7
            };

            Mesh mesh = new Mesh();

            mesh.vertices = vertexList;
            mesh.triangles = faceList;
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}