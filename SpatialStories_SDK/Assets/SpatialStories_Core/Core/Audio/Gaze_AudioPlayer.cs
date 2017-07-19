// <copyright file="Gaze_AudioPlayer.cs" company="apelab sàrl">
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

        private class Gaze_Audio
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
            public int key;

            public Gaze_Audio(AudioSource audioSource)
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

        private List<Gaze_Audio> audios;
        private int MAX_AUDIOS = 8;

        private List<Gaze_AudioPlayList> clips = new List<Gaze_AudioPlayList>();
        private Gaze_Actions.AUDIO_LOOP[] loopAudio = new Gaze_Actions.AUDIO_LOOP[5];
        private Gaze_Actions.AUDIO_SEQUENCE[] sequence = new Gaze_Actions.AUDIO_SEQUENCE[5];
        private bool[] fadeInBetween = new bool[5];
        private bool ducking = true;
        private float audioVolumeMin = .2f;
        private float audioVolumeMax = 1f;
        private float fadeSpeed = .15f;
        private float fadeInTime = 0f;
        private float fadeOutTime = 0f;
        private float fadeOutDeactTime = 0f;
        private bool fadeInEnabled = false;
        private bool fadeOutEnabled = false;
        private bool fadeOutDeactEnabled = false;

        private int trackIndex;

        private bool randomizePitch = false;
        private float minPitch = 0;
        private float maxPitch = 2;

        private AudioSource audioSource;

        private bool forceStop = false;
        private bool allowMultiple = false;

        private AnimationCurve fadeInCurve;
        private AnimationCurve fadeOutCurve;
        private AnimationCurve fadeOutDeactCurve;

        private bool triggered = false;
        private bool stopping = false;

        private int frame_count;

        private int key;

        public int setParameters(Gaze_AudioPlayList audioClips, Gaze_Actions.AUDIO_LOOP[] loop, Gaze_Actions.AUDIO_SEQUENCE[] sequence, bool[] fadeInBetween, float volumeMin, float volumeMax, bool duckingEnabled, float fadeSpeed, float fadeInTime, float fadeOutTime, float fadeOutDeactTime, bool fadeInEnabled, bool fadeOutEnabled, bool fadeOutDeactEnabled, AnimationCurve fadeInCurve, AnimationCurve fadeOutCurve, AnimationCurve fadeOutDeactCurve, bool[] activeTriggerStatesAudio, bool forceStop, bool allowMultiple, int maxConcurrentSound, bool randomizePitch, float minPitch, float maxPitch)
        {
            clips.Add(new Gaze_AudioPlayList());

            for (int i = 0; i < activeTriggerStatesAudio.Length; i++)
            {
                if (activeTriggerStatesAudio[i])
                {
                    for (int k = 0; k < audioClips.Count(i); k++)
                    {
                        if (audioClips.Get(i, k) != null)
                        {
                            clips[clips.Count - 1].Add(i, audioClips.Get(i, k));
                        }
                    }
                    this.loopAudio[i] = loop[i];
                    this.sequence[i] = sequence[i];
                    this.fadeInBetween[i] = fadeInBetween[i];
                }
            }

            this.forceStop |= forceStop;
            this.allowMultiple |= allowMultiple;

            this.audioVolumeMin = volumeMin;
            this.audioVolumeMax = volumeMax;

            this.MAX_AUDIOS = maxConcurrentSound;

            if (duckingEnabled)
            {
                this.ducking = true;
                this.fadeSpeed = fadeSpeed;
            }

            if (fadeInEnabled)
            {
                this.fadeInEnabled = true;
                this.fadeInTime = fadeInTime;
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
                this.fadeOutDeactTime = fadeOutDeactTime;
                this.fadeOutDeactEnabled = true;
                this.fadeOutDeactCurve = fadeOutDeactCurve;
            }

            if (randomizePitch)
            {
                this.randomizePitch = true;
                this.minPitch = minPitch;
                this.maxPitch = maxPitch;
            }

            return clips.Count - 1;
        }

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audios = new List<Gaze_Audio>(MAX_AUDIOS);
            audios.Add(new Gaze_Audio(audioSource));
        }

        void Update()
        {
            for (int i = 0; i < audios.Count; i++)
            {
                audios[i].state2Next();

                if (triggered && forceStop)
                {
                    startPlaying(i);
                    triggered = false;
                    return;
                }

                switch (audios[i].audioState)
                {
                    case AudioState.STOPPED:
                        {
                            if (triggered)
                            {
                                audios[i].key = key;
                                startPlaying(i);
                                triggered = false;
                            }
                            break;
                        }
                    case AudioState.PLAYING:
                        {
                            if (stopping)
                            {
                                if (fadeOutDeactEnabled)
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
                            else if (fadeOutEnabled && audios[i].timer <= fadeOutTime && (!audios[i].loop || audios[i].loopPlaylist && fadeInBetween[audios[i].track]))
                            {
                                audios[i].setNextState(AudioState.FADING_OUT);
                                audios[i].setFadeStartingVolume(audios[i].audioSource.volume);
                                audios[i].setFadeTimer(0);

                                if (audios[i].loopPlaylist && fadeInBetween[audios[i].track])
                                {
                                    gameObject.AddComponent<AudioSource>();
                                    audios.Add(new Gaze_Audio(gameObject.GetComponents<AudioSource>()[audios.Count]));
                                    audios[audios.Count - 1].track = audios[i].track;
                                    audios[audios.Count - 1].clipIndex = audios[i].clipIndex;
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
                                if (randomizePitch)
                                {
                                    audios[i].audioSource.pitch = Random.Range(minPitch, maxPitch);
                                }
                                audios[i].setTimer(audios[i].audioSource.clip.length);
                            }

                            break;
                        }
                    case AudioState.FADING_IN:
                        {
                            if (stopping)
                            {
                                if (fadeOutDeactEnabled)
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
                            else if (fadeOutEnabled && audios[i].timer <= fadeOutTime && (!audios[i].loop || audios[i].loopPlaylist && fadeInBetween[audios[i].track]))
                            {
                                audios[i].setNextState(AudioState.FADING_OUT);
                                audios[i].setFadeStartingVolume(audios[i].audioSource.volume);
                                audios[i].setFadeTimer(0);

                                if (audios[i].loopPlaylist && fadeInBetween[audios[i].track])
                                {
                                    gameObject.AddComponent<AudioSource>();
                                    audios.Add(new Gaze_Audio(gameObject.GetComponents<AudioSource>()[audios.Count]));
                                    audios[audios.Count - 1].track = audios[i].track;
                                    audios[audios.Count - 1].clipIndex = audios[i].clipIndex;
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
                                if (randomizePitch)
                                {
                                    audios[i].audioSource.pitch = Random.Range(minPitch, maxPitch);
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
                            if (stopping && !fadeOutDeactEnabled)
                            {
                                audios[i].Stop();
                            }
                            else
                            {
                                fadeOut(i, fadeOutCurve);
                            }
                            break;
                        }
                    case AudioState.FADING_OUT_STOP:
                        {
                            if (triggered)
                            {
                                if (audios[i].audioSource.loop)
                                {
                                    audios[i].setNextState(AudioState.FADING_IN);
                                    audios[i].setFadeStartingVolume(audios[i].audioSource.volume);
                                }
                            }
                            else
                            {
                                fadeOut(i, fadeOutDeactCurve);
                            }
                            break;
                        }
                }

                audios[i].setTimer(Mathf.Max(audios[i].timer - Time.deltaTime, 0f));
            }
            stopping = false;
            triggered = false;
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
            if (playAudioTrack(audios[i].key, i))
            {
                if (randomizePitch)
                {
                    audios[i].audioSource.pitch = Random.Range(minPitch, maxPitch);
                }
                if (fadeInEnabled)
                {
                    audios[i].setFadeTimer(0);
                    audios[i].setFadeStartingVolume(0f);
                    audios[i].audioSource.volume = 0;
                    audios[i].setNextState(AudioState.FADING_IN);
                }
                else
                {
                    audios[i].audioSource.volume = ducking ? audioVolumeMin : audioVolumeMax;
                    audios[i].setNextState(AudioState.PLAYING);
                }

                if (sequence[audios[i].track] == Gaze_Actions.AUDIO_SEQUENCE.Random)
                {
                    audios[i].clipIndex = (audios[i].clipIndex + Random.Range(0, clips[audios[i].key].Count(audios[i].track) - 1));
                }
                else
                {
                    audios[i].clipIndex++;
                }

                audios[i].clipIndex %= clips[audios[i].key].Count(audios[i].track);

                audios[i].loop = loopAudio[trackIndex] == Gaze_Actions.AUDIO_LOOP.Single;
                audios[i].loopPlaylist = loopAudio[trackIndex] == Gaze_Actions.AUDIO_LOOP.Playlist;
                audios[i].track = trackIndex;
                audios[i].audioSource.Play();
                audios[i].setTimer(audios[i].audioSource.clip.length);
            }
        }

        public bool playAudioTrack(int key, int _trackNumber, int audioSourceIndex, bool _loop, bool _priority)
        {
            if (_trackNumber >= clips[key].Length())
            {
                return false;
            }
            if (clips[key].Count(_trackNumber) < 1)
            {
                return false;
            }
            audios[audioSourceIndex].audioSource.clip = clips[key].Get(_trackNumber, audios[audioSourceIndex].clipIndex);
            audios[audioSourceIndex].audioSource.loop = _loop;
            return true;
        }

        public bool playAudioTrack(int key, int _trackNumber, int audioSourceIndex, bool _loop)
        {
            return playAudioTrack(key, _trackNumber, audioSourceIndex, _loop, false);
        }

        public bool playAudioTrack(int key, int audioSourceIndex)
        {
            return playAudioTrack(key, audios[audioSourceIndex].track, audioSourceIndex, loopAudio[audios[audioSourceIndex].track] == Gaze_Actions.AUDIO_LOOP.Single, false);
        }

        public void playAudio(int key, int index)
        {
            if (allowMultiple && !forceStop && audios.Count < MAX_AUDIOS && nonAvailable() || !isPlaying(key))
            {
                gameObject.AddComponent<AudioSource>();
                audios.Add(new Gaze_Audio(gameObject.GetComponents<AudioSource>()[audios.Count]));
            }

            trackIndex = index;
            foreach (var a in audios)
            {
                if (a.audioSource.isPlaying && a.key == key && !allowMultiple)
                {
                    return;
                }
            }

            this.key = key;
            triggered = true;
        }

        public void stopAudio()
        {
            stopping = true;
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
            audios[i].audioSource.volume = audios[i].fadeStartingVolume + fadeInCurve.Evaluate(audios[i].fadeTimer) * (audioVolumeMax - audios[i].fadeStartingVolume);

            if (audios[i].audioSource.volume >= audioVolumeMax)
            {
                audios[i].audioSource.volume = audioVolumeMax;
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