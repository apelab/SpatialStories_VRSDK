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
        #region public Members
        public Gaze_Conditions CurrentDragAndDropCondition { get { return currentDragAndDropCondition; } set { currentDragAndDropCondition = value; } }

        public float m_MinDistance = 1f;
        // in unity units
        public float angleThreshold = 1f;
        // 0 is perpendicular, 1 is same direction
        public bool respectXAxis = false;
        public bool respectXAxisMirrored = false;
        public bool respectYAxis = false;
        public bool respectYAxisMirrored = false;
        public bool respectZAxis = false;
        public bool respectZAxisMirrored = false;
        public bool m_SnapBeforeDrop = true;

        // snaps only if attached on drop
        public float m_TimeToSnap = 0.5f;

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
        private Gaze_Conditions currentDragAndDropCondition;
        private bool isCurrentlyAligned = false;
        private Gaze_InteractiveObject IO;
        private Coroutine m_SnapCoroutine;
        private Gaze_InteractiveObject interactiveObject;
        #endregion

        void OnEnable()
        {
            // to know if the object has entered its DnD target or not
            Gaze_EventManager.OnProximityEvent += OnProximityEvent;

            // to snap/unsnap by grabbing the object
            Gaze_InputManager.OnControllerGrabEvent += OnControllerGrabEvent;

            // to snap/unsnap by levitating the object
            Gaze_EventManager.OnLevitationEvent += OnLevitationEvent;

            interactiveObject = GetComponent<Gaze_InteractiveObject>();
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
            //if (currentDragAndDropCondition == null)
            //    return;

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
                    Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, currentDragAndDropCondition.DnDTargetObject.gameObject, Gaze_DragAndDropStates.DROPREADYCANCELED));
                }

                // update flags
                wasAligned = isCurrentlyAligned;
            }
        }

        public void SetupDragAndDropProcess(Gaze_Conditions _dndCondition)
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

            Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, currentDragAndDropCondition.DnDTargetObject.gameObject, Gaze_DragAndDropStates.DROPREADY));
        }

        private void Remove()
        {
            if (m_SnapBeforeDrop)
            {
                UnSnap();
                m_Snapped = false;
            }
            Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, currentDragAndDropCondition.DnDTargetObject.gameObject, Gaze_DragAndDropStates.REMOVE));

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
                transform.SetParent(currentDragAndDropCondition.DnDTargetObject.transform);
                Gaze_InteractiveObject IO = GetComponent<Gaze_InteractiveObject>();

                if (currentDragAndDropCondition != null && currentDragAndDropCondition.DnDAttached)
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

            Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, currentDragAndDropCondition.DnDTargetObject.gameObject, Gaze_DragAndDropStates.DROP));
        }

        private void PickUp()
        {
            if (m_SnapBeforeDrop)
            {
                Snap();
                m_Snapped = true;
            }

            Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, currentDragAndDropCondition.DnDTargetObject.gameObject, Gaze_DragAndDropStates.PICKUP));
        }

        private bool IsObjectAlignedWithItsTarget()
        {
            // if already snapped, unsnap it
            if (m_Snapped)
                UnSnap();

            // get the list size of drop targets for this manager
            int targetCount = interactiveObject.DnD_TargetsIndexes.Count;

            // exit if there's no target
            if (targetCount < 1)
                return false;

            // for each drop target in the list
            bool isWithinDistance = false;
            for (int i = 0; i < targetCount; i++)
            {
                // compare distance between me (the drop object) and the drop target
                if (Vector3.Distance(transform.position, Gaze_SceneInventory.Instance.InteractiveObjects[interactiveObject.DnD_TargetsIndexes[i]].transform.position) >= m_MinDistance)
                {
                    isWithinDistance = true;
                    break;
                }
            }

            // exit if none of the targets are aligned
            if (!isWithinDistance)
                return false;


            // calculation of dot products 
            float[] validArray = { 1, 1 };
            float[] xDotProducts = { Vector3.Dot(transform.up, currentDragAndDropCondition.DnDTargetObject.transform.up), Vector3.Dot(transform.forward, currentDragAndDropCondition.DnDTargetObject.transform.forward) };
            float[] yDotProducts = { Vector3.Dot(transform.right, currentDragAndDropCondition.DnDTargetObject.transform.right), Vector3.Dot(transform.forward, currentDragAndDropCondition.DnDTargetObject.transform.forward) };
            float[] zDotProducts = { Vector3.Dot(transform.right, currentDragAndDropCondition.DnDTargetObject.transform.right), Vector3.Dot(transform.up, currentDragAndDropCondition.DnDTargetObject.transform.up) };

            // is respectAxis checked?
            float[] xAxisSimilarity = respectXAxis ? xDotProducts : validArray;
            if (respectXAxisMirrored)
            {
                xAxisSimilarity[0] = Mathf.Abs(xAxisSimilarity[0]);
                xAxisSimilarity[1] = Mathf.Abs(xAxisSimilarity[1]);
            }

            float[] yAxisSimilarity = respectYAxis ? yDotProducts : validArray;
            if (respectYAxisMirrored)
            {
                yAxisSimilarity[0] = Mathf.Abs(yAxisSimilarity[0]);
                yAxisSimilarity[1] = Mathf.Abs(yAxisSimilarity[1]);
            }

            float[] zAxisSimilarity = respectZAxis ? zDotProducts : validArray;
            if (respectZAxisMirrored)
            {
                zAxisSimilarity[0] = Mathf.Abs(zAxisSimilarity[0]);
                zAxisSimilarity[1] = Mathf.Abs(zAxisSimilarity[1]);
            }

            // check if rotations are valid
            bool xValidRotation = xAxisSimilarity[0] > (angleThreshold / 100) || xAxisSimilarity[1] > (angleThreshold / 100);
            bool yValidRotation = yAxisSimilarity[0] > (angleThreshold / 100) || yAxisSimilarity[1] > (angleThreshold / 100);
            bool zValidRotation = zAxisSimilarity[0] > (angleThreshold / 100) || zAxisSimilarity[1] > (angleThreshold / 100);
            bool validRotation = xValidRotation && yValidRotation && zValidRotation;


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

        private void Snap(float timeToSnap = 0f)
        {
            if (timeToSnap == 0)
            {
                transform.position = currentDragAndDropCondition.DnDTargetObject.transform.position;
                transform.rotation = currentDragAndDropCondition.DnDTargetObject.transform.rotation;
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
                transform.position = Vector3.Lerp(startPos, currentDragAndDropCondition.DnDTargetObject.transform.position, eased);
                transform.rotation = Quaternion.Lerp(startRot, currentDragAndDropCondition.DnDTargetObject.transform.rotation, eased);
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
            // if the sender is me (the DropObject)
            if (((Gaze_InteractiveObject)e.Sender).gameObject.Equals(gameObject))
            {
                // get the drop target from the received event
                GameObject dropTarget = e.Other.GetComponentInParent<Gaze_InteractiveObject>().gameObject;
                Debug.Log("drop object [" + gameObject + "] colliding with dropTarget [" + dropTarget + "]");

                // get the list size of drop targets for this manager
                int targetCount = interactiveObject.DnD_TargetsIndexes.Count;
                Debug.Log("targetCount )" + targetCount);

                // for each drop target in the list
                for (int i = 0; i < targetCount; i++)
                {
                    Debug.Log("Gaze_SceneInventory.InteractiveObjects[interactiveObject.DnD_TargetsIndexes[i]] = " + Gaze_SceneInventory.Instance.InteractiveObjects[interactiveObject.DnD_TargetsIndexes[i]]);
                    // TODO @apelab:mike check if the target is in the list
                    if (dropTarget.Equals(Gaze_SceneInventory.Instance.InteractiveObjects[interactiveObject.DnD_TargetsIndexes[i]]))
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