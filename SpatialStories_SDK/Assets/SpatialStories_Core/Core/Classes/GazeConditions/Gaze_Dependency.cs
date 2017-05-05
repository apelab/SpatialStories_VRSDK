using UnityEditor;
using UnityEngine;

namespace Gaze
{
    [System.Serializable]
    public class Gaze_Dependency : Gaze_AbstractCondition
    {
        /// <summary>
        /// The dependent game object.
        /// </summary>
        public GameObject dependentGameObject;

        /// <summary>
        /// The index of the trigger state.
        /// </summary>
        public int triggerStateIndex;

        /// <summary>
        /// TRUE if dependent on Trigger
        /// </summary>
        public bool onTrigger;


        public Gaze_Dependency(Gaze_Conditions _gazeConditionsScript) : base(_gazeConditionsScript)
        {
        }

        protected override void CustomDispose()
        {
        }

        public override bool IsValidated()
        {
            return IsValid;
        }

        protected override void CustomSetup()
        {

        }

        public override void ToEditorGUI()
        {
            EditorGUILayout.BeginHorizontal();

            if (IsValid)
            {
                RenderSatisfiedLabel(dependentGameObject.name + ": ");
                RenderSatisfiedLabel("Satisfied");
            }
            else
            {
                RenderNonSatisfiedLabel(dependentGameObject.name + ": ");
                RenderNonSatisfiedLabel("Not satisfied");
            }

            EditorGUILayout.EndHorizontal();
        }

        public void SetSatisfied(bool isSatisfied)
        {
            IsValid = isSatisfied;
        }
    }

}
