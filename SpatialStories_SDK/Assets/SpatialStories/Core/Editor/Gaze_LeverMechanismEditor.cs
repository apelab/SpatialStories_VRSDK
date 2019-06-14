using UnityEditor;
using UnityEngine;

namespace Gaze
{
    [InitializeOnLoad]
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Gaze_LeverMechanism))]
    public class Gaze_LeverMechanismEditor : Gaze_Editor
    {
        private const float SPHERE_HANDLE_RADIUS = 0.01f;
        private Gaze_LeverMechanism m_Gaze_LeverMechanismScript;
        private Gaze_InteractiveObject m_Gaze_InteractiveObjectScript;

        private void OnEnable()
        {
            m_Gaze_LeverMechanismScript = (Gaze_LeverMechanism)target;
            m_Gaze_InteractiveObjectScript = m_Gaze_LeverMechanismScript.GetComponent<Gaze_InteractiveObject>();
        }

        public override void Gaze_OnInspectorGUI()
        {
            base.BeginChangeComparision();

            GUILayout.BeginVertical();
            m_Gaze_LeverMechanismScript.leverType = (Gaze_LeverMechanism.LeverType)EditorGUILayout.EnumPopup("Lever type", m_Gaze_LeverMechanismScript.leverType);
            switch (m_Gaze_LeverMechanismScript.leverType)
            {
                case Gaze_LeverMechanism.LeverType.Linear:
                    m_Gaze_LeverMechanismScript.snappingDistance = EditorGUILayout.FloatField("Snapping distance", m_Gaze_LeverMechanismScript.snappingDistance);
                    m_Gaze_LeverMechanismScript.directionAxis = (Gaze_LeverMechanism.Axis)EditorGUILayout.EnumPopup("Direction axis", m_Gaze_LeverMechanismScript.directionAxis);
                    m_Gaze_LeverMechanismScript.displacementDistance = EditorGUILayout.FloatField("Distance from start", m_Gaze_LeverMechanismScript.displacementDistance);
                    break;
                case Gaze_LeverMechanism.LeverType.Rotational:
                    m_Gaze_LeverMechanismScript.rotationAxis = (Gaze_LeverMechanism.Axis)EditorGUILayout.EnumPopup("Rotation axis", m_Gaze_LeverMechanismScript.rotationAxis);
                    m_Gaze_LeverMechanismScript.rotationPoint = (Transform)EditorGUILayout.ObjectField("Rotation point", m_Gaze_LeverMechanismScript.rotationPoint, typeof(Transform), true);
                    m_Gaze_LeverMechanismScript.maxLeverAngle = EditorGUILayout.FloatField("Max lever angle", m_Gaze_LeverMechanismScript.maxLeverAngle);
                    m_Gaze_LeverMechanismScript.snappingAngleDelta = EditorGUILayout.FloatField("Snapping angle", m_Gaze_LeverMechanismScript.snappingAngleDelta);
                    break;
            }

            m_Gaze_LeverMechanismScript.stepsNumber = Mathf.Clamp(EditorGUILayout.IntField("Steps (5 max)", m_Gaze_LeverMechanismScript.stepsNumber), 0, 5);
            if (m_Gaze_LeverMechanismScript.stepsNumber != 0)
            {
                for (int i = 0; i < m_Gaze_LeverMechanismScript.stepsNumber; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(10.0f);
                    m_Gaze_LeverMechanismScript.steps[i] = Mathf.Clamp(EditorGUILayout.IntField(string.Format("Step {0} (%)", i + 1), (int)m_Gaze_LeverMechanismScript.steps[i]), 0, 100);
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndVertical();

            base.EndChangeComparision();
            EditorUtility.SetDirty(m_Gaze_LeverMechanismScript);
        }

        public void OnSceneGUI()
        {
            if (m_Gaze_LeverMechanismScript.leverType == Gaze_LeverMechanism.LeverType.Rotational &&
                (m_Gaze_LeverMechanismScript.rotationPoint == null))
            {
                return;
            }

            Color arcColor = Color.cyan;
            arcColor.a = 0.7f;
            Handles.color = arcColor;

            switch (m_Gaze_LeverMechanismScript.leverType)
            {
                case Gaze_LeverMechanism.LeverType.Linear:
                    DrawLinearLeverHint();
                    break;
                case Gaze_LeverMechanism.LeverType.Rotational:
                    DrawRotationalLeverHint();
                    break;
            }
        }

        private void DrawLinearLeverHint()
        {
            // Convert local axis from editor to world axis
            Vector3 worldDirectionAxis = m_Gaze_InteractiveObjectScript.transform.TransformDirection(Gaze_LeverMechanism.AxisEnumToVector3(m_Gaze_LeverMechanismScript.directionAxis));

            Vector3 startPosition = m_Gaze_InteractiveObjectScript.transform.position;
            Vector3 endPosition = m_Gaze_InteractiveObjectScript.transform.position + m_Gaze_LeverMechanismScript.displacementDistance * worldDirectionAxis.normalized;

            // Draw start position
            Handles.SphereHandleCap(0, startPosition, Quaternion.identity, SPHERE_HANDLE_RADIUS, EventType.Repaint);

            // Draw end position
            Handles.SphereHandleCap(0, endPosition, Quaternion.identity, SPHERE_HANDLE_RADIUS, EventType.Repaint);

            // Draw steps if any
            for (int i = 0; i < m_Gaze_LeverMechanismScript.stepsNumber; i++)
            {
                float stepDistance = m_Gaze_LeverMechanismScript.displacementDistance * (m_Gaze_LeverMechanismScript.steps[i] / 100.0f);
                Handles.SphereHandleCap(0, startPosition + stepDistance * worldDirectionAxis.normalized, Quaternion.identity, SPHERE_HANDLE_RADIUS, EventType.Repaint);
            }

            // Finally draw line connecting everything
            Handles.DrawLine(startPosition, endPosition);
        }

        private void DrawRotationalLeverHint()
        {
            Vector3 startPosition = m_Gaze_InteractiveObjectScript.transform.position;
            Vector3 rotationPointPosition = m_Gaze_LeverMechanismScript.rotationPoint.position;
            Vector3 rotationPointToStart = startPosition - rotationPointPosition;

            // Convert local axis from editor to world axis
            Vector3 worldRotationAxis = m_Gaze_InteractiveObjectScript.transform.TransformDirection(Gaze_LeverMechanism.AxisEnumToVector3(m_Gaze_LeverMechanismScript.rotationAxis));

            // Draw start position
            Handles.SphereHandleCap(0, startPosition, Quaternion.identity, SPHERE_HANDLE_RADIUS, EventType.Repaint);

            // Draw end position
            Quaternion endRotation = Quaternion.AngleAxis(m_Gaze_LeverMechanismScript.maxLeverAngle, worldRotationAxis);
            Vector3 endPosition = rotationPointPosition + endRotation * rotationPointToStart;
            Handles.SphereHandleCap(0, endPosition, endRotation, SPHERE_HANDLE_RADIUS, EventType.Repaint);

            // Draw steps if any
            for (int i = 0; i < m_Gaze_LeverMechanismScript.stepsNumber; i++)
            {
                float stepAngle = m_Gaze_LeverMechanismScript.maxLeverAngle * (m_Gaze_LeverMechanismScript.steps[i] / 100.0f);
                Quaternion stepRotation = Quaternion.AngleAxis(stepAngle, worldRotationAxis);
                Vector3 stepPosition = m_Gaze_LeverMechanismScript.rotationPoint.position + stepRotation * (m_Gaze_InteractiveObjectScript.transform.position - m_Gaze_LeverMechanismScript.rotationPoint.position);
                Handles.SphereHandleCap(0, stepPosition, stepRotation, SPHERE_HANDLE_RADIUS, EventType.Repaint);
            }

            // Finally draw arc connecting everything
            float arcRadius = rotationPointToStart.magnitude;
            Handles.DrawWireArc(rotationPointPosition,
                worldRotationAxis,
                rotationPointToStart,
                m_Gaze_LeverMechanismScript.maxLeverAngle,
                arcRadius);
        }
    }
}