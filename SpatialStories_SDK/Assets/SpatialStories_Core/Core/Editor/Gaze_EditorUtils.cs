using UnityEditor;
using UnityEngine;

namespace Gaze
{
    public static class Gaze_EditorUtils
    {
        private static int lastFontsize;
        private static FontStyle lastFontStyle;

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

        public static void DrawEditorHint(string _hint)
        {
            StoreLastStyles();
            EditorStyles.label.fontStyle = FontStyle.Italic;
            EditorStyles.label.fontSize = 8;
            EditorGUILayout.LabelField(_hint);
            RestoreLastStyles();
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
    }
}
