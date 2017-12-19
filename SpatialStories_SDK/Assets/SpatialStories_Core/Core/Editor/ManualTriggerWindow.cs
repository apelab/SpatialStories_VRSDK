using UnityEditor;
using UnityEngine;

using System.Collections.Generic;

namespace Gaze{

	public class ManualTriggerWindow : EditorWindow
	{
		private static List<Gaze_Interaction> interactionsToValidate;
        
		[MenuItem("SpatialStories/Utils/Manual Trigger Validator")]
		public static void ShowWindow()
		{
			if(interactionsToValidate == null)
				interactionsToValidate = new List<Gaze_Interaction>();
			
			GetWindow(typeof(ManualTriggerWindow));
		}

		void OnGUI()
		{
            if (interactionsToValidate == null)
                interactionsToValidate = new List<Gaze_Interaction>();

			GUILayout.BeginHorizontal();
			
			GUILayout.BeginVertical();
			ShowManualTriggerWindow();
			GUILayout.EndVertical();
			
			GUILayout.EndHorizontal();
		}

		public void ShowManualTriggerWindow()
		{
			GUILayout.Label("Interactions To Validate:", EditorStyles.boldLabel);
			if (interactionsToValidate != null)
			{
				for (int i = interactionsToValidate.Count -1; i >= 0; i--)
				{
					Gaze_Interaction interaction = interactionsToValidate[i];
					EditorGUILayout.BeginHorizontal();
					interactionsToValidate[i] = EditorGUILayout.ObjectField(interaction, typeof(Gaze_Interaction), true) as Gaze_Interaction;
					if (interaction != null)
					{
						Gaze_Conditions condition = interaction.GetComponent<Gaze_Conditions>();
						if (condition.canBeTriggered)
						{
							if (GUILayout.Button("Validate Interaction"))
							{
								interaction.GetComponent<Gaze_Conditions>().ValidateCondition();
							}
						}
						else
						{
							GUILayout.Label("Validated!");
							if (GUILayout.Button("Revalidate"))
							{
								interaction.GetComponent<Gaze_Conditions>().ValidateCondition();
							}
						}
     
					}
                    if (GUILayout.Button("-"))
                    {
                        interactionsToValidate.RemoveAt(i);
                    }

                    EditorGUILayout.EndHorizontal();
				}	
			}


            if (GUILayout.Button("+ Interaction"))
			{
				interactionsToValidate.Add(null);
			}
		}
	}
}