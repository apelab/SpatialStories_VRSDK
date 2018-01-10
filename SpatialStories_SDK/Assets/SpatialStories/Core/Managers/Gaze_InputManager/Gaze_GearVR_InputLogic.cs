﻿using UnityEngine;
using UnityEngine.VR;

namespace Gaze
{
    public class Gaze_GearVR_InputLogic : Gaze_InputLogic
    {
        private bool isControllerConnected = false;
        private float touchpadNeutralTimeout = .3f;
        private OVRInput.Controller handedRemote;
        private float touchpadX = 0f, touchpadY = 0f;
        private Vector2 touchpadValue = Vector2.zero;
        private float lastTouchpadInputTime;

        Gaze_InputEventArgs eventArgs;
        bool touchpadDown;

        public static Gaze_InputManager.InputEvent OnHomeButtonDown;
        public static Gaze_InputManager.InputEvent OnHomeButtonUp;

        public Gaze_GearVR_InputLogic(Gaze_InputManager _inputManager) : base(_inputManager)
        {
            eventArgs = new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.PAD_RIGHT_TOUCH, touchpadValue);
            CheckIfControllerConnected();
        }

        public static Vector2 SamsungGearVR_TouchpadPos
        {
            get { return OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad); }
        }

        public override void Update()
        {
            CheckTouchpadState();
            HandleTouchPad();
            HandleTrigger();
        }

        private void HandleTrigger()
        {
            // Returns true if the trigger was pressed down this frame
            bool triggerPressedThisFrame = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, handedRemote);

            // Returns true if the trigger was released this frame
            bool triggerReleasedThisFrame = OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, handedRemote);

            // Queries the touchpad position of the GearVR Controller explicitly
            if (triggerPressedThisFrame)
                Gaze_InputManager.FireOnHandRightDownEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.HAND_RIGHT_DOWN, Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_HAND_RIGHT)));

            if (triggerReleasedThisFrame)
                Gaze_InputManager.FireOnHandRightUpEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.HAND_RIGHT_UP));
        }

        private void HandleTouchPad()
        {
            // Touchpad touch
            bool touchPadTouchThisFrame = OVRInput.GetDown(OVRInput.Touch.PrimaryTouchpad, handedRemote);
            if (touchPadTouchThisFrame)
            {
                Gaze_InputManager.FireStickRightAxisEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.PAD_RIGHT_TOUCH, touchpadValue));
                touchpadDown = true;
            }

            // Touchpad Released
            bool touchPadUpThisFrame = OVRInput.GetUp(OVRInput.Touch.PrimaryTouchpad, handedRemote); ;
            if (touchPadUpThisFrame)
            {
                Gaze_InputManager.FireStickRightAxisEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.PAD_RIGHT_UNTOUCH, Vector2.zero));
                touchpadDown = false;
            }

            // Returns true if the touchpad is currently pressed down
            bool touchpadPressedThisFrame = OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad, handedRemote);
            if (touchpadPressedThisFrame)
            {
                Gaze_InputManager.FireOnButtonAEvent(new Gaze_InputEventArgs(this, Gaze_InputTypes.STICK_RIGHT_DOWN));
                inputManager.IsRightStickDown = true;
            }

            // Returns true if the touchpad is currently pressed down
            bool touchpadPressed = OVRInput.Get(OVRInput.Button.PrimaryTouchpad, handedRemote);
            if (touchpadPressed)
            {
                Gaze_InputManager.FireOnButtonAEvent(new Gaze_InputEventArgs(this, Gaze_InputTypes.STICK_RIGHT));
            }

            // Returns true if the touchpad was released this frame
            bool touchpadReleasedThisFrame = OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad, handedRemote);
            if (touchpadReleasedThisFrame)
            {
                Gaze_InputManager.FireOnOnButtonAUpEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.STICK_RIGHT_UP, Vector2.zero));
                inputManager.IsRightStickDown = false;
            }

            if (touchpadDown)
            {
                eventArgs.AxisValue = touchpadValue;
                inputManager.StickRightAxisEvent(eventArgs);
            }
        }

        public override bool CheckIfControllerConnected()
        {
            var actualHandedRemote = OVRInput.GetConnectedControllers() & OVRInput.Controller.RTrackedRemote;

            // if not, check if we have a left handed remote
            if (actualHandedRemote == 0)
            {
                actualHandedRemote = OVRInput.GetConnectedControllers() & OVRInput.Controller.LTrackedRemote;
            }
            
#if UNITY_EDITOR
            // Use Touch controller in editor
            actualHandedRemote = OVRInput.GetConnectedControllers() & OVRInput.Controller.RTouch;
#endif

            if (actualHandedRemote == handedRemote)
                return isControllerConnected;

            if (actualHandedRemote == OVRInput.Controller.None)
            {
                isControllerConnected = false;
            }
            else
            {
                isControllerConnected = true;
            }

            handedRemote = actualHandedRemote;

            return isControllerConnected;
        }

        private void CheckTouchpadState()
        {
            touchpadValue = SamsungGearVR_TouchpadPos;
        }

        public override void SetPosition(GameObject _rightHand, GameObject _leftHand)
        {
            _rightHand.transform.localPosition = handedRemote == OVRInput.Controller.RTrackedRemote ? UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.RightHand) : UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.LeftHand);
        }

        // Hand position is only set in right hand object
        public override void SetOrientation(GameObject _rightHand, GameObject _leftHand)
        {
            inputManager.FixedRightPosition.localPosition = handedRemote == OVRInput.Controller.RTrackedRemote ? inputManager.OriginalRightHandFixedPosition : inputManager.FixedLeftPosition.localPosition;
            _rightHand.transform.localRotation = handedRemote == OVRInput.Controller.RTrackedRemote ? OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote) : OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTrackedRemote);
        }
    }
}
