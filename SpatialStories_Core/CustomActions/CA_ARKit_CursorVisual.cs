using System;
using UnityEngine;

namespace Gaze
{
    public class CA_ARKit_CursorVisual : Gaze_AbstractBehaviour
    {
        public enum CursorType { OVER_NOTHING, OVER_OBJECT, OVER_PLANE }

        public CursorType cursorType;
        public GameObject NewVisualPrefab;

        ARKitCursor cursorToChange;

        private void Start()
        {
            cursorToChange = FindObjectOfType<ARKitCursor>();
            if (cursorToChange == null)
                Debug.LogError("No cursor found on the scene, please add the cursor prefab");
        }

        protected override void OnTrigger()
        {
            GameObject newCursor = GameObject.Instantiate(NewVisualPrefab);
            switch(cursorType)
            {
                case CursorType.OVER_NOTHING:
                    SetCursorTransform(newCursor, cursorToChange.OverNothing);
                    Destroy(cursorToChange.OverNothing);
                    cursorToChange.OverNothing = newCursor;
                    break;
                case CursorType.OVER_OBJECT:
                    SetCursorTransform(newCursor, cursorToChange.OnObject);
                    Destroy(cursorToChange.OnObject);
                    cursorToChange.OnObject = newCursor;
                    break;
                case CursorType.OVER_PLANE:
                    SetCursorTransform(newCursor, cursorToChange.OnPlane);
                    Destroy(cursorToChange.OnPlane);
                    cursorToChange.OnPlane = newCursor;
                    break;
            }
        }

        private void SetCursorTransform(GameObject _newCursor, GameObject _lastCursor)
        {
            _newCursor.transform.SetParent(_lastCursor.transform.parent);
            _newCursor.transform.localPosition = _lastCursor.transform.localPosition;
        }

        public override void SetupUsingApi(GameObject _interaction)
        {
            throw new NotImplementedException();
        }
    }
}
