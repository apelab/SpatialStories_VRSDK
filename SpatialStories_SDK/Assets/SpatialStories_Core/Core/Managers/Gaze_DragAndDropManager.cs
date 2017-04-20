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
using System.Collections;
using UnityEngine;
using UnityEngine.VR;

namespace Gaze
{
    public class Gaze_DragAndDropManager : MonoBehaviour
    {
        [Gaze_ShowOnly]
        private bool m_Grabbed, isLevitating;
        [Gaze_ShowOnly]
        private bool m_InProximity;
        [Gaze_ShowOnly]
        private bool m_InPlace;
        [Gaze_ShowOnly]
        private bool m_Snapped;

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

        private Gaze_HandController[] grabbingControllers = new Gaze_HandController[2];
        private bool m_attachOnDrop = true;
        private bool m_SnapOnDrop = true;

        // snaps only if attached on drop
        public float m_TimeToSnap = 0.5f;
        private Vector3 m_StartGrabLocalPosition;
        private Quaternion m_StartGrabLocalRotation;

        public Gaze_DragAndDropCondition CurrentDragAndDropCondition { get { return currentDragAndDropCondition; } set { currentDragAndDropCondition = value; } }

        [SerializeField]
        private Gaze_DragAndDropCondition currentDragAndDropCondition;

        Gaze_InteractiveObject IO;

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
            IO = GetComponent<Gaze_InteractiveObject>();

            // Unlock the gravity in case it has been locked for another game object
            Gaze_GravityManager.ChangeGravityState(IO, Gaze_GravityRequestType.UNLOCK);

            m_InPlace = false;
            m_Snapped = false;
        }

        void Update()
        {
            if (currentDragAndDropCondition == null)
                return;

            if (!m_Grabbed && !isLevitating)
                return;

            bool _inPlace = IsInPlace();

            // if the value changed
            if (_inPlace != m_InPlace)
            {
                m_InPlace = _inPlace;
                if (m_InPlace)
                {
                    DropReady();
                }
                else
                {
                    Remove();
                    Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, currentDragAndDropCondition.TargetObject.gameObject, Gaze_DragAndDropStates.DROPREADYCANCELED));
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

            if (m_SnapBeforeDrop)
            {
                Snap(m_TimeToSnap);
                m_Snapped = true;
            }

            Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, currentDragAndDropCondition.TargetObject.gameObject, Gaze_DragAndDropStates.DROPREADY));
        }

        void Remove()
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

        void Drop()
        {
            if (m_attachOnDrop)
            {
                // parent to target object
                transform.SetParent(currentDragAndDropCondition.TargetObject.transform);
                Gaze_InteractiveObject IO = GetComponent<Gaze_InteractiveObject>();

                if (currentDragAndDropCondition != null && currentDragAndDropCondition.attached)
                {
                    IO.DisableManipulationMode(Gaze_ManipulationModes.GRAB);
                    IO.isManupulable = false;
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

        private bool IsInPlace()
        {
            // check position
            if (m_Snapped)
            {
                // replace the object temporarily at its original local position
                UnSnap();
            }
            if (Vector3.Distance(transform.position, currentDragAndDropCondition.TargetObject.transform.position) > m_MinDistance)
                return false;
            // check rotation bz checking axis
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
            if (true)
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
                transform.position = currentDragAndDropCondition.TargetObject.transform.position;
                transform.rotation = currentDragAndDropCondition.TargetObject.transform.rotation;
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
                transform.position = Vector3.Lerp(startPos, currentDragAndDropCondition.TargetObject.transform.position, eased);
                transform.rotation = Quaternion.Lerp(startRot, currentDragAndDropCondition.TargetObject.transform.rotation, eased);
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
            if (currentDragAndDropCondition == null)
                return;

            if ((GameObject)e.Sender == gameObject)
            {   // we collided with something
                if (e.Other.GetComponentInParent<Gaze_InteractiveObject>() == currentDragAndDropCondition.TargetObject)
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
            if (Gaze_EventUtils.AreUnderSameIO(Gaze_EventUtils.ConvertIntoGameObject(e.Sender), this.gameObject))
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