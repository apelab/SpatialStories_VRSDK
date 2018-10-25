using System;
using System.Collections;
using System.Collections.Generic;
using Gaze;
using UnityEngine;

[System.Serializable]
public class Gaze_AnimationPlaylist : System.Object
{

    [SerializeField]
    public List<AnimationClip> ClipEntries00 = new List<AnimationClip>();

    [SerializeField]
    public List<AnimationClip> ClipEntries01 = new List<AnimationClip>();

    [SerializeField]
    public List<AnimationClip> ClipEntries02 = new List<AnimationClip>();

    [SerializeField]
    public List<AnimationClip> ClipEntries03 = new List<AnimationClip>();

    [SerializeField]
    public List<AnimationClip> ClipEntries04 = new List<AnimationClip>();

    public void Add(int i, AnimationClip a)
    {
        Get(i).Add(a);
    }

    public AnimationClip Add(int i)
    {
        AnimationClip a = new AnimationClip();
        Add(i, a);
        return a;
    }

    public AnimationClip Get(int i, int k)
    {
        List<AnimationClip> l = Get(i);
        if (l.Count < k) return null;
        else return l[k];
    }

    public void Set(int i, int k, AnimationClip a)
    {
        Get(i)[k] = a;
    }

    public void Remove(int i, AnimationClip a)
    {
        Get(i).Remove(a);
    }

    public void Remove(int i, int k)
    {
        Get(i).RemoveAt(k);
    }

    public int Count(int i)
    {
        return Get(i).Count;
    }

    public int Length()
    {
        return 5;
    }

    public List<AnimationClip> Get(int i)
    {
        switch (i)
        {
            case 0: return ClipEntries00;
            case 1: return ClipEntries01;
            case 2: return ClipEntries02;
            case 3: return ClipEntries03;
            case 4: return ClipEntries04;
            default: return ClipEntries00;
        }
    }

    public void setLoop(int k, Gaze_Actions.LOOP_MODES mode, Gaze_Actions.ANIMATION_LOOP loop, bool loopOnLast)
    {
        foreach (var a in Get(k))
        {
            if (mode == Gaze_Actions.LOOP_MODES.Single)
            {
                if (loop == Gaze_Actions.ANIMATION_LOOP.Loop) a.wrapMode = WrapMode.Loop;
                else a.wrapMode = WrapMode.PingPong;
            }
            else
            {
                a.wrapMode = WrapMode.Clamp;
            }
        }

        if (mode == Gaze_Actions.LOOP_MODES.PlaylistOnce && loopOnLast)
        {
            Get(k)[Count(k) - 1].wrapMode = WrapMode.Loop;
        }
    }
}
