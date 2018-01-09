﻿using Gaze;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class Gaze_SceneInventory : MonoBehaviour
{
    public List<Gaze_InteractiveObject> InteractiveObjectScripts;
    public List<GameObject> InteractiveObjects;
    public int InteractiveObjectsCount;

    public Gaze_SceneInventory()
    {
        InteractiveObjectScripts = new List<Gaze_InteractiveObject>();
        InteractiveObjects = new List<GameObject>();
    }

    private void OnEnable()
    {
        Gaze_EventManager.OnIODestroyed += OnIODestroyed;
    }

    private void OnDisable()
    {
        Gaze_EventManager.OnIODestroyed -= OnIODestroyed;
    }

    private void OnIODestroyed(Gaze_IODestroyEventArgs _args)
    {
        InteractiveObjects.Remove(_args.IO.gameObject);
        InteractiveObjectsCount -= 1;
    }

    void Start()
    {
        UpdateListsFromHierarchy();
    }

    private void Update()
    {
        if (!Application.isPlaying)
            UpdateListsFromHierarchy();
    }

    private void UpdateListsFromHierarchy()
    {
        // clear list
        InteractiveObjects.Clear();

        // repopulate them
        InteractiveObjectScripts = (FindObjectsOfType(typeof(Gaze_InteractiveObject)) as Gaze_InteractiveObject[]).ToList();
        InteractiveObjectsCount = InteractiveObjectScripts.Count;
        for (int i = 0; i < InteractiveObjectsCount; i++)
        {
            InteractiveObjects.Add(InteractiveObjectScripts[i].gameObject);
        }
    }
}