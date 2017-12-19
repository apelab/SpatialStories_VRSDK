using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gaze_AudioPlayList : System.Object
{

    [SerializeField]
    public List<AudioClip> ClipEntries00 = new List<AudioClip>();

    [SerializeField]
    public List<AudioClip> ClipEntries01 = new List<AudioClip>();

    [SerializeField]
    public List<AudioClip> ClipEntries02 = new List<AudioClip>();

    [SerializeField]
    public List<AudioClip> ClipEntries03 = new List<AudioClip>();

    [SerializeField]
    public List<AudioClip> ClipEntries04 = new List<AudioClip>();

    public void Add(int i, AudioClip a)
    {
        Get(i).Add(a);
    }

    public AudioClip Add(int i)
    {
        AudioClip a = new AudioClip();
        Add(i, a);
        return a;
    }

    public AudioClip Get(int i, int k)
    {
        List<AudioClip> l = Get(i);
        if (l.Count < k) return null;
        else return l[k];
    }

    public void Set(int i, int k, AudioClip a)
    {
        Get(i)[k] = a;
    }

    public void Remove(int i, AudioClip a)
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

    private List<AudioClip> Get(int i)
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

}
