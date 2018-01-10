﻿using UnityEngine;


namespace Gaze
{
    public class Gaze_OculusRift_InputLogic : Gaze_InputLogic
    {
        private bool isControllerConnected = false;
        private float touchpadNeutralTimeout = .3f;
        private OVRInput.Controller handedRemote;


        private float touchpadX = 0f, touchpadY = 0f;
        private Vector2 touchpadValue = Vector2.zero;
        private float lastTouchpadInputTime;

        public Gaze_OculusRift_InputLogic(Gaze_InputManager _inputManager) : base(_inputManager)
        {
            CheckIfControllerConnected();
        }

        public static Vector2 SamsungGearVR_TouchpadPos
        {
            get { return OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad); }
        }

        public override void Update()
        {

            CheckTouchpadState();

            // Returns true if the Index trigger was pressed
            bool rightIndexTriggerPressedThisFrame = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
            bool leftIndexTriggerPressedThisFrame = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch);

            // Returns true if the hand trigger was pressed
            bool rightHandTriggerPressedThisFrame = OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch);
            bool leftHandTriggerPressedThisFrame = OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch);


            // Return true if the correspective button was pressed
            bool buttonAPressedThisFrame = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch);
            bool buttonBPressedThisFrame = OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch);
            bool buttonXPressedThisFrame = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch);
            bool buttonYPressedThisFrame = OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch);




            //===========================================================================================//

            // Returns true if the trigger was pressed down this frame
            bool triggerPressedThisFrame = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, handedRemote);

            // Returns true if the trigger was released this frame
            bool triggerReleasedThisFrame = OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, handedRemote);

            // Returns true if the touchpad is currently pressed down
            bool touchpadPressed = OVRInput.Get(OVRInput.Button.PrimaryTouchpad, handedRemote);

            // Returns true if the touchpad was released this frame
            bool touchpadReleasedThisFrame = OVRInput.GetUp(OVRInput.Button.PrimaryTouchpad, handedRemote);

            // Queries the touchpad position of the GearVR Controller explicitly
            if (triggerPressedThisFrame)
                Gaze_InputManager.FireOnHandRightDownEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.HAND_RIGHT_DOWN, Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_HAND_RIGHT)));

            if (triggerReleasedThisFrame)
                Gaze_InputManager.FireOnHandRightUpEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.HAND_RIGHT_UP));

            if (touchpadPressed)
            {
                if (Gaze_InputManager.instance.debug)
                    Debug.Log("A Button");
                Gaze_InputManager.FireOnButtonAEvent(new Gaze_InputEventArgs(this, Gaze_InputTypes.A_BUTTON));
            }
            if (triggerPressedThisFrame)
            {
                if (Gaze_InputManager.instance.debug)
                    Debug.Log("A Button Down");
                Gaze_InputManager.FireOnButtonADownEvent(new Gaze_InputEventArgs(this, Gaze_InputTypes.A_BUTTON_DOWN));
            }
            if (touchpadReleasedThisFrame)
            {
                if (Gaze_InputManager.instance.debug)
                    Debug.Log("A Button Up");
                Gaze_InputManager.FireOnOnButtonAUpEvent(new Gaze_InputEventArgs(this, Gaze_InputTypes.A_BUTTON_UP));
            }
        }

        private void CheckTouchpadState()
        {
            // track touchpad position to fire event if any change
            touchpadValue = SamsungGearVR_TouchpadPos;
            if (touchpadValue.x != touchpadX || touchpadValue.y != touchpadY)
            {
                // notify touch event as a Generic Axis event
                Gaze_InputManager.FireStickRightAxisEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.PAD_RIGHT_TOUCH, touchpadValue));

                // update values
                touchpadX = touchpadValue.x;
                touchpadY = touchpadValue.y;

                lastTouchpadInputTime = Time.time;
            }

            //detect when pad NEUTRAL and fire event with FireRightTouchpadEvent
            if (Time.time - lastTouchpadInputTime > touchpadNeutralTimeout)
            {
                Gaze_InputManager.FireRightTouchpadEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.PAD_RIGHT_TOUCH, Vector2.zero));
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


            //#if UNITY_EDITOR

            //            // Use Touch controller in editor
            //            actualHandedRemote = OVRInput.GetConnectedControllers() & OVRInput.Controller.RTouch & OVRInput.Controller.LTouch;
            //#endif

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

        public override void SetPosition(GameObject _rightHand, GameObject _leftHand)
        {
            _rightHand.transform.localPosition = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.RightHand);
            _leftHand.transform.localPosition = UnityEngine.XR.InputTracking.GetLocalPosition(UnityEngine.XR.XRNode.LeftHand);
        }


        public override void SetOrientation(GameObject _rightHand, GameObject _leftHand)
        {
            inputManager.FixedRightPosition.localPosition = inputManager.OriginalRightHandFixedPosition;
            inputManager.FixedLeftPosition.localPosition = inputManager.FixedLeftPosition.localPosition;
            _rightHand.transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote);
            _leftHand.transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTrackedRemote);
        }
    }
}


