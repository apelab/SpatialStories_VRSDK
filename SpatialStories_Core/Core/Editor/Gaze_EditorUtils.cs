using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gaze
{
    public static class Gaze_EditorUtils
    {
        private static int lastFontsize;
        private static FontStyle lastFontStyle;
        private static EditorBuildSettingsScene[] scenes;
        private static List<string> sceneNames;

        public static List<string> GetScenesFromBuildSettings()
        {
            // get all scenes
            scenes = EditorBuildSettings.scenes;

            // store name of the scenes
            sceneNames = new List<string>();
            for (int i = 0; i < scenes.Length; i++)
            {
                if (scenes[i].enabled)
                    sceneNames.Add(GetSceneName(scenes[i]));
            }

            return sceneNames;
        }

        private static string GetSceneName(EditorBuildSettingsScene S)
        {
            string name = S.path.Substring(S.path.LastIndexOf('/') + 1);
            return name.Substring(0, name.Length - 6);
        }

        public static void DisplayBuildSettingsSceneNames()
        {
            GetScenesFromBuildSettings();
            for (int i = 0; i < sceneNames.Count; i++)
            {
                Debug.Log("Build settings scene : " + sceneNames[i]);
            }
        }

        public static void StoreLastStyles()
        {
            lastFontsize = EditorStyles.label.fontSize;
            lastFontStyle = EditorStyles.label.fontStyle;
        }

        public static void RestoreLastStyles()
        {
            EditorStyles.label.fontStyle = lastFontStyle;
            EditorStyles.label.fontSize = lastFontsize;
        }

        public static void DrawSectionTitle(string _title)
        {
            StoreLastStyles();
            EditorStyles.label.fontStyle = FontStyle.Bold;
            EditorStyles.label.fontSize = 12;
            EditorGUILayout.LabelField(_title);
            RestoreLastStyles();
        }

        public static void DrawEditorHint(string _hint, bool _spaceAfterHint = true)
        {
            StoreLastStyles();
            EditorStyles.label.fontStyle = FontStyle.Italic;
            EditorStyles.label.fontSize = 9;
            EditorGUILayout.LabelField(_hint);
            RestoreLastStyles();
            if (_spaceAfterHint)
                EditorGUILayout.Space();
        }

        public static int Gaze_HintPopup(string _title, int _currentPopupIndex, string[] _options, string _hint, int _labelWidth)
        {
            int index;
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(_title, _hint), GUILayout.MaxWidth(_labelWidth));
            index = EditorGUILayout.Popup("", _currentPopupIndex, _options);
            GUILayout.EndHorizontal();
            return index;
        }

        public static void SubTitle(string _text)
        {
            EditorGUILayout.LabelField(_text, EditorStyles.boldLabel);
        }

        public static void DrawIndentedSection(Action _sectionDrawer)
        {
            EditorGUI.indentLevel++;
            _sectionDrawer();
            EditorGUI.indentLevel--;
        }
    }
}