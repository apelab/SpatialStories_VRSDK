using UnityEngine;

namespace Gaze
{
    /// <summary>
    /// Script component which adds a lever mechanism to the InteractiveObject it is attached to.
    /// Once attached, the InteractiveObject can be grabbed and dragged following a set of rules
    /// depending on which type of lever was chosen
    /// </summary>
    public class Gaze_LeverMechanism : MonoBehaviour
    {
        /// <summary>
        /// Handy class to retrieve and hold information when grabbing
        /// </summary>
        class GrabbingData
        {
            public GrabbingData(bool _isRightController, Vector3 _grabbedObjectPosition)
            {
                if (_isRightController)
                {
                    controllerTransform = Gaze_InputManager.instance.RightController.transform;
                }
                else
                {
                    controllerTransform = Gaze_InputManager.instance.LeftController.transform;
                }

                grabbingPoint = controllerTransform.position;
                objectPositionWhenGrabbing = _grabbedObjectPosition;

                handTransform = controllerTransform.Find("Visuals/HandModels/Hand");
                handParent = handTransform.parent;

                originalHandLocalPosition = handTransform.localPosition;
                originalHandLocalScale = handTransform.localScale;
                originalHandLocalRotation = handTransform.localRotation;
            }

            public readonly Vector3 grabbingPoint;
            public readonly Transform controllerTransform;
            public readonly Transform handTransform;
            public readonly Transform handParent;
            public readonly Vector3 objectPositionWhenGrabbing;
            public readonly Vector3 originalHandLocalPosition;
            public readonly Quaternion originalHandLocalRotation;
            public readonly Vector3 originalHandLocalScale;
        }

        private GrabbingData m_GrabbingData;

        public delegate void OnLeverValidatedAction(int _stepNumber);
        /// <summary>
        /// An event sent out by the lever mechanism when the lever
        /// either reached the end or any step in between
        /// </summary>
        public OnLeverValidatedAction LeverValidatedEvent;

        public static readonly int MAX_STEP_NUMBER = 5;

        public enum LeverType
        {
            Linear,
            Rotational
        }

        public LeverType leverType;

        public enum Axis
        {
            XAxis,
            YAxis,
            ZAxis
        }

        // Linear
        public Axis directionAxis;
        public float displacementDistance;
        public float snappingDistance = 0.05f;

        // Rotational
        public Axis rotationAxis;
        public Transform rotationPoint;
        public float maxLeverAngle;
        public float snappingAngleDelta;

        // Steps, for both linear and rotational levers
        public int stepsNumber;
        public float[] steps = new float[MAX_STEP_NUMBER];

        private Gaze_InteractiveObject m_InteractiveObject;

        private ILeverMechanism m_Lever;

        private enum GrabbingState
        {
            Idle,
            Started,
            Continues,
            Stopped
        }

        private GrabbingState m_GrabbingState = GrabbingState.Idle;

        private bool m_RightPressed = false;
        private bool m_RightColliding = false;
        private bool m_LeftPressed = false;
        private bool m_LeftColliding = false;
        private bool m_RightPressedPreviousState = false;
        private bool m_RightCollidingPreviousState = false;
        private bool m_LeftPressedPreviousState = false;
        private bool m_LeftCollidingPreviousState = false;

        private void OnEnable()
        {
            m_InteractiveObject = GetComponent<Gaze_InteractiveObject>();

            Gaze_EventManager.OnHandleEvent += OnManipulateCollision;

            if (Gaze_InputManager.instance.CurrentController != Gaze_Controllers.HTC_VIVE)
            {
                Gaze_InputManager.OnHandRightDownEvent += OnHandRightDown;
                Gaze_InputManager.OnHandRightUpEvent += OnHandRightUp;
                Gaze_InputManager.OnHandLeftDownEvent += OnHandLeftDown;
                Gaze_InputManager.OnHandLeftUpEvent += OnHandLeftUp;
            }
            else
            {
                Gaze_InputManager.OnIndexRightDownEvent += OnIndexRightDown;
                Gaze_InputManager.OnIndexRightUpEvent += OnIndexRightUp;
                Gaze_InputManager.OnIndexLeftDownEvent += OnIndexLeftDown;
                Gaze_InputManager.OnIndexLeftUpEvent += OnIndexLeftUp;
            }
        }

        private void OnDisable()
        {
            Gaze_EventManager.OnHandleEvent -= OnManipulateCollision;

            if (Gaze_InputManager.instance.CurrentController != Gaze_Controllers.HTC_VIVE)
            {
                Gaze_InputManager.OnHandRightDownEvent -= OnHandRightDown;
                Gaze_InputManager.OnHandRightUpEvent -= OnHandRightUp;
                Gaze_InputManager.OnHandLeftDownEvent -= OnHandLeftDown;
                Gaze_InputManager.OnHandLeftUpEvent -= OnHandLeftUp;
            }
            else
            {
                Gaze_InputManager.OnIndexRightDownEvent -= OnIndexRightDown;
                Gaze_InputManager.OnIndexRightUpEvent -= OnIndexRightUp;
                Gaze_InputManager.OnIndexLeftDownEvent -= OnIndexLeftDown;
                Gaze_InputManager.OnIndexLeftUpEvent -= OnIndexLeftUp;
            }
        }

        void Start()
        {
            m_InteractiveObject = GetComponent<Gaze_InteractiveObject>();

            switch (leverType)
            {
                case LeverType.Linear:
                    m_Lever = new Gaze_LinearLeverMechanism(m_InteractiveObject.transform, AxisEnumToVector3(directionAxis), displacementDistance, snappingDistance, stepsNumber, steps);
                    break;
                case LeverType.Rotational:
                    m_Lever = new Gaze_RotationalLeverMechanism(m_InteractiveObject.transform, AxisEnumToVector3(rotationAxis), rotationPoint.position, maxLeverAngle, snappingAngleDelta, stepsNumber, steps);
                    break;
            }
        }

        void Update()
        {
            UpdateGrabbingState();

            switch(m_GrabbingState)
            {
                case GrabbingState.Idle: break;
                case GrabbingState.Started:
                    OnLeverGrabbingStart();
                    break;
                case GrabbingState.Continues:
                    OnLeverGrabbingContinue();
                    break;
                case GrabbingState.Stopped:
                    OnLeverGrabbingStop();
                    break;
            }

            m_LeftPressedPreviousState = m_LeftPressed;
            m_RightPressedPreviousState = m_RightPressed;
            m_RightCollidingPreviousState = m_RightColliding;
            m_LeftCollidingPreviousState = m_LeftColliding;
        }

        void OnLeverGrabbingStart()
        {
            Transform handTransform = m_GrabbingData.handTransform;
            handTransform.position = transform.position;
            handTransform.SetParent(transform);
        }

        void OnLeverGrabbingContinue()
        {
            int validationResult = m_Lever.ComputeLeverPosition(m_GrabbingData.objectPositionWhenGrabbing + (m_GrabbingData.controllerTransform.position - m_GrabbingData.grabbingPoint));
            if (validationResult != -1)
            {
                LeverValidatedEvent(validationResult);
            }
        }

        void OnLeverGrabbingStop()
        {
            Transform handTransform = m_GrabbingData.handTransform;
            handTransform.SetParent(m_GrabbingData.handParent);
            handTransform.localPosition = m_GrabbingData.originalHandLocalPosition;
            handTransform.localScale = m_GrabbingData.originalHandLocalScale;
            handTransform.localRotation = m_GrabbingData.originalHandLocalRotation;
        }

        private void UpdateGrabbingState()
        {
            if (m_GrabbingState == GrabbingState.Idle &&
                (m_RightColliding) &&
                 (m_RightPressed && !m_RightPressedPreviousState) )
            {
                m_GrabbingState = GrabbingState.Started;
                m_GrabbingData = new GrabbingData(_isRightController: true, _grabbedObjectPosition: m_InteractiveObject.transform.position);

                return;
            }

            if (m_GrabbingState == GrabbingState.Idle &&
                (m_LeftColliding) &&
                 (m_LeftPressed && !m_LeftPressedPreviousState))
            {
                m_GrabbingState = GrabbingState.Started;
                m_GrabbingData = new GrabbingData(_isRightController: false, _grabbedObjectPosition: m_InteractiveObject.transform.position);

                return;
            }

            if (m_GrabbingState == GrabbingState.Started &&
                (m_RightColliding) &&
                 (m_RightPressed))
            {
                m_GrabbingState = GrabbingState.Continues;

                return;
            }

            if (m_GrabbingState == GrabbingState.Started &&
                (m_LeftColliding && m_LeftCollidingPreviousState) &&
                 (m_LeftPressed && m_LeftPressedPreviousState))
            {
                m_GrabbingState = GrabbingState.Continues;

                return;
            }

            if (m_GrabbingState == GrabbingState.Continues &&
                (!m_RightPressed && m_RightPressedPreviousState))
            {
                m_GrabbingState = GrabbingState.Stopped;

                return;
            }

            if (m_GrabbingState == GrabbingState.Continues &&
                (!m_LeftPressed && m_LeftPressedPreviousState))
            {
                m_GrabbingState = GrabbingState.Stopped;

                return;
            }

            if (m_GrabbingState == GrabbingState.Stopped)
            {
                m_GrabbingState = GrabbingState.Idle;
                m_GrabbingData = null;
            }
        }

        void OnManipulateCollision(Gaze_HandleEventArgs e)
        {
            bool rightController = e.Other.Equals(Gaze_InputManager.instance.RightController.GetComponentInChildren<Gaze_Manipulation>().GetComponent<Collider>());
            bool leftController = e.Other.Equals(Gaze_InputManager.instance.LeftController.GetComponentInChildren<Gaze_Manipulation>().GetComponent<Collider>());
            if (!rightController &&
                !leftController)
            {
                return;
            }

            if (!e.Sender.Equals(GetComponentInChildren<Gaze_Manipulation>()))
            {
                return;
            }

            if (rightController)
            {
                m_RightColliding = e.IsColliding;
            }
            else
            {
                m_LeftColliding = e.IsColliding;
            }
        }

        void OnHandRightDown(Gaze_InputEventArgs args)
        {
            m_RightPressed = true;
        }

        void OnHandRightUp(Gaze_InputEventArgs args)
        {
            m_RightPressed = false;
        }

        void OnHandLeftDown(Gaze_InputEventArgs args)
        {
            m_LeftPressed = true;
        }

        void OnHandLeftUp(Gaze_InputEventArgs args)
        {
            m_LeftPressed = false;
        }

        void OnIndexRightDown(Gaze_InputEventArgs args)
        {
            m_RightPressed = true;
        }

        void OnIndexRightUp(Gaze_InputEventArgs args)
        {
            m_RightPressed = false;
        }

        void OnIndexLeftDown(Gaze_InputEventArgs args)
        {
            m_LeftPressed = true;
        }

        void OnIndexLeftUp(Gaze_InputEventArgs args)
        {
            m_LeftPressed = false;
        }

        /// <summary>
        /// Converts an axis described by the enumerator Axis into a usable Vector3
        /// </summary>
        /// <param name="axis">Either XAxis, YAxis or ZAxis</param>
        /// <returns>The axis as a Vector3</returns>
        public static Vector3 AxisEnumToVector3(Axis axis)
        {
            switch (axis)
            {
                default:
                case Axis.XAxis:
                    return Vector3.right;
                case Axis.YAxis:
                    return Vector3.up;
                case Axis.ZAxis:
                    return Vector3.forward;
            }
        }
    }
}
