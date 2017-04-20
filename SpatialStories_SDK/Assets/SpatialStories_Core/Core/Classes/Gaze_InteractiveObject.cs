// <copyright file="Gaze_InteractiveObject.cs" company="apelab sàrl">
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
// </copyright>
// <author>Michaël Martin</author>
// <email>dev@apelab.ch</email>
// <web>https://twitter.com/apelab_ch</web>
// <web>http://www.apelab.ch</web>
// <date>2014-06-01</date>
using System;
using System.Collections;
using UnityEngine;

namespace Gaze
{
    [Serializable]
    [SelectionBase]
    public class Gaze_InteractiveObject : MonoBehaviour
    {
        /// <summary>
        /// Defines how the user can interact with the IO by using 
        /// his controllers.
        /// </summary>
        public Gaze_ManipulationModes ManipulationMode { private set; get; }

        /// <summary>
        /// Can this object be grabbed.
        /// </summary>
        public bool grab { get { return ManipulationMode == Gaze_ManipulationModes.GRAB; } }

        /// <summary>
        /// Can this object be activated by :
        /// TOUCH, POINT or BOTH
        /// </summary>
        public bool touch { get { return ManipulationMode == Gaze_ManipulationModes.TOUCH; } }
        public int touchIndex;

        /// <summary>
        /// The grab mode is either : AUTOMATIC or MANUAL
        /// </summary>
        public int grabModeIndex;

        /// <summary>
        /// It's the collider used to catch the object.
        /// </summary>
        //public Collider catchableHandle;

        /// <summary>
        /// It's the collider used to manipulate the object.
        /// </summary>
        public Collider manipulabeHandle;

        public bool hasGrabPositionner = false;
        public Collider grabPositionnerCollider;

        public Transform GrabPositionnerTransform { get { return grabPositionnerCollider.gameObject.transform; } }

        /// <summary>
        /// If true, the object being catched will vibrate the controllers while grabbed.
        /// </summary>
        public bool vibratesOnGrab = false;

        /// <summary>
        /// Defines the maximum distance that an object can be cached
        /// </summary>
        public float grabDistance;

        /// <summary>
        /// Defines the maximum distance that an object can be distanced touched
        /// </summary>
        public float touchDistance;

        /// <summary>
        /// Defines the maximum distance that an object can be levitated
        /// </summary>
        public float levitateDistance = 1f;


        /// <summary>
        /// Defines the speed that a certain object will be atracted to the hand
        /// </summary>
        public float attractionSpeed = 1f;


        /// <summary>
        /// Defines if an object can be grabbed from no matter where once is grabbed
        /// </summary>
        public bool isManupulable = false;


        /// <summary>
        /// Is this catchable object using gravity
        /// </summary>
        //		public bool hasGravity;

        // TODO test if this works with a FBX that already has a root motion
        public Transform rootMotion;

        private Gaze_Handle handle;

        private float DISABLE_MANIPULATION_TIME = 1f;
        private static Vector3 NULL_VECTOR_3 = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        /// <summary>
        /// This bool is used to check if the object is being manipulated
        /// </summary>
        private bool isBeingManipulated = false;
        public bool IsBeingManipulated { get { return isBeingManipulated; } }

        private GameObject actualGrabPoint = null;
        //private Gaze_Handle handle;

        private Coroutine cancelManipulation;
        private Vector3 lastPosition;

        /// <summary>
        /// This flag defines when an object has been grabbed.
        /// </summary>
        private bool isBeingGrabbed = false;
        public bool IsBeingGrabbed { get { return isBeingGrabbed; } }

        /// <summary>
        /// If this is active the gameobject won't be detached of the hand
        /// </summary>
        public bool IsSticky { get { return isSticky; } }
        private bool isSticky = false;

        /// <summary>
        /// The current grab manager that is grabbing this object
        /// </summary>
        public Gaze_GrabManager GrabbingManager { get { return grabbingMananger; } }
        private Gaze_GrabManager grabbingMananger;

        public Gaze_GravityState ActualGravityState { get { return actualGravityState; } }
        private Gaze_GravityState actualGravityState;

        private Gaze_GravityState initialGravityState;

        public Gaze_Transform InitialTransform { get { return initialTransform; } }
        private Gaze_Transform initialTransform;

        private void Awake()
        {
            DetermineInitialGravityState();

            initialTransform = new Gaze_Transform(transform);

            if (rootMotion != null)
            {
                transform.SetParent(rootMotion);
            }
        }

        void Update()
        {
            Debug.Log(grab);
            Debug.Log(touch);
            if (rootMotion != null)
            {
                FollowRoot();
            }
        }

        private void OnEnable()
        {
            Gaze_InputManager.OnControllerGrabEvent += OnControllerGrabEvent;
        }

        private void OnDisable()
        {
            Gaze_InputManager.OnControllerGrabEvent -= OnControllerGrabEvent;
        }

        /// <summary>
        /// Changes the sticky state of a game object
        /// </summary>
        /// <param name="_isSticky">If true the object will be attached to the hand</param>
        /// <param name="_dropInmediatelly">If true the grabmanager will detach the object</param>
        public void SetStickykMode(bool _isSticky, bool _dropInmediatelly = true)
        {
            isSticky = _isSticky;
            if (_dropInmediatelly && !isSticky)
                grabbingMananger.TryDetach();
        }

        /// <summary>
        /// Used to update the grab state of this game s
        /// </summary>
        /// <param name="e"></param>
        public void OnControllerGrabEvent(Gaze_ControllerGrabEventArgs e)
        {
            // Mark the object as grabbed or ungrabbed
            if (e.ControllerObjectPair.Value == gameObject)
            {
                isBeingGrabbed = e.IsGrabbing;
            }

            // Handle all the manipulation states
            if (isBeingGrabbed)
            {
                grabbingMananger = (Gaze_GrabManager)e.Sender;
                rootMotion = grabbingMananger.grabPosition;
                if (isManupulable && !IsBeingManipulated)
                    SetManipulationMode(true);
                else if (IsBeingManipulated)
                    ContinueManipulation();
            }
            else
            {
                grabbingMananger = null;
                rootMotion = null;
                if (isManupulable)
                    SetManipulationMode(false);
            }
        }

        private void FollowRoot()
        {
            if (handle != null)
            {
                transform.localPosition = Vector3.zero + handle.transform.position;
            }
        }

        /// <summary>
        /// Returns the grab point in order to inform from where this object should be taken
        /// </summary>
        /// <returns></returns>
        public Vector3 GetGrabPoint()
        {
            Gaze_Handle handle = GetComponentInChildren<Gaze_Handle>();
            if (handle == null)
                Debug.LogAssertion("An interactive object should have a Gaze_Handle child.");
            Vector3 point = actualGrabPoint != null ? actualGrabPoint.transform.position : GetComponentInChildren<Gaze_Handle>().transform.position;
            return point - transform.position;
        }

        /// <summary>
        /// This method will be called when we need to change the grab point of the
        /// gaze interactive object.
        /// </summary>
        /// <param name="hit"></param>
        public void SetGrabPoint(Vector3 point)
        {
            if (actualGrabPoint != null)
                Destroy(actualGrabPoint);
            //actualGrabPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //actualGrabPoint.GetComponent<Collider>().enabled = false;
            //actualGrabPoint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            actualGrabPoint = new GameObject();
            actualGrabPoint.transform.position = point;
            actualGrabPoint.transform.parent = transform;
        }

        public void ContinueManipulation()
        {
            if (cancelManipulation == null) return;
            StopCoroutine(cancelManipulation);
            cancelManipulation = null;
        }

        /// <summary>
        /// In order to be able to manipulate a gaze interactive object we need to
        /// set the manipulation mode to on.
        /// </summary>
        /// <param name="isOn"></param>
        public void SetManipulationMode(bool isOn, bool inmeditelly = false)
        {
            if (isOn)
            {
                // TODO:
                isBeingManipulated = true;
            }
            else
            {
                lastPosition = transform.position;
                if (!inmeditelly)
                    cancelManipulation = StartCoroutine(DisableManipulationModeInTime());
                else
                {
                    RemoveManipulationData();
                }
            }
        }

        private IEnumerator DisableManipulationModeInTime()
        {
            yield return new WaitForSeconds(DISABLE_MANIPULATION_TIME);
            DisableManipulationMode();
        }

        private void RemoveManipulationData()
        {
            cancelManipulation = null;
            isBeingManipulated = false;
            if (actualGrabPoint != null)
                Destroy(actualGrabPoint);
            actualGrabPoint = null;
        }

        private void DisableManipulationMode()
        {

            if (Vector3.Distance(lastPosition, transform.position) <= 0.05f)
            {
                RemoveManipulationData();
            }
            else
            {
                lastPosition = transform.position;
                cancelManipulation = StartCoroutine(DisableManipulationModeInTime());
            }
        }

        /// <summary>
        /// Notify to everyone that this IO has been destroyed 
        /// </summary>
        private void OnDestroy()
        {
            Gaze_EventManager.FireOnIODestroyed(new Gaze_IODestroyEventArgs(this, this));
        }

        #region GravityManagement
        /// <summary>
        /// Checks the initial gravity state in order to return to it if required
        /// </summary>
        private void DetermineInitialGravityState()
        {
            Rigidbody rigidBody = GetRigitBodyOrError();
            if (rigidBody == null)
                return;

            if (rigidBody.isKinematic)
            {

                initialGravityState = rigidBody.useGravity ? Gaze_GravityState.ACTIVE_KINEMATIC : Gaze_GravityState.UNACTIVE_KINEMATIC;
            }
            else
            {
                initialGravityState = rigidBody.useGravity ? Gaze_GravityState.ACTIVE_NOT_KINEMATIC : Gaze_GravityState.UNACTIVE_NOT_KINEMATIC;
            }

            actualGravityState = initialGravityState;
        }

        // Make the IO not listen gravity requests
        public void LockGravity()
        {
            actualGravityState = Gaze_GravityState.LOCKED;
        }

        /// <summary>
        /// Make the IO listen for gravity requests.
        /// </summary>
        public void UnlockGravity()
        {
            Rigidbody rigidBody = GetRigitBodyOrError();
            if (rigidBody == null)
                return;

            if (rigidBody.isKinematic)
            {
                actualGravityState = rigidBody.useGravity ? Gaze_GravityState.ACTIVE_KINEMATIC : Gaze_GravityState.UNACTIVE_KINEMATIC;
            }
            else
            {
                actualGravityState = rigidBody.useGravity ? Gaze_GravityState.ACTIVE_NOT_KINEMATIC : Gaze_GravityState.UNACTIVE_NOT_KINEMATIC;
            }
        }

        /// <summary>
        /// Sets ONLY the gravity of a game object to true or false and changes
        /// its state.
        /// </summary>
        /// <param name="_hasGravity"></param>
        public void SetGravity(bool _hasGravity)
        {
            if (IsGravityLocked())
            {
                WarnUnauthorizedGravityChange();
                return;
            }


            Rigidbody rb = GetRigitBodyOrError();

            if (rb == null)
                return;
            rb.useGravity = _hasGravity;

            if (rb.isKinematic)
            {
                actualGravityState = _hasGravity ? Gaze_GravityState.ACTIVE_KINEMATIC : Gaze_GravityState.UNACTIVE_KINEMATIC;
            }
            else
            {
                actualGravityState = _hasGravity ? Gaze_GravityState.ACTIVE_NOT_KINEMATIC : Gaze_GravityState.UNACTIVE_NOT_KINEMATIC;
            }

        }

        /// <summary>
        /// Sets the gravity of a game object and also the kinematic state in order to 
        /// attach it or not.
        /// </summary>
        /// <param name="_hasGravity"></param>
        public void SetGravityAndAttach(bool _hasGravity)
        {
            if (IsGravityLocked())
            {
                WarnUnauthorizedGravityChange();
                return;
            }

            Rigidbody rb = GetRigitBodyOrError();

            if (rb == null)
                return;

            rb.isKinematic = !_hasGravity;
            rb.useGravity = _hasGravity;

            actualGravityState = _hasGravity ? Gaze_GravityState.ACTIVE_NOT_KINEMATIC : Gaze_GravityState.UNACTIVE_NOT_KINEMATIC;

        }

        public Rigidbody GetRigitBodyOrError()
        {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb == null && Gaze_GravityManager.SHOW_GRAVITY_WARNINGS)
            {
                Debug.LogWarning(String.Format("The interactive object {0} has not a rigidbody and it should, please add a rigidbody to it.", gameObject.name));
            }

            return rb;
        }

        /// <summary>
        /// This method is called if we detect gravity inconsistencies in order to prevent
        /// developpers to change the gravity directly, 
        /// </summary>
        public void WarnUnauthorizedGravityChange()
        {
            Debug.LogWarning("Gravity chages should be only performed by the GravityManager!");
        }

        public bool IsGravityLocked()
        {
            return actualGravityState == Gaze_GravityState.LOCKED;
        }

        public bool IsAffectedByGravity()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            return rb != null && !rb.isKinematic && rb.useGravity;
        }

        #endregion GravityManagement

        #region ManipulationManagement

        public void EnableManipulationMode(Gaze_ManipulationModes _manipulationMode)
        {
            ManipulationMode = _manipulationMode;
        }

        public void DisableManipulationMode(Gaze_ManipulationModes manipulationMode)
        {
            if (ManipulationMode == manipulationMode)
                ManipulationMode = Gaze_ManipulationModes.NONE;
        }

        #endregion ManipulationManagement
    }

}
