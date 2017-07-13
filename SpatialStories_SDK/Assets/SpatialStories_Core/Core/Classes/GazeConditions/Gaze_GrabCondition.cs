//-----------------------------------------------------------------------
// <copyright file="LevitationEventArgs.cs" company="apelab sàrl">
// © apelab. All Rights Reserved.
//
// This source is subject to the apelab license.
// All other rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// <author>Michaël Martin</author>
// <email>dev@apelab.ch</email>
// <web>https://twitter.com/apelab_ch</web>
// <web>http://www.apelab.ch</web>
// <date>2016-01-25</date>
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.VR;

namespace Gaze
{
    public class Gaze_GrabCondition : Gaze_AbstractCondition
    {
        private bool grabLeftValid = false;
        private bool grabRightValid = false;
        private bool grabStateLeftValid = false;
        private bool grabStateRightValid = false;

        public Gaze_GrabCondition(Gaze_Conditions _gazeConditionsScript) : base(_gazeConditionsScript) { }

        public override bool IsValidated()
        {
            return IsValid;
        }

#if UNITY_EDITOR
        public override void ToEditorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (IsValid)
            {
                RenderSatisfiedLabel("Grab:");
                RenderSatisfiedLabel("True");
            }
            else
            {
                RenderNonSatisfiedLabel("Grab:");
                RenderNonSatisfiedLabel("False");
            }
            EditorGUILayout.EndHorizontal();
        }
#endif

        protected override void CustomDispose()
        {
            Gaze_InputManager.OnControllerGrabEvent -= OnControllerGrabEvent;
        }

        protected override void CustomSetup()
        {
            Gaze_InputManager.OnControllerGrabEvent += OnControllerGrabEvent;
        }

        private bool IsGrabbingControllerStateValid(bool _isGrabbing, Gaze_HandsEnum _mapHand, VRNode _dicoHand)
        {
            if (_mapHand.Equals(Gaze_HandsEnum.BOTH))
            {
                // check left hand state is ok
                if (_dicoHand.Equals(VRNode.LeftHand))
                    grabStateLeftValid = (_isGrabbing && gazeConditionsScript.grabMap.grabStateLeftIndex.Equals((int)Gaze_GrabStates.GRAB)) || (!_isGrabbing && gazeConditionsScript.grabMap.grabStateLeftIndex.Equals((int)Gaze_GrabStates.UNGRAB));

                // check right hand state is ok
                if (_dicoHand.Equals(VRNode.RightHand))
                    grabStateRightValid = (_isGrabbing && gazeConditionsScript.grabMap.grabStateRightIndex.Equals((int)Gaze_GrabStates.GRAB)) || (!_isGrabbing && gazeConditionsScript.grabMap.grabStateRightIndex.Equals((int)Gaze_GrabStates.UNGRAB));

                return grabStateLeftValid || grabStateRightValid;
            }
            else
            {
                int state = _mapHand.Equals(Gaze_HandsEnum.LEFT) ? gazeConditionsScript.grabMap.grabStateLeftIndex : gazeConditionsScript.grabMap.grabStateRightIndex;

                if (_isGrabbing && state.Equals((int)Gaze_GrabStates.GRAB))
                    return true;

                if (!_isGrabbing && state.Equals((int)Gaze_GrabStates.UNGRAB))
                    return true;
            }

            return false;
        }

        private bool IsGrabbingObjectValid(GameObject _grabbedObject, int _handIndex)
        {
            return _grabbedObject.Equals(gazeConditionsScript.grabMap.grabEntryList[0].interactiveObject);
        }

        private void ValidateGrab(Gaze_ControllerGrabEventArgs e)
        {
            // by default, invalidate grab
            IsValid = ValidateGrabController(e);
        }

        private bool ValidateGrabController(Gaze_ControllerGrabEventArgs e)
        {
            VRNode dicoVRNode = e.ControllerObjectPair.Key;
            GameObject grabbedObject = e.ControllerObjectPair.Value;

            // get the hand VRNode from the event
            bool isGrabbingControllerLeft = e.ControllerObjectPair.Key == VRNode.LeftHand;
            VRNode eventVRNode = isGrabbingControllerLeft ? VRNode.LeftHand : VRNode.RightHand;

            bool grabbedObjectValid = IsGrabbingObjectValid(grabbedObject, gazeConditionsScript.grabMap.grabHandsIndex);

            // if we've configured
            switch (gazeConditionsScript.grabMap.grabHandsIndex)
            {
                case (int)Gaze_HandsEnum.BOTH:
                    bool isGrabbingControllerStateValid = IsGrabbingControllerStateValid(e.IsGrabbing, Gaze_HandsEnum.BOTH, eventVRNode);
                    bool isGrabbingControllerInMap = IsGrabbingControllerInMap(dicoVRNode);

                    return isGrabbingControllerInMap && isGrabbingControllerStateValid && grabbedObjectValid;
                    break;

                //  the LEFT hand
                case (int)Gaze_HandsEnum.LEFT:
                    grabLeftValid = IsGrabbingControllerInMap(dicoVRNode) && IsGrabbingControllerStateValid(e.IsGrabbing, Gaze_HandsEnum.LEFT, eventVRNode) && grabbedObjectValid;
                    return grabLeftValid;
                    break;

                //  the RIGHT hand
                case (int)Gaze_HandsEnum.RIGHT:
                    grabRightValid = IsGrabbingControllerInMap(dicoVRNode) && IsGrabbingControllerStateValid(e.IsGrabbing, Gaze_HandsEnum.RIGHT, eventVRNode) && grabbedObjectValid;
                    return grabRightValid;
                    break;
            }

            return false;
        }

        private void OnControllerGrabEvent(Gaze_ControllerGrabEventArgs e)
        {
            ValidateGrab(e);
        }


        private bool IsGrabbingControllerInMap(VRNode grabbingController)
        {
            for (int i = 0; i < gazeConditionsScript.grabMap.grabEntryList.Count; i++)
            {
                if (gazeConditionsScript.grabMap.grabHandsIndex == (int)Gaze_HandsEnum.BOTH)
                {
                    if (gazeConditionsScript.grabMap.grabEntryList[i].hand.Equals(VRNode.RightHand) ||
                        gazeConditionsScript.grabMap.grabEntryList[i].hand.Equals(VRNode.LeftHand))
                        return true;
                }
                else
                    if (gazeConditionsScript.grabMap.grabEntryList[i].hand.Equals(grabbingController))
                    return true;
            }

            return false;
        }


    }
}