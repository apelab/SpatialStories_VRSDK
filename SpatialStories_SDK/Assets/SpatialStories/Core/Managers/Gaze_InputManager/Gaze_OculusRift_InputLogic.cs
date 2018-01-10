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


            bool buttonAWasTouched = OVRInput.Get(OVRInput.Touch.One, OVRInput.Controller.RTouch);
            bool buttonBWasTouched = OVRInput.Get(OVRInput.Touch.Two, OVRInput.Controller.RTouch);
            bool buttonXWasTouched = OVRInput.Get(OVRInput.Touch.One, OVRInput.Controller.LTouch);
            bool buttonYWasTouched = OVRInput.Get(OVRInput.Touch.Two, OVRInput.Controller.LTouch);

            bool rightPrimaryIndexWasTouched = OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
            bool rightPrimaryThumbrestWasTouched = OVRInput.Get(OVRInput.Touch.PrimaryThumbRest, OVRInput.Controller.RTouch);
            bool rightPrimaryThumbstickWasTouched = OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, OVRInput.Controller.RTouch);

            bool leftPrimaryIndexWasTouched = OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
            bool leftPrimaryThumbrestWasTouched = OVRInput.Get(OVRInput.Touch.PrimaryThumbRest, OVRInput.Controller.LTouch);
            bool lefttPrimaryThumbstickWasTouched = OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, OVRInput.Controller.LTouch);

            //===========================================================================================//

            // Queries the touchpad position of the GearVR Controller explicitly
            if (buttonBWasTouched)
                Gaze_InputManager.FireOnButtonBTouchEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.B_BUTTON_TOUCH));
            if (buttonAWasTouched)
                Gaze_InputManager.FireOnButtonATouchEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.RightHand, Gaze_InputTypes.A_BUTTON_TOUCH));
            if (buttonXWasTouched)
                Gaze_InputManager.FireOnButtonXTouchEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.LeftHand, Gaze_InputTypes.X_BUTTON_TOUCH));
            if (buttonYWasTouched)
                Gaze_InputManager.FireOnButtonYTouchEvent(new Gaze_InputEventArgs(this, UnityEngine.XR.XRNode.LeftHand, Gaze_InputTypes.Y_BUTTON_TOUCH));


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


