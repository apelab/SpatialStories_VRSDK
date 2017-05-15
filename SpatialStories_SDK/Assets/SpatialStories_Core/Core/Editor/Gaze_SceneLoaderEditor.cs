using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace Gaze
{
	[InitializeOnLoad]
	[CustomEditor(typeof(Gaze_SceneLoader))]
	public class Gaze_SceneLoaderEditor : Gaze_Editor
    {
		private Gaze_SceneLoader sceneLoader;

		void OnEnable ()
		{
            sceneLoader = (Gaze_SceneLoader)target;
		}

		public override void Gaze_OnInspectorGUI ()
		{
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target Scene");
            sceneLoader.targetSceneName = EditorGUILayout.TextField(sceneLoader.targetSceneName);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Loading Scene");
            sceneLoader.loadingScreen = EditorGUILayout.TextField(sceneLoader.loadingScreen);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Load on Trigger Status");
            sceneLoader.triggerStateIndex = EditorGUILayout.Popup(sceneLoader.triggerStateIndex, Enum.GetNames(typeof(TriggerEventsAndStates)));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Load Delay");
            sceneLoader.loadDelay = EditorGUILayout.FloatField(sceneLoader.loadDelay);
            EditorGUILayout.LabelField("[s]");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Loading Screen");
            sceneLoader.displayLoadingScreen = EditorGUILayout.Toggle(sceneLoader.displayLoadingScreen);
            EditorGUILayout.EndHorizontal();

            if (sceneLoader.targetSceneName == null || sceneLoader.targetSceneName.Length < 1)
            {
                EditorGUILayout.HelpBox("Fill the scene name to load.", MessageType.Warning);
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(sceneLoader);
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
        }
	}
}
