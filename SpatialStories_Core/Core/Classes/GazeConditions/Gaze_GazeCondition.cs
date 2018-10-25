using SpatialStories;
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
        private bool validToEditorGUI;
        private Collider gazeCollider;
        private Gaze_GazeConstraints constraints;

        public Gaze_GazeCondition(Gaze_Conditions _gazeConditionsScript, Collider _gazeCollider, Gaze_GazeConstraints _constraints) : base(_gazeConditionsScript)
        {
            constraints = _constraints;
            Setup(_gazeCollider);
        }

        public Gaze_GazeCondition() { }

        public void Setup(Collider _gazeCollider)
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

        /// <summary>
        /// Important to override the parent I don't want to set this to false
        /// </summary>
        /// <param name="_reloadDependencies"></param>
        protected override void Reset(bool _reloadDependencies)
        {
        }

        protected override void CustomDispose()
        {
            Gaze_EventManager.OnGazeEvent -= OnGazeEvent;
        }

        private void PerformPlaneLogic(Gaze_GazeEventArgs _e)
        {
            if (gazeConditionsScript.gazeIn)
            {
                IsValid = _e.IsGazed;
                validToEditorGUI = IsValid;
            }
            else
            {
                IsValid = !_e.IsGazed;
                validToEditorGUI = IsValid;

            }
        }

        private void PerformObjectLogic(Gaze_GazeEventArgs _e)
        {
            // if sender is the gazable collider GameObject specified in the InteractiveObject Gaze field
            if (_e.Sender != null && gazeCollider != null)
            {
                if(constraints == Gaze_GazeConstraints.ANY_OBJECT || (GameObject)_e.Sender == gazeCollider.gameObject)
                {
                    // check if gaze is set to IN or OUT, and set IsValid accordingly
                    if (gazeConditionsScript.gazeIn)
                    {
                        IsValid = _e.IsGazed;
                        validToEditorGUI = IsValid;
                    }
                    else
                    {
                        IsValid = !_e.IsGazed;
                        validToEditorGUI = IsValid;
                    }
                }
            }
        }

        private void PerformImageLogic(Gaze_GazeEventArgs _e)
        {
            // if sender is the gazable collider GameObject specified in the InteractiveObject Gaze field
            if (_e.Sender != null && gazeCollider != null)
            {
                if (constraints == Gaze_GazeConstraints.IMAGE && ((string)_e.Sender == gazeConditionsScript.arAnchorImage.name))
                {
                    // check if gaze is set to IN or OUT, and set IsValid accordingly                    
                    if (gazeConditionsScript.gazeIn)
                    {
                        IsValid = _e.IsGazed;
                        validToEditorGUI = IsValid;
                    }
                    else
                    {
                        IsValid = !_e.IsGazed;
                        validToEditorGUI = IsValid;
                    }
                }
            }
        }        

        private void OnGazeEvent(Gaze_GazeEventArgs _e)
        {
            if (_e.TargetType == Gaze_GazeConstraints.PLANE && constraints == Gaze_GazeConstraints.PLANE)
            {
                PerformPlaneLogic(_e);
            }
            else if (_e.TargetType == Gaze_GazeConstraints.IMAGE)
            {
                PerformImageLogic(_e);
            }
            else if(_e.TargetType != Gaze_GazeConstraints.PLANE)
            {
                PerformObjectLogic(_e);
            }            
        }

        public override void ToEditorGUI()
        {
#if UNITY_EDITOR
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
#endif
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            // Get the data and cast it
            string objectToGazeGUID = creationData[0].ToString();
            bool gazeIn = (bool)creationData[1];
            Gaze_GazeConstraints gazeConstraints = (Gaze_GazeConstraints)creationData[2];

            // Get the conditions obj
            Gaze_Conditions conditions = _interaction.GetComponent<Gaze_Conditions>();

            // Set al the necessary parameters
            conditions.gazeEnabled = true;
            conditions.gazeIn = gazeIn;
            constraints = gazeConstraints;
            conditions.gazeConstraintsIndex = (int)gazeConstraints;
            conditions.gazeStateIndex = gazeIn ? (int)Gaze_HoverStates.IN : (int)Gaze_HoverStates.OUT;
            

            if(gazeConstraints == Gaze_GazeConstraints.OBJECT)
            {
                Gaze_InteractiveObject io = SpatialStoriesAPI.GetInteractiveObjectWithGUID(objectToGazeGUID);
                conditions.gazeColliderIO = io;
                // Setup the gaze collider
                Setup(io.GetComponentInChildren<Gaze_Gaze>().GetComponent<Collider>());
            }
            else
            {
                // Setup the gaze collider
                conditions.gazeColliderIO = conditions.RootIO;
                Setup(conditions.RootIO.GetComponentInChildren<Gaze_Gaze>().GetComponent<Collider>());
            }
        }
    }

    /// <summary>
    /// Example of how making the API more friendly by creating wrapper classes outside
    /// the API definition
    /// </summary>
    public static partial class APIExtensions
    {
        public static Gaze_GazeCondition CreateGazeCondition(this S_InteractionDefinition _def,
            string _objectToGazeGUID, Gaze_HoverStates _hoveState, Gaze_GazeConstraints _constraints)
        {
            bool gazeIn = _hoveState == Gaze_HoverStates.IN;
            return _def.CreateCondition<Gaze_GazeCondition>(_objectToGazeGUID, gazeIn, _constraints);
        }
    }
}