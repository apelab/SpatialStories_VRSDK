using UnityEngine;
using SpatialStories;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Gaze
{
    public class Gaze_ARKitScreenTouchCondition : Gaze_AbstractCondition
    {
        private bool touchIO;
        private Gaze_InteractiveObject IO;

        public Gaze_ARKitScreenTouchCondition()
        {
        }

        public Gaze_ARKitScreenTouchCondition(Gaze_Conditions _gazeConditionsScript, Gaze_InteractiveObject _IO,
            bool _touchIO) : base(_gazeConditionsScript)
        {
            Setup(_gazeConditionsScript, _IO, _touchIO);
        }

        public void Setup(Gaze_Conditions _gazeConditionsScript, Gaze_InteractiveObject _IO, bool _touchIO)
        {
            gazeConditionsScript = _gazeConditionsScript;
            touchIO = _touchIO;
            IO = _IO;

            if (touchIO)
            {
                Gaze_InputManager.OnObjectTouchedDown += GazeInputManagerOnOnObjectTouchedDown;
                Gaze_InputManager.OnObjectTouchUp += GazeInputManagerOnOnObjectTouchUp;
            }
            else
            {
                Gaze_InputManager.OnScreenTouched += OnScreenTouched;
            }
        }

        protected override void CustomDispose()
        {
            if (touchIO)
            {
                Gaze_InputManager.OnObjectTouchedDown -= GazeInputManagerOnOnObjectTouchedDown;
                Gaze_InputManager.OnObjectTouchUp -= GazeInputManagerOnOnObjectTouchUp;
            }
            else
            {
                Gaze_InputManager.OnScreenTouched -= OnScreenTouched;   
            }
        }
        
        private void GazeInputManagerOnOnObjectTouchedDown(Gaze_InteractiveObject _io)
        {
            if (_io != IO)
                return;

            IsValid = true;
        }

        private void GazeInputManagerOnOnObjectTouchUp(Gaze_InteractiveObject _io)
        {
            if (_io != IO)
                return;

            IsValid = false;
        }

        public override bool IsValidated()
        {
            if (gazeConditionsScript.focusDuration == 0)
            {
                bool val = IsValid;
                IsValid = false;
                return val;
            }
            else
            {
                return IsValid;
            }
        }

        public override void ToEditorGUI()
        {
#if UNITY_EDITOR
            EditorGUILayout.BeginHorizontal();
            if (IsValid)
            {
                Gaze_AbstractCondition.RenderSatisfiedLabel("ARKit Screen Touch Condition:");
                Gaze_AbstractCondition.RenderSatisfiedLabel("Valid");
            }
            else
            {
                Gaze_AbstractCondition.RenderNonSatisfiedLabel("ARKit Screen Touch Condition:");
                Gaze_AbstractCondition.RenderNonSatisfiedLabel("Not Valid");
            }

            EditorGUILayout.EndHorizontal();
#endif
        }


        protected override void CustomSetup()
        {
        }

        public void OnScreenTouched(Touch _touch)
        {
            if (gazeConditionsScript.triggerStateIndex != (int) Gaze_TriggerState.ACTIVE)
                return;

            if (touchIO)
                return;

            IsValid = _touch.phase != TouchPhase.Ended;
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            // Get the data and cast it

            bool touchIO = (bool) creationData[1];
            string touchObjGUID = string.Empty;
            if (touchIO)
            {
                touchObjGUID = creationData[0].ToString();
            }

            // Get the conditions obj
            Gaze_Conditions conditions = _interaction.GetComponent<Gaze_Conditions>();

            // Enable touch for the conditions
            conditions.arkitTouchEnabled = true;

            Gaze_InteractiveObject io = null;
            if (touchIO)
            {
                io = SpatialStoriesAPI.GetInteractiveObjectWithGUID(touchObjGUID);
            }

            // this condition has the touch options on the gaze conditions script some we need to set it there too
            conditions.arkitTouchOption = touchIO
                ? Gaze_Conditions.ARKIT_TOUCH_OPTIONS.Object
                : Gaze_Conditions.ARKIT_TOUCH_OPTIONS.Anywhere;

            // Setup the touch condition
            Setup(conditions, io, touchIO);
        }
    }

    /// <summary>
    /// Example of how making the API more friendly by creating wrapper classes outside
    /// the API definition
    /// </summary>
    public static partial class APIExtensions
    {
        /// <summary>
        /// Helper method to create a touch condition with the SpatialStoriesAPI
        /// </summary>
        /// <param name="_objectToTouchGUID">Object to touch</param>
        /// <param name="_touchIO">Touch on object or everywhere on the screen ?</param>
        /// <returns></returns>
        public static Gaze_ARKitScreenTouchCondition CreateTouchCondition(this S_InteractionDefinition _def,
            string _objectToTouchGUID, bool _touchIO)
        {
            return _def.CreateCondition<Gaze_ARKitScreenTouchCondition>(_objectToTouchGUID, _touchIO);
        }

        /// <summary>
        /// Creates a touch condition definition setup as Touch Everywhere
        /// </summary>
        /// <param name="_def"></param>
        /// <returns></returns>
        public static Gaze_ARKitScreenTouchCondition CreateTouchCondition(this S_InteractionDefinition _def)
        {
            return _def.CreateCondition<Gaze_ARKitScreenTouchCondition>(null, false);
        }
    }
}