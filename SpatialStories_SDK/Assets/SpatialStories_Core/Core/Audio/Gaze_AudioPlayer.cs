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
            public int track;
            public int clipIndex;
            public float timer;
            public float fadeTimer;
            public float fadeStartingVolume;

            public float audioVolumeMin = .2f;
            public float audioVolumeMax = 1f;

            public int key;

            public Gaze_AudioSource(AudioSource audioSource)
            {
                this.audioState = AudioState.STOPPED;
                this.nextAudioState = AudioState.STOPPED;

                this.audioSource = audioSource;
                this.timer = 0f;
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

            public void setTimer(float time)
            {
                this.timer = time;
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
                DestroyImmediate(audioSource);
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
            public bool fadeOutDeactEnabled = false;

            public bool randomizePitch = false;
            public float minPitch = 0;
            public float maxPitch = 2;

            public bool forceStop = false;
            public bool allowMultiple = false;

            public AudioSource audioSource;

            public AnimationCurve fadeInCurve;
            public AnimationCurve fadeOutCurve;
            public AnimationCurve fadeOutDeactCurve;

            public int key;

            public Gaze_AudioAction(AudioSource targetaudioSource, Gaze_AudioPlayList audioClips, Gaze_Actions.LOOP_MODES[] loop, Gaze_Actions.AUDIO_SEQUENCE[] sequence, bool[] fadeInBetween, float volumeMin, float volumeMax, bool duckingEnabled, float fadeSpeed, float fadeInTime, float fadeOutTime, float fadeOutDeactTime, bool fadeInEnabled, bool fadeOutEnabled, bool fadeOutDeactEnabled, AnimationCurve fadeInCurve, AnimationCurve fadeOutCurve, AnimationCurve fadeOutDeactCurve, bool[] activeTriggerStatesAudio, bool forceStop, bool allowMultiple, bool randomizePitch, float minPitch, float maxPitch)
            {
                this.audioSource = targetaudioSource;
                clips = audioClips;
                this.volumeMax = (volumeMax);
                this.volumeMin = (volumeMin);

                for (int i = 0; i < activeTriggerStatesAudio.Length; i++)
                {
                    if (activeTriggerStatesAudio[i])
                    {
                        this.loopAudio[i] = loop[i];
                        this.sequence[i] = sequence[i];
                        this.fadeInBetween[i] = fadeInBetween[i];
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

                if (fadeOutDeactEnabled)
                {
                    this.fadeOutDeactEnabled = true;
                    this.fadeOutDeactCurve = fadeOutDeactCurve;
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
        private int MAX_AUDIOS = 8;

        private Queue<int> triggered = new Queue<int>();
        private bool stopping = false;

        private int frame_count;

        private int key;

        private int[] lastClipIndex = new int[5];

        public int setParameters(AudioSource targetaudioSource, Gaze_AudioPlayList audioClips, Gaze_Actions.LOOP_MODES[] loop, Gaze_Actions.AUDIO_SEQUENCE[] sequence, bool[] fadeInBetween, float volumeMin, float volumeMax, bool duckingEnabled, float fadeSpeed, float fadeInTime, float fadeOutTime, float fadeOutDeactTime, bool fadeInEnabled, bool fadeOutEnabled, bool fadeOutDeactEnabled, AnimationCurve fadeInCurve, AnimationCurve fadeOutCurve, AnimationCurve fadeOutDeactCurve, bool[] activeTriggerStatesAudio, bool forceStop, bool allowMultiple, int maxConcurrentSound, bool randomizePitch, float minPitch, float maxPitch)
        {
            audiosActions.Add(new Gaze_AudioAction(targetaudioSource, audioClips, loop, sequence, fadeInBetween, volumeMin, volumeMax, duckingEnabled, fadeSpeed, fadeInTime, fadeOutTime, fadeOutDeactTime, fadeInEnabled, fadeOutEnabled, fadeOutDeactEnabled, fadeInCurve, fadeOutCurve, fadeOutDeactCurve, activeTriggerStatesAudio, forceStop, allowMultiple, randomizePitch, minPitch, maxPitch));
            key = audiosActions.Count - 1;

            audios.Add(new Gaze_AudioSource(audiosActions[key].audioSource));

            this.MAX_AUDIOS = maxConcurrentSound;            

            return key;
        }

        void Awake()
        {
            audios = new List<Gaze_AudioSource>(MAX_AUDIOS);
        }

        void Update()
        {
            for (int i = 0; i < audios.Count; i++)
            {

                int key_ = audios[i].key;
                audios[i].state2Next();

                if (triggered.Count > 0 && audiosActions[key_].forceStop)
                {
                    startPlaying(i);
                    audios[i].track = triggered.Dequeue();
                    return;
                }

                switch (audios[i].audioState)
                {
                    case AudioState.STOPPED:
                        {
                            if (triggered.Count > 0)
                            {
                                audios[i].key = key;
                                audios[i].track = triggered.Dequeue();
                                startPlaying(i);
                            }
                            break;
                        }
                    case AudioState.PLAYING:
                        {
                            if (stopping)
                            {
                                if (audiosActions[key_].fadeOutDeactEnabled)
                                {
                                    audios[i].setNextState(AudioState.FADING_OUT_STOP);
                                    audios[i].setFadeStartingVolume(audios[i].audioSource.volume);
                                    audios[i].setFadeTimer(0);
                                }
                                else
                                {
                                    audios[i].Stop();
                                }
                            }
                            else if (audiosActions[key_].fadeOutEnabled && audios[i].timer <= audiosActions[key_].fadeOutTime && (!audios[i].loop || audios[i].loopPlaylist && audiosActions[key_].fadeInBetween[audios[i].track]))
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
                                    lastClipIndex[audios[i].track] = audios[i].clipIndex;
                                    audios[audios.Count - 1].key = audios[i].key;
                                    startPlaying(audios.Count - 1);
                                }
                            }
                            else if (audios[i].timer <= 0 && !audios[i].loop)
                            {
                                if (audios[i].loopPlaylist)
                                {
                                    startPlaying(i);
                                }
                                else
                                {
                                    audios[i].Stop();
                                }
                            }
                            else if (audios[i].timer <= 0 && audios[i].audioSource.loop)
                            {
                                if (audiosActions[key_].randomizePitch)
                                {
                                    audios[i].audioSource.pitch = Random.Range(audiosActions[key_].minPitch, audiosActions[key_].maxPitch);
                                }
                                audios[i].setTimer(audios[i].audioSource.clip.length);
                            }

                            break;
                        }
                    case AudioState.FADING_IN:
                        {
                            if (stopping)
                            {
                                if (audiosActions[key_].fadeOutDeactEnabled)
                                {
                                    audios[i].setNextState(AudioState.FADING_OUT_STOP);
                                    audios[i].setFadeStartingVolume(audios[i].audioSource.volume);
                                    audios[i].setFadeTimer(0);
                                }
                                else
                                {
                                    audios[i].Stop();
                                }
                                stopping = false;
                            }
                            else if (audiosActions[key_].fadeOutEnabled && audios[i].timer <= audiosActions[key_].fadeOutTime && (!audios[i].loop || audios[i].loopPlaylist && audiosActions[key_].fadeInBetween[audios[i].track]))
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
                                    lastClipIndex[audios[i].track] = audios[i].clipIndex;
                                    audios[audios.Count - 1].key = audios[i].key;
                                    startPlaying(audios.Count - 1);
                                }
                            }
                            else if (audios[i].timer <= 0 && !audios[i].loop)
                            {
                                audios[i].Stop();
                            }
                            else if (audios[i].timer <= 0 && audios[i].loop)
                            {
                                if (audiosActions[key_].randomizePitch)
                                {
                                    audios[i].audioSource.pitch = Random.Range(audiosActions[key_].minPitch, audiosActions[key_].maxPitch);
                                }
                                audios[i].setTimer(audios[i].audioSource.clip.length);
                            }
                            else
                            {
                                fadeIn(i);
                            }
                            break;
                        }
                    case AudioState.FADING_OUT:
                        {
                            if (stopping && !audiosActions[key_].fadeOutDeactEnabled)
                            {
                                audios[i].Stop();
                            }
                            else
                            {
                                fadeOut(i, audiosActions[key_].fadeOutCurve);
                            }
                            break;
                        }
                    case AudioState.FADING_OUT_STOP:
                        {
                            if (triggered.Count > 1)
                            {
                                if (audios[i].audioSource.loop)
                                {
                                    audios[i].setNextState(AudioState.FADING_IN);
                                    audios[i].setFadeStartingVolume(audios[i].audioSource.volume);
                                    audios[i].track = triggered.Dequeue();
                                }
                            }
                            else
                            {
                                fadeOut(i, audiosActions[key_].fadeOutDeactCurve);
                            }
                            break;
                        }
                }

                audios[i].setTimer(Mathf.Max(audios[i].timer - Time.deltaTime, 0f));
            }
            stopping = false;
            frame_count++;

            // Delete unused audio every 100 frames
            if (frame_count == 100)
            {
                RemoveAudios();
                frame_count = 0;
            }
        }

        private void RemoveAudios()
        {
            for (int i = audios.Count - 1; i > 0; i--)
            {
                if (audios[i].audioState == AudioState.STOPPED && audios[i].nextAudioState == AudioState.STOPPED)
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
                audios[i].clipIndex = (audios[i].clipIndex + Random.Range(0, audiosActions[key_].clips.Count(audios[i].track) - 1));
            }
            else
            {
                audios[i].clipIndex++;
            }

            audios[i].clipIndex %= audiosActions[key_].clips.Count(audios[i].track);
            lastClipIndex[audios[i].track] = audios[i].clipIndex;

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
                audios[i].audioSource.Play();
                audios[i].setTimer(audios[i].audioSource.clip.length);
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
            if (index == 3)
            {
                stopTrack(2);
            }
            else if (index == 4)
            {
                stopTrack(3);
            }

            if (audiosActions[key].allowMultiple && !audiosActions[key].forceStop && audios.Count < MAX_AUDIOS && nonAvailable() || !isPlaying(key))
            {
                gameObject.AddComponent<AudioSource>();
                audios.Add(new Gaze_AudioSource(gameObject.GetComponents<AudioSource>()[audios.Count]));
                audios[audios.Count - 1].clipIndex = lastClipIndex[index];
            }

            if (!audiosActions[key].allowMultiple && !audiosActions[key].forceStop)
            {
                foreach (var a in audios)
                {
                    if (a.audioSource.isPlaying && a.key == key)
                    {
                        return;
                    }
                }
            }

            this.key = key;
            triggered.Enqueue(index);
        }

        public void stopAudio()
        {
            stopping = true;
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
                    }
                }
            }
        }

        private bool isPlaying(int key)
        {
            foreach (var a in audios)
            {
                if (a.key == key && a.audioSource.isPlaying) return true;
            }
            return false;
        }

        private bool nonAvailable()
        {
            foreach (var a in audios)
            {
                if (a.nextAudioState == AudioState.STOPPED && a.audioState == AudioState.STOPPED)
                {
                    return false;
                }
            }
            return true;
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