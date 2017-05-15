using UnityEngine;

namespace Gaze
{
    public class Gaze_GearVrTeleport : Gaze_TeleportLogic
    {
        public Gaze_GearVrTeleport(Gaze_Teleporter _teleporter) : base(_teleporter)
        {

        }

        public override void Setup()
        {
            Gaze_InputManager.OnButtonAEvent += OnButtonAEvent;
            Gaze_InputManager.OnButtonAUpEvent += OnButtonAUpEvent;
        }

        public override void Dispose()
        {
            Gaze_InputManager.OnButtonAEvent -= OnButtonAEvent;
            Gaze_InputManager.OnButtonAUpEvent -= OnButtonAUpEvent;
        }

        public override void Update()
        {
            teleporter.CalculateArc();
        }

        private void OnButtonAEvent(Gaze_InputEventArgs e)
        {
            if (e.InputType != Gaze_InputTypes.A_BUTTON)
                return;

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
                teleporter.angle = Mathf.Atan2(Gaze_GearVR_InputLogic.SamsungGearVR_TouchpadPos.x, Gaze_GearVR_InputLogic.SamsungGearVR_TouchpadPos.y) * Mathf.Rad2Deg;

                // angle take hand's rotation into account
                teleporter.angle += teleporter.transform.eulerAngles.y;
            }
            else
                teleporter.gyroInstance.SetActive(false);
        }

        private void OnButtonAUpEvent(Gaze_InputEventArgs e)
        {
            if (e.InputType != Gaze_InputTypes.A_BUTTON_UP)
                return;

            if (teleporter._goodSpot)
            {
                teleporter.Teleport();
            }

            teleporter.DisableTeleport();
        }

    }
}
