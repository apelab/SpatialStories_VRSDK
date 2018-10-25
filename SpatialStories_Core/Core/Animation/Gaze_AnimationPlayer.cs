using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    public class Gaze_AnimationPlayer : MonoBehaviour
    {

        private class Gaze_Animation
        {
            public Gaze_AnimationPlaylist animationClip = new Gaze_AnimationPlaylist();
            public Gaze_Actions.ANIMATION_LOOP[] loop = new Gaze_Actions.ANIMATION_LOOP[5];
            public Gaze_Actions.AUDIO_SEQUENCE[] sequence = new Gaze_Actions.AUDIO_SEQUENCE[5];
            public Gaze_Actions.LOOP_MODES[] playlistLoop = new Gaze_Actions.LOOP_MODES[5];
            public bool[] loopOnLast = new bool[5];

            public bool looping = false;
            public int clipIndex = -1;

            public int trackPlaying = 0;
            public int key = 0;

            public Animator animator;
            public bool isPlaying = false;

            public bool reversing = false;
        }

        private List<Gaze_Animation> animations = new List<Gaze_Animation>();

        private bool stopping;

        public int setParameters(Animator targetAnimator, Gaze_AnimationPlaylist clips, bool[] activeTriggerStatesAnim, Gaze_Actions.ANIMATION_LOOP[] loop, Gaze_Actions.LOOP_MODES[] playlistLoop, Gaze_Actions.AUDIO_SEQUENCE[] sequence, bool[] loopOnLast)
        {
            animations.Add(new Gaze_Animation());
            int key = animations.Count - 1;
            animations[key].key = key;

            for (int i = 0; i < activeTriggerStatesAnim.Length; i++)
            {
                if (activeTriggerStatesAnim[i])
                {
                    for (int k = 0; k < clips.Count(i); k++)
                    {
                        if (clips.Get(i, k) != null)
                        {
                            animations[key].animationClip.Add(i, clips.Get(i, k));
                        }
                    }

                    this.animations[key].sequence[i] = sequence[i];
                    this.animations[key].loop[i] = loop[i];
                    this.animations[key].playlistLoop[i] = playlistLoop[i];
                    this.animations[key].loopOnLast[i] = loopOnLast[i];
                    this.animations[key].animator = targetAnimator;

                }
            }
            return key;
        }

        public void Update()
        {
            foreach (var anim in animations)
            {
                if (anim.isPlaying)
                {
                    if (stopping)
                    {
                        anim.animator.enabled = false;
                        anim.looping = false;
                        anim.isPlaying = false;
                    }
                    else if (anim.reversing)
                    {
                        anim.animator.playbackTime = 10;
                        Debug.Log(anim.animator.playbackTime);

                        if (anim.animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0)
                        {
                            anim.animator.speed = 1f;
                            anim.animator.StopPlayback();
                            anim.reversing = false;
                        }

                    }
                    else if (anim.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99)
                    {
                        if (anim.looping)
                        {
                            if (anim.loop[anim.trackPlaying] == Gaze_Actions.ANIMATION_LOOP.PingPong)
                            {
                                anim.reversing = true;
                                anim.animator.StartPlayback();
                                anim.animator.speed = -1f;
                                anim.animator.playbackTime = 1f;
                                Debug.Log(anim.animator.speed);
                            }
                        }

                        else if (anim.playlistLoop[anim.trackPlaying] == Gaze_Actions.LOOP_MODES.None)
                        {
                            anim.animator.enabled = false;
                            anim.looping = false;
                            anim.isPlaying = false;
                        }

                        else if (anim.playlistLoop[anim.trackPlaying] == Gaze_Actions.LOOP_MODES.Playlist)
                        {
                            nextClip(anim.key, anim.trackPlaying);
                            anim.animator.Play(anim.animationClip.Get(anim.trackPlaying, anim.clipIndex).name);
                        }

                        else if (anim.playlistLoop[anim.trackPlaying] == Gaze_Actions.LOOP_MODES.PlaylistOnce)
                        {
                            if (anim.clipIndex >= anim.animationClip.Count(anim.trackPlaying))
                            {
                                if (anim.loopOnLast[anim.trackPlaying])
                                {
                                    anim.looping = true;
                                    anim.animator.Play(anim.animationClip.Get(anim.trackPlaying, anim.animationClip.Count(anim.trackPlaying) - 1).name);
                                }
                                else
                                {
                                    anim.animator.enabled = false;
                                    anim.looping = false;
                                    anim.isPlaying = false;
                                }
                                anim.clipIndex = 0;
                            }

                            else
                            {
                                anim.animator.Play(anim.animationClip.Get(anim.trackPlaying, anim.clipIndex).name);
                                anim.clipIndex++;
                            }
                        }
                    }
                }
            }

            stopping = false;
        }

        public void PlayAnim(int key, int track)
        {
            //if (!animations[key].isPlaying)
            {
                nextClip(key, track);

                animations[key].isPlaying = true;
                animations[key].animator.enabled = true;
                animations[key].trackPlaying = track;
                animations[key].animator.speed = 1f;
                animations[key].reversing = false;


                if (animations[key].playlistLoop[track] == Gaze_Actions.LOOP_MODES.Single)
                {
                    animations[key].looping = true;

                }
                else if (animations[key].playlistLoop[track] == Gaze_Actions.LOOP_MODES.Playlist)
                {
                    animations[key].looping = false;

                }
                else if (animations[key].playlistLoop[track] == Gaze_Actions.LOOP_MODES.PlaylistOnce)
                {
                    animations[key].looping = false;
                }
                else
                {
                    animations[key].looping = false;
                }
                animations[key].clipIndex %= animations[key].animationClip.Count(track);
                animations[key].animator.CrossFade(animations[key].animationClip.Get(track, animations[key].clipIndex).name, 0.5f);
            }
        }

        public void Stop()
        {
            stopping = true;
        }

        private void nextClip(int key, int track)
        {
            if (animations[key].sequence[track] == Gaze_Actions.AUDIO_SEQUENCE.Random)
            {
                animations[key].clipIndex += UnityEngine.Random.Range(0, animations[key].animationClip.Count(track));
            }
            else
            {
                animations[key].clipIndex++;
            }
            animations[key].clipIndex %= animations[key].animationClip.Count(track);
        }

    }

}

