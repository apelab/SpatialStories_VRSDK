using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    /// <summary>
    /// Class describing the movement of a rotational lever, that is a lever following a specified arc around a rotation point
    /// </summary>
    public class Gaze_RotationalLeverMechanism : ILeverMechanism
    {
        private Transform m_LeverTransform;
        private Transform m_LeverParentTransform;
        private Vector3 m_StartPosition;
        private Vector3 m_VectorToStart;
        private Vector3 m_RotationPoint;
        private Vector3 m_RotationAxis;
        private float m_FullArcAngle;
        private float m_SnappingAngleDelta;
        private int m_StepNumber;
        private float[] m_Steps;

        /// <summary>
        /// Constructor to initialize the rotational lever state
        /// </summary>
        /// <param name="_leverTransform">Transform that will act as lever and be moved</param>
        /// <param name="_rotationAxis">An axis local to the lever</param>
        /// <param name="_rotationPoint">A rotation point in world space</param>
        /// <param name="_maxLeverAngle">The angle the lever is able to reach from its starting position</param>
        /// <param name="_snappingAngleDelta">The delta at which the lever is considered at start, end or step position</param>
        /// <param name="_stepsNumber">The number of steps the lever should have</param>
        /// <param name="_steps">An array of percentages describing the steps for this lever</param>
        public Gaze_RotationalLeverMechanism(Transform _leverTransform, Vector3 _rotationAxis, Vector3 _rotationPoint, float _maxLeverAngle, float _snappingAngleDelta, int _stepsNumber, float[] _steps)
        {
            m_LeverTransform = _leverTransform;

            // We also cache the lever's parent transform to later make
            // our calculations in its local space
            m_LeverParentTransform = m_LeverTransform.parent;

            m_StartPosition = _leverTransform.localPosition;
            m_RotationPoint = m_LeverParentTransform.InverseTransformPoint(_rotationPoint);
            m_VectorToStart = m_StartPosition - m_RotationPoint;
            m_FullArcAngle = _maxLeverAngle;
            m_RotationAxis = _rotationAxis;
            m_SnappingAngleDelta = _snappingAngleDelta;
            m_StepNumber = _stepsNumber;
            m_Steps = _steps;
        }

        public int ComputeLeverPosition(Vector3 _controllerWorldPosition)
        {
            Vector3 controllerLocalPosition = m_LeverParentTransform.InverseTransformPoint(_controllerWorldPosition);
            Vector3 rotationPointToController = controllerLocalPosition - m_RotationPoint;
            Vector3 rotationAxisProjection = Vector3.Project(rotationPointToController.normalized, m_RotationAxis.normalized);
            Vector3 rotationPointToControllerOnPlane = rotationPointToController.normalized - rotationAxisProjection;

            float signedAngle = Vector3.SignedAngle(m_VectorToStart, rotationPointToControllerOnPlane, m_RotationAxis);

            float angle = Mathf.Clamp(signedAngle, Mathf.Min(0.0f, m_FullArcAngle), Mathf.Max(0.0f, m_FullArcAngle));

            if (ShouldSnapToAngle(angle, 0.0f))
            {
                RotateTo(0.0f);

                return 0;
            }

            for (int i = 0; i < m_StepNumber; i++)
            {
                float stepAngle = m_Steps[i] / 100.0f * m_FullArcAngle;
                if (ShouldSnapToAngle(angle, stepAngle))
                {
                    RotateTo(stepAngle);

                    return i + 1;
                }
            }

            if (ShouldSnapToAngle(angle, m_FullArcAngle))
            {
                RotateTo(m_FullArcAngle);

                return Gaze_LeverMechanism.MAX_STEP_NUMBER + 1;

            }

            RotateTo(angle);

            return -1;
        }

        private void RotateTo(float angle)
        {
            m_LeverTransform.localRotation = Quaternion.AngleAxis(angle, m_RotationAxis);
            m_LeverTransform.localPosition = (m_RotationPoint + m_LeverTransform.localRotation * m_VectorToStart);
        }

        private bool ShouldSnapToAngle(float currentAngle, float angleToCheck)
        {
            if (Mathf.Abs(angleToCheck - currentAngle) < m_SnappingAngleDelta)
            {
                return true;
            }

            return false;
        }

#if UNITY_EDITOR
        // This function attempts to draw useful information for debugging
        private void DebugDraw(Vector3 _controllerLocalPosition, Vector3 _rotationPointToControllerOnPlaneLocal)
        {
            Vector3 startWorldPosition = m_LeverParentTransform.TransformPoint(m_StartPosition);
            Vector3 rotationPointWorldPosition = m_LeverParentTransform.TransformPoint(m_RotationPoint);
            Vector3 rotationAxisWorld = m_LeverParentTransform.TransformDirection(m_RotationAxis);
            Vector3 controllerWorldPosition = m_LeverParentTransform.TransformPoint(_controllerLocalPosition);
            Vector3 rotationPointToControllerOnPlaneWorld = m_LeverParentTransform.TransformVector(_rotationPointToControllerOnPlaneLocal);
            Debug.DrawLine(rotationPointWorldPosition, startWorldPosition, Color.blue, Time.deltaTime);
            Debug.DrawLine(rotationPointWorldPosition, controllerWorldPosition, Color.red, Time.deltaTime);
            Debug.DrawLine(rotationPointWorldPosition, rotationPointWorldPosition + rotationAxisWorld, Color.green, Time.deltaTime);
            Debug.DrawLine(rotationPointWorldPosition, rotationPointWorldPosition + rotationPointToControllerOnPlaneWorld, Color.yellow, Time.deltaTime);
        }
#endif
    }
}
