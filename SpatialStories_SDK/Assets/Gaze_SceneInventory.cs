using Gaze;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Gaze_SceneInventory : MonoBehaviour
{
    public static Gaze_SceneInventory Instance { get; private set; }
    public List<Gaze_InteractiveObject> InteractiveObjectScripts;
    public List<GameObject> InteractiveObjects;
    public int InteractiveObjectsCount;

    public Gaze_SceneInventory()
    {
        Instance = this;
        InteractiveObjectScripts = new List<Gaze_InteractiveObject>();
        InteractiveObjects = new List<GameObject>();
    }

    void Awake()
    {
        // Here we save our singleton instance
        Instance = this;
    }

    void Start()
    {
        InteractiveObjectScripts = new List<Gaze_InteractiveObject>();
        InteractiveObjects = new List<GameObject>();
    }

    private void Update()
    {
        UpdateListsFromHierarchy();
    }

    private void UpdateListsFromHierarchy()
    {
        // clear lists
        InteractiveObjectScripts.Clear();
        InteractiveObjects.Clear();


        // repopulate them
        InteractiveObjectScripts = (FindObjectsOfType(typeof(Gaze_InteractiveObject)) as Gaze_InteractiveObject[]).ToList();
        InteractiveObjectsCount = InteractiveObjectScripts.Count;
        for (int i = 0; i < InteractiveObjectsCount; i++)
        {
            InteractiveObjects.Add(InteractiveObjectScripts[i].gameObject);
            //Debug.Log("hiererachy index [" + i + "] " + InteractiveObjectScripts[i]);
        }
    }
}