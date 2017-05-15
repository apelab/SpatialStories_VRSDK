using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;


namespace Gaze
{

	[InitializeOnLoad]
	[CustomEditor(typeof(Gaze_TriggerCustom))]
	public class Gaze_TriggerCustomEditor : Gaze_Editor
    {


        private Gaze_TriggerCustom triggerCustom;
		private int[] objectIndex = new int[Enum<TriggerEventsAndStates>.Count];
		private int[] methodIndex = new int[Enum<TriggerEventsAndStates>.Count];
		private List<MonoBehaviour> attachedScripts;
		private List<string> scriptNames, methodNames;
		private BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;

		void OnEnable ()
		{
			triggerCustom = (Gaze_TriggerCustom)target;

			attachedScripts = new List<MonoBehaviour> ();
			scriptNames = new List<string> ();
			methodNames = new List<string> ();

			refreshScripts ();

			// load values from instance
			if (scriptNames != null && scriptNames.Count > 0)
			{
				for (int i=0; i< Enum.GetNames(typeof(TriggerEventsAndStates)).Length; i++)
				{
					if (triggerCustom.activeTriggerStates [i])
					{
						objectIndex [i] = attachedScripts.IndexOf (triggerCustom.targetScripts [i]);

						if (objectIndex [i] > 0)
						{
							refreshMethods (i);

							methodIndex [i] = methodNames.IndexOf (triggerCustom.targetMethods [i]);
							
							if (methodIndex [i] <= 0)
							{
								methodIndex [i] = 0;
							}
						} else
						{
							objectIndex [i] = 0;
						}
					}
				}
			}
		}

        public override void Gaze_OnInspectorGUI()
        {
            refreshScripts();


            for (int i = 0; i < Enum.GetNames(typeof(TriggerEventsAndStates)).Length; i++)
            {
                triggerCustom.activeTriggerStates[i] = EditorGUILayout.ToggleLeft(Enum.GetNames(typeof(TriggerEventsAndStates))[i], triggerCustom.activeTriggerStates[i]);

                if (triggerCustom.activeTriggerStates[i])
                {

                    if (scriptNames != null && scriptNames.Count > 0)
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField("Target Component");
                        objectIndex[i] = EditorGUILayout.Popup(objectIndex[i], scriptNames.ToArray());
                        EditorGUILayout.EndHorizontal();

                        triggerCustom.targetScripts[i] = attachedScripts[objectIndex[i]];
                        refreshMethods(i);

                        if (methodNames != null && methodNames.Count > 0)
                        {
                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.LabelField("Target Function");
                            methodIndex[i] = EditorGUILayout.Popup(methodIndex[i], methodNames.ToArray());
                            EditorGUILayout.EndHorizontal();

                            triggerCustom.targetMethods[i] = methodNames[methodIndex[i]];
                        }
                        else
                        {
                            // else display a warning text
                            EditorGUILayout.HelpBox("No Methods Available", MessageType.Warning);
                        }
                    }
                    else
                    {
                        // else display a warning text
                        EditorGUILayout.HelpBox("No Scripts Available", MessageType.Warning);
                    }
                }
            }

            // save changes
            EditorUtility.SetDirty(triggerCustom);
        }

        /// <summary>
        /// list all the MonoBehaviours available from the Gaze_Gazable.Root GameObject
        /// </summary>
        private void refreshScripts ()
		{
			attachedScripts.Clear ();
			scriptNames.Clear ();
			foreach (MonoBehaviour m in triggerCustom.GetComponent<Gaze_Conditions>().Root.GetComponentsInChildren<MonoBehaviour> ())
			{
				attachedScripts.Add (m);
				scriptNames.Add (m.GetType ().Name);
			}
		}

		/// <summary>
		/// For the selected MonoBehaviour list all the public methods available (with 0 parameters)
		/// </summary>
		private void refreshMethods (int i)
		{
			methodNames.Clear ();

			foreach (MethodInfo m in attachedScripts[objectIndex[i]].GetType().GetMethods(flags))
			{
				ParameterInfo[] parameters = m.GetParameters ();

				if (parameters == null || parameters.Length == 0)
				{
					methodNames.Add (m.Name);
				}
			}
		}
	}
}
