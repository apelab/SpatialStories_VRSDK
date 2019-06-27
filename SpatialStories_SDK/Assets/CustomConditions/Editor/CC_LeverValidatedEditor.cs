using UnityEditor;
using UnityEngine;

namespace Gaze
{
    [InitializeOnLoad]
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CC_LeverValidated))]
    public class CC_LeverValidatedEditor : Gaze_Editor
    {
        private CC_LeverValidated m_CC_LeverValidatedScript;
        private Gaze_LeverMechanism m_Gaze_LeverMechanismScript;

        private void OnEnable()
        {
            m_CC_LeverValidatedScript = (CC_LeverValidated)target;
            m_Gaze_LeverMechanismScript = m_CC_LeverValidatedScript.GetComponentInParent<Gaze_LeverMechanism>();
        }

        public override void Gaze_OnInspectorGUI()
        {
            base.BeginChangeComparision();

            GUILayout.BeginVertical();

            m_CC_LeverValidatedScript.ValidateOnEnd = EditorGUILayout.Toggle("Validate on end", m_CC_LeverValidatedScript.ValidateOnEnd);
            if (!m_CC_LeverValidatedScript.ValidateOnEnd &&
                m_Gaze_LeverMechanismScript.stepsNumber != 0)
            {
                m_CC_LeverValidatedScript.StepToValidate = Mathf.Clamp(EditorGUILayout.IntField("Step to validate", m_CC_LeverValidatedScript.StepToValidate), 1, m_Gaze_LeverMechanismScript.stepsNumber);
            }

            GUILayout.EndVertical();

            base.EndChangeComparision();
            EditorUtility.SetDirty(m_CC_LeverValidatedScript);
        }
    }
}