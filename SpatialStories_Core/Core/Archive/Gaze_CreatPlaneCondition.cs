using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaze
{
	public class Gaze_CreatPlaneCondition : Gaze_AbstractCondition
	{
		private Gaze_CreatePlayPlaneAction playspaceAction;

		public Gaze_CreatPlaneCondition(Gaze_Conditions _gazeConditionsScript) : base(_gazeConditionsScript)
		{
		}

        public override void SetupUsingApi(GameObject _interaction)
        {
            throw new NotImplementedException();
        }

        public override bool IsValidated()
		{
			return IsValid;
		}

		public override void ToEditorGUI()
		{
			#if UNITY_EDITOR
			EditorGUILayout.BeginHorizontal();
			if (IsValid)
			{
				Gaze_AbstractCondition.RenderSatisfiedLabel("ARKit Playspace Condition:");
				Gaze_AbstractCondition.RenderSatisfiedLabel("Valid");
			}
			else
			{
				Gaze_AbstractCondition.RenderNonSatisfiedLabel("ARKit Playspace Condition:");
				Gaze_AbstractCondition.RenderNonSatisfiedLabel("Not Valid");
			}
			EditorGUILayout.EndHorizontal();
			#endif
		}

		protected override void CustomDispose()
		{
		}

		protected override void CustomSetup()
		{
			playspaceAction = GameObject.FindObjectOfType<Gaze_CreatePlayPlaneAction> (); 
			playspaceAction.PlaySpaceStateChanged += StateChanged;
		}

		void StateChanged()
		{
				IsValid = playspaceAction.PlaneCreated;
		}
	}
}