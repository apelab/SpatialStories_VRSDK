// <copyright file="Gaze_AudioSourcePlayer.cs" company="apelab sàrl">
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
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    [RequireComponent(typeof(AudioSource))]
    public class Gaze_AudioPlayer : MonoBehaviour
    {
        enum AudioState
        {
            FADING_IN,
            PLAYING,
            FADING_OUT,
            FADING_OUT_STOP,
            STOPPED
        }

        private class Gaze_AudioSource
        {
            public AudioSource audioSource;
            public AudioState audioState;
            public AudioState nextAudioState;
            public bool loop;
            public bool loopPlaylist;
            public bool loopPlaylistOnce;
            public int track;
            public int clipIndex = -1;
            public float fadeTimer;
            public float fadeStartingVolume;

            public float audioVolumeMin = .2f;
            public float audioVolumeMax = 1f;

            public int key;

            public float timer
            {
                get { return audioSource.time; }
            }

            public bool isPlaying
            {
                get { return audioSource.isPlaying; }
            }

            public Gaze_AudioSource(AudioSource audioSource)
            {
                this.audioState = AudioState.STOPPED;
                this.nextAudioState = AudioState.STOPPED;

                this.audioSource = audioSource;
                this.fadeTimer = 0f;
                this.fadeStartingVolume = 0f;
            }

            public void state2Next()
            {
                audioState = nextAudioState;
            }

            public void setNextState(AudioState newAudioState)
            {
                nextAudioState = newAudioState;
            }

            public void setFadeTimer(float time)
            {
                this.fadeTimer = time;
            }

            public void setFadeStartingVolume(float vol)
            {
                fadeStartingVolume = vol;
            }

            public void Stop()
            {
                audioSource.Stop();
                nextAudioState = AudioState.STOPPED;
            }


            public void destroy()
            {
                Destroy(audioSource);
            }

        }

        private class Gaze_AudioAction
        {
            public Gaze_AudioPlayList clips;
            public float volumeMin;
            public float volumeMax;

            public Gaze_Actions.LOOP_MODES[] loopAudio = new Gaze_Actions.LOOP_MODES[5];
            public Gaze_Actions.AUDIO_SEQUENCE[] sequence = new Gaze_Actions.AUDIO_SEQUENCE[5];
            public bool[] fadeInBetween = new bool[5];
            public bool ducking = true;

            public float fadeOutTime = 0f;
            public bool fadeInEnabled = false;
            public bool fadeOutEnabled = false;

            public bool randomizePitch = false;
            public float minPitch = 0;
            public float maxPitch = 2;

            public bool forceStop = false;
            public bool allowMultiple = false;

            public AudioSource audioSource;

            public AnimationCurve fadeInCurve;
            public AnimationCurve fadeOutCurve;

            public bool[] loopOnLast = new bool[5];

            public int MAX_AUDIOS = 0;

            public int[] lastClipIndex = new int[5];

            public int key;

            public int nAudioPlaying;
            public Queue<int> triggered = new Queue<int>();

            public bool stopOthers;

            public Gaze_AudioAction(AudioSource targetaudioSource, Gaze_AudioPlayList audioClips, Gaze_Actions.LOOP_MODES[] loop, Gaze_Actions.AUDIO_SEQUENCE[] sequence, bool[] fadeInBetween, float volumeMin, float volumeMax, bool duckingEnabled, float fadeSpeed, float fadeInTime, float fadeOutTime, bool fadeInEnabled, bool fadeOutEnabled, AnimationCurve fadeInCurve, AnimationCurve fadeOutCurve, bool[] activeTriggerStatesAudio, bool forceStop, bool allowMultiple, bool randomizePitch, float minPitch, float maxPitch, int max_audios, bool[] loopOnLast, bool stopOthers)
            {
                this.audioSource = targetaudioSource;
                clips = audioClips;
                this.volumeMax = (volumeMax);
                this.volumeMin = (volumeMin);
                this.MAX_AUDIOS = max_audios;
                this.stopOthers = stopOthers;

                for (int i = 0; i < activeTriggerStatesAudio.Length; i++)
                {
                    if (activeTriggerStatesAudio[i])
                    {
                        this.loopAudio[i] = loop[i];
                        this.sequence[i] = sequence[i];
                        this.fadeInBetween[i] = fadeInBetween[i];
                        this.loopOnLast[i] = loopOnLast[i];
                        this.lastClipIndex[i] = -1;

                    }
                }

                this.forceStop |= forceStop;
                this.allowMultiple |= allowMultiple;

                if (duckingEnabled)
                {
                    this.ducking = true;
                }

                if (fadeInEnabled)
                {
                    this.fadeInEnabled = true;
                    this.fadeInCurve = fadeInCurve;
                }

                if (fadeOutEnabled)
                {
                    this.fadeOutEnabled = true;
                    this.fadeOutTime = fadeOutTime;
                    this.fadeOutCurve = fadeOutCurve;
                }

                if (randomizePitch)
                {
                    this.randomizePitch = true;
                    this.minPitch = minPitch;
                    this.maxPitch = maxPitch;
                }
            }
        }

        private List<Gaze_AudioAction> audiosActions = new List<Gaze_AudioAction>();

        private List<Gaze_AudioSource> audios = new List<Gaze_AudioSource>();

        private AudioSource audioSource;

        private int frame_count;

        private bool fadeOutDeactEnabled = false;
        private float fadeOutDeactTime;
        private AnimationCurve fadeOutDeactCurve;

        public bool NeedsToUpdate = false;

        public int setParameters(AudioSource targetaudioSource, Gaze_AudioPlayList audioClips, Gaze_Actions.LOOP_MODES[] loop, Gaze_Actions.AUDIO_SEQUENCE[] sequence, bool[] fadeInBetween, float volumeMin, float volumeMax, bool duckingEnabled, float fadeSpeed, float fadeInTime, float fadeOutTime, bool fadeInEnabled, bool fadeOutEnabled, AnimationCurve fadeInCurve, AnimationCurve fadeOutCurve, bool[] activeTriggerStatesAudio, bool forceStop, bool allowMultiple, int maxConcurrentSound, bool randomizePitch, float minPitch, float maxPitch, bool[] loopOnLast, bool stopOthers)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            audios.Add(new Gaze_AudioSource(audioSource));

            audiosActions.Add(new Gaze_AudioAction(targetaudioSource, audioClips, loop, sequence, fadeInBetween, volumeMin, volumeMax, duckingEnabled, fadeSpeed, fadeInTime, fadeOutTime, fadeInEnabled, fadeOutEnabled, fadeInCurve, fadeOutCurve, activeTriggerStatesAudio, forceStop, allowMultiple, randomizePitch, minPitch, maxPitch, maxConcurrentSound, loopOnLast, stopOthers));
            int key = audiosActions.Count - 1;

            audios.Add(new Gaze_AudioSource(audiosActions[key].audioSource));

            return key;
        }

        public void setFadeOutDeactivate(float fadeOutDeactTime, AnimationCurve fadeOutDeactCurve)
        {
            this.fadeOutDeactEnabled = true;
            this.fadeOutDeactTime = fadeOutDeactTime;
            this.fadeOutDeactCurve = fadeOutDeactCurve;
        }

        void Awake()
        {
            audios = new List<Gaze_AudioSource>();
        }

        void Update()
        {
            if (!NeedsToUpdate)
                return;

            for (int i = 0; i < audios.Count; i++)
            {
                int key_ = audios[i].key;
                audios[i].state2Next();

                if (audiosActions[key_].triggered.Count > 0 && audiosActions[key_].forceStop)
                {
                    startPlaying(i);
                    audios[i].track = audiosActions[key_].triggered.Dequeue();
                    return;
                }

                switch (audios[i].audioState)
                {
                    case AudioState.STOPPED:
                        {
                            if (audiosActions[key_].triggered.Count > 0)
                            {
                                audios[i].track = audiosActions[key_].triggered.Dequeue();
                                startPlaying(i);
                            }
                            break;
                        }
                    case AudioState.PLAYING:
                        {
                            if (audiosActions[key_].fadeOutEnabled && audios[i].timer <= audiosActions[key_].fadeOutTime && (!audios[i].loop || audios[i].loopPlaylist && audiosActions[key_].fadeInBetween[audios[i].track]))
                            {
                                audios[i].setNextState(AudioState.FADING_OUT);
                                audios[i].setFadeStartingVolume(audios[i].audioSource.volume);
                                audios[i].setFadeTimer(0);

                                if (audios[i].loopPlaylist && audiosActions[key_].fadeInBetween[audios[i].track])
                                {
                                    gameObject.AddComponent<AudioSource>();
                                    audios.Add(new Gaze_AudioSource(gameObject.GetComponents<AudioSource>()[audios.Count]));
                                    audios[audios.Count - 1].track = audios[i].track;
                                    audios[audios.Count - 1].clipIndex = audios[i].clipIndex;
                                    audiosActions[audios[i].key].lastClipIndex[audios[i].track] = audios[i].clipIndex;
                                    audios[audios.Count - 1].key = audios[i].key;
                                    startPlaying(audios.Count - 1);
                                }
                            }
                            else if (!audios[i].isPlaying && !audios[i].loop)
                            {
                                if (audios[i].loopPlaylist)
                                {
                                    startPlaying(i);
                                }
                                else if (audios[i].loopPlaylistOnce)
                                {
                                    if (audios[i].clipIndex < audiosActions[key_].clips.Count(audios[i].track) - 1)
                                    {
                                        startPlaying(i);
                                    }
                                }
                                else
                                {
                                    audios[i].Stop();
                                    audiosActions[key_].nAudioPlaying--;
                                }
                            }
                            else if (!audios[i].isPlaying && audios[i].audioSource.loop)
                            {
                                if (audiosActions[key_].randomizePitch)
                                {
                                    audios[i].audioSource.pitch = Random.Range(audiosActions[key_].minPitch, audiosActions[key_].maxPitch);
                                }
                            }

                            break;
                        }
                    case AudioState.FADING_IN:
                        {
                            if (audiosActions[key_].fadeOutEnabled && audios[i].timer <= audiosActions[key_].fadeOutTime && (!audios[i].loop || audios[i].loopPlaylist && audiosActions[key_].fadeInBetween[audios[i].track]))
                            {
                                audios[i].setNextState(AudioState.FADING_OUT);
                                audios[i].setFadeStartingVolume(audios[i].audioSource.volume);
                                audios[i].setFadeTimer(0);

                                if (audios[i].loopPlaylist && audiosActions[key_].fadeInBetween[audios[i].track])
                                {
                                    gameObject.AddComponent<AudioSource>();
                                    audios.Add(new Gaze_AudioSource(gameObject.GetComponents<AudioSource>()[audios.Count]));
                                    audios[audios.Count - 1].track = audios[i].track;
                                    audios[audios.Count - 1].clipIndex = audios[i].clipIndex;
                                    audiosActions[audios[i].key].lastClipIndex[audios[i].track] = audios[i].clipIndex;
                                    audios[audios.Count - 1].key = audios[i].key;
                                    startPlaying(audios.Count - 1);
                                }
                            }
                            else if (!audios[i].isPlaying && !audios[i].loop)
                            {
                                audios[i].Stop();
                                audiosActions[key_].nAudioPlaying--;

                            }
                            else if (!audios[i].isPlaying && audios[i].loop)
                            {
                                if (audiosActions[key_].randomizePitch)
                                {
                                    audios[i].audioSource.pitch = Random.Range(audiosActions[key_].minPitch, audiosActions[key_].maxPitch);
                                }
                            }
                            else
                            {
                                fadeIn(i);
                            }
                            break;
                        }
                    case AudioState.FADING_OUT:
                        {
                            fadeOut(i, audiosActions[key_].fadeOutCurve);
                            break;
                        }
                    case AudioState.FADING_OUT_STOP:
                        {
                            if (audiosActions[key_].triggered.Count > 1)
                            {
                                if (audios[i].audioSource.loop)
                                {
                                    audios[i].setNextState(AudioState.FADING_IN);
                                    audios[i].setFadeStartingVolume(audios[i].audioSource.volume);
                                    audios[i].track = audiosActions[key_].triggered.Dequeue();
                                }
                            }
                            else
                            {
                                fadeOut(i, fadeOutDeactCurve);
                            }
                            break;
                        }
                }
            }
            frame_count++;

            /// DISABLE BECAUSE CAUSING UNEXPECTED ERROR. MIGHT BE USEFUL TO ENABLE THAT AGAIN AND SEARCH WHY THE ERROR OCCURS...
            // Delete unused audio every 120 frames 
            /*
            if (frame_count == 120)
            {
                RemoveAudios();
                frame_count = 0;
            }            
            */
        }

        private void RemoveAudios()
        {
            for (int i = audios.Count - 1; i > 2; i--)
            {
                if (!audios[i].audioSource.isPlaying && !audios[i].Equals(audioSource))
                {
                    audios[i].destroy();
                    audios.RemoveAt(i);
                }
            }
        }

        private void startPlaying(int i)
        {
            int key_ = audios[i].key;

            if (audiosActions[key_].sequence[audios[i].track] == Gaze_Actions.AUDIO_SEQUENCE.Random)
            {
                audios[i].clipIndex = (Random.Range(0, audiosActions[key_].clips.Count(audios[i].track) - 1));
            }
            else
            {
                audios[i].clipIndex++;
            }

            if (audiosActions[key_].clips.Count(audios[i].track) == 0)
            {
                Debug.LogWarning(GetComponentInParent<Gaze_InteractiveObject>() + " has no audio clip to play !");
                return;
            }

            audios[i].clipIndex %= audiosActions[key_].clips.Count(audios[i].track);
            audiosActions[audios[i].key].lastClipIndex[audios[i].track] = audios[i].clipIndex;

            if (playAudioTrack(audios[i].key, i))
            {
                if (audiosActions[key_].randomizePitch)
                {
                    audios[i].audioSource.pitch = Random.Range(audiosActions[key_].minPitch, audiosActions[key_].maxPitch);
                }

                if (audiosActions[key_].fadeInEnabled)
                {
                    audios[i].setFadeTimer(0);
                    audios[i].setFadeStartingVolume(0f);
                    audios[i].audioSource.volume = 0;
                    audios[i].setNextState(AudioState.FADING_IN);
                }

                else
                {
                    audios[i].audioSource.volume = audios[i].audioVolumeMax;
                    audios[i].setNextState(AudioState.PLAYING);
                }

                audios[i].loop = audiosActions[key_].loopAudio[audios[i].track] == Gaze_Actions.LOOP_MODES.Single;
                audios[i].loopPlaylist = audiosActions[key_].loopAudio[audios[i].track] == Gaze_Actions.LOOP_MODES.Playlist;
                audios[i].loopPlaylistOnce = audiosActions[key_].loopAudio[audios[i].track] == Gaze_Actions.LOOP_MODES.PlaylistOnce;

                if (audios[i].clipIndex == audiosActions[key_].clips.Count(audios[i].track) - 1 && audiosActions[key_].loopOnLast[audios[i].track])
                {
                    audios[i].loop = true;
                    audios[i].audioSource.loop = true;
                }

                audios[i].audioSource.Play();
            }
        }

        public bool playAudioTrack(int key, int _trackNumber, int audioSourceIndex, bool _loop, bool _priority)
        {
            if (_trackNumber >= audiosActions[key].clips.Length())
            {
                return false;
            }
            if (audiosActions[key].clips.Count(_trackNumber) < 1)
            {
                return false;
            }

            audios[audioSourceIndex].audioVolumeMax = audiosActions[key].volumeMax;
            audios[audioSourceIndex].audioVolumeMin = audiosActions[key].volumeMin;

            audios[audioSourceIndex].audioSource.clip = audiosActions[key].clips.Get(_trackNumber, audios[audioSourceIndex].clipIndex);
            audios[audioSourceIndex].audioSource.loop = _loop;
            return true;
        }

        public bool playAudioTrack(int key, int _trackNumber, int audioSourceIndex, bool _loop)
        {
            return playAudioTrack(key, _trackNumber, audioSourceIndex, _loop, false);
        }

        public bool playAudioTrack(int key, int audioSourceIndex)
        {
            return playAudioTrack(key, audios[audioSourceIndex].track, audioSourceIndex, audiosActions[key].loopAudio[audios[audioSourceIndex].track] == Gaze_Actions.LOOP_MODES.Single, false);
        }

        public void playAudio(int key, int index)
        {
            if (audiosActions[key].stopOthers)
            {
                stopAudio();
            }
            if (index == 3)
            {
                stopTrack(2);
            }
            else if (index == 4)
            {
                stopTrack(3);
            }

            if (((audiosActions[key].allowMultiple && !audiosActions[key].forceStop && audiosActions[key].nAudioPlaying < audiosActions[key].MAX_AUDIOS)
                    || (!isPlaying(key))))
            {
                Gaze_AudioSource available_AS = null;

                if (!getAvailable(out available_AS))
                {
                    AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
                    newAudioSource.hideFlags = HideFlags.HideInInspector;
                    Gaze_AudioSource toAdd = new Gaze_AudioSource(newAudioSource);
                    toAdd.clipIndex = audiosActions[key].lastClipIndex[index];
                    toAdd.key = key;
                    audios.Add(toAdd);
                }
                else
                {
                    available_AS.clipIndex = audiosActions[key].lastClipIndex[index];
                    available_AS.key = key;
                }
            }

            if (!audiosActions[key].allowMultiple)
            {
                if (isPlaying(key) && !audiosActions[key].forceStop) return;
                else
                {
                    audiosActions[key].triggered.Enqueue(index);
                    if (!audiosActions[key].forceStop) audiosActions[key].nAudioPlaying++;
                }
            }

            if ((audiosActions[key].allowMultiple && audiosActions[key].nAudioPlaying < audiosActions[key].MAX_AUDIOS) || audiosActions[key].forceStop)
            {
                audiosActions[key].triggered.Enqueue(index);
                if (!audiosActions[key].forceStop) audiosActions[key].nAudioPlaying++;
            }
        }

        private void stopAudio(int key)
        {
            foreach (var a in audios)
            {
                a.setNextState(AudioState.STOPPED);
            }
        }


        public void stopAudio()
        {
            foreach (var a in audios)
            {
                if (fadeOutDeactEnabled)
                {
                    a.setNextState(AudioState.FADING_OUT_STOP);
                    a.setFadeStartingVolume(a.audioSource.volume);
                    a.setFadeTimer(0);
                }
                else
                {
                    a.Stop();
                    audiosActions[a.key].nAudioPlaying--;
                }
            }
        }


        public void stopTrack(int i)
        {
            foreach (var a in audios)
            {
                int key = a.key;
                if (a.track == i)
                {
                    if (audiosActions[key].fadeOutEnabled)
                    {
                        a.setNextState(AudioState.FADING_OUT);
                        a.setFadeStartingVolume(a.audioSource.volume);
                        a.setFadeTimer(0);
                    }
                    else
                    {
                        a.Stop();
                        audiosActions[key].nAudioPlaying--;
                    }
                }
            }
        }

        private bool isPlaying(int key)
        {
            foreach (var a in audios)
            {
                if (a.key == key && (a.audioState != AudioState.STOPPED && a.nextAudioState != AudioState.STOPPED)) return true;
            }
            return false;
        }

        private bool getAvailable(out Gaze_AudioSource gas)
        {
            gas = null;
            if (audios.Count < 1) return false;
            foreach (var a in audios)
            {
                if (a.nextAudioState == AudioState.STOPPED && a.audioState == AudioState.STOPPED && !a.isPlaying)
                {
                    gas = a;
                    return true;
                }
            }
            return false;
        }

        private void fadeIn(int i)
        {
            audios[i].audioSource.volume = audios[i].fadeStartingVolume + audiosActions[audios[i].key].fadeInCurve.Evaluate(audios[i].fadeTimer) * (audios[i].audioVolumeMax - audios[i].fadeStartingVolume);

            if (audios[i].audioSource.volume >= audios[i].audioVolumeMax)
            {
                audios[i].audioSource.volume = audios[i].audioVolumeMax;
                audios[i].setNextState(AudioState.PLAYING);
            }

            audios[i].setFadeTimer(audios[i].fadeTimer + +Time.deltaTime);
        }

        private void fadeOut(int i, AnimationCurve curve)
        {
            audios[i].audioSource.volume = curve.Evaluate(audios[i].fadeTimer) * audios[i].fadeStartingVolume;

            if (audios[i].audioSource.volume <= 0)
            {
                audios[i].audioSource.volume = 0;
                audios[i].setNextState(AudioState.STOPPED);
            }

            audios[i].setFadeTimer(audios[i].fadeTimer + +Time.deltaTime);
        }
    }
}