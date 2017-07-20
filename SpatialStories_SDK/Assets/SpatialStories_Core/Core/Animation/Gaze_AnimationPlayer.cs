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

            public int clipIndex = 0;
        }

        private List<Gaze_Animation> animations = new List<Gaze_Animation>();
        private Animation animationSource;


        void Awake()
        {
            animationSource = GetComponent<Animation>();
        }

        public int setParameters(Gaze_AnimationPlaylist clips, bool[] activeTriggerStatesAnim, Gaze_Actions.ANIMATION_LOOP[] loop, Gaze_Actions.AUDIO_SEQUENCE[] sequence)
        {
            animations.Add(new Gaze_Animation());
            int key = animations.Count - 1;

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
                }
            }
            return key;
        }

        public void PlayAnim(int key, int track)
        {
            if (!animationSource.isPlaying)
            {
                if (animations[key].sequence[track] == Gaze_Actions.AUDIO_SEQUENCE.Random)
                {
                    animations[key].clipIndex = (animations[key].clipIndex + UnityEngine.Random.Range(0, animations[key].animationClip.Count(track) - 1));
                }
                else
                {
                    animations[key].clipIndex++;
                }


                if (animations[key].loop[track] == Gaze_Actions.ANIMATION_LOOP.Loop)
                {
                    animationSource.GetClip(animations[key].animationClip.Get(animations[key].clipIndex, track).name).wrapMode = WrapMode.Loop;
                }
                else if (animations[key].loop[track] == Gaze_Actions.ANIMATION_LOOP.PingPong)
                {
                    animationSource.GetClip(animations[key].animationClip.Get(animations[key].clipIndex, track).name).wrapMode = WrapMode.PingPong;
                }
                animationSource.PlayQueued(animations[key].animationClip.Get(animations[key].clipIndex, track).name);
            }
        }

    }
}
