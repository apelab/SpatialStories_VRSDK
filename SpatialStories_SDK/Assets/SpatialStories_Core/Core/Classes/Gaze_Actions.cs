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

        public bool ActionReset;
        public ACTIVABLE_OPTION ActionVisuals;
        public ACTIVABLE_OPTION ActionGrab;
        public ACTIVABLE_OPTION ActionTouch;
        public ACTIVABLE_OPTION ActionGravity;
        public ACTIVABLE_OPTION ActionAudio;
        public ACTIVABLE_OPTION ActionColliders;

        public bool DestroyOnTrigger;

        // grab and touch distances
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


        // Notification
        public bool triggerNotification;

        public override void OnEnable()
        {
            base.OnEnable();
            IO = GetIO();
            Gaze_EventManager.OnGazeEvent += onGazeEvent;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Gaze_EventManager.OnGazeEvent -= onGazeEvent;
        }

        void Start()
        {
            if (ActionAudio == ACTIVABLE_OPTION.ACTIVATE && targetAudioSource != null)
            {
                targetAudioSource.volume = duckingEnabled ? audioVolumeMin : audioVolumeMax;
            }
        }


        private void playAnim(int i)
        {
            if (triggerAnimation)
            {
                targetAnimator.SetTrigger(animatorTriggers[i]);
            }
        }

        private void playAudio(int i)
        {
            if (ActionAudio == ACTIVABLE_OPTION.ACTIVATE)
            {
                targetAudioSource.clip = audioClips[i];
                targetAudioSource.loop = loopAudio[i];
                targetAudioSource.Play();
            }
        }

        private IEnumerator lowerAudioVolume()
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

        private IEnumerator raiseAudioVolume()
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
            GetIO().touchDistance = touchDistance;

            if (ActionTouch == ACTIVABLE_OPTION.NOTHING)
                return;
            if (ActionTouch == ACTIVABLE_OPTION.ACTIVATE)
            {
                GetIO().touch = true;

            }
            else
            {
                GetIO().touch = false;
            }
        }

        private void HandleGrab()
        {
            GetIO().grabDistance = grabDistance;
            GetIO().grabModeIndex = grabModeIndex;

            if (ActionGrab == ACTIVABLE_OPTION.NOTHING)
                return;

            Gaze_InteractiveObject IO = GetIO();
            if (ActionGrab == ACTIVABLE_OPTION.ACTIVATE)
            {
                IO.grab = true;
            }
            else
            {
                if (IO.GrabbingManager != null)
                    IO.GrabbingManager.TryDetach();
                IO.grab = false;

            }
        }

        /// <summary>
        /// DEstroys and IO if the 
        /// </summary>
        private void HandleDestroy()
        {
            if (!DestroyOnTrigger)
                return;

            GameObject.Destroy(GetIO().gameObject);
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
                    playAudio(0);
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
                playAnim(0);
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
        protected override void onTrigger()
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
        }

        protected override void onReload()
        {
            if (activeTriggerStatesAnim.Length > 1 && activeTriggerStatesAnim[1])
            {
                playAnim(1);
            }
            if (activeTriggerStatesAudio.Length > 1 && activeTriggerStatesAudio[1])
            {
                playAudio(1);
            }
        }

        protected override void onBefore()
        {
            if (activeTriggerStatesAudio.Length > 2 && activeTriggerStatesAnim[2])
            {
                playAnim(2);
            }
            if (activeTriggerStatesAudio.Length > 2 && activeTriggerStatesAudio[2])
            {
                playAudio(2);
            }
        }

        protected override void onActive()
        {
            if (activeTriggerStatesAnim.Length > 3 && activeTriggerStatesAnim[3])
            {
                playAnim(3);
            }
            if (activeTriggerStatesAudio.Length > 3 && activeTriggerStatesAudio[3])
            {
                playAudio(3);
            }
        }

        protected override void onAfter()
        {
            if (activeTriggerStatesAnim.Length > 4 && activeTriggerStatesAnim[4])
            {
                playAnim(4);
            }
            if (activeTriggerStatesAudio.Length > 4 && activeTriggerStatesAudio[4])
            {
                playAudio(4);
            }
        }
        #endregion


        private void onGazeEvent(Gaze_GazeEventArgs e)
        {
            // if sender is the gazable collider GameObject
            if (e.Sender != null && gazable.gazeCollider != null && ((GameObject)e.Sender).Equals(gazable.gazeCollider.gameObject) && duckingEnabled && ActionAudio == Gaze_Actions.ACTIVABLE_OPTION.ACTIVATE)
            {
                StopAllCoroutines();
                if (e.IsGazed)
                {
                    StartCoroutine(raiseAudioVolume());
                }
                else
                {
                    StartCoroutine(lowerAudioVolume());
                }
            }
        }

    }
}