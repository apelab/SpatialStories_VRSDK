using Gaze;
using System;
using System.Collections;
using UnityEngine;

public class CA_Vibrate : Gaze_AbstractBehaviour
{
    [Header("General Setup: ")]
    public bool IsRightHand = false;

    [Header("Oculus Setup: ")]
    public AudioClip OculusHaptic;

    [Header("Vive Setup: ")]
    public float ViveDuration = 0.5f;
    public ushort ViveStrength = 100;

    protected override void OnTrigger()
    {
        switch (Gaze_InputManager.instance.CurrentController)
        {
            case Gaze_Controllers.NOT_DETERMINED:
                break;
            case Gaze_Controllers.OCULUS_RIFT:
                OVRHapticsClip haptic = new OVRHapticsClip(OculusHaptic);
                if (IsRightHand)
                    OVRHaptics.RightChannel.Preempt(haptic);
                else
                    OVRHaptics.LeftChannel.Preempt(haptic);
                break;
            case Gaze_Controllers.HTC_VIVE:
                Debug.Log("Vive Haptics not implemented!");
                break;
            case Gaze_Controllers.GEARVR_CONTROLLER:
                break;
        }
    }
}

