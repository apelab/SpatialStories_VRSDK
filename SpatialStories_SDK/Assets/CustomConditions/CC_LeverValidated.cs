using System;
using UnityEngine;

namespace Gaze
{
    /// <summary>
    /// Custon condition that is triggered when a Gaze_LeverMechanism either reaches its end position
    /// or any of its steps position
    /// </summary>
    public class CC_LeverValidated : Gaze_AbstractConditions
    {
        /// <summary>
        /// If true, the condition will be validated on end regardless of any steps.
        /// If false, the condition will be validated on step number stepToValidate
        /// </summary>
        public bool ValidateOnEnd = true;
        /// <summary>
        /// The step number that, when reached, will validate this condition
        /// </summary>
        public int StepToValidate = Gaze_LeverMechanism.MAX_STEP_NUMBER + 1;

        private Gaze_LeverMechanism m_LeverMechanism;

        private void OnEnable()
        {
            m_LeverMechanism.LeverValidatedEvent += OnLeverValidated;
        }

        private void OnDisable()
        {
            m_LeverMechanism.LeverValidatedEvent -= OnLeverValidated;
        }

        private void Awake()
        {
            m_LeverMechanism = GetComponentInParent<Gaze_LeverMechanism>();

            if (m_LeverMechanism == null)
            {
                throw new NullReferenceException("A lever validated custom condition must have a Gaze_LeverMechanism attached to the IO");
            }
        }

        private void OnLeverValidated(int _stepNumber)
        {
            if ( (_stepNumber == (Gaze_LeverMechanism.MAX_STEP_NUMBER + 1) && ValidateOnEnd) ||
                 (_stepNumber == StepToValidate && !ValidateOnEnd))
            {
                ValidateCustomCondition(true);
            }
        }
    }
}
