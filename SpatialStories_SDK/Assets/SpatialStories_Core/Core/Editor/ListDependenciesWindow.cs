using UnityEditor;
using UnityEngine;

using System.Collections.Generic;

namespace Gaze{

	public class ListDependenciesWindow : EditorWindow
	{
		private static List<Gaze_Interaction> interactionsToValidate;
        
		[MenuItem("SpatialStories/Utils/List Dependencies")]
		public static void ShowWindow()
		{
			if(interactionsToValidate == null)
				interactionsToValidate = new List<Gaze_Interaction>();
			
			GetWindow(typeof(ListDependenciesWindow));
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
		public List<Gaze_Interaction> DependentObjects = new List<Gaze_Interaction>();

        private void ShowDepententObjectsInspector()
        {
            GUILayout.Label("Drop The Trigger to see all its dependencies:", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();

            Dependency = EditorGUILayout.ObjectField(Dependency, typeof(Gaze_Interaction), true) as Gaze_Interaction;

            if (GUILayout.Button("List Dependent Objects"))
            {
                DependentObjects.Clear();
                Gaze_Conditions[] AllConditions = Object.FindObjectsOfType<Gaze_Conditions>();
                foreach (var condition in AllConditions)
                {
                    if (condition.dependent)
                    {
                        foreach (Gaze_Dependency dependency in condition.ActivateOnDependencyMap.dependencies)
                        {
                            if (dependency.dependentGameObject == Dependency.gameObject)
                                DependentObjects.Add(condition.GetComponent<Gaze_Interaction>());
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();
            if (DependentObjects != null)
            {
                foreach (Gaze_Interaction dependentObject in DependentObjects)
                {
                    var a = EditorGUILayout.ObjectField(dependentObject, typeof(Gaze_Interaction), true) as Gaze_Interaction;
                }
            }

        }
	}
}