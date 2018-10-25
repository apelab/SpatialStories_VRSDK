using Gaze;
using System;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[InitializeOnLoad]
[CustomEditor(typeof(CC_ARKitBlendShapes))]
public class Gaze_ActionsEditor : Gaze_Editor
{
    CC_ARKitBlendShapes targetShapes;

    private void OnEnable()
    {
        targetShapes = (CC_ARKitBlendShapes)target;
    }

    public override void Gaze_OnInspectorGUI()
    {

        DisplayOptions();
    }

    private void DisplayOptions()
    {

        // help message if no input is specified
        if (targetShapes.SelectedBlendShapes.ShapesToTrack.Count < 1)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox("No blend shape parameter selected!", MessageType.Info);
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            targetShapes.RequireAll = EditorGUILayout.ToggleLeft("Require all", targetShapes.RequireAll);

            for (int i = 0; i < targetShapes.SelectedBlendShapes.ShapesToTrack.Count; i++)
            {
                // display the entry
                EditorGUILayout.BeginHorizontal();

                // and a '-' button to remove it if needed
                if (GUILayout.Button("-"))
                    targetShapes.SelectedBlendShapes.ShapesToTrack.RemoveAt(i);

                targetShapes.SelectedBlendShapes.ShapesToTrack[i].PositionInList = EditorGUILayout.Popup(targetShapes.SelectedBlendShapes.ShapesToTrack[i].PositionInList, targetShapes.ARBlendShapes);
                targetShapes.SelectedBlendShapes.ShapesToTrack[i].comparisonType = (BlendValueType)EditorGUILayout.Popup((int)targetShapes.SelectedBlendShapes.ShapesToTrack[i].comparisonType, Enum.GetNames(typeof(BlendValueType)));
                targetShapes.SelectedBlendShapes.ShapesToTrack[i].Value = EditorGUILayout.Slider(targetShapes.SelectedBlendShapes.ShapesToTrack[i].Value, 0.0f, 1.0f);
                EditorGUILayout.EndHorizontal();
            }

        }

        // display 'add' button
        if (GUILayout.Button("+"))
        {
            targetShapes.SelectedBlendShapes.ShapesToTrack.Add(new ShapeToTrack());
        }

        EditorGUILayout.Space();
    }
}
