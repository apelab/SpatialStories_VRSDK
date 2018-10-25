using SpatialStories;
#if UNITY_EDITOR
using UnityEditor;
#endif
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

        public Gaze_Dependency()
        {

        }

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
#if UNITY_EDITOR
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
#endif
        }

        public void SetSatisfied(bool isSatisfied)
        {
            IsValid = isSatisfied;
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            // Get the dependency object
            string dependencyGUID = creationData[0].ToString();
            GameObject dependency = SpatialStoriesAPI.GetObjectOfTypeWithGUID(dependencyGUID);

            // Get the conditions of this depdenency
            Gaze_Conditions condition = _interaction.GetComponent<Gaze_Conditions>();

            // Set this condition as dependent
            condition.dependent = true;

            // Add a new depdenency on the dependency map
            gazeConditionsScript = condition;
            dependentGameObject = dependency;
            triggerStateIndex = (int)DependencyTriggerEventsAndStates.Triggered;
            onTrigger = true;
            condition.ActivateOnDependencyMap.dependencies.Add(this);
        }
    }

    /// <summary>
    /// Example of how making the API more friendly by creating wrapper classes outside
    /// the API definition
    /// </summary>
    public static partial class APIExtensions
    {
        public static Gaze_Dependency CreateDependency(this S_InteractionDefinition _def, string _dependentGUID)
        {
            return _def.CreateCondition<Gaze_Dependency>(_dependentGUID);
        }
    }
}
