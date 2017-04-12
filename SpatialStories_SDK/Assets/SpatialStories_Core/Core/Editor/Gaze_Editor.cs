using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Gaze
{
    [CanEditMultipleObjects]
    public abstract class Gaze_Editor : Editor
    {
        public abstract void Gaze_OnInspectorGUI();

        private Object objectBeforeChanges;

        public Gaze_Editor() : base()
        {


        }

        public void BeginChangeComparision()
        {
            Undo.RecordObject(target, "Changes");
            EditorGUI.BeginChangeCheck();
        }


        public void EndChangeComparision()
        {
            serializedObject.ApplyModifiedProperties();
            if (!Application.isPlaying && EditorGUI.EndChangeCheck())
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        public override void OnInspectorGUI()
        {
            BeginChangeComparision();
            Gaze_OnInspectorGUI();
            EndChangeComparision();
        }

    }
}
