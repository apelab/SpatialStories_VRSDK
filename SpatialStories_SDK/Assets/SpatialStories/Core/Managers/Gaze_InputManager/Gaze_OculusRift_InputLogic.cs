using UnityEngine;

namespace Gaze
{
    public class Gaze_OculusRift_InputLogic : Gaze_InputLogic
    {
        private bool isControllerConnected = false;
        private float touchpadNeutralTimeout = .3f;
        private OVRInput.Controller handedRemote;


        private float lStickX = 0f, lStickY = 0f;
        private float rStickX = 0f, rStickY = 0f;
        private Vector2 leftStick = Vector2.zero;
        private Vector2 rightStick = Vector2.zero;

        private float lastLeftStickInputTime;
        private float lastRightStickInputTime;


        public Gaze_OculusRift_InputLogic(Gaze_InputManager _inputManager) : base(_inputManager)
        {
            CheckIfControllerConnected();
        }

        public static Vector2 leftStickPosition
        {
            get { return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch); }
        }

        public static Vector2 rightStickPosition
        {
            get { return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch); }
        }

        public override void Update()
        {

            // CHechk if the buttons were touched
            bool buttonAWasTouched = OVRInput.Get(OVRInput.Touch.One, OVRInput.Controller.RTouch);
            bool buttonBWasTouched = OVRInput.Get(OVRInput.Touch.Two, OVRInput.Controller.RTouch);
            bool buttonXWasTouched = OVRInput.Get(OVRInput.Touch.One, OVRInput.Controller.LTouch);
            bool buttonYWasTouched = OVRInput.Get(OVRInput.Touch.Two, OVRInput.Controller.LTouch);
            // Check if the buttons were untouched
            bool buttonAWasUntouched = OVRInput.GetUp(OVRInput.Touch.One, OVRInput.Controller.RTouch);
            bool buttonBWasUntouched = OVRInput.GetUp(OVRInput.Touch.Two, OVRInput.Controller.RTouch);
            bool buttonXWasUntouched = OVRInput.GetUp(OVRInput.Touch.One, OVRInput.Controller.LTouch);
            bool buttonYWasUntouched = OVRInput.GetUp(OVRInput.Touch.Two, OVRInput.Controller.LTouch);

            // Check if the triggers were touched
            // Right hand
            bool rightPrimaryIndexWasTouched = OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
            bool rightPrimaryThumbrestWasTouched = OVRInput.Get(OVRInput.Touch.PrimaryThumbRest, OVRInput.Controller.RTouch);
            bool rightPrimaryThumbstickWasTouched = OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, OVRInput.Controller.RTouch);
            // Left hand
            bool leftPrimaryIndexWasTouched = OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
            bool leftPrimaryThumbrestWasTouched = OVRInput.Get(OVRInput.Touch.PrimaryThumbRest, OVRInput.Controller.LTouch);
            bool lefttPrimaryThumbstickWasTouched = OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, OVRInput.Controller.LTouch);

            // Check if the triggers were untouched
            // Right Hand
            bool rightPrimaryIndexWasUntouched = OVRInput.GetUp(OVRInput.Touch.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
            bool rightPrimaryThumbrestWasUntouched = OVRInput.GetUp(OVRInput.Touch.PrimaryThumbRest, OVRInput.Controller.RTouch);
            bool rightPrimaryThumbstickWasUntouched = OVRInput.GetUp(OVRInput.Touch.PrimaryThumbstick, OVRInput.Controller.RTouch);

            // Left hand
            bool leftPrimaryIndexWasUntouched = OVRInput.GetUp(OVRInput.Touch.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
            bool leftPrimaryThumbrestWasUntouched = OVRInput.GetUp(OVRInput.Touch.PrimaryThumbRest, OVRInput.Controller.LTouch);
            bool leftPrimaryThumbstickWasUntouched = OVRInput.GetUp(OVRInput.Touch.PrimaryThumbstick, OVRInput.Controller.LTouch);

            //===========================================================================================//
            //
            // When buttons get touched fire the event
            // Touch
            if (buttonBWasTouched)
                Gaze_InputManager.FireOnButtonBTouchEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.B_BUTTON_TOUCH));
            if (buttonBWasUntouched)
                Gaze_InputManager.FireOnButtonBUntouchEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.B_BUTTON_UNTOUCH));
            if (buttonAWasTouched)
                Gaze_InputManager.FireOnButtonATouchEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.A_BUTTON_TOUCH));
            if (buttonXWasTouched)
                Gaze_InputManager.FireOnButtonXTouchEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.LeftHand, Gaze_InputTypes.X_BUTTON_TOUCH));
            if (buttonYWasTouched)
                Gaze_InputManager.FireOnButtonYTouchEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.LeftHand, Gaze_InputTypes.Y_BUTTON_TOUCH));

            // Untouch
            if (buttonAWasUntouched)
                Gaze_InputManager.FireOnButtonAUntouchEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.A_BUTTON_UNTOUCH));
            if (buttonBWasUntouched)
                Gaze_InputManager.FireOnButtonBUntouchEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.B_BUTTON_UNTOUCH));
            if (buttonXWasUntouched)
                Gaze_InputManager.FireOnButtonXUntouchEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.X_BUTTON_UNTOUCH));
            if (buttonYWasUntouched)
                Gaze_InputManager.FireOnButtonYUntouchEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.Y_BUTTON_UNTOUCH));

            // When triggers get touched fire the event
            // Touch
            if (rightPrimaryIndexWasTouched)
                Gaze_InputManager.FireOnButtonRightIndexTouch(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.INDEX_RIGHT_TOUCH));
            if (rightPrimaryThumbrestWasTouched)
                Gaze_InputManager.FireOnButtonRightThumbrestTouch(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.THUMBREST_RIGHT_TOUCH));
            if (rightPrimaryThumbstickWasTouched)
                Gaze_InputManager.FireOnButtonRightThumbstickTouch(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.STICK_RIGHT_TOUCH));
            if (leftPrimaryIndexWasTouched)
                Gaze_InputManager.FireOnButtonLeftIndexTouch(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.INDEX_LEFT_TOUCH));
            if (leftPrimaryThumbrestWasTouched)
                Gaze_InputManager.FireOnButtonLeftThumbrestTouch(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.THUMBREST_LEFT_TOUCH));
            if (lefttPrimaryThumbstickWasTouched)
                Gaze_InputManager.FireOnButtonLeftThumbstickTouch(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.STICK_LEFT_TOUCH));

            // Untouch
            if (rightPrimaryIndexWasUntouched)
                Gaze_InputManager.FireOnButtonRightIndexUntouch(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.INDEX_RIGHT_UNTOUCH));
            if (rightPrimaryThumbrestWasUntouched)
                Gaze_InputManager.FireOnButtonRightThumbrestUntouch(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.THUMBREST_RIGHT_UNTOUCH));
            if (rightPrimaryThumbstickWasUntouched)
                Gaze_InputManager.FireOnButtonRightThumbstickUntouch(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.STICK_RIGHT_UNTOUCH));
            if (leftPrimaryIndexWasUntouched)
                Gaze_InputManager.FireOnButtonLeftIndexUntouch(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.INDEX_LEFT_UNTOUCH));
            if (leftPrimaryThumbrestWasUntouched)
                Gaze_InputManager.FireOnButtonLeftThumbrestUntouch(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.THUMBREST_LEFT_UNTOUCH));
            if (leftPrimaryThumbstickWasUntouched)
                Gaze_InputManager.FireOnButtonLeftThumbstickUntouch(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.STICK_LEFT_UNTOUCH));
        }

        public override bool CheckIfControllerConnected()
        {
            // TODO (Giuseppe)
            return true;
        }

        public override void SetOrientation(GameObject _rightHand, GameObject _leftHand)
        {
            // TODO (Giuseppe)
        }

        public override void SetPosition(GameObject _rightHand, GameObject _leftHand)
        {
            // TODO (Giuseppe)
        }
    }
}


