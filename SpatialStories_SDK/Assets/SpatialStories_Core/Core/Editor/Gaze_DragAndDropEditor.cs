using UnityEditor;

namespace Gaze
{
    [CanEditMultipleObjects]
    [InitializeOnLoad]
    //[CustomEditor(typeof(Gaze_DragAndDropManager))]
    public class Gaze_DragAndDropEditor : Gaze_Editor
    {
        private Gaze_InteractiveObject rootIO;

        //private Gaze_DragAndDropManager target_DragAndDropManager;

        void OnEnable()
        {
            //target_DragAndDropManager = (Gaze_DragAndDropManager)target;
           // rootIO = target_DragAndDropManager.GetComponentInParent<Gaze_InteractiveObject>();
        }

        void OnDisable()
        {
        }

        public override void Gaze_OnInspectorGUI()
        {
            // GUI components here...


            EditorGUILayout.BeginHorizontal();
            //target_DragAndDropManager.m_MinDistance = EditorGUILayout.FloatField("Minimum Distance", target_DragAndDropManager.m_MinDistance);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            //target_DragAndDropManager.respectXAxis = EditorGUILayout.ToggleLeft("Respect X Axis", target_DragAndDropManager.respectXAxis);
            //if (target_DragAndDropManager.respectXAxis)
            {
                //target_DragAndDropManager.respectXAxisMirrored = EditorGUILayout.ToggleLeft("Respect X Axis Mirrored", target_DragAndDropManager.respectXAxisMirrored);
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            //target_DragAndDropManager.respectYAxis = EditorGUILayout.ToggleLeft("Respect Y Axis", target_DragAndDropManager.respectYAxis);
            //if (target_DragAndDropManager.respectYAxis)
            {
                //target_DragAndDropManager.respectYAxisMirrored = EditorGUILayout.ToggleLeft("Respect Y Axis Mirrored", target_DragAndDropManager.respectYAxisMirrored);
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            //target_DragAndDropManager.respectZAxis = EditorGUILayout.ToggleLeft("Respect Z Axis", target_DragAndDropManager.respectZAxis);
            //if (target_DragAndDropManager.respectZAxis)
            {
            //    target_DragAndDropManager.respectZAxisMirrored = EditorGUILayout.ToggleLeft("Respect Z Axis Mirrored", target_DragAndDropManager.respectZAxisMirrored);
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
            target_DragAndDropManager.m_TimeToSnap = EditorGUILayout.FloatField("Time To Snap", target_DragAndDropManager.m_TimeToSnap);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            targetConditions.CurrentDragAndDropCondition = (Gaze_DragAndDropCondition)EditorGUILayout.ObjectField("Current Drag And Drop Condition", targetConditions.CurrentDragAndDropCondition, typeof(Gaze_DragAndDropCondition), true);
            EditorGUILayout.EndHorizontal();


            // save changes
            //EditorUtility.SetDirty(target_DragAndDropManager);
        }
    }
}
