using Gaze;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Gaze_SceneInventory : MonoBehaviour {

    public static List<Gaze_InteractiveObject> InteractiveObjectScripts;
    public static List<GameObject> InteractiveObjects;

    void Start ()
    {
        InteractiveObjectScripts = new List<Gaze_InteractiveObject>();
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
        for (int i = 0; i < InteractiveObjectScripts.Count; i++)
        {
            InteractiveObjects.Add(InteractiveObjectScripts[i].gameObject);
        }
    }
}
