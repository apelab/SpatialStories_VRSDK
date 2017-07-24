using UnityEngine;

namespace Gaze
{
    public class Gaze_ViveTeleport : Gaze_GenericTeleport
    {
        private bool touchpadDown;

        public Gaze_ViveTeleport(Gaze_Teleporter _teleporter) : base(_teleporter)
        {
            touchpadDown = false;
        }

        public override void Setup()
        {
            if (teleporter.GetComponentInChildren<Gaze_HandController>().leftHand)
            {
                Gaze_InputManager.OnStickLeftAxisEvent += StickAxisEvent;
                Gaze_InputManager.OnStickLeftDownEvent += OnTouchPadDownEvent;
                Gaze_InputManager.OnStickLeftUpEvent += OnTouchpadUpEvent;

            }
            else
            {
                Gaze_InputManager.OnStickRightAxisEvent += OnStickLeftAxisEvent;
                Gaze_InputManager.OnStickRightDownEvent += OnTouchPadDownEvent;
                Gaze_InputManager.OnStickRightUpEvent += OnTouchpadUpEvent;

            }
        }

        public override void Dispose()
        {
            if (teleporter.GetComponentInChildren<Gaze_HandController>().leftHand)
            {
                Gaze_InputManager.OnStickLeftAxisEvent -= StickAxisEvent;
                Gaze_InputManager.OnStickLeftDownEvent -= OnTouchPadDownEvent;
                Gaze_InputManager.OnStickLeftUpEvent -= OnTouchpadUpEvent;

            }
            else
            {
                Gaze_InputManager.OnStickRightAxisEvent -= OnStickLeftAxisEvent;
                Gaze_InputManager.OnStickRightDownEvent -= OnTouchPadDownEvent;
                Gaze_InputManager.OnStickRightUpEvent -= OnTouchpadUpEvent;

            }
        }

        private void OnTouchPadDownEvent(Gaze_InputEventArgs _args)
        {
            touchpadDown = true;
            Debug.Log("TouchpadDown = True");
        }

        private void OnTouchpadUpEvent(Gaze_InputEventArgs _args)
        {

            Debug.Log("TouchpadDown = False");
            touchpadDown = false;
        }

        private void StickAxisEvent(Gaze_InputEventArgs _e)
        {
            if (!touchpadDown)
                return;

            OnStickLeftAxisEvent(_e);
        }
    }
}
