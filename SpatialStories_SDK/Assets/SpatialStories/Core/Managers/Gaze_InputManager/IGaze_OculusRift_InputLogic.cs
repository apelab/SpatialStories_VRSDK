using UnityEngine;

namespace Gaze
{
    public interface IGaze_OculusRift_InputLogic
    {
        bool CheckIfControllerConnected();
        void SetOrientation(GameObject _rightHand, GameObject _leftHand);
        void SetPosition(GameObject _rightHand, GameObject _leftHand);
        void Update();
    }
}