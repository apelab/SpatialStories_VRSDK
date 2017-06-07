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
        #region OwnEvents
        public delegate void OnReloadHandler(bool _reloadDependencies);
        public event OnReloadHandler OnReload;
        public void FireOnReloadEvent(bool _reloadDependencies)
        {
            if (OnReload != null)
                OnReload(_reloadDependencies);
        }
        #endregion OwnEvents


        #region members

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
        public static bool showDependencies;
        public static Mesh cubeMesh;

        public Gaze_ProximityMap proximityMap = new Gaze_ProximityMap();
        public Gaze_GrabMap grabMap = new Gaze_GrabMap();

        // change Component to interface when ready
        public List<Gaze_AbstractConditions> customConditions = new List<Gaze_AbstractConditions>();
        public Dictionary<int, bool> customConditionsDico = new Dictionary<int, bool>();

        /// <summary>
        /// index of this gazable's current trigger status
        /// </summary>
        public int triggerStateIndex;

        /// <summary>
        /// This trigger must wait 'waitTime' before being in acisdelayedRandomtive mode.
        /// This wait time is dependant on triggerObject start if any otherwise
        /// its dependant on the load level scene time.
        /// </summary>
        /// <value>The duration in seconds</value>
        public float TFdelayDuration;

        /// <summary>
        /// The toggle to display delay duration in the Editor
        /// Used in Gaze_TriggerSettingsEditor.cs
        /// </summary>
        public bool TFDelayed;

        /// <summary>
        /// If true, 'TFdelayDuration' will be set to a random value between TFDelayRange[0] and TFDelayRange[1]
        /// </summary>
        public bool isTFDelayRandom;

        public float[] TFDelayRange = { 0.0f, 0.0f };

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
        public int TriggerCount;

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
        /// Is Proximity condition enabled.
        /// Corresponds to the editor checkbox as a trigger condition
        /// </summary>
        public bool proximityEnabled;

        /// <summary>
        /// Is Grab condition enabled.
        /// Corresponds to the editor checkbox as a trigger condition
        /// </summary>
        public bool grabEnabled;

        /// <summary>
        /// The index corresponding to the chosen teleport enum in the editor
        /// </summary>
        public int teleportIndex;

        /// <summary>
        /// Is teleport condition enabled.
        /// Corresponds to the editor checkbox as a trigger condition
        /// </summary>
        public bool teleportEnabled;

        /// <summary>
        /// Is Grab condition enabled.
        /// Corresponds to the editor checkbox as a trigger condition
        /// </summary>
        public bool inputsEnabled;
        public Gaze_InputsMap InputsMap = new Gaze_InputsMap();

        /// <summary>
        /// if FALSE, require only one input to be validated among the specified list
        /// </summary>
        public bool requireAllInputs;

        /// <summary>
        /// Are Custom conditions enabled.
        /// Corresponds to the editor checkbox as a trigger condition
        /// </summary>
        public bool customConditionsEnabled;

        /// <summary>
        /// All the proximities in the list are required in order to be validated.
        /// </summary>
        public bool requireAllProximities;

        private List<GameObject> collidingProximities;

        public List<GameObject> CollidingProximities { get { return collidingProximities; } }

        private GameObject root;

        public GameObject Root { get { return root; } }

        private Gaze_InteractiveObject rootIO;

        public Gaze_InteractiveObject RootIO { get { return rootIO; } }

        public Gaze_TouchMap touchMap = new Gaze_TouchMap();
        /// <summary>
        /// Is Gaze condition enabled.
        /// Corresponds to the editor checkbox as a trigger condition.
        /// </summary>
        public bool gazeEnabled;
        public bool touchEnabled;

        public Gaze_DependencyMap ActivateOnDependencyMap = new Gaze_DependencyMap();
        public Gaze_DependencyMap DeactivateOnDependencyMap = new Gaze_DependencyMap();

        public List<Gaze_AbstractCondition> activeConditions = new List<Gaze_AbstractCondition>();

        // This list has all the AbstractConditions in order to setup & dipose them automatically
        // it contains Dependencies, Conditions, Tiframe, everything don't remove anything from here.
        public List<Gaze_AbstractCondition> allConditions = new List<Gaze_AbstractCondition>();

        public Gaze_InteractiveObject gazeColliderIO;

        public bool ReloadDependencies = false;

        public bool isActive;

        #endregion

        private void Awake()
        {
            if (Application.isPlaying)
                SetupConditionsToCheck();
        }

        private void SetupConditionsToCheck()
        {
            activeConditions.Clear();

            if (gazeEnabled)
                activeConditions.Add(new Gaze_GazeCondition(this, gazeColliderIO.GetComponentInChildren<Gaze_Gaze>().GetComponent<Collider>()));

            if (proximityEnabled)
                activeConditions.Add(new Gaze_ProximityCondition(this));

            if (touchEnabled)
                activeConditions.Add(new Gaze_TouchCondition(this, touchMap.TouchEnitry.interactiveObject.GetComponentInChildren<Gaze_Gaze>().GetComponent<Collider>()));

            if (grabEnabled)
                activeConditions.Add(new Gaze_GrabCondition(this));

            if (inputsEnabled)
                activeConditions.Add(new Gaze_InputsCondition(this));

            if (teleportEnabled)
                activeConditions.Add(new Gaze_TeleportCondition(this));
        }

        void OnEnable()
        {

            rootIO = GetComponentInParent<Gaze_InteractiveObject>();
            root = rootIO.gameObject;

            if (Application.isPlaying)
            {
                ActivateOnDependencyMap.OnEnable(this);
                DeactivateOnDependencyMap.OnEnable(this);

                foreach (Gaze_AbstractCondition conditions in allConditions)
                    conditions.Enable();

                Gaze_EventManager.OnTriggerStateEvent += OnTriggerStateEvent;
                Gaze_EventManager.OnTriggerEvent += OnTriggerEvent;
                Gaze_EventManager.OnCustomConditionEvent += OnCustomConditionEvent;

                if (customConditionsDico.Count != customConditions.Count)
                {
                    foreach (Gaze_AbstractConditions condition in customConditions)
                    {
                        customConditionsDico.Add(condition.GetInstanceID(), false);
                    }
                }
            }
        }

        void OnDisable()
        {
            if (Application.isPlaying)
            {
                ActivateOnDependencyMap.OnDisable(this);
                DeactivateOnDependencyMap.OnDisable(this);

                foreach (Gaze_AbstractCondition conditions in allConditions)
                    conditions.Disable();

                Gaze_EventManager.OnTriggerStateEvent -= OnTriggerStateEvent;
                Gaze_EventManager.OnTriggerEvent -= OnTriggerEvent;
                Gaze_EventManager.OnCustomConditionEvent -= OnCustomConditionEvent;
            }
        }

        void Start()
        {
            SetRandomTFDelay();
            focusInProgress = false;
            focusComplete = focusDuration <= 0 ? true : false;
            ActivateOnDependencyMap.AreDependenciesSatisfied = dependent ? false : true;
            InputsMap.AreDependenciesSatisfied = inputsEnabled ? false : true;
            startTime = Time.time;
            reloadCount = 0;

            // notify we're before the activity window time
            SetTriggerState(Gaze_TriggerState.BEFORE, true);

            Gaze_EventManager.FireTriggerEvent(new Gaze_TriggerEventArgs(gameObject, Time.time, false, false, -1, GetAutoTriggerMode(), GetReloadMode(), reloadMaxRepetitions));

            // check loss speed is valid if used
            FocusLossSpeed = (GetFocusLossMode() == Gaze_FocusLossMode.FADE && FocusLossSpeed > 0) ? FocusLossSpeed : 1f;

            CheckReloadParams();

            collidingProximities = new List<GameObject>();
        }

        void Update()
        {
            UpdateTimeFrameStatus();

            // if in the appropriate time frame (ACTIVE)
            if (triggerStateIndex == (int)Gaze_TriggerState.ACTIVE)
            {
                // check if we need to reload
                HandleReload();

                // check if a trigger occurs
                HandleTrigger();
            }
        }

        private void HandleTrigger()
        {
            if (!Application.isPlaying)
                return;

            // Thats the only line of code that should be here after the ref
            foreach (Gaze_AbstractCondition condition in activeConditions)
            {
                if (!condition.IsValidated())
                    return;
            }

            // stop function if custom conditions are not fulfilled
            if (customConditionsEnabled)
            {
                if (!ValidateCustomConditions())
                    return;
            }

            // if all interaction's conditions are met
            if (canBeTriggered && Time.time > nextReloadTime)
            {
                var gazeFlag = (gazeEnabled && GetConditionOfType<Gaze_GazeCondition>().IsValidated()) || !gazeEnabled;

                var proximitiesValidated = GetConditionOfType<Gaze_ProximityCondition>() == null ||
                    GetConditionOfType<Gaze_ProximityCondition>().IsValidated();

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
                    //Debug.Log(name + " Is triggered!!!");

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
            if (triggerStateIndex != (int)Gaze_TriggerState.ACTIVE)
            {
                // check if we're now in the time frame
                IsWithinTimeFrame();
            }
            // while we were in the time frame but not after
            else
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
            if (ActivateOnDependencyMap.AreDependenciesSatisfied)
            {
                // if time is beyond the specified wait time
                if (TFDelayed && Time.time >= startTime + TFdelayDuration || !TFDelayed)
                {
                    // if it never expires OR expires and below ending time frame
                    if (!expires || (expires && Time.time < startTime + TFdelayDuration + activeDuration))
                    {
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
                TFdelayDuration = TFDelayed ? TFdelayDuration : 0;

                // check if we're over the time limit
                if (Time.time >= startTime + TFdelayDuration + activeDuration)
                {

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
            if (!ActivateOnDependencyMap.AreDependenciesSatisfied)
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

            // Update reload pending flag
            // All of these should go away
            reloadScheduled = false;

            FireOnReloadEvent(ReloadDependencies);

            // If we have dependencies we need to set the trigger state to before.
            if (ActivateOnDependencyMap.dependencies.Count > 0)
            {

                SetTriggerState(Gaze_TriggerState.BEFORE);
                canBeTriggered = false;
            }
            else
            {
                canBeTriggered = true;
            }
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
            if (!ActivateOnDependencyMap.AreDependenciesSatisfied && !ActivateOnDependencyMap.isEmpty())
            {
                Gaze_Dependency activator = ActivateOnDependencyMap.Get(sender);

                if (activator != null)
                {
                    ValidateDependency(activator, triggerStateIndex);

                    if (ValidateDependencyMap(ActivateOnDependencyMap, requireAllActivators))
                    {
                        ActivateOnDependencyMap.AreDependenciesSatisfied = true;

                        // reset start time reference for next delay computation
                        startTime = Time.time;
                    }
                }
            }

            if (!DeactivateOnDependencyMap.isEmpty())
            {
                Gaze_Dependency deactivator = DeactivateOnDependencyMap.Get(sender);

                if (deactivator != null)
                {
                    ValidateDependency(deactivator, triggerStateIndex);

                    if (ValidateDependencyMap(DeactivateOnDependencyMap, requireAllDeactivators))
                    {
                        ActivateOnDependencyMap.AreDependenciesSatisfied = false;
                    }
                }
            }
        }

        private void ValidateDependency(Gaze_Dependency d, int triggerStateIndex)
        {
            if (triggerStateIndex == -1)
            {
                // dependend on trigger
                d.SetSatisfied(d.onTrigger);
            }
            else
            {
                // dependent on state
                d.SetSatisfied(triggerStateIndex.Equals(d.triggerStateIndex));
            }
        }

        private bool ValidateDependencyMap(Gaze_DependencyMap dependencies, bool requireAll)
        {
            bool validated = requireAll;

            foreach (Gaze_Dependency d in dependencies.dependencies)
            {
                if (requireAll)
                {
                    validated &= d.IsValidated();
                }
                else
                {
                    validated |= d.IsValidated();
                }
            }

            return validated;
        }

        private bool ValidateCustomConditions()
        {
            foreach (Gaze_AbstractConditions cond in customConditions)
            {
                if (cond.IsValid == false)
                    return false;
            }
            return true;
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

        private void OnCustomConditionEvent(Gaze_CustomConditionEventArgs e)
        {
            customConditionsDico[(int)e.Sender] = e.IsValid;
        }


        private void SetRandomTFDelay()
        {
            if (isTFDelayRandom)
                TFdelayDuration = UnityEngine.Random.Range(TFDelayRange[0], TFDelayRange[1]);
        }


        #region ConditionListManagement

        public T GetConditionOfType<T>() where T : Gaze_AbstractCondition
        {
            if (activeConditions == null)
                return null;

            foreach (Gaze_AbstractCondition cond in activeConditions)
                if (cond is T)
                    return (T)cond;
            return null;

        }

        public bool IsConditionOfTypeValid<T>() where T : Gaze_AbstractCondition
        {
            if (activeConditions == null)
                return false;

            T cond = GetConditionOfType<T>();

            if (cond == null)
                return false;

            return cond.IsValidated();
        }

        #endregion ConditionListManagement

        #region VarsToDestroyAfterRefactoring
        /// <summary>
        /// This group of vars are here in order to allow us to create a fast
        /// integration on the new SDK without modifying a lot of things
        /// but should be removed as fast as we can in order to clean the project
        /// </summary> 
        [System.Obsolete("IsGazed is deprectaded use IsConditionOfTypeValid<Gaze_GazeCondition>()")]
        public bool IsGazed
        {
            get
            {
                Gaze_GazeCondition gazeCondition = GetConditionOfType<Gaze_GazeCondition>();
                return IsConditionOfTypeValid<Gaze_GazeCondition>();
            }
        }

        [System.Obsolete("isInProximity is deprectaded use IsConditionOfTypeValid<Gaze_ProximityCondition>()")]
        public bool isInProximity
        {
            get
            {
                return IsConditionOfTypeValid<Gaze_ProximityCondition>();
            }
        }

        #endregion VarsToDestroyAfterRefactring
    }
}
