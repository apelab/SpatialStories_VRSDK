using System;
using System.Collections;
using UnityEngine;

namespace Gaze
{
    [Serializable]
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

        // grab and touch distances
        public ALTERABLE_OPTION ModifyGrabDistance = ALTERABLE_OPTION.NOTHING;
        public ALTERABLE_OPTION ModifyTouchDistance = ALTERABLE_OPTION.NOTHING;
        public ALTERABLE_OPTION ModifyGrabMode = ALTERABLE_OPTION.NOTHING;
        public float grabDistance, touchDistance;
        public int grabModeIndex = 0;

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
            Gaze_EventManager.OnGazeEvent += OnGazeEvent;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Gaze_EventManager.OnGazeEvent -= OnGazeEvent;
        }

        void Start()
        {
            if (ActionAudio == ACTIVABLE_OPTION.ACTIVATE && targetAudioSource != null)
            {
                targetAudioSource.volume = duckingEnabled ? audioVolumeMin : audioVolumeMax;
            }

            if (!gazeInteraction.HasConditions)
            {
                OnTrigger();
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

            Gaze_InteractiveObject io = GetIO();
            // HACK: this is done as a hotfix for BALMOB in a future we need to create an scripts visuals add it into each IO and then track it.
            Renderer[] AllRenderers = io.transform.FindChild("Visuals").GetComponentsInChildren<Renderer>();
            bool isEnabled = ActionVisuals == ACTIVABLE_OPTION.ACTIVATE;

            foreach (Renderer renderer in AllRenderers)
                renderer.enabled = isEnabled;
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


        #region implemented abstract members of Gaze_AbstractBehaviour
        protected override void OnTrigger()
        {
            // Check if the trigger should be fired
            if (!gazeInteraction.HasActions)
                return;

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

        protected override void OnReload()
        {
            if (activeTriggerStatesAnim.Length > 1 && activeTriggerStatesAnim[1])
            {
                PlayAnim(1);
            }
            if (activeTriggerStatesAudio.Length > 1 && activeTriggerStatesAudio[1])
            {
                PlayAudio(1);
            }
        }

        protected override void OnBefore()
        {
            if (activeTriggerStatesAudio.Length > 2 && activeTriggerStatesAnim[2])
            {
                PlayAnim(2);
            }
            if (activeTriggerStatesAudio.Length > 2 && activeTriggerStatesAudio[2])
            {
                PlayAudio(2);
            }
        }

        protected override void OnActive()
        {
            if (activeTriggerStatesAnim.Length > 3 && activeTriggerStatesAnim[3])
            {
                PlayAnim(3);
            }
            if (activeTriggerStatesAudio.Length > 3 && activeTriggerStatesAudio[3])
            {
                PlayAudio(3);
            }
        }

        protected override void OnAfter()
        {
            if (activeTriggerStatesAnim.Length > 4 && activeTriggerStatesAnim[4])
            {
                PlayAnim(4);
            }
            if (activeTriggerStatesAudio.Length > 4 && activeTriggerStatesAudio[4])
            {
                PlayAudio(4);
            }
        }
        #endregion


        private void OnGazeEvent(Gaze_GazeEventArgs e)
        {
            // if sender is the gazable collider GameObject
            if (e.Sender != null && gazable.gazeColliderIO != null && ((GameObject)e.Sender).Equals(gazable.gazeColliderIO.gameObject) && duckingEnabled && ActionAudio == Gaze_Actions.ACTIVABLE_OPTION.ACTIVATE)
            {
                StopAllCoroutines();
                if (e.IsGazed)
                {
                    StartCoroutine(RaiseAudioVolume());
                }
                else
                {
                    StartCoroutine(LowerAudioVolume());
                }
            }
        }

    }
}