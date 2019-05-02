using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    [Serializable]
    [ExecuteInEditMode]
    public class Gaze_Actions : Gaze_AbstractBehaviour
    {
        private Gaze_Interaction gazeInteraction;
        public Gaze_InteractiveObject rootIO;

        public bool isActive = true;

        public enum ACTIVABLE_OPTION { NOTHING, ACTIVATE, DEACTIVATE }
        public enum LOOP_MODES { None, Single, Playlist, PlaylistOnce }
        public enum ALTERABLE_OPTION { NOTHING, MODIFY }
        public enum AUDIO_SEQUENCE { InOrder, Random }
        public enum ANIMATION_OPTION { NOTHING, MECANIM, CLIP, DEACTIVATE }
        public enum ANIMATION_LOOP { Loop, PingPong }

        public bool ActionReset;
        public ACTIVABLE_OPTION ActionVisuals;
        public ACTIVABLE_OPTION ActionGrab;
        public ACTIVABLE_OPTION ActionTouch;
        public ACTIVABLE_OPTION ActionGravity;
        public ACTIVABLE_OPTION ActionAudio;
        public ACTIVABLE_OPTION ActionColliders;
        public ACTIVABLE_OPTION ActionDragAndDrop;
        public ANIMATION_OPTION ActionAnimation;

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
        public ACTIVABLE_OPTION ModifyDnDAttached = ACTIVABLE_OPTION.NOTHING;
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
        public bool[] loopOnLast = new bool[Enum<TriggerEventsAndStates>.Count];


        public Gaze_AnimationPlaylist animationClip = new Gaze_AnimationPlaylist();

        public LOOP_MODES[] loopAnim = new LOOP_MODES[Enum<TriggerEventsAndStates>.Count];
        public ANIMATION_LOOP[] loopAnimType = new ANIMATION_LOOP[Enum<TriggerEventsAndStates>.Count];
        public AUDIO_SEQUENCE[] animationSequence = new AUDIO_SEQUENCE[Enum<TriggerEventsAndStates>.Count];

        // Audio
        public AudioSource targetAudioSource;
        public bool[] activeTriggerStatesAudio = new bool[Enum<TriggerEventsAndStates>.Count];

        [SerializeField]
        public Gaze_AudioPlayList audioClips = new Gaze_AudioPlayList();
        public LOOP_MODES[] loopAudio = new LOOP_MODES[Enum<TriggerEventsAndStates>.Count];
        public AUDIO_SEQUENCE[] audio_sequence = new AUDIO_SEQUENCE[Enum<TriggerEventsAndStates>.Count];
        public bool[] fadeInBetween = new bool[Enum<TriggerEventsAndStates>.Count];
        public Gaze_AudioPlayList audioClipsNew = new Gaze_AudioPlayList();
        public LOOP_MODES[] loopAudioNew = new LOOP_MODES[Enum<TriggerEventsAndStates>.Count];
        public bool[] audioLoopOnLast = new bool[Enum<TriggerEventsAndStates>.Count];

        public bool duckingEnabled = false;
        public float fadeInTime = 1f;
        public float fadeOutTime = 1f;
        public float fadeOutDeactTime = 1f;
        public AnimationCurve fadeInCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public AnimationCurve fadeOutDeactCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        public AnimationCurve fadeOutCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        public bool fadeInEnabled = false;
        public bool fadeOutEnabled = false;
        public bool fadeOutDeactEnabled = false;
        public float audioVolumeMin = .2f;
        public float audioVolumeMax = 1f;
        public float fadeSpeed = .005f;
        public bool audio_ForceStop = false;
        public bool audio_AllowMultiple = true;
        public bool audio_randomizePitch = false;
        public float audio_minPitch = 0f;
        public float audio_maxPitch = 2f;
        public int audio_MaxConcurrentSound = 8;
        public bool audio_stopOthers = false;
        public Gaze_InteractiveObject IO;

        private Gaze_AudioPlayer gazeAudioPlayer;
        private int Audio_PlayList_Key;
        private int Animation_PlayList_Key;
        private Gaze_AnimationPlayer gazeAnimationPlayer;

        // Notification
        public bool triggerNotification;
        private void Awake()
        {
            activeTriggerStatesAudio[0] = true;
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

            if (loopAudio.Length < 5)
            {
                loopAudio = new LOOP_MODES[5];
            }

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

            if (ActionAudio != ACTIVABLE_OPTION.NOTHING && targetAudioSource != null)
            {
                if (targetAudioSource.GetComponent<Gaze_AudioPlayer>() == null)
                {
                    targetAudioSource.gameObject.AddComponent<Gaze_AudioPlayer>();
                }


                gazeAudioPlayer = targetAudioSource.GetComponent<Gaze_AudioPlayer>();

                if (ActionAudio == ACTIVABLE_OPTION.ACTIVATE)
                {
                    Audio_PlayList_Key = gazeAudioPlayer.setParameters(targetAudioSource, audioClips, loopAudio, audio_sequence, fadeInBetween, audioVolumeMin, audioVolumeMax, duckingEnabled, fadeSpeed, fadeInTime, fadeOutTime, fadeInEnabled, fadeOutEnabled, fadeInCurve, fadeOutCurve, activeTriggerStatesAudio, audio_ForceStop, audio_AllowMultiple, audio_MaxConcurrentSound, audio_randomizePitch, audio_minPitch, audio_maxPitch, audioLoopOnLast, audio_stopOthers);
                }
                else if (ActionAudio == ACTIVABLE_OPTION.DEACTIVATE && fadeOutDeactEnabled)
                {
                    gazeAudioPlayer.setFadeOutDeactivate(fadeOutDeactTime, fadeOutDeactCurve);
                }

            }

            if (ActionAnimation == ANIMATION_OPTION.CLIP)
            {
                if (targetAnimator.GetComponent<Gaze_AnimationPlayer>() == null)
                {
                    targetAnimator.gameObject.AddComponent<Gaze_AnimationPlayer>();
                }

                gazeAnimationPlayer = targetAnimator.GetComponent<Gaze_AnimationPlayer>();
                gazeAnimationPlayer.hideFlags = HideFlags.HideInInspector;
                Animation_PlayList_Key = gazeAnimationPlayer.setParameters(targetAnimator, animationClip, activeTriggerStatesAnim, loopAnimType, loopAnim, animationSequence, loopOnLast);
            }

            if (!gazeInteraction.HasConditions)
            {
                OnTrigger();
            }
        }

        public bool DontHaveAudioLoop()
        {
            foreach (LOOP_MODES loop in loopAudioNew)
            {
                if (loop != LOOP_MODES.Single) return true;
            }
            return false;
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
            if (ActionAnimation == ANIMATION_OPTION.MECANIM)
            {
                targetAnimator.enabled = true;
                targetAnimator.SetTrigger(animatorTriggers[i]);
            }
            else if (ActionAnimation == ANIMATION_OPTION.CLIP)
            {
                gazeAnimationPlayer.PlayAnim(Animation_PlayList_Key, i);
            }
        }

        private void HandleReset()
        {
            if (ActionReset)
            {
                // Reset transform
                //if (GetIO().GrabLogic.IsBeingGrabbed && GetIO().GrabLogic.GrabbingManager != null)
                //    GetIO().GrabLogic.GrabbingManager.TryDetach();

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

            Gaze_InteractiveObject IO = GetIO();

            if (ActionTouch == ACTIVABLE_OPTION.ACTIVATE)
            {
                GetIO().EnableManipulationMode(Gaze_ManipulationModes.TOUCH);

            }
            else if (ActionTouch == ACTIVABLE_OPTION.DEACTIVATE)
            {
                if (IO.ManipulationMode == Gaze_ManipulationModes.GRAB)
                    IO.DisableManipulationMode(Gaze_ManipulationModes.GRAB);
                else if (IO.ManipulationMode == Gaze_ManipulationModes.LEVITATE)
                    IO.DisableManipulationMode(Gaze_ManipulationModes.LEVITATE);
                else if (IO.ManipulationMode == Gaze_ManipulationModes.TOUCH)
                    IO.DisableManipulationMode(Gaze_ManipulationModes.TOUCH);
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
                if (IO.ManipulationMode == Gaze_ManipulationModes.GRAB)
                    IO.DisableManipulationMode(Gaze_ManipulationModes.GRAB);
                else if (IO.ManipulationMode == Gaze_ManipulationModes.LEVITATE)
                    IO.DisableManipulationMode(Gaze_ManipulationModes.LEVITATE);
                else if (IO.ManipulationMode == Gaze_ManipulationModes.TOUCH)
                    IO.DisableManipulationMode(Gaze_ManipulationModes.TOUCH);
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
                Gaze_GravityManager.ChangeGravityState(IO, Gaze_GravityRequestType.UNLOCK, false);

            if (ActionGravity == ACTIVABLE_OPTION.ACTIVATE)
                Gaze_GravityManager.ChangeGravityState(IO, Gaze_GravityRequestType.ACTIVATE_AND_DETACH, false);
            else
                Gaze_GravityManager.ChangeGravityState(IO, Gaze_GravityRequestType.DEACTIVATE_AND_ATTACH, false);


            Gaze_GravityManager.ChangeGravityState(IO, Gaze_GravityRequestType.SET_AS_DEFAULT, true);
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

            if (grabModeIndex == (int)Gaze_GrabMode.GRAB)
            {
                GetIO().ManipulationModeIndex = (int)Gaze_ManipulationModes.GRAB;
            }
            else if (grabModeIndex == (int)Gaze_GrabMode.LEVITATE)
            {
                GetIO().ManipulationModeIndex = (int)Gaze_ManipulationModes.LEVITATE;
            }
            else if (grabModeIndex == (int)Gaze_GrabMode.TOUCH)
            {
                if(grabDistance != touchDistance)
                 GetIO().TouchDistance = grabDistance;
                
                GetIO().ManipulationModeIndex = (int)Gaze_ManipulationModes.TOUCH;

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

            switch (ModifyDnDAttached)
            {
                case ACTIVABLE_OPTION.ACTIVATE:
                    GetIO().DnD_attached = true;
                    GetIO().ChangeDnDAttach(true);
                    break;
                case ACTIVABLE_OPTION.DEACTIVATE:
                    GetIO().DnD_attached = false;
                    GetIO().ChangeDnDAttach(false);
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
                if (activeTriggerStatesAudio[0])
                {
                    gazeAudioPlayer.NeedsToUpdate = true;
                    gazeAudioPlayer.playAudio(Audio_PlayList_Key, 0);
                }
            }
            else //Stop the current audio.
            {
                gazeAudioPlayer.stopAudio();

            }
        }

        /// <summary>
        /// Plays an animation if required.
        /// </summary>
        private void HandleAnimation()
        {
            if (ActionAnimation == ANIMATION_OPTION.DEACTIVATE)
            {
                if (targetAnimator != null)
                {
                    if (targetAnimator.GetComponent<Gaze_AnimationPlayer>() != null)
                    {
                        targetAnimator.GetComponent<Gaze_AnimationPlayer>().Stop();
                    }

                    targetAnimator.enabled = false;
                }

            }
            else if (activeTriggerStatesAnim.Length > 0 && activeTriggerStatesAnim[0])
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
                PlayAnim(_animIndex);
            }

            if (activeTriggerStatesAudio.Length > _animIndex && activeTriggerStatesAudio[_animIndex])
            {
                gazeAudioPlayer.playAudio(Audio_PlayList_Key, _animIndex);
            }
            else if (activeTriggerStatesAudio.Length > _animIndex && _animIndex >= 3 && ActionAudio == ACTIVABLE_OPTION.ACTIVATE)
            {
                gazeAudioPlayer.stopTrack(_animIndex - 1);
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
    }
}
