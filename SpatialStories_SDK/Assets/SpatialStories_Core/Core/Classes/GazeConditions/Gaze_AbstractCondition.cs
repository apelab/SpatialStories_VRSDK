using System;
using UnityEditor;
using UnityEngine;

namespace Gaze
{
    public abstract class Gaze_AbstractCondition : IDisposable
    {
        private bool hasBeenSetup = false;
        protected Gaze_Conditions gazeConditionsScript;
        protected bool IsValid;

        public Gaze_AbstractCondition(Gaze_Conditions _gazeConditionsScript)
        {
            if (_gazeConditionsScript == null)
                Debug.LogError("The conditions script can't be null!");

            IsValid = false;
            gazeConditionsScript = _gazeConditionsScript;
            gazeConditionsScript.allConditions.Add(this);
            Enable();
        }

        public void Enable()
        {
            if (hasBeenSetup)
                return;

            gazeConditionsScript.OnReload += Reset;
            CustomSetup();
            hasBeenSetup = true;
        }

        public void Disable()
        {
            //TODO: Register to the reload event
            gazeConditionsScript.OnReload -= Reset;
            CustomDispose();
            hasBeenSetup = false;
        }

        // This dispose method will called by the 
        // GC if the object gets destroyed.
        public void Dispose()
        {
            gazeConditionsScript.OnReload -= Reset;
            CustomDispose();
            gazeConditionsScript.allConditions.Remove(this);
        }

        protected abstract void CustomSetup();
        protected abstract void CustomDispose();


        public abstract bool IsValidated();

        protected void Reset(bool _reloadDependencies)
        {
            if (_reloadDependencies && this is Gaze_Dependency)
                return;

            IsValid = false;
        }

        // This allows us to show the condition state on the GUI
        public abstract void ToEditorGUI();

        public static void RenderDefaultLabel(string text)
        {
            EditorGUILayout.LabelField(text);
        }

        public static void RenderNonSatisfiedLabel(string text)
        {
            GUI.color = Color.gray;
            EditorGUILayout.LabelField(text);
            GUI.color = Color.white;
        }

        public static void RenderSatisfiedLabel(string text)
        {
            EditorGUILayout.LabelField(text, EditorStyles.whiteLabel);
        }


    }
}
