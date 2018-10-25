using Gaze;
using SpatialStories;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Behaviour_ObjectMover : EditorWindow
{
    private static Vector2 MIN_SIZE = new Vector2(840, 340);
    private string behaviourName = "My Object Mover Behaviour";

    // Holds the objects and the cursors
    private List<GameObject> objectsToPlace = new List<GameObject>() { null };
    private List<GameObject> objectsToPlaceCursor = new List<GameObject>() { null };
    private Vector2 scrollPosition = Vector2.zero;

    private int constraintsIndex = 0;
    private float heightOffset = 0.0f;
    private float distanceFromCamera = 1.5f;
    private bool deactivateAtStart = true;

    void OnGUI()
    {

        Gaze_EditorUtils.DrawSectionTitle("Name your behaviour:");
        Gaze_EditorUtils.DrawIndentedSection(DrawBehaivourName);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MinHeight(position.height - 195));
        Gaze_EditorUtils.DrawSectionTitle("Objects to be moved:");
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
        Gaze_EditorUtils.DrawEditorHint("Select all the objects to move (THEY NEED TO BE ALREADY IN THE SCENE)");
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
        objectsToPlace[_index] = (GameObject)EditorGUILayout.ObjectField("Object to move", objectsToPlace[_index], typeof(GameObject), true);
        objectsToPlaceCursor[_index] = (GameObject)EditorGUILayout.ObjectField("Cursor Visual", objectsToPlaceCursor[_index], typeof(GameObject), false);

        if (objectsToPlace[_index] != null && objectsToPlace[_index].activeInHierarchy == false)
        {
            ShowNotification(new GUIContent("\"" + objectsToPlace[_index].name + "\"" + " can't be a prefab from the Project ! \n Please choose an object from the Hierarchy."));
            objectsToPlace[_index] = null;
        }

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
        deactivateAtStart = EditorGUILayout.Toggle("Deactivate At Start", deactivateAtStart);
        heightOffset = EditorGUILayout.FloatField("Height Offset", heightOffset);

        constraintsIndex = EditorGUILayout.Popup("Constraints", constraintsIndex, Enum.GetNames(typeof(Gaze_ArkitPlaceConstraints)));

        if (constraintsIndex == (int)Gaze_ArkitPlaceConstraints.IN_FRONT_OF_CAMERA)
        {
            distanceFromCamera = EditorGUILayout.FloatField("Distance From Camera: ", distanceFromCamera);
        }
    }

    private void GenerateBehaivour()
    {
        if (Behaviour_ObjectInstantiator.HasEmptyObjects(objectsToPlace, this))
        {
            return;
        }

        S_IODefinition ioDef = SpatialStoriesAPI.CreateIODefinition(behaviourName);

        int counter = objectsToPlace.Count;

        S_InteractionDefinition start = ioDef.CreateInteractionDefinition(string.Concat("Start ", behaviourName));
        string guidLastInteraction = start.GUID;

        for (int i = counter - 1; i >= 0; i--)
        {
            GameObject objectToPlace = objectsToPlace[i];
            if (objectToPlace != null)
            {
                GameObject cursor = objectsToPlaceCursor[i];
                if (cursor != null)
                {
                    S_InteractionDefinition cursorDef = ioDef.CreateInteractionDefinition(string.Concat("Enable Cursor ", objectToPlace.name));
                    cursorDef.CreateDependency(guidLastInteraction);
                    cursorDef.CreateArkitPlaceCursorAction(cursor, (Gaze_ArkitPlaceConstraints)constraintsIndex, heightOffset, distanceFromCamera);
                    guidLastInteraction = cursorDef.GUID;
                }

                S_InteractionDefinition placeObjectDef = ioDef.CreateInteractionDefinition(string.Concat("Place ", objectToPlace.name));
                placeObjectDef.CreateTouchCondition();

                if (constraintsIndex == (int)Gaze_ArkitPlaceConstraints.OVER_A_PLANE)
                {
                    placeObjectDef.CreateGazeCondition(ioDef.GUID, Gaze_HoverStates.IN, Gaze_GazeConstraints.PLANE);
                }
                else if (constraintsIndex == (int)Gaze_ArkitPlaceConstraints.OVER_AN_OBJECT)
                {
                    placeObjectDef.CreateGazeCondition(ioDef.GUID, Gaze_HoverStates.IN, Gaze_GazeConstraints.ANY_OBJECT);
                }

                placeObjectDef.CreateArkitMoveAction(objectToPlace, (Gaze_ArkitPlaceConstraints)constraintsIndex, heightOffset, distanceFromCamera, deactivateAtStart);

                placeObjectDef.CreateDependency(guidLastInteraction);

                // Check if we need to place a cursor
                if (cursor != null)
                {
                    S_InteractionDefinition cursorDeactivateDef = ioDef.CreateInteractionDefinition(string.Concat("Disable Cursor ", objectToPlace.name));
                    cursorDeactivateDef.CreateDependency(placeObjectDef.GUID);
                    cursorDeactivateDef.CreateArkitHidePlaceCursor(guidLastInteraction);
                    guidLastInteraction = cursorDeactivateDef.GUID;
                }
                else
                {
                    guidLastInteraction = placeObjectDef.GUID;
                }
            }
        }
        S_InteractionDefinition end = ioDef.CreateInteractionDefinition(string.Concat("End ", behaviourName));
        end.Delay = 0.03f;
        end.CreateDependency(guidLastInteraction);

        SpatialStoriesAPI.CreateInteractiveObject(ioDef, false);
        SpatialStoriesAPI.WirePendingDependencies();
    
    }

    [MenuItem("SpatialStories/Behaviours/Object Mover")]
    public static void ShowWindow()
    {
        EditorWindow win = EditorWindow.GetWindow(typeof(Behaviour_ObjectMover));
        win.minSize = MIN_SIZE;
    }
}
