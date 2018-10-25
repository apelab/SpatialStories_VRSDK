using Gaze;
using SpatialStories;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Behaviour_ObjectInstantiator : EditorWindow
{
    enum ChooseMode { LIST_ORDER, RANDOM }
    enum NumInstantiatiations { FINITE, INFINITE }

    private static Vector2 MIN_SIZE = new Vector2(840, 340);
    private string behaviourName = "My Object Instantiator Behaviour";

    // Holds the objects and the cursors
    private List<GameObject> objectsToPlace = new List<GameObject>() { null };
    private List<GameObject> objectsToPlaceCursor = new List<GameObject>() { null };
    private Vector2 scrollPosition = Vector2.zero;

    private int constraintsIndex = 0;
    private float heightOffset = 0.0f;
    private float distanceFromCamera = 1.5f;

    private int chooseModeIndex;
    private int numInstantiationsIndex;

    private int numObjectsToInstantiate = 1;

    void OnGUI()
    {

        Gaze_EditorUtils.DrawSectionTitle("Name your behaviour:");
        Gaze_EditorUtils.DrawIndentedSection(DrawBehaivourName);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MinHeight(position.height - 235));
        Gaze_EditorUtils.DrawSectionTitle("Objects to be Instantiated:");
        Gaze_EditorUtils.DrawIndentedSection(DisplayObjectsList);
        EditorGUILayout.EndScrollView();

        Gaze_EditorUtils.DrawSectionTitle("Options:");
        Gaze_EditorUtils.DrawIndentedSection(DisplayOptions);

        EditorGUILayout.Space();

        if (GUILayout.Button("\n Generate Behaviour \n"))
        {
            GenerateBehaivour();
        }
    }


    private void DrawBehaivourName()
    {
        behaviourName = EditorGUILayout.TextField(new GUIContent("Behaviour Name", "The name that will be displayed on the hierarchy."), behaviourName);
        Gaze_EditorUtils.DrawEditorHint("Name your behaviour: ");
    }

    private void DisplayObjectsList()
    {
        Gaze_EditorUtils.DrawEditorHint("Select all the objects to instantiate (EVERYTHING NEEDS TO BE A PREFAB)");
        for (int i = objectsToPlace.Count - 1; i >= 0; i--)
        {
            DisplayObjectOption(i);
        }
        if (GUILayout.Button("+ Add another"))
        {
            objectsToPlace.Insert(0, null);
            objectsToPlaceCursor.Insert(0, null);
        }
    }

    private void DisplayObjectOption(int _index)
    {
        EditorGUILayout.BeginHorizontal();
        objectsToPlace[_index] = (GameObject)EditorGUILayout.ObjectField("Prefab to Instantiate", objectsToPlace[_index], typeof(GameObject), false);

        objectsToPlaceCursor[_index] = (GameObject)EditorGUILayout.ObjectField("Cursor Visual", objectsToPlaceCursor[_index], typeof(GameObject), false);

        if (objectsToPlace.Count > 1)
        {
            if (GUILayout.Button("- Remove Object"))
            {
                objectsToPlace.RemoveAt(_index);
                objectsToPlaceCursor.RemoveAt(_index);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DisplayOptions()
    {
        chooseModeIndex = EditorGUILayout.Popup("Next Object to spawn", chooseModeIndex, Enum.GetNames(typeof(ChooseMode)));
        numInstantiationsIndex = EditorGUILayout.Popup("Number Of Instantiations", numInstantiationsIndex, Enum.GetNames(typeof(NumInstantiatiations)));

        if (numInstantiationsIndex == (int)NumInstantiatiations.FINITE)
        {
            numObjectsToInstantiate = EditorGUILayout.IntField("Num Objects To Instantiate:", numObjectsToInstantiate);
        }

        heightOffset = EditorGUILayout.FloatField("Height Offset", heightOffset);

        constraintsIndex = EditorGUILayout.Popup("Constraints", constraintsIndex, Enum.GetNames(typeof(Gaze_ArkitPlaceConstraints)));

        if (constraintsIndex == (int)Gaze_ArkitPlaceConstraints.IN_FRONT_OF_CAMERA)
        {
            distanceFromCamera = EditorGUILayout.FloatField("Distance From Camera: ", distanceFromCamera);
        }
    }

    private void GenerateBehaivour()
    {
        if (HasEmptyObjects(objectsToPlace, this))
        {
            return;
        }

        S_IODefinition ioDef = SpatialStoriesAPI.CreateIODefinition(behaviourName);

        int counter = objectsToPlace.Count;

        S_InteractionDefinition start = ioDef.CreateInteractionDefinition(string.Concat("Start ", behaviourName));
        S_InteractionDefinition place = ioDef.CreateInteractionDefinition(string.Concat("Instantiate ", behaviourName));
        place.CreateArkitInstantiateAction((Gaze_ArkitPlaceConstraints)constraintsIndex, heightOffset, distanceFromCamera, ChooseMode.RANDOM == (ChooseMode)chooseModeIndex, NumInstantiatiations.FINITE == (NumInstantiatiations)numInstantiationsIndex, numObjectsToInstantiate, objectsToPlace, objectsToPlaceCursor);
        place.CreateDependency(start.GUID);

        place.CreateTouchCondition();
        S_InteractionDefinition end = null;

        if (numInstantiationsIndex == (int)NumInstantiatiations.FINITE)
        {
            end = ioDef.CreateInteractionDefinition(string.Concat("End ", behaviourName));
            end.Delay = 0.03f;
        }

        if (constraintsIndex == (int)Gaze_ArkitPlaceConstraints.OVER_A_PLANE)
        {
            place.CreateGazeCondition(ioDef.GUID, Gaze_HoverStates.IN, Gaze_GazeConstraints.PLANE);
        }
        else if (constraintsIndex == (int)Gaze_ArkitPlaceConstraints.OVER_AN_OBJECT)
        {
            place.CreateGazeCondition(ioDef.GUID, Gaze_HoverStates.IN, Gaze_GazeConstraints.ANY_OBJECT);
        }
        else if (constraintsIndex == (int)Gaze_ArkitPlaceConstraints.OVER_AN_IMAGE)
        {
            place.CreateGazeCondition(ioDef.GUID, Gaze_HoverStates.IN, Gaze_GazeConstraints.IMAGE);
        }


        Gaze_InteractiveObject obj = SpatialStoriesAPI.CreateInteractiveObject(ioDef, false);
        SpatialStoriesAPI.WirePendingDependencies();

        // Set the trigger counter condition
        if (numInstantiationsIndex == (int)NumInstantiatiations.FINITE)
        {
            CC_CountTriggers c = SpatialStoriesAPI.GetObjectOfTypeWithGUID(end.GUID).gameObject.AddComponent<CC_CountTriggers>();
            c.NumTimes = numObjectsToInstantiate;
            c.Interaction = SpatialStoriesAPI.GetObjectOfTypeWithGUID(place.GUID).GetComponent<Gaze_Conditions>();
            Gaze_Conditions conditions = c.GetComponent<Gaze_Conditions>();
            conditions.customConditionsEnabled = true;
            conditions.customConditions.Add(c);
        }

        Close();
    }

    public static bool HasEmptyObjects(List<GameObject> _objects, EditorWindow _window)
    {
        for (int i = 0; i < _objects.Count; i++)
        {
            if (_objects[i] == null)
            {
                _window.ShowNotification(new GUIContent("The object at position " + i + " Can't be null (Only the cursor can be null)"));
                return true;
            }
        }

        return false;
    }

    [MenuItem("SpatialStories/Behaviours/Object Instantiator")]
    public static void ShowWindow()
    {
        EditorWindow win = EditorWindow.GetWindow(typeof(Behaviour_ObjectInstantiator));
        win.minSize = MIN_SIZE;
    }
}
