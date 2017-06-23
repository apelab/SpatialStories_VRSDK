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
        public ACTIVABLE_OPTION ActionDragAndDrop;

        public bool DestroyOnTrigger;

        // grab and touch distances
        public ALTERABLE_OPTION ModifyGrabDistance = ALTERABLE_OPTION.NOTHING;
        public ALTERABLE_OPTION ModifyTouchDistance = ALTERABLE_OPTION.NOTHING;
        public ALTERABLE_OPTION ModifyGrabMode = ALTERABLE_OPTION.NOTHING;
        public float grabDistance, touchDistance;
        public int grabModeIndex = 0;

        // Drag and drop
        public ALTERABLE_OPTION ModifyDragAndDrop = ALTERABLE_OPTION.NOTHING;
        public ALTERABLE_OPTION ModifyDragAndDropTargets = ALTERABLE_OPTION.NOTHING;
        public ALTERABLE_OPTION ModifyDnDMinDistance = ALTERABLE_OPTION.NOTHING;
        public ACTIVABLE_OPTION ModifyDnDSnapBeforeDrop = ACTIVABLE_OPTION.NOTHING;
        public ALTERABLE_OPTION ModifyDnDRespectAxis = ALTERABLE_OPTION.NOTHING;
        public float dnDMinDistance;
        public bool dnDSnapBeforeDrop;
        public bool dnDRespectXAxis;
        public bool dnDRespectYAxis;
        public bool dnDRespectZAxis;
        public bool dnDRespectXAxisMirror;
        public bool dnDRespectYAxisMirror;
        public bool dnDRespectZAxisMirror;
        public ALTERABLE_OPTION ModifyDnDAngleThreshold = ALTERABLE_OPTION.NOTHING;
        public float dnDAngleThreshold;
        public List<GameObject> DnD_Targets = new List<GameObject>();

        // Visuals
        public Gaze_InteractiveObjectVisuals visualsScript;
        public bool AlterAllVisuals = true;
        public List<int> selectedRenderers = new List<int>();

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
            // Set the delay if it is random
            if (isDelayRandom)
                delayTime = UnityEngine.Random.Range(delayRange[0], delayRange[1]);

            SetCustomActionsDelay();

            if (ActionAudio == ACTIVABLE_OPTION.ACTIVATE && targetAudioSource != null)
            {
                targetAudioSource.volume = duckingEnabled ? audioVolumeMin : audioVolumeMax;
            }

            if (!gazeInteraction.HasConditions)
            {
                OnTrigger();
            }
        }

        private void SetCustomActionsDelay()
        {
            Gaze_AbstractBehaviour[] behaviors = GetComponents<Gaze_AbstractBehaviour>();
            for (int i = 0; i < behaviors.Length; i++)
            {
                if (behaviors[i] == this) continue;
                else
                {
                    behaviors[i].isDelayed = this.isDelayed;
                    behaviors[i].delayTime = this.delayTime;
                    behaviors[i].multipleActionsInTime = this.multipleActionsInTime;
                }
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
                visualsScript.AlterSelectedVisuals(isEnabled, selectedRenderers);
        }

        public void UpdateSelectedRenderers(int n)
        {
            for (int i = 0; i < selectedRenderers.Count; i++)
            {
                if (selectedRenderers[i] >= n)
                {
                    selectedRenderers.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Adds the first index of allRenderers to selectedRenderers.
        /// </summary>
        public void AddSelectedRenderer(int n)
        {
            if (n > 0 && selectedRenderers.Count < n)
                selectedRenderers.Add(0);
            else
                return;
        }

        /// <summary>
        /// Removes from selectedRenderers the int corresponding to the index of allRenderers.
        /// </summary>
        public void RemoveSelectedRenderer(int r)
        {
            if (selectedRenderers.Contains(r))
                selectedRenderers.Remove(r);
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

        private void HandleDragAndDrop()
        {
            // exit if no action is required
            if (ModifyDragAndDrop == Gaze_Actions.ALTERABLE_OPTION.NOTHING)
                return;

            // change values according to Actions required
            switch (ActionDragAndDrop)
            {
                case ACTIVABLE_OPTION.ACTIVATE:
                    GetIO().IsDragAndDropEnabled = true;
                    break;
                case ACTIVABLE_OPTION.DEACTIVATE:
                    GetIO().IsDragAndDropEnabled = false;
                    break;
            }

            if (ModifyDnDMinDistance == Gaze_Actions.ALTERABLE_OPTION.MODIFY)
                GetIO().DnD_minDistance = dnDMinDistance;

            if (ModifyDragAndDropTargets == Gaze_Actions.ALTERABLE_OPTION.MODIFY)
                GetIO().DnD_Targets = DnD_Targets;

            if (ModifyDnDRespectAxis == Gaze_Actions.ALTERABLE_OPTION.MODIFY)
            {
                GetIO().DnD_respectXAxis = dnDRespectXAxis;
                GetIO().DnD_respectYAxis = dnDRespectYAxis;
                GetIO().DnD_respectZAxis = dnDRespectZAxis;
                GetIO().DnD_respectXAxisMirrored = dnDRespectXAxisMirror;
                GetIO().DnD_respectYAxisMirrored = dnDRespectYAxisMirror;
                GetIO().DnD_respectZAxisMirrored = dnDRespectZAxisMirror;
            }

            if (ModifyDnDAngleThreshold == Gaze_Actions.ALTERABLE_OPTION.MODIFY)
                GetIO().DnD_angleThreshold = dnDAngleThreshold;

            switch (ModifyDnDSnapBeforeDrop)
            {
                case ACTIVABLE_OPTION.ACTIVATE:
                    GetIO().DnD_snapBeforeDrop = true;
                    break;
                case ACTIVABLE_OPTION.DEACTIVATE:
                    GetIO().DnD_snapBeforeDrop = false;
                    break;
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
            HandleDragAndDrop();
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
            ActionLogic();
        }

        protected override void OnReload()
        {
            TimeFrameLogic(1);
        }

        protected override void OnBefore()
        {
            TimeFrameLogic(2);
        }

        protected override void OnActive()
        {
            TimeFrameLogic(3);
        }

        protected override void OnAfter()
        {
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

    }
}