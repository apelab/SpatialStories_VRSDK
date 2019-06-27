using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    /// <summary>
    /// Class describing the movement of a linear lever, that is a lever following a specified line
    /// </summary>
    public class Gaze_LinearLeverMechanism : ILeverMechanism
    {
        private Transform m_LeverTransform;
        private Transform m_LeverParentTransform;
        private Vector3 m_StartPosition;
        private Vector3 m_EndPosition;
        private Vector3 m_RailDirection;
        private float m_SnappingDistance;
        private int m_StepsNumber;
        private float[] m_Steps;

        /// <summary>
        /// Constructor to initialize the linear lever state
        /// </summary>
        /// <param name="_leverTransform">Transform that will act as lever and be moved</param>
        /// <param name="_directionAxis">The direction in which the lever will travel</param>
        /// <param name="_displacementDistance">The distance from the start along the direction axis describing the end position</param>
        /// <param name="_snappingDistance">The distance at which the lever is considered either at start, end or step position</param>
        /// <param name="_stepsNumber">The number of steps the lever should have</param>
        /// <param name="_steps">An array of percentages describing the steps for this lever</param>
        public Gaze_LinearLeverMechanism(Transform _leverTransform, Vector3 _directionAxis, float _displacementDistance, float _snappingDistance, int _stepsNumber, float[] _steps)
        {
            m_LeverTransform = _leverTransform;
            // We also cache the lever's parent transform to later make
            // our calculations in its local space
            m_LeverParentTransform = m_LeverTransform.parent;

            m_StartPosition = m_LeverTransform.localPosition;
            m_EndPosition = m_StartPosition + _displacementDistance * _directionAxis;
            m_RailDirection = m_EndPosition - m_StartPosition;

            m_SnappingDistance = _snappingDistance;
            m_StepsNumber = _stepsNumber;
            m_Steps = _steps;
        }

        public int ComputeLeverPosition(Vector3 _controllerWorldPosition)
        {
            Vector3 controllerLocalPosition = m_LeverParentTransform.InverseTransformPoint(_controllerWorldPosition);
            Vector3 moveVector = controllerLocalPosition - m_StartPosition;

            float scalarProjection = Vector3.Dot(moveVector, m_RailDirection.normalized);
            float coefficient = scalarProjection / m_RailDirection.magnitude;

            Vector3 candidatePosition = Vector3.Lerp(m_StartPosition, m_EndPosition, coefficient);

            if (ShouldSnapToPosition(candidatePosition, m_StartPosition))
            {
                MoveTo(m_StartPosition);

                return 0;
            }

            for (int i = 0; i < m_StepsNumber; i++)
            {
                Vector3 position = m_StartPosition + m_RailDirection * (m_Steps[i] / 100.0f);

                if (ShouldSnapToPosition(candidatePosition, position))
                {
                    MoveTo(position);

                    return i + 1;
                }
            }

            if (ShouldSnapToPosition(candidatePosition, m_EndPosition))
            {
                MoveTo(m_EndPosition);

                return Gaze_LeverMechanism.MAX_STEP_NUMBER + 1;
            }

            MoveTo(candidatePosition);

            return -1;
        }

        private bool ShouldSnapToPosition(Vector3 candidatePosition, Vector3 snappingPosition)
        {
            if (Vector3.Distance(candidatePosition, snappingPosition) <= m_SnappingDistance)
            {
                return true;
            }

            return false;
        }

        private void MoveTo(Vector3 position)
        {
            m_LeverTransform.localPosition = position;
        }

#if UNITY_EDITOR
        // This function attempts to draw useful information for debugging
        private void DebugDraw(Vector3 _controllerLocalPosition, float _scalarProjection)
        {
            Vector3 startWorldPosition = m_LeverParentTransform.TransformPoint(m_StartPosition);
            Vector3 endWorldPosition = m_LeverParentTransform.TransformPoint(m_EndPosition);
            Vector3 candidateWorldPosition = m_LeverParentTransform.TransformPoint(m_StartPosition + _scalarProjection * m_RailDirection.normalized);
            Vector3 controllerWorldPosition = m_LeverParentTransform.TransformPoint(_controllerLocalPosition);
            Debug.DrawLine(startWorldPosition, endWorldPosition, Color.blue, Time.deltaTime);
            Debug.DrawLine(startWorldPosition, controllerWorldPosition, Color.red, Time.deltaTime);
            Debug.DrawLine(startWorldPosition, candidateWorldPosition, Color.green, Time.deltaTime);
        }
#endif
    }
}
