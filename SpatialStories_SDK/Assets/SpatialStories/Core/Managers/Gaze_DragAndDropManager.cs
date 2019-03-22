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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

namespace Gaze
{
    public class Gaze_DragAndDropManager : MonoBehaviour
    {
        #region public Members
        // in unity units
        [Gaze_ShowOnly]
        private bool m_Grabbed, isLevitating;
        [Gaze_ShowOnly]
        private bool m_InProximity;
        [Gaze_ShowOnly]
        private bool wasAligned;
        [Gaze_ShowOnly]
        private bool m_Snapped;
        public bool IsSnapped { get { return m_Snapped; } }

        private Gaze_HandController[] grabbingControllers = new Gaze_HandController[2];

        private Vector3 m_StartGrabLocalPosition;
        private Quaternion m_StartGrabLocalRotation;

        private bool isCurrentlyAligned = false;
        private Gaze_InteractiveObject IO;
        private Coroutine m_SnapCoroutine;
        private Gaze_InteractiveObject interactiveObject;
        private Gaze_Manipulation IO_Manipulation;
        private Gaze_HandHover IO_HandHover;
        private bool isCurrentlyDropped = false;

        private Transform targetTransform;

        private Gaze_ManipulationModes lastManipulationMode;
        #endregion

        /// <summary>
        /// Stores all the ids of ocupped drag and drop targets
        /// </summary>
        public static HashSet<int> UsedDropTargets = new HashSet<int>();

        void OnEnable()
        {
            // to snap/unsnap by grabbing the object
            Gaze_InputManager.OnControllerGrabEvent += OnControllerGrabEvent;

            // to snap/unsnap by levitating the object
            Gaze_EventManager.OnLevitationEvent += OnLevitationEvent;

            interactiveObject = GetComponent<Gaze_InteractiveObject>();
        }

        void OnDisable()
        {
            Gaze_EventManager.OnProximityEvent -= OnProximityEvent;
            Gaze_EventManager.OnLevitationEvent -= OnLevitationEvent;
            // to snap/unsnap by grabbing the object
            Gaze_InputManager.OnControllerGrabEvent -= OnControllerGrabEvent;
        }

        void Start()
        {
            IO = GetComponent<Gaze_InteractiveObject>();
            IO_HandHover = IO.GetComponentInChildren<Gaze_HandHover>();
            IO_Manipulation = IO.GetComponentInChildren<Gaze_Manipulation>();
            grabbingControllers = new Gaze_HandController[2];
        }

        void Update()
        {
            if ((!m_Grabbed && !isLevitating) || !interactiveObject.IsDragAndDropEnabled)
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
                    Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(this, gameObject, targetTransform.gameObject, Gaze_DragAndDropStates.DROPREADYCANCELED));
                }

                // update flags
                wasAligned = isCurrentlyAligned;
            }
        }

        public void SetupDragAndDropProcess(Gaze_Conditions _dndCondition)
        {
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
            if (interactiveObject.DnD_snapBeforeDrop)
            {
                Snap(interactiveObject.DnD_TimeToSnap);
                m_Snapped = true;
            }

            //Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, targetTransform.gameObject, Gaze_DragAndDropStates.DROPREADY));
            Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(this, gameObject, targetTransform.gameObject, Gaze_DragAndDropStates.DROPREADY));
        }

        public void Remove()
        {
            isCurrentlyDropped = false;
            if (interactiveObject.DnD_snapBeforeDrop)
            {
                UnSnap();
                m_Snapped = false;
            }
            //Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(gameObject, targetTransform.gameObject, Gaze_DragAndDropStates.REMOVE));
            Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(this, gameObject, targetTransform.gameObject, Gaze_DragAndDropStates.REMOVE));

            if (IO.ActualGravityState == Gaze_GravityState.LOCKED)
            {
                Gaze_GravityManager.ChangeGravityState(IO, Gaze_GravityRequestType.UNLOCK);
                if (!IO.GrabLogic.IsBeingGrabbed)
                    Gaze_GravityManager.ChangeGravityState(IO, Gaze_GravityRequestType.ACTIVATE_AND_DETACH);
            }
        }

        /// <summary>
        /// This method will be called on every FireNewDragAndDropEvent in order to check
        /// what is the current state of the drop targets, this will allow us to know if a
        /// target is already occuped or not.
        /// </summary>
        /// <param name="e"></param>
        public static void UpdateDropTargetsStates(Gaze_DragAndDropEventArgs e)
        {
            switch (e.State)
            {
                case Gaze_DragAndDropStates.DROP:
                    UsedDropTargets.Add(((GameObject)e.DropTarget).GetInstanceID());
                    break;
                case Gaze_DragAndDropStates.REMOVE:
                    UsedDropTargets.Remove(((GameObject)e.DropTarget).GetInstanceID());
                    break;
            }
        }

        /// <summary>
        /// Check if a drop target is already used or not
        /// </summary>
        /// <param name="_dropTargetToCheck"></param>
        /// <returns></returns>
        public static bool IsDropTargetAlreadyUsed(GameObject _dropTargetToCheck)
        {
            return UsedDropTargets.Contains(_dropTargetToCheck.GetInstanceID());
        }

        private void OnLevelWasLoaded(int level)
        {
            if (UsedDropTargets == null)
                UsedDropTargets = new HashSet<int>();
            //If we are loading a new level just make sure that we don't have references to other levels
            else if (UsedDropTargets != null && UsedDropTargets.Count > 0)
                UsedDropTargets.Clear();
        }

        private void Drop()
        {
            isCurrentlyDropped = true;

            // parent to target object
            transform.SetParent(targetTransform.transform);

            // Don't snap if not necesary
            if (IO.DnD_SnapOnDrop)
                Snap(interactiveObject.DnD_TimeToSnap);

            Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(this, gameObject, targetTransform.gameObject, Gaze_DragAndDropStates.DROP));
            if (IO.DnD_attached)
                IO.EnableManipulationMode(Gaze_ManipulationModes.NONE);
        }

        private void PickUp()
        {
            if (interactiveObject.DnD_snapBeforeDrop)
            {
                Snap();
                m_Snapped = true;
            }

            Gaze_EventManager.FireDragAndDropEvent(new Gaze_DragAndDropEventArgs(this, gameObject, targetTransform.gameObject, Gaze_DragAndDropStates.PICKUP));
        }

        public bool IsInDistance(Vector3 _position, float _tolerance = 0f)
        {
            bool inDistance = false;
            float closestTargetInRangeDistance = float.MaxValue;
            for (int i = 0; i < interactiveObject.DnD_Targets.Count; i++)
            {
                // if target doesn't exist anymore, remove it !
                if (interactiveObject.DnD_Targets[i] == null)
                {
                    interactiveObject.DnD_Targets.RemoveAt(i);
                }
                else
                {
                    float targetDistance = Vector3.Distance(_position, interactiveObject.DnD_Targets[i].transform.position);
                    // compare distance between me (the drop object) and the drop target
                    if (targetDistance <= interactiveObject.DnD_minDistance + _tolerance)
                    {
                        // store the target in proximity's transform if it's closer than any other target in range
                        if (targetDistance < closestTargetInRangeDistance)
                        {
                            // Check if this object wants to be the only one in a target
                            if (!IO.DnD_DropOnAlreadyOccupiedTargets)
                            {
                                // if the target is not used we can use it
                                if (!Gaze_DragAndDropManager.IsDropTargetAlreadyUsed(interactiveObject.DnD_Targets[i].gameObject))
                                {
                                    targetTransform = interactiveObject.DnD_Targets[i].transform;
                                    closestTargetInRangeDistance = targetDistance;
                                    inDistance = true;
                                }
                            }
                            else // If the object doesn't care about being in the same position of other targets no more checking is needed
                            {
                                targetTransform = interactiveObject.DnD_Targets[i].transform;
                                closestTargetInRangeDistance = targetDistance;
                                inDistance = true;
                            }

                        }
                    }
                }
            }
            return inDistance;
        }

        private bool IsObjectAlignedWithItsTarget()
        {
            // if already snapped, unsnap it
            if (m_Snapped)
                UnSnap();

            if (interactiveObject.DnD_Targets == null)
                return false;

            // get the list size of drop targets for this manager
            int targetCount = interactiveObject.DnD_Targets.Count;

            // exit if there's no target
            if (targetCount < 1)
                return false;

            // for each drop target in the list
            bool isWithinDistance = IsInDistance(transform.position);

            // exit if none of the targets are aligned
            if (!isWithinDistance)
                return false;

            // calculation of dot products 
            float[] validArray = { 1, 1 };
            float[] xDotProducts = { Vector3.Dot(transform.up, targetTransform.up), Vector3.Dot(transform.forward, targetTransform.forward) };
            float[] yDotProducts = { Vector3.Dot(transform.right, targetTransform.right), Vector3.Dot(transform.forward, targetTransform.forward) };
            float[] zDotProducts = { Vector3.Dot(transform.right, targetTransform.right), Vector3.Dot(transform.up, targetTransform.up) };

            // is respectAxis checked?
            float[] xAxisSimilarity = interactiveObject.DnD_respectXAxis ? xDotProducts : validArray;
            if (interactiveObject.DnD_respectXAxisMirrored)
            {
                xAxisSimilarity[0] = Mathf.Abs(xAxisSimilarity[0]);
                xAxisSimilarity[1] = Mathf.Abs(xAxisSimilarity[1]);
            }

            float[] yAxisSimilarity = interactiveObject.DnD_respectYAxis ? yDotProducts : validArray;
            if (interactiveObject.DnD_respectYAxisMirrored)
            {
                yAxisSimilarity[0] = Mathf.Abs(yAxisSimilarity[0]);
                yAxisSimilarity[1] = Mathf.Abs(yAxisSimilarity[1]);
            }

            float[] zAxisSimilarity = interactiveObject.DnD_respectZAxis ? zDotProducts : validArray;
            if (interactiveObject.DnD_respectZAxisMirrored)
            {
                zAxisSimilarity[0] = Mathf.Abs(zAxisSimilarity[0]);
                zAxisSimilarity[1] = Mathf.Abs(zAxisSimilarity[1]);
            }

            // check if rotations are valid
            bool xValidRotation = xAxisSimilarity[0] > (interactiveObject.DnD_angleThreshold / 100) || xAxisSimilarity[1] > (interactiveObject.DnD_angleThreshold / 100);
            bool yValidRotation = yAxisSimilarity[0] > (interactiveObject.DnD_angleThreshold / 100) || yAxisSimilarity[1] > (interactiveObject.DnD_angleThreshold / 100);
            bool zValidRotation = zAxisSimilarity[0] > (interactiveObject.DnD_angleThreshold / 100) || zAxisSimilarity[1] > (interactiveObject.DnD_angleThreshold / 100);
            bool validRotation = xValidRotation && yValidRotation && zValidRotation;


            if (validRotation)
            {
                if (m_Snapped)
                {
                    // resnap
                    if (IO.DnD_SnapOnDrop)
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
                transform.position = targetTransform.position;
                transform.rotation = targetTransform.transform.rotation;
                Gaze_GravityManager.ChangeGravityState(GetComponent<Gaze_InteractiveObject>(), Gaze_GravityRequestType.DEACTIVATE_AND_ATTACH);
                Gaze_GravityManager.ChangeGravityState(GetComponent<Gaze_InteractiveObject>(), Gaze_GravityRequestType.LOCK);
            }
            else
            {
                m_SnapCoroutine = StartCoroutine(SnapCoroutine(timeToSnap));

            }
        }

        private IEnumerator SnapCoroutine(float timeToSnap)
        {
            // Don't teleport if an object is snapping (This is a hack for placing objects on the head)
            bool wasTeleportAllowed = Gaze_Teleporter.IsTeleportAllowed;
            Gaze_Teleporter.IsTeleportAllowed = false;
            float time = 0f;
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            Gaze_GravityManager.ChangeGravityState(GetComponent<Gaze_InteractiveObject>(), Gaze_GravityRequestType.DEACTIVATE_AND_ATTACH);
            Gaze_GravityManager.ChangeGravityState(GetComponent<Gaze_InteractiveObject>(), Gaze_GravityRequestType.LOCK);

            while (time < timeToSnap)
            {
                float eased = QuadEaseOut(time, 0f, 1f, timeToSnap);
                transform.position = Vector3.Lerp(startPos, targetTransform.position, eased);
                transform.rotation = Quaternion.Lerp(startRot, targetTransform.rotation, eased);
                time += Time.deltaTime;
                yield return null;
            }

            transform.position = targetTransform.position;
            transform.rotation = targetTransform.transform.rotation;

            if (wasTeleportAllowed)
                Gaze_Teleporter.IsTeleportAllowed = true;

            //Attempt n.1
            ///Previously this statement was if(wasTeleportAllowed) and it made the teleport inactive after dropping an object.
            ///Now this is always true so Teleport gets always reactivated.
            ///This fix apparently solves this bug but further investigations are suggested while maybe keeping on-hold the whole "Snap Before Drop" option.
           
            //Attempt n.2
            //Made the if statement dependent to previous Gaze_Teleporter.IsTeleportAllowe bool
            //Attempt n.3 Teleport Always reactivated at the end of the coroutine! NOTE: If this doesn't work we can try force reactivation everytime Coroutine is Stopped in UnSnap()
            //if(!Gaze_Teleporter.IsTeleportAllowed)
            // 
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

            Gaze_Teleporter.IsTeleportAllowed = true;
            transform.SetParent(null);
        }

        public void Grab(GameObject _controller)
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

            interactiveObject = GetComponent<Gaze_InteractiveObject>();

            // if the sender is me (the DropObject)
            if (((Gaze_InteractiveObject)e.Sender).gameObject.Equals(gameObject))
            {
                // get the drop target from the received event
                GameObject dropTarget = e.Other.GetComponentInParent<Gaze_InteractiveObject>().gameObject;

                // get the list size of drop targets for this manager
                int targetCount = interactiveObject.DnD_Targets.Count;

                // for each drop target in the list
                for (int i = 0; i < targetCount; i++)
                {
                    // check if the target is in the list
                    if (dropTarget.Equals(interactiveObject.DnD_Targets[i]))
                    {
                        m_InProximity = e.IsInProximity;

                        // if it's not in proximity
                        if (!m_InProximity)
                        {
                            // but was previously aligned
                            if (wasAligned)
                            {
                                wasAligned = false;

                                // that means the user removed the drop object from its target
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
                    // to know if the object has entered its DnD target or not
                    Gaze_EventManager.OnProximityEvent += OnProximityEvent;

                    if (e.ControllerObjectPair.Key.Equals(UnityEngine.XR.XRNode.LeftHand))
                        Grab(Gaze_InputManager.instance.LeftController);
                    else if (e.ControllerObjectPair.Key.Equals(UnityEngine.XR.XRNode.RightHand))
                        Grab(Gaze_InputManager.instance.RightController);
                }
                else
                {
                    Ungrab();
                    Gaze_EventManager.OnProximityEvent -= OnProximityEvent;
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

        public void ChangeAttach(bool attach)
        {
            if (isCurrentlyDropped)
            {
                if (!attach)
                    IO.EnableManipulationMode(lastManipulationMode);
                else
                    IO.EnableManipulationMode(Gaze_ManipulationModes.NONE);

            }
        }

        /// <summary>
        /// Allows object to be dropped even if they are super far
        /// </summary>
        public void AutoDrop(Transform _target)
        {
            targetTransform = _target;
            transform.position = targetTransform.position;
            wasAligned = true;
            Drop();
        }
    }
}