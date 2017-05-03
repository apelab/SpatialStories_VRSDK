using UnityEditor;
using UnityEngine;

namespace Gaze
{
    public class Gaze_GazeCondition : Gaze_AbstractCondition
    {
        private Collider gazeCollider;

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
                IsValid = e.IsGazed;
            }
        }

        public override void ToEditorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (IsValid)
            {
                RenderSatisfiedLabel("Gazed:");
                RenderSatisfiedLabel("Gazed");
            }
            else
            {
                RenderNonSatisfiedLabel("Gazed:");
                RenderNonSatisfiedLabel("Ungazed");
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
