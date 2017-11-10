using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gaze
{
    [InitializeOnLoad]
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Gaze_Teleporter))]
    public class Gaze_TeleporterEditor : Gaze_Editor
    {

        Gaze_Teleporter teleporter;
        SerializedProperty Hotspots, Layers;

        private void OnEnable()
        {
            teleporter = (Gaze_Teleporter)target;
            Hotspots = serializedObject.FindProperty("HotSpots");
            Layers = serializedObject.FindProperty("AllowedLayers");

        }

        public override void Gaze_OnInspectorGUI()
        {
            EditorGUILayout.Space();
            Gaze_EditorUtils.DrawSectionTitle("ORIENTATION");
            Gaze_EditorUtils.DrawEditorHint("Will the user be reoriented after the teleport?");
            teleporter.OrientOnTeleport = EditorGUILayout.Toggle("Orient on Teleport", teleporter.OrientOnTeleport);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            Gaze_EditorUtils.DrawSectionTitle("VISUAL PROPERTIES");
            Gaze_EditorUtils.DrawEditorHint("Customize the visual aspect of the teleport");
            teleporter.GyroPrefab = (GameObject)EditorGUILayout.ObjectField("Target Destination", teleporter.GyroPrefab, typeof(GameObject), true);
            teleporter.GoodDestinationColor = EditorGUILayout.ColorField(new GUIContent("Allowed Destination", "The color that the teleport stream will have when the destination is valid"), teleporter.GoodDestinationColor);
            teleporter.BadDestinationColor = EditorGUILayout.ColorField(new GUIContent("Not Allowed Destination", "The color that the teleport stream will have when the destination is not valid"), teleporter.BadDestinationColor);
            teleporter.LineWidth = EditorGUILayout.FloatField(new GUIContent("Line width", "With of the line in metters"), teleporter.LineWidth);
            teleporter.LineMaterial = (Material)EditorGUILayout.ObjectField("Line Material", teleporter.LineMaterial, typeof(Material), true);

            EditorGUILayout.Space();
            Gaze_EditorUtils.DrawSectionTitle("CONSTRAINTS");
            Gaze_EditorUtils.DrawEditorHint("Conditions that the user will need to meet to teleport");
            DisplayLayers();
            teleporter.MaxTeleportDistance = EditorGUILayout.FloatField(new GUIContent("Max Teleport Distance", "Ho far in metters the user can teleport"), teleporter.MaxTeleportDistance);
            teleporter.MaxSlope = EditorGUILayout.FloatField(new GUIContent("Max Slope", "Max difference in heigth where the user can teleport"), teleporter.MaxSlope);
            
            EditorGUILayout.Space();
            Gaze_EditorUtils.DrawSectionTitle("HOTSPOTS");
            Gaze_EditorUtils.DrawEditorHint("Use them to make the user teleports in precise places");
            DisplayHotspots();
            teleporter.MinHotspotDistance = EditorGUILayout.FloatField(new GUIContent("Minimum detection distance", "Distance to use a teleport hotspot"), teleporter.MinHotspotDistance);

            EditorGUILayout.Space();
            Gaze_EditorUtils.DrawSectionTitle("ERROR PREVENTION");
            Gaze_EditorUtils.DrawEditorHint("Use this parameters to avoid user input errors");
            teleporter.HoldTimeToAppear = EditorGUILayout.FloatField(new GUIContent("Hold Duration To Activate", "Time that the user needs to be holding the teleport button in order to make it appear"), teleporter.HoldTimeToAppear);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Input Sensitivity", "Tolerance in the joystic in order to avoid false inputs, where 0 is VERY sensitive"));
            teleporter.InptuThreshold = EditorGUILayout.Slider(teleporter.InptuThreshold, 0.0f, 1.0f);
            EditorGUILayout.EndHorizontal(); 
            teleporter.Cooldown = EditorGUILayout.FloatField(new GUIContent("Cooldown", "Time that the user needs to wait between teleports"), teleporter.Cooldown);
        }

        public void DisplayLayers()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(Layers, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        public void DisplayHotspots()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(Hotspots, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
