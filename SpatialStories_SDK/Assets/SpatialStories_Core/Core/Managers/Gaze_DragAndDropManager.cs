﻿//-----------------------------------------------------------------------
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
using System.Collections;
using UnityEngine;
using UnityEngine.VR;

namespace Gaze
{
    public class Gaze_DragAndDropManager : MonoBehaviour
    {
        #region public Members
        public Gaze_DragAndDropCondition CurrentDragAndDropCondition { get { return currentDragAndDropCondition; } set { currentDragAndDropCondition = value; } }

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

        // snaps only if attached on drop
        public float m_TimeToSnap = 0.5f;
        #endregion

        #region private Members
        [Gaze_ShowOnly]
        private bool m_Grabbed, isLevitating;
        [Gaze_ShowOnly]
        private bool m_InProximity;
        [Gaze_ShowOnly]
        private bool wasAligned;
        [Gaze_ShowOnly]
        private bool m_Snapped;

        private Gaze_HandController[] grabbingControllers = new Gaze_HandController[2];
        private bool m_attachOnDrop = true;
        private bool m_SnapOnDrop = true;

        private Vector3 m_StartGrabLocalPosition;
        private Quaternion m_StartGrabLocalRotation;

        [SerializeField]
        private Gaze_DragAndDropCondition currentDragAndDropCondition;
        private bool isCurrentlyAligned = false;
        private Gaze_InteractiveObject IO;
        private Coroutine m_SnapCoroutine;
        #endregion

        void OnEnable()
        {
            // to know if the object has entered its DnD target or not
            Gaze_EventManager.OnProximityEvent += OnProximityEvent;

            // to snap/unsnap by grabbing the object
            Gaze_InputManager.OnControllerGrabEvent += OnControllerGrabEvent;

            // to snap/unsnap by levitating the object
            Gaze_EventManager.OnLevitationEvent += OnLevitationEvent;
        }

        void OnDisable()
        {
            Gaze_EventManager.OnProximityEvent -= OnProximityEvent;
            Gaze_InputManager.OnControllerGrabEvent -= OnControllerGrabEvent;
            Gaze_EventManager.OnLevitationEvent -= OnLevitationEvent;
        }

        void Start()
        {
            IO = GetComponent<Gaze_InteractiveObject>();
            grabbingControllers = new Gaze_HandController[2];
        }

        void Update()
        {
            if (currentDragAndDropCondition == null)
                return;

            if (!m_Grabbed && !isLevitating)
                return;

            isCurrentlyAligned = IsObjectAlignedWithItsTarget();

            // if the value has changed
            if (isCurrentlyAligned != wasAligned)
            {
                // if the user aligned the object with its DnD target
                if (isCurrentlyAligned)
                {
                    DropReady();
                }
                // if the user removed the object from its DnD target
                else
                {
                    Remove();
                    Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, currentDragAndDropCondition.TargetObject.gameObject, Gaze_DragAndDropStates.DROPREADYCANCELED));
                }

                // update flags
                wasAligned = isCurrentlyAligned;
            }
        }

        public void SetupDragAndDropProcess(Gaze_DragAndDropCondition _dndCondition)
        {
            if (_dndCondition == currentDragAndDropCondition)
                return;


            currentDragAndDropCondition = _dndCondition;
            ResetVariables();

        }

        private void ResetVariables()
        {

            // Unlock the gravity in case it has been locked for another game object
            Gaze_GravityManager.ChangeGravityState(IO, Gaze_GravityRequestType.UNLOCK);

            wasAligned = false;
            m_Snapped = false;
        }

        private void DropReady()
        {
            if (m_attachOnDrop)
            {
                Gaze_GravityManager.ChangeGravityState(GetComponent<Gaze_InteractiveObject>(), Gaze_GravityRequestType.DEACTIVATE_AND_ATTACH);
                Gaze_GravityManager.ChangeGravityState(GetComponent<Gaze_InteractiveObject>(), Gaze_GravityRequestType.LOCK);
            }

            if (m_SnapBeforeDrop)
            {
                Snap(m_TimeToSnap);
                m_Snapped = true;
            }

            Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, currentDragAndDropCondition.TargetObject.gameObject, Gaze_DragAndDropStates.DROPREADY));
        }

        private void Remove()
        {
            if (m_SnapBeforeDrop)
            {
                UnSnap();
                m_Snapped = false;
            }
            Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, currentDragAndDropCondition.TargetObject.gameObject, Gaze_DragAndDropStates.REMOVE));

            if (IO.ActualGravityState == Gaze_GravityState.LOCKED)
            {
                Gaze_GravityManager.ChangeGravityState(IO, Gaze_GravityRequestType.UNLOCK);
                if (!IO.IsBeingGrabbed)
                    Gaze_GravityManager.ChangeGravityState(IO, Gaze_GravityRequestType.ACTIVATE_AND_DETACH);
            }
        }

        private void Drop()
        {
            if (m_attachOnDrop)
            {
                // parent to target object
                transform.SetParent(currentDragAndDropCondition.TargetObject.transform);
                Gaze_InteractiveObject IO = GetComponent<Gaze_InteractiveObject>();

                if (currentDragAndDropCondition != null && currentDragAndDropCondition.attached)
                {
                    IO.DisableManipulationMode(Gaze_ManipulationModes.GRAB);
                    IO.IsManipulable = false;
                    IO.SetManipulationMode(false, true);
                }

                if (m_SnapOnDrop)
                {
                    Snap(m_TimeToSnap);
                }
            }

            Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, currentDragAndDropCondition.TargetObject.gameObject, Gaze_DragAndDropStates.DROP));
        }

        private void PickUp()
        {
            if (m_SnapBeforeDrop)
            {
                Snap();
                m_Snapped = true;
            }

            Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, currentDragAndDropCondition.TargetObject.gameObject, Gaze_DragAndDropStates.PICKUP));
        }

        private bool IsObjectAlignedWithItsTarget()
        {
            // if already snapped, unsnap it
            if (m_Snapped)
                UnSnap();

            // return if object is too far from its DnD target
            if (Vector3.Distance(transform.position, currentDragAndDropCondition.TargetObject.transform.position) >= m_MinDistance)
                return false;

            // NOTE hack to always return true (no rotation check)
            return true;

            /*
            // check rotation by checking axis
            float xAxisSimilarity = respectXAxis ? Vector3.Dot(transform.right, currentDragAndDropCondition.TargetObject.transform.right) : 1;
            if (respectXAxisMirrored)
                xAxisSimilarity = Mathf.Abs(xAxisSimilarity);
            float yAxisSimilarity = respectYAxis ? Vector3.Dot(transform.up, currentDragAndDropCondition.TargetObject.transform.up) : 1;
            if (respectYAxisMirrored)
                yAxisSimilarity = Mathf.Abs(yAxisSimilarity);
            float zAxisSimilarity = respectZAxis ? Vector3.Dot(transform.forward, currentDragAndDropCondition.TargetObject.transform.forward) : 1;
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
            */
        }

        private void Snap(float timeToSnap = 0f)
        {
            if (timeToSnap == 0)
            {
                transform.position = currentDragAndDropCondition.TargetObject.transform.position;
                transform.rotation = currentDragAndDropCondition.TargetObject.transform.rotation;
            }
            else
            {
                m_SnapCoroutine = StartCoroutine(SnapCoroutine(timeToSnap));
            }
        }

        private IEnumerator SnapCoroutine(float timeToSnap)
        {
            float time = 0f;
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;
            while (time < timeToSnap)
            {
                float eased = QuadEaseOut(time, 0f, 1f, timeToSnap);
                transform.position = Vector3.Lerp(startPos, currentDragAndDropCondition.TargetObject.transform.position, eased);
                transform.rotation = Quaternion.Lerp(startRot, currentDragAndDropCondition.TargetObject.transform.rotation, eased);
                time += Time.deltaTime;
                yield return null;
            }
        }

        private float QuadEaseOut(float time, float startVal, float changeInVal, float duration)
        {
            float elapsedTime = (time > duration) ? 1.0f : time / duration;
            return -changeInVal * elapsedTime * (elapsedTime - 2) + startVal;
        }

        private void UnSnap()
        {
            if (m_SnapCoroutine != null)
            {
                StopCoroutine(m_SnapCoroutine);
                m_SnapCoroutine = null;
            }
            transform.localPosition = m_StartGrabLocalPosition;
            transform.localRotation = m_StartGrabLocalRotation;
        }

        private void Grab(GameObject _controller)
        {
            m_Grabbed = true;

            grabbingControllers[1] = _controller.GetComponentInChildren<Gaze_HandController>();
            m_StartGrabLocalPosition = transform.localPosition;
            m_StartGrabLocalRotation = transform.localRotation;
            if (m_SnapCoroutine != null)
            {
                StopCoroutine(m_SnapCoroutine);
                m_SnapCoroutine = null;
            }
            if (wasAligned)
            {
                PickUp();
            }
        }

        private void Ungrab()
        {
            m_Grabbed = false;
            if (wasAligned)
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
            if (wasAligned)
            {
                PickUp();
            }
        }

        private void Unlevitate()
        {
            isLevitating = false;
            if (wasAligned)
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

        private void OnProximityEvent(Gaze_ProximityEventArgs e)
        {
            if (currentDragAndDropCondition == null)
                return;

            // if I'm the sender
            if (((Gaze_InteractiveObject)e.Sender).gameObject == gameObject)
            {
                // we collided with something
                if (e.Other.GetComponentInParent<Gaze_InteractiveObject>() == currentDragAndDropCondition.TargetObject)
                {
                    m_InProximity = e.IsInProximity;
                    if (!m_InProximity)
                    {
                        if (wasAligned)
                        {
                            wasAligned = false;
                            Remove();
                        }
                    }
                }
            }
        }

        private void OnControllerGrabEvent(Gaze_ControllerGrabEventArgs e)
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
            if (Gaze_Utils.AreUnderSameIO(Gaze_Utils.ConvertIntoGameObject(e.Sender), this.gameObject))
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
}