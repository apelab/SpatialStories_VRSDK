using UnityEngine;

namespace Gaze
{
    public abstract class Gaze_InputLogic
    {
        protected Gaze_InputManager inputManager;

        public Gaze_InputLogic(Gaze_InputManager _inputManager)
        {
            inputManager = _inputManager;
        }

        public abstract void Update();
        public abstract bool CheckIfControllerConnected();

        public abstract void SetOrientation(GameObject _rightHand, GameObject _leftHand);
        public abstract void SetPosition(GameObject _rightHand, GameObject _leftHand);
    }
}
