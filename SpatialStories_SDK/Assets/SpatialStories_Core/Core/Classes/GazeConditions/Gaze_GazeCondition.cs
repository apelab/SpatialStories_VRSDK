#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Gaze
{
    public class Gaze_GazeCondition : Gaze_AbstractCondition
    {
        // this boolean is a HOTFIX to be able to display the state of the condition in ToEditorGUI(),
        // if the condition is set to reload on infinite, isValid is only true one frame and thus can't be used to display the state of the condition
        private Collider gazeCollider;
        private bool validToEditorGUI;

        public Gaze_GazeCondition(Gaze_Conditions _gazeConditionsScript, Collider _gazeCollider) : base(_gazeConditionsScript)
        {
            gazeCollider = _gazeCollider;
        }

        public override bool IsValidated()
        {
            return IsValid;
        }

        protected override void CustomSetup()
        {
            Gaze_EventManager.OnGazeEvent += OnGazeEvent;
        }

        protected override void CustomDispose()
        {
            Gaze_EventManager.OnGazeEvent -= OnGazeEvent;
        }

        private void OnGazeEvent(Gaze_GazeEventArgs e)
        {
            // if sender is the gazable collider GameObject specified in the InteractiveObject Gaze field
            if (e.Sender != null && gazeCollider != null && (GameObject)e.Sender == gazeCollider.gameObject)
            {
                // check if gaze is set to IN or OUT, and set IsValid accordingly
                if (gazeConditionsScript.gazeIn)
                {
                    IsValid = e.IsGazed;
                    validToEditorGUI = IsValid;
                }
                else
                {
                    IsValid = !e.IsGazed;
                    validToEditorGUI = IsValid;
                }
            }
        }

#if UNITY_EDITOR
        public override void ToEditorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (gazeConditionsScript.gazeIn)
            {
                if (validToEditorGUI)
                {
                    RenderSatisfiedLabel("Gazed:");
                    RenderSatisfiedLabel("Gazed");
                }
                else
                {
                    RenderNonSatisfiedLabel("Gazed:");
                    RenderNonSatisfiedLabel("Ungazed");
                }
            }
            else
            {
                if (validToEditorGUI)
                {
                    RenderSatisfiedLabel("Gazed:");
                    RenderSatisfiedLabel("Ungazed");
                }
                else
                {
                    RenderNonSatisfiedLabel("Gazed:");
                    RenderNonSatisfiedLabel("Gazed");
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
#endif
}
