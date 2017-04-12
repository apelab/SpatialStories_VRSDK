﻿using UnityEngine;

namespace Gaze
{
    public class Gaze_GenericTeleport : Gaze_TeleportLogic
    {
        public Gaze_GenericTeleport(Gaze_Teleporter _teleporter) : base(_teleporter)
        {

        }

        public override void Setup()
        {
            if (teleporter.GetComponentInChildren<Gaze_HandController>().leftHand)
            {
                Gaze_InputManager.OnStickLeftAxisEvent += OnStickLeftAxisEvent;

            }
            else
            {
                Gaze_InputManager.OnStickRightAxisEvent += OnStickLeftAxisEvent;

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
            if (teleporter.axisValue > teleporter.inputThreshold)
                teleporter.CalculateArc();
        }

        private void OnStickLeftAxisEvent(Gaze_InputEventArgs e)
        {

            teleporter.axisValue = e.AxisValue.magnitude;

            //If the teleport is not allowed we need to deactivate it and return
            if (!Gaze_Teleporter.IsTeleportAllowed)
            {
                teleporter.DisableTeleport();
                return;
            }

            if (!teleporter.teleportActive && teleporter.IsInputValid())
                teleporter.EnableTeleport();

            if (teleporter._goodSpot)
            {
                teleporter.gyroInstance.SetActive(true);
                teleporter.angle = Mathf.Atan2(e.AxisValue.x, -e.AxisValue.y) * Mathf.Rad2Deg;

                // angle take hand's rotation into account
                teleporter.angle += teleporter.transform.eulerAngles.y;

            }
            else
            {
                if (teleporter.gyroInstance)
                    teleporter.gyroInstance.SetActive(false);
            }

            // if stick is released
            if (e.AxisValue == Vector2.zero)
            {
                if (teleporter._goodSpot)
                {
                    teleporter.Teleport();
                }

                teleporter.DisableTeleport();
            }
        }
    }
}
