using UnityEditor;
using UnityEngine;

namespace Gaze
{
    public class Gaze_HandHoverCondition : Gaze_AbstractCondition
    {
        // this boolean is a HOTFIX to be able to display the state of the condition in ToEditorGUI(),
        // if the hover condition is set to reload on infinite, isValid is only true one frame and thus can't be used to display the state of the condition
        private bool validToEditorGUI;
        private Gaze_InteractiveObject handHoverIO;

        public Gaze_HandHoverCondition(Gaze_Conditions _gazeConditionsScript, Gaze_InteractiveObject _handHoverIO) : base(_gazeConditionsScript)
        {
            handHoverIO = _handHoverIO;
        }

        public override bool IsValidated()
        {
            return IsValid;
        }


        protected override void CustomSetup()
        {
            Gaze_EventManager.OnControllerPointingEvent += OnControllerPointingEvent;
        }

        protected override void CustomDispose()
        {
            Gaze_EventManager.OnControllerPointingEvent -= OnControllerPointingEvent;
        }

        private void OnControllerPointingEvent(Gaze_ControllerPointingEventArgs e)
        {
            // if we are hovering at the selected object
            if (e.Dico.Value != null && handHoverIO != null && (GameObject)e.Dico.Value == handHoverIO.gameObject)
            {
                // if we can hover with both hands, we can set IsValid
                if (gazeConditionsScript.hoverHandIndex == (int)Gaze_HandsEnum.BOTH)
                {
                    // check if hover is set to IN or OUT, and set IsValid accordingly
                    if (gazeConditionsScript.hoverIn)
                    {
                        IsValid = e.IsPointed;
                        validToEditorGUI = IsValid;
                    }
                    else
                    {
                        IsValid = !e.IsPointed;
                        validToEditorGUI = IsValid;
                    }
                }

                // else, we have to check if this is the good hand before setting isValid
                else
                {
                    if (e.Dico.Key == gazeConditionsScript.hoverHand)
                    {
                        // check if hover is set to IN or OUT, and set IsValid accordingly
                        if (gazeConditionsScript.hoverIn)
                        {
                            IsValid = e.IsPointed;
                            validToEditorGUI = IsValid;
                        }
                        else
                        {
                            IsValid = !e.IsPointed;
                            validToEditorGUI = IsValid;
                        }
                    }
                }
            }
        }


        public override void ToEditorGUI()
        {
            EditorGUILayout.BeginHorizontal();


            if (gazeConditionsScript.hoverIn)
            {
                if (validToEditorGUI)
                {
                    RenderSatisfiedLabel("Hand Hover:");
                    RenderSatisfiedLabel("Hovered");
                }

                else
                {
                    RenderNonSatisfiedLabel("Hand Hover:");
                    RenderNonSatisfiedLabel("UnHovered");
                }

            }

            else
            {
                if (validToEditorGUI)
                {
                    RenderSatisfiedLabel("Hand Hover:");
                    RenderSatisfiedLabel("UnHovered");
                }

                else
                {
                    RenderNonSatisfiedLabel("Hand Hover:");
                    RenderNonSatisfiedLabel("Hovered");
                }
            }


            EditorGUILayout.EndHorizontal();
        }
    }
}

