using UnityEditor;

namespace Gaze
{
    [CanEditMultipleObjects]
    [InitializeOnLoad]
    [CustomEditor(typeof(Gaze_DragAndDropManager))]
    public class Gaze_DragAndDropEditor : Gaze_Editor
    {
        private Gaze_InteractiveObject rootIO;

        private Gaze_DragAndDropManager targetConditions;

        void OnEnable()
        {
            targetConditions = (Gaze_DragAndDropManager)target;
            rootIO = targetConditions.GetComponentInParent<Gaze_InteractiveObject>();
        }

        void OnDisable()
        {
        }

        public override void Gaze_OnInspectorGUI()
        {
            // GUI components here...


            EditorGUILayout.BeginHorizontal();
            targetConditions.m_MinDistance = EditorGUILayout.FloatField("Minimum Distance", targetConditions.m_MinDistance);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            targetConditions.respectXAxis = EditorGUILayout.ToggleLeft("Respect X Axis", targetConditions.respectXAxis);
            if (targetConditions.respectXAxis)
            {
                targetConditions.respectXAxisMirrored = EditorGUILayout.ToggleLeft("Respect X Axis Mirrored", targetConditions.respectXAxisMirrored);
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            targetConditions.respectYAxis = EditorGUILayout.ToggleLeft("Respect Y Axis", targetConditions.respectYAxis);
            if (targetConditions.respectYAxis)
            {
                targetConditions.respectYAxisMirrored = EditorGUILayout.ToggleLeft("Respect Y Axis Mirrored", targetConditions.respectYAxisMirrored);
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            targetConditions.respectZAxis = EditorGUILayout.ToggleLeft("Respect Z Axis", targetConditions.respectZAxis);
            if (targetConditions.respectZAxis)
            {
                targetConditions.respectZAxisMirrored = EditorGUILayout.ToggleLeft("Respect Z Axis Mirrored", targetConditions.respectZAxisMirrored);
            }
            EditorGUILayout.EndHorizontal();


            if (targetConditions.respectXAxis || targetConditions.respectYAxis || targetConditions.respectZAxis)
            {
                EditorGUILayout.BeginHorizontal();
                targetConditions.angleThreshold = EditorGUILayout.Slider("Angle Threshold", targetConditions.angleThreshold, 1, 100);
                EditorGUILayout.EndHorizontal();
            }


            EditorGUILayout.BeginHorizontal();
            targetConditions.m_SnapBeforeDrop = EditorGUILayout.ToggleLeft("Snap Before Drop", targetConditions.m_SnapBeforeDrop);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            targetConditions.m_TimeToSnap = EditorGUILayout.FloatField("Time To Snap", targetConditions.m_TimeToSnap);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            targetConditions.CurrentDragAndDropCondition = (Gaze_DragAndDropCondition)EditorGUILayout.ObjectField("Current Drag And Drop Condition", targetConditions.CurrentDragAndDropCondition, typeof(Gaze_DragAndDropCondition), true);
            EditorGUILayout.EndHorizontal();


            // save changes
            EditorUtility.SetDirty(targetConditions);
        }
    }
}
