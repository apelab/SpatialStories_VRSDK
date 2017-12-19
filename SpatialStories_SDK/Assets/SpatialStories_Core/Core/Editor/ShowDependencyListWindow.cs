using UnityEditor;
using UnityEngine;

using System.Collections.Generic;

namespace Gaze
{

    public class ShowDependencyListWindow : EditorWindow
    {
        private static List<Gaze_Interaction> interactionsToValidate;

        [MenuItem("SpatialStories/Utils/Show Legacy Dependencies On IO")]
        public static void ShowWindow()
        {
            if (interactionsToValidate == null)
                interactionsToValidate = new List<Gaze_Interaction>();

            GetWindow(typeof(ShowDependencyListWindow));
        }

        void OnGUI()
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            ShowDepententObjectsInspector();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }


        public Gaze_Interaction Dependency;
        public List<Gaze_Dependency> DependentObjects;

        private void ShowDepententObjectsInspector()
        {
            GUILayout.Label("Drop the interaction to see their hidden data:", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();

            Dependency = EditorGUILayout.ObjectField(Dependency, typeof(Gaze_Interaction), true) as Gaze_Interaction;

            if (GUILayout.Button("Reveal your secrets!"))
            {
                DependentObjects = Dependency.GetComponent<Gaze_Conditions>().ActivateOnDependencyMap.dependencies;
            }
            GUILayout.EndHorizontal();
            if (DependentObjects != null)
            {
                GUILayout.Label("Num Dependencies: " + DependentObjects.Count, EditorStyles.boldLabel);
                foreach (Gaze_Dependency dependentObject in DependentObjects)
                {
                    var a = EditorGUILayout.ObjectField(dependentObject.dependentGameObject != null ? dependentObject.dependentGameObject : null, typeof(Gaze_Interaction), true) as Gaze_Interaction;
                }
            }

        }
    }
}