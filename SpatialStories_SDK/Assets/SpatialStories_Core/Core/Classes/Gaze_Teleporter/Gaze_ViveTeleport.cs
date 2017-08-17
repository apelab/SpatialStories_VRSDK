using UnityEngine;

namespace Gaze
{
    public class Gaze_ViveTeleport : Gaze_TeleportLogic
    {
        private bool touchpadDown;

        /// <summary>
        /// TODO: This will be moved to the InputManger after the release
        /// </summary>
        private const string TUMBSTICK_LEFT = "apelab Thumbstick Left";
        private const string TUMBSTICK_RIGHT = "apelab Thumbstick Right";
        private string buttonToCheck;

        public Gaze_ViveTeleport(Gaze_Teleporter _teleporter) : base(_teleporter)
        {
            touchpadDown = false;
        }

        public override void Setup()
        {
            if (teleporter.GetComponentInChildren<Gaze_HandController>().leftHand)
            {
                Gaze_InputManager.OnStickLeftAxisEvent += OnStickLeftAxisEvent;
                buttonToCheck = TUMBSTICK_LEFT;
            }
            else
            {
                Gaze_InputManager.OnStickRightAxisEvent += OnStickLeftAxisEvent;
                buttonToCheck = TUMBSTICK_RIGHT;
            }
        }

        public override void Dispose()
        {
            if (teleporter.GetComponentInChildren<Gaze_HandController>().leftHand)
            {
                Gaze_InputManager.OnStickLeftAxisEvent -= OnStickLeftAxisEvent;
            }
            else
            {
                Gaze_InputManager.OnStickRightAxisEvent -= OnStickLeftAxisEvent;
            }
        }

        public override void Update()
        {
            if (!Gaze_Teleporter.IsTeleportAllowed)
                return;

            if (Input.GetButtonDown(buttonToCheck))
            {
                touchpadDown = true;
            }

            if (Input.GetButtonUp(buttonToCheck))
            {
                touchpadDown = false;

                if (teleporter._goodSpot)
                {
                    teleporter.Teleport();
                }

                teleporter.DisableTeleport();

                if (teleporter.gyroInstance)
                    teleporter.gyroInstance.SetActive(false);
            }

            if (touchpadDown)
                teleporter.CalculateArc();

        }

        protected void OnStickLeftAxisEvent(Gaze_InputEventArgs e)
        {
            if (!touchpadDown)
                return;

            teleporter.axisValue = e.AxisValue.magnitude;

            //If the teleport is not allowed we need to deactivate it and return
            if (!Gaze_Teleporter.IsTeleportAllowed)
            {
                teleporter.DisableTeleport();
                return;
            }

            if (!teleporter.teleportActive)
                teleporter.EnableTeleport();

            if (teleporter._goodSpot)
            {
                teleporter.gyroInstance.SetActive(true);

                if (teleporter.OrientOnTeleport)
                {
                    teleporter.angle = Mathf.Atan2(e.AxisValue.x, -e.AxisValue.y) * Mathf.Rad2Deg;

                    // angle take hand's rotation into account
                    teleporter.angle += teleporter.transform.eulerAngles.y;
                }
                else
                    teleporter.angle = teleporter.cam.transform.eulerAngles.y;

            }
        }
    }
}

