using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    [Serializable]
    [ExecuteInEditMode]
    public class Gaze_Actions : Gaze_AbstractBehaviour
    {
        public Gaze_InteractiveObject rootIO;

        public bool isActive = true;

        public enum ACTIVABLE_OPTION { NOTHING, ACTIVATE, DEACTIVATE }
        public enum ALTERABLE_OPTION { NOTHING, MODIFY }

        public bool ActionReset;
        public ACTIVABLE_OPTION ActionVisuals;
        public ACTIVABLE_OPTION ActionGrab;
        public ACTIVABLE_OPTION ActionTouch;
        public ACTIVABLE_OPTION ActionGravity;
        public ACTIVABLE_OPTION ActionAudio;
        public ACTIVABLE_OPTION ActionColliders;

        public bool DestroyOnTrigger;

        // delay actions
        public bool isDelayed;
        public float delayTime;
        public bool isDelayRandom;
        public float[] delayRange = { 0.0f, 0.0f };
        private List<Request> requests = new List<Request>();
        private delegate void ActionHandler();
        public bool multipleActionsInTime;

        // grab and touch distances
        public ALTERABLE_OPTION ModifyGrabDistance = ALTERABLE_OPTION.NOTHING;
        public ALTERABLE_OPTION ModifyTouchDistance = ALTERABLE_OPTION.NOTHING;
        public ALTERABLE_OPTION ModifyGrabMode = ALTERABLE_OPTION.NOTHING;
        public float grabDistance, touchDistance;
        public int grabModeIndex = 0;

        // Visuals
        public Gaze_InteractiveObjectVisuals visualsScript;
        public bool AlterAllVisuals = true;

        // Animation
        public bool triggerAnimation;
        public Animator targetAnimator;
        public bool[] activeTriggerStatesAnim = new bool[Enum<TriggerEventsAndStates>.Count];
        public string[] animatorTriggers = new string[Enum<TriggerEventsAndStates>.Count];

        // Audio
        public AudioSource targetAudioSource;
        public bool[] activeTriggerStatesAudio = new bool[Enum<TriggerEventsAndStates>.Count];
        public AudioClip[] audioClips = new AudioClip[Enum<TriggerEventsAndStates>.Count];
        public bool[] loopAudio = new bool[Enum<TriggerEventsAndStates>.Count];
        public bool duckingEnabled = true;
        public float audioVolumeMin = .2f;
        public float audioVolumeMax = 1f;
        public float fadeSpeed = .005f;
        private Coroutine raiseAudioVolume;
        private Coroutine lowerAudioVolume;

        public Gaze_InteractiveObject IO;

        private Gaze_Interaction gazeInteraction;

        // Notification
        public bool triggerNotification;


        private void Awake()
        {
            gazeInteraction = GetComponent<Gaze_Interaction>();
            if (gazeInteraction == null)
            {
                Debug.LogError("Spatial Stories SDK: Can't find a Gaze Interacton Script on the interaction: " + name);
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            IO = GetIO();
            visualsScript = IO.GetComponentInChildren<Gaze_InteractiveObjectVisuals>();
            //Gaze_EventManager.OnGazeEvent += OnGazeEvent;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            //Gaze_EventManager.OnGazeEvent -= OnGazeEvent;
        }

        void Start()
        {
            SetDelayRandom();
            if (ActionAudio == ACTIVABLE_OPTION.ACTIVATE && targetAudioSource != null)
            {
                targetAudioSource.volume = duckingEnabled ? audioVolumeMin : audioVolumeMax;
            }

            if (!gazeInteraction.HasConditions)
            {
                OnTrigger();
            }
        }

        void Update()
        {
            for (int i = requests.Count - 1; i >= 0; i--)
            {
                if (Time.time > requests[i].GetTime())
                {
                    requests[i].CallHandler();
                    requests.RemoveAt(i);
                }
            }
        }

        private void SetDelayRandom()
        {
            if (isDelayRandom)
            {
                delayTime = UnityEngine.Random.Range(delayRange[0], delayRange[1]);
            }
        }

        private void PlayAnim(int i)
        {
            if (triggerAnimation)
            {
                targetAnimator.SetTrigger(animatorTriggers[i]);
            }
        }

        private void PlayAudio(int i)
        {
            if (ActionAudio == ACTIVABLE_OPTION.ACTIVATE)
            {
                targetAudioSource.clip = audioClips[i];
                targetAudioSource.loop = loopAudio[i];
                targetAudioSource.Play();
            }
        }

        private IEnumerator LowerAudioVolume()
        {
            while (targetAudioSource.volume > audioVolumeMin)
            {
                if (targetAudioSource.volume - fadeSpeed > audioVolumeMin)
                {
                    targetAudioSource.volume -= fadeSpeed;
                }
                else
                {
                    targetAudioSource.volume = audioVolumeMin;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator RaiseAudioVolume()
        {
            if (targetAudioSource != null)
            {
                while (targetAudioSource.volume < audioVolumeMax)
                {

                    if (targetAudioSource.volume + fadeSpeed < audioVolumeMax)
                    {
                        targetAudioSource.volume += fadeSpeed;
                    }
                    else
                    {
                        targetAudioSource.volume = audioVolumeMax;
                    }
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        private void HandleReset()
        {
            if (ActionReset)
            {
                // Reset transform
                GetIO().transform.position = GetIO().InitialTransform.position;
                GetIO().transform.rotation = GetIO().InitialTransform.rotation;
                GetIO().transform.localScale = GetIO().InitialTransform.scale;
            }
        }

        private void HandleTouch()
        {
            if (ModifyTouchDistance == ALTERABLE_OPTION.MODIFY)
                GetIO().TouchDistance = touchDistance;


            if (ActionTouch == ACTIVABLE_OPTION.ACTIVATE)
            {
                GetIO().EnableManipulationMode(Gaze_ManipulationModes.TOUCH);

            }
            else if (ActionTouch == ACTIVABLE_OPTION.DEACTIVATE)
            {
                GetIO().DisableManipulationMode(Gaze_ManipulationModes.TOUCH);
            }
        }

        private void HandleGrab()
        {
            if (ModifyGrabDistance == ALTERABLE_OPTION.MODIFY)
                GetIO().GrabDistance = grabDistance;

            Gaze_InteractiveObject IO = GetIO();

            if (ActionGrab == ACTIVABLE_OPTION.ACTIVATE)
            {
                IO.EnableManipulationMode(Gaze_ManipulationModes.GRAB);
            }
            else if (ActionGrab == ACTIVABLE_OPTION.DEACTIVATE)
            {
                if (IO.GrabbingManager != null)
                    IO.GrabbingManager.TryDetach();

                IO.DisableManipulationMode(Gaze_ManipulationModes.GRAB);
            }
        }

        /// <summary>
        /// DEstroys and IO if the 
        /// </summary>
        private void HandleDestroy()
        {
            if (!DestroyOnTrigger)
                return;
            GetIO().enabled = false;
            GameObject.Destroy(GetIO().gameObject, 0.1f);
        }

        private void HandleGravity()
        {
            if (ActionGravity == ACTIVABLE_OPTION.NOTHING)
                return;

            // It's important to give full controll to the user in terms of gravity management, this instruction
            // is usefull for example when the user drops and object that its attachable it will lock causing
            // the problem of not being able to activate gravity again (By unlocking it this problem is solved).
            if (IO.ActualGravityState == Gaze_GravityState.LOCKED)
                Gaze_GravityManager.ChangeGravityState(IO, Gaze_GravityRequestType.UNLOCK);

            if (ActionGravity == ACTIVABLE_OPTION.ACTIVATE)
                Gaze_GravityManager.ChangeGravityState(IO, Gaze_GravityRequestType.ACTIVATE_AND_DETACH);
            else
                Gaze_GravityManager.ChangeGravityState(IO, Gaze_GravityRequestType.DEACTIVATE_AND_ATTACH);


            Gaze_GravityManager.ChangeGravityState(IO, Gaze_GravityRequestType.SET_AS_DEFAULT);
        }

        /// <summary>
        /// Enables or disables visuals according with the user preferences.
        /// </summary>
        private void HandleVisuals()
        {
            if (ActionVisuals == ACTIVABLE_OPTION.NOTHING)
                return;

            bool isEnabled = ActionVisuals == ACTIVABLE_OPTION.ACTIVATE;

            if (AlterAllVisuals)
                visualsScript.AlterAllVisuals(isEnabled);

            else
                visualsScript.AlterSelectedVisuals(isEnabled);
        }

        /// <summary>
        /// Enables or disables visuals according with the user preferences.
        /// </summary>
        private void HandleColliders()
        {
            if (ActionColliders == ACTIVABLE_OPTION.NOTHING)
                return;

            Gaze_InteractiveObject io = GetIO();
            Collider[] AllColliders = io.GetComponentsInChildren<Collider>();
            bool isEnabled = ActionColliders == ACTIVABLE_OPTION.ACTIVATE;

            foreach (Collider collider in AllColliders)
                collider.enabled = isEnabled;
        }

        private void HandleGrabMode()
        {
            if (ModifyGrabMode == ALTERABLE_OPTION.NOTHING)
                return;

            if (grabModeIndex == (int)Gaze_GrabMode.ATTRACT)
            {
                GetIO().ManipulationModeIndex = (int)Gaze_ManipulationModes.GRAB;
            }
            else if (grabModeIndex == (int)Gaze_GrabMode.LEVITATE)
            {
                GetIO().ManipulationModeIndex = (int)Gaze_ManipulationModes.LEVITATE;
            }
        }


        /// <summary>
        /// Enables or disables audio acording with the user preferences
        /// </summary>
        private void HandleAudio()
        {
            if (ActionAudio == ACTIVABLE_OPTION.NOTHING)
                return;

            // If we want to activate audio just do it.
            if (ActionAudio == ACTIVABLE_OPTION.ACTIVATE)
            {
                if (activeTriggerStatesAudio.Length > 0 && activeTriggerStatesAudio[0])
                {
                    PlayAudio(0);
                }

            }
            else //Stop the current audio.
            {
                if (targetAudioSource != null && targetAudioSource.isPlaying)
                {
                    targetAudioSource.Stop();
                }
            }
        }

        /// <summary>
        /// Plays an animation if required.
        /// </summary>
        private void HandleAnimation()
        {
            if (activeTriggerStatesAnim.Length > 0 && activeTriggerStatesAnim[0])
            {
                PlayAnim(0);
            }
        }

        /// <summary>
        /// Gets the root IO of GazeActions
        /// </summary>
        /// <returns></returns>
        public Gaze_InteractiveObject GetIO()
        {
            Gaze_InteractiveObject io = GetComponentInParent<Gaze_InteractiveObject>();
            if (io == null)
                Debug.LogWarning("Interactive object could not be found");
            return io;
        }

        // Actions executed when OnTrigger
        protected void ActionLogic()
        {
            HandleReset();
            HandleAnimation();
            HandleAudio();
            HandleVisuals();
            HandleGravity();
            HandleDestroy();
            HandleGrab();
            HandleTouch();
            HandleColliders();
            HandleGrabMode();
        }

        // Actions executed when OnReload, OnBefore, OnActive or OnAfter
        protected void TimeFrameLogic(int _animIndex)
        {
            if (activeTriggerStatesAnim.Length > _animIndex && activeTriggerStatesAnim[_animIndex])
            {
                PlayAnim(1);
            }

            if (activeTriggerStatesAudio.Length > _animIndex && activeTriggerStatesAudio[_animIndex])
            {
                PlayAudio(_animIndex);
            }
        }


        #region implemented abstract members of Gaze_AbstractBehaviour
        protected override void OnTrigger()
        {
            // Check if the trigger should be fired
            if (!gazeInteraction.HasActions)
                return;
            //check if the action should be delayed
            if (isDelayed)
            {
                if (multipleActionsInTime)
                    requests.Add(new Request(Time.time + delayTime, () => ActionLogic(), TriggerEventsAndStates.OnTrigger));
                else
                {
                    for (int i = 0; i < requests.Count; i++)
                    {
                        if (requests[i].GetRequestType() == (TriggerEventsAndStates.OnTrigger))
                            return;
                    }
                    requests.Add(new Request(Time.time + delayTime, () => ActionLogic(), TriggerEventsAndStates.OnTrigger));
                }
            }

            else
                ActionLogic();
        }


        protected override void OnReload()
        {
            if (isDelayed)
            {
                if (multipleActionsInTime)
                    requests.Add(new Request(Time.time + delayTime, () => TimeFrameLogic(1), TriggerEventsAndStates.OnReload));
                else
                {
                    for (int i = 0; i < requests.Count; i++)
                    {
                        if (requests[i].GetRequestType() == (TriggerEventsAndStates.OnReload))
                            return;
                    }
                    requests.Add(new Request(Time.time + delayTime, () => TimeFrameLogic(1), TriggerEventsAndStates.OnReload));
                }
            }

            else
                TimeFrameLogic(1);
        }



        protected override void OnBefore()
        {
            if (isDelayed)
            {
                if (multipleActionsInTime)
                    requests.Add(new Request(Time.time + delayTime, () => TimeFrameLogic(2), TriggerEventsAndStates.OnBefore));
                else
                {
                    for (int i = 0; i < requests.Count; i++)
                    {
                        if (requests[i].GetRequestType() == (TriggerEventsAndStates.OnBefore))
                            return;
                    }
                    requests.Add(new Request(Time.time + delayTime, () => TimeFrameLogic(2), TriggerEventsAndStates.OnBefore));
                }
            }

            else
                TimeFrameLogic(2);
        }

        protected override void OnActive()
        {
            if (isDelayed)
            {
                if (multipleActionsInTime)
                    requests.Add(new Request(Time.time + delayTime, () => TimeFrameLogic(3), TriggerEventsAndStates.OnActive));
                else
                {
                    for (int i = 0; i < requests.Count; i++)
                    {
                        if (requests[i].GetRequestType() == (TriggerEventsAndStates.OnActive))
                            return;
                    }
                    requests.Add(new Request(Time.time + delayTime, () => TimeFrameLogic(3), TriggerEventsAndStates.OnActive));
                }
            }

            else
                TimeFrameLogic(3);
        }

        protected override void OnAfter()
        {
            if (isDelayed)
            {
                if (multipleActionsInTime)
                    requests.Add(new Request(Time.time + delayTime, () => TimeFrameLogic(4), TriggerEventsAndStates.OnAfter));
                else
                {
                    for (int i = 0; i < requests.Count; i++)
                    {
                        if (requests[i].GetRequestType() == (TriggerEventsAndStates.OnAfter))
                            return;
                    }
                    requests.Add(new Request(Time.time + delayTime, () => TimeFrameLogic(4), TriggerEventsAndStates.OnAfter));
                }
            }

            else
                TimeFrameLogic(4);
        }
        #endregion


        // TODO (Arthur): see if commenting this method causes any problem in production.
        // Commented because eventual source of bugs with the delay routine.
        /*
        private void OnGazeEvent(Gaze_GazeEventArgs e)
        {
            // if sender is the gazable collider GameObject
            if (e.Sender != null && gazable.gazeColliderIO != null && ((GameObject)e.Sender).Equals(gazable.gazeColliderIO.gameObject) && duckingEnabled && ActionAudio == Gaze_Actions.ACTIVABLE_OPTION.ACTIVATE)
            {
                StopAllCoroutines();
                if (e.IsGazed)
                {
                    raiseAudioVolume = StartCoroutine(RaiseAudioVolume());
                }
                else
                {
                    lowerAudioVolume = StartCoroutine(LowerAudioVolume());
                }
            }
        }
        */

        private struct Request
        {
            private float time;
            public float GetTime() { return time; }

            private ActionHandler handler;
            public void CallHandler() { handler(); }

            private TriggerEventsAndStates requestType;
            public TriggerEventsAndStates GetRequestType() { return requestType; }

            public Request(float tm, ActionHandler h, TriggerEventsAndStates tp)
            {
                time = tm;
                handler = h;
                requestType = tp;
            }
        }
    }
}