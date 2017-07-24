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

            public bool looping = false;
            public int clipIndex = -1;

            public int trackPlaying = 0;
            public int key = 0;
        }

        private List<Gaze_Animation> animations = new List<Gaze_Animation>();
        private Animation animationSource;

        private bool stopping;

        void Awake()
        {
            animationSource = GetComponent<Animation>();
        }

        public int setParameters(Gaze_AnimationPlaylist clips, bool[] activeTriggerStatesAnim, Gaze_Actions.ANIMATION_LOOP[] loop, Gaze_Actions.LOOP_MODES[] playlistLoop, Gaze_Actions.AUDIO_SEQUENCE[] sequence)
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

                }
            }
            return key;
        }

        public void Update()
        {
            foreach (var anim in animations)
            {
                if (stopping)
                {
                    anim.looping = false;
                }
                else if (anim.looping && !animationSource.isPlaying)
                {
                    nextClip(anim.key, anim.trackPlaying);
                    anim.animationClip.Get(anim.trackPlaying, anim.clipIndex).wrapMode = WrapMode.Once;
                    animationSource.PlayQueued(anim.animationClip.Get(anim.trackPlaying, anim.clipIndex).name);
                }
            }

            stopping = false;
        }

        public void PlayAnim(int key, int track)
        {
            if (!animationSource.isPlaying)
            {
                animations[key].trackPlaying = track;
                nextClip(key, track);

                if (animations[key].playlistLoop[track] == Gaze_Actions.LOOP_MODES.Single)
                {
                    if (animations[key].loop[track] == Gaze_Actions.ANIMATION_LOOP.Loop)
                    {
                        animationSource.GetClip(animations[key].animationClip.Get(track, animations[key].clipIndex).name).wrapMode = WrapMode.Loop;
                    }
                    else if (animations[key].loop[track] == Gaze_Actions.ANIMATION_LOOP.PingPong)
                    {
                        animationSource.GetClip(animations[key].animationClip.Get(track, animations[key].clipIndex).name).wrapMode = WrapMode.PingPong;
                    }
                    animationSource.PlayQueued(animations[key].animationClip.Get(track, animations[key].clipIndex).name);

                }
                else if (animations[key].playlistLoop[track] == Gaze_Actions.LOOP_MODES.Playlist)
                {
                    animationSource.GetClip(animations[key].animationClip.Get(track, animations[key].clipIndex).name).wrapMode = WrapMode.Once;
                    animations[key].looping = true;

                }
                else
                {
                    animationSource.GetClip(animations[key].animationClip.Get(track, animations[key].clipIndex).name).wrapMode = WrapMode.Once;
                    animationSource.PlayQueued(animations[key].animationClip.Get(track, animations[key].clipIndex).name);
                }
            }
        }

        public void Stop()
        {
            stopping = true;
            animationSource.Stop();
        }

        private void nextClip(int key, int track)
        {
            if (animations[key].sequence[track] == Gaze_Actions.AUDIO_SEQUENCE.Random)
            {
                animations[key].clipIndex = Mathf.Max(animations[key].clipIndex + UnityEngine.Random.Range(0, animations[key].animationClip.Count(track) - 1), 0);
            }
            else
            {
                animations[key].clipIndex++;
            }

            animations[key].clipIndex %= animations[key].animationClip.Count(track);
        }

    }

}

