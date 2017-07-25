//-----------------------------------------------------------------------
// <copyright file="Gaze_GearVr_DragAndDripManager.cs" company="apelab sàrl">
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
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VR;

namespace Gaze
{
    public class Gaze_GearVr_DragAndDropManager : MonoBehaviour
    {
        public Gaze_InteractiveObject m_TargetObject;
        public Gaze_HandController[] grabbingControllers = new Gaze_HandController[2];
        [Gaze_ShowOnly]
        public bool m_Grabbed, isLevitating;
        [Gaze_ShowOnly]
        public bool m_InProximity;
        [Gaze_ShowOnly]
        public bool m_InPlace;
        [Gaze_ShowOnly]
        public bool m_Snapped;
        public float m_MinDistance = 0.1f;
        // in unity units
        public float angleThreshold = 0.9f;
        // 0 is perpendicular, 1 is same direction
        public bool respectXAxis = true;
        public bool respectXAxisMirrored = false;
        public bool respectYAxis = true;
        public bool respectYAxisMirrored = false;
        public bool respectZAxis = true;
        public bool respectZAxisMirrored = false;
        public bool m_SnapBeforeDrop = false;
        private bool m_attachOnDrop = true;
        private bool m_SnapOnDrop = true;
        // snaps only if attached on drop
        public float m_TimeToSnap = 0.5f;
        public bool m_PulseOnDropReady = true;
        private Vector3 m_StartGrabLocalPosition;
        private Quaternion m_StartGrabLocalRotation;


        void OnEnable()
        {
            Gaze_InputManager.OnControllerGrabEvent += OnControllerGrabEvent;
            Gaze_EventManager.OnProximityEvent += OnProximityEvent;
            Gaze_EventManager.OnLevitationEvent += OnLevitationEvent;
        }
        void OnDisable()
        {
            Gaze_InputManager.OnControllerGrabEvent -= OnControllerGrabEvent;
            Gaze_EventManager.OnProximityEvent -= OnProximityEvent;
            Gaze_EventManager.OnLevitationEvent -= OnLevitationEvent;
        }
        void Start()
        {
            grabbingControllers = new Gaze_HandController[2];
            //		grabbingControllers [0] = Gaze_InputManager.instance.LeftController.GetComponentInChildren<Gaze_HandController> ();
            //		grabbingControllers [1] = Gaze_InputManager.instance.RightController.GetComponentInChildren<Gaze_HandController> ();
        }

        void Update()
        {
            if (!m_InProximity || !m_Grabbed && !isLevitating)
                return;

            bool _inPlace = IsInPlace();

            if (_inPlace != m_InPlace)
            {   // if the value changed
                m_InPlace = _inPlace;
                if (m_InPlace)
                {
                    DropReady();
                    //Gaze_InputManager.instance.HapticFeedback(true);
                }
                else
                {
                    Remove();
                    //				if (m_PulseOnDropReady) {
                    //					Gaze_InputManager.instance.HapticFeedback (false);
                    //				}
                }
            }
        }
        void DropReady()
        {
            if (m_attachOnDrop)
            {
                Gaze_GravityManager.ChangeGravityState(GetComponent<Gaze_InteractiveObject>(), Gaze_GravityRequestType.DEACTIVATE_AND_ATTACH);
                Gaze_GravityManager.ChangeGravityState(GetComponent<Gaze_InteractiveObject>(), Gaze_GravityRequestType.LOCK);
            }

            //		Debug.Log ("DROP READY");
            if (m_SnapBeforeDrop)
            {
                Snap(m_TimeToSnap);
                m_Snapped = true;
            }
            if (m_PulseOnDropReady)
            {
                //if (grabbingControllers[0] != null)
                //    Gaze_InputManager.instance.HapticFeedback(true, Gaze_InputManager.instance.LeftController);
                //if (grabbingControllers[1] != null)
                //    Gaze_InputManager.instance.HapticFeedback(true, Gaze_InputManager.instance.RightController);
            }
            //Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, m_TargetObject.gameObject, Gaze_DragAndDropStates.DROPREADY));
            Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(this, gameObject, m_TargetObject.gameObject, Gaze_DragAndDropStates.DROPREADY));
        }
        void Remove()
        {
            //		Debug.Log ("REMOVE");
            if (m_SnapBeforeDrop)
            {
                UnSnap();
                m_Snapped = false;
            }
            //Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, m_TargetObject.gameObject, Gaze_DragAndDropStates.REMOVE));
            Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(this, gameObject, m_TargetObject.gameObject, Gaze_DragAndDropStates.REMOVE));
        }
        void Drop()
        {
            if (m_attachOnDrop)
            {
                // parent to target object
                transform.SetParent(m_TargetObject.transform);
                Gaze_InteractiveObject IO = GetComponent<Gaze_InteractiveObject>();
                Gaze_DragAndDropCondition conditions = GetComponentInChildren<Gaze_DragAndDropCondition>();
                if (IO.DnD_attached)
                {
                    IO.DisableManipulationMode(Gaze_ManipulationModes.GRAB);
                    IO.IsManipulable = false;
                    IO.SetManipulationMode(false, true);
                    if (IO.GrabbingManager != null)
                        IO.GrabbingManager.TryDetach();
                }
                if (m_SnapOnDrop)
                {
                    Snap(m_TimeToSnap);
                }
            }
            //if (m_PulseOnDropReady)
            //    Gaze_InputManager.instance.HapticFeedback(false);
            //Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, m_TargetObject.gameObject, Gaze_DragAndDropStates.DROP));
            Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(this, gameObject, m_TargetObject.gameObject, Gaze_DragAndDropStates.DROP));
        }
        private void PickUp()
        {
            if (m_SnapBeforeDrop)
            {
                Snap();
                m_Snapped = true;
            }
            if (m_PulseOnDropReady && grabbingControllers.Length > 0)
            {
                // vibrates the controllers in the array (the ones grabbing)
                // HACK: le'ts try to catch this error until we solve the problem on the pulses
                try
                {
                    //if (grabbingControllers[0] != null)
                    //    Gaze_InputManager.instance.HapticFeedback(grabbingControllers[0].gameObject);
                    //if (grabbingControllers[1] != null)
                    //    Gaze_InputManager.instance.HapticFeedback(grabbingControllers[1].gameObject);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("Start Pulse not implmented! " + ex.Message);
                }
            }
            //Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, m_TargetObject.gameObject, Gaze_DragAndDropStates.PICKUP));
            Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(this, gameObject, m_TargetObject.gameObject, Gaze_DragAndDropStates.PICKUP));
        }
        private bool IsInPlace()
        {
            // check position
            if (m_Snapped)
            {
                // replace the object temporarily at its original local position
                UnSnap();
            }
            if (Vector3.Distance(transform.position, m_TargetObject.transform.position) > m_MinDistance)
                return false;
            // check rotation bz checking axis
            float xAxisSimilarity = respectXAxis ? Vector3.Dot(transform.right, m_TargetObject.transform.right) : 1;
            if (respectXAxisMirrored)
                xAxisSimilarity = Mathf.Abs(xAxisSimilarity);
            float yAxisSimilarity = respectYAxis ? Vector3.Dot(transform.up, m_TargetObject.transform.up) : 1;
            if (respectYAxisMirrored)
                yAxisSimilarity = Mathf.Abs(yAxisSimilarity);
            float zAxisSimilarity = respectZAxis ? Vector3.Dot(transform.forward, m_TargetObject.transform.forward) : 1;
            if (respectZAxisMirrored)
                zAxisSimilarity = Mathf.Abs(zAxisSimilarity);
            bool validRotation = xAxisSimilarity > angleThreshold && yAxisSimilarity > angleThreshold && zAxisSimilarity > angleThreshold;
            if (validRotation)
            {
                if (m_Snapped)
                {
                    // resnap
                    Snap();
                }
                return true;
            }
            else
                return false;
        }
        Coroutine m_SnapCoroutine;
        void Snap(float timeToSnap = 0f)
        {
            if (timeToSnap == 0)
            {
                transform.position = m_TargetObject.transform.position;
                transform.rotation = m_TargetObject.transform.rotation;
            }
            else
            {
                m_SnapCoroutine = StartCoroutine(SnapCoroutine(timeToSnap));
            }
        }
        IEnumerator SnapCoroutine(float timeToSnap)
        {
            float time = 0f;
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;
            while (time < timeToSnap)
            {
                float eased = QuadEaseOut(time, 0f, 1f, timeToSnap);
                transform.position = Vector3.Lerp(startPos, m_TargetObject.transform.position, eased);
                transform.rotation = Quaternion.Lerp(startRot, m_TargetObject.transform.rotation, eased);
                time += Time.deltaTime;
                yield return null;
            }
        }
        float QuadEaseOut(float time, float startVal, float changeInVal, float duration)
        {
            float elapsedTime = (time > duration) ? 1.0f : time / duration;
            return -changeInVal * elapsedTime * (elapsedTime - 2) + startVal;
        }
        void UnSnap()
        {
            if (m_SnapCoroutine != null)
            {
                StopCoroutine(m_SnapCoroutine);
                m_SnapCoroutine = null;
            }
            transform.localPosition = m_StartGrabLocalPosition;
            transform.localRotation = m_StartGrabLocalRotation;
        }
        void Grab(GameObject _controller)
        {
            m_Grabbed = true;
            //		if (grabbingControllers != null && m_PulseOnDropReady)
            //			FindObjectOfType<Gaze_InputManager> ().StopPulse ();
            // add the grabbing controller to the array of grabbing controllers
            //if (_controller.GetComponentInChildren<Gaze_HandController>().leftHand)
            //    grabbingControllers[0] = _controller.GetComponentInChildren<Gaze_HandController>();
            //else
            grabbingControllers[1] = _controller.GetComponentInChildren<Gaze_HandController>();
            m_StartGrabLocalPosition = transform.localPosition;
            m_StartGrabLocalRotation = transform.localRotation;
            if (m_SnapCoroutine != null)
            {
                StopCoroutine(m_SnapCoroutine);
                m_SnapCoroutine = null;
            }
            if (m_InPlace)
            {
                PickUp();
            }
        }
        void Ungrab()
        {
            m_Grabbed = false;
            if (m_InPlace)
            {
                Drop();
            }
            // remove controllers
            grabbingControllers[0] = null;
            grabbingControllers[1] = null;
        }
        private void Levitate()
        {
            isLevitating = true;
            // get controllers
            //grabbingControllers[0] = Gaze_InputManager.instance.LeftController.GetComponentInChildren<Gaze_HandController>();
            //grabbingControllers[1] = Gaze_InputManager.instance.RightController.GetComponentInChildren<Gaze_HandController>();
            if (m_InPlace)
            {
                PickUp();
            }
        }
        void Unlevitate()
        {
            isLevitating = false;
            if (m_InPlace)
            {
                Drop();
            }
            // remove controllers
            if (grabbingControllers != null && grabbingControllers.Length == 2)
            {
                grabbingControllers[0] = null;
                grabbingControllers[1] = null;
            }
        }
        void OnProximityEvent(Gaze_ProximityEventArgs e)
        {
            if ((GameObject)e.Sender == gameObject)
            {   // we collided with something
                if (e.Other.GetComponentInParent<Gaze_InteractiveObject>() == m_TargetObject)
                {
                    m_InProximity = e.IsInProximity;
                    if (!m_InProximity)
                    {
                        if (m_InPlace)
                        {
                            m_InPlace = false;
                            Remove();
                        }
                    }
                }
            }
        }
        void OnControllerGrabEvent(Gaze_ControllerGrabEventArgs e)
        {
            if (e.ControllerObjectPair.Value == gameObject)
            {
                if (e.IsGrabbing)
                {
                    if (e.ControllerObjectPair.Key.Equals(VRNode.LeftHand))
                        Grab(Gaze_InputManager.instance.LeftController);
                    else if (e.ControllerObjectPair.Key.Equals(VRNode.RightHand))
                        Grab(Gaze_InputManager.instance.RightController);
                }
                else
                {
                    Ungrab();
                }
            }

        }
        private void OnLevitationEvent(Gaze_LevitationEventArgs e)
        {
            if (e.Type.Equals(Gaze_LevitationTypes.LEVITATE_START))
            {
                Levitate();
            }
            else if (e.Type.Equals(Gaze_LevitationTypes.LEVITATE_STOP))
            {
                Unlevitate();
            }
        }
    }
}