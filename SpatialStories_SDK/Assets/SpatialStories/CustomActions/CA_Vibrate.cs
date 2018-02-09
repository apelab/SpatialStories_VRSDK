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
                StartCoroutine(LongVibration(ViveDuration, ViveStrength));
                break;
            case Gaze_Controllers.GEARVR_CONTROLLER:
                break;
        }
    }

    private IEnumerator LongVibration(float length, float strength)
    {
        var deviceIndex = SteamVR_Controller.GetDeviceIndex(IsRightHand ? SteamVR_Controller.DeviceRelation.Rightmost : SteamVR_Controller.DeviceRelation.Leftmost);
        for (float i = 0; i < length; i += Time.deltaTime)
        {
            SteamVR_Controller.Input(deviceIndex).TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
            yield return null;
        }
    }
}

