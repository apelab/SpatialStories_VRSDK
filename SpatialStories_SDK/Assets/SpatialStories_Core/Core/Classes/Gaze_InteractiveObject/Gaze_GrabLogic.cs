using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VR;

namespace Gaze
{
    public class Gaze_GrabLogic
    {
        #region constants

        /// <summary>
        /// USed to recalibrate the center of mass when the object is being manipulated.
        /// </summary>
        private const float CENTER_OF_MASS_CORRECTION_THRESHOLD = 0.05f;

        /// <summary>
        /// How many seconds the object will resist far of the point before dropping it.
        /// </summary>
        const float DEFAULT_TIME_UNTIL_DETACH = 1f;

        private float actualTimeUntilDetach = DEFAULT_TIME_UNTIL_DETACH;

        /// <summary>
        /// How much time will the object be considered in maniplation after 
        /// he is not moving anymore and it has been released.
        /// </summary>
        public float DISABLE_MANIPULATION_TIME = 1f;

        /// <summary>
        /// If the distance of the io to to the controler is more than this constant for more thanb
        /// a certain time it will be detached.
        /// </summary>
        public const float DEFAULT_DETACH_DISTANCE = 0.05f;

        private float actualDetachDistance = DEFAULT_DETACH_DISTANCE;

        /// <summary>
        /// This const determines how agressive will be the position correction to track an object
        /// as bigger as it gets more attached to the tracking point the IO will be.
        /// </summary>
        private const float VELOCITY_CONST = 6000f;

        /// <summary>
        /// How fast the rotation of the object will be corrected.
        /// </summary>
        private const float ANGULAR_VELOCITY_CONST = 100f;

        /// <summary>
        /// Expected delta time.
        /// </summary>
        private const float EXPECTED_DELTA_TIME = 0.0111f;

        /// <summary>
        /// Number of samples that will be used to calculate the throw parameters when an object is released.
        /// </summary>
        public const int SAMPLES = 21;

        private const float MAX_VELOCITY_CHANGE = 10f;
        private const float MAX_ANGULAR_VELOCITY_CHANGE = 20f;

        #endregion constants

        #region vars

        /// <summary>
        /// IO
        /// </summary>
        private Gaze_InteractiveObject owner;

        /// <summary>
        /// /History of angular velocities and positions usefull in the moment of calculating the throw force of an object
        /// </summary>
        public Vector3?[] AngularVelocityHistory = new Vector3?[SAMPLES];

        private int angularVelocityTrackingIndex = 0;
        public List<Vector3> PositionHistory = new List<Vector3>();

        /// <summary>
        /// As we are changing the center of mass in order to avoid flickering of long objects
        /// we need to have a way to go back to the initial center of mass.
        /// </summary>
        private Vector3 originalCenterOfMass;

        /// <summary>
        /// The current grab manager that is grabbing this object
        /// </summary>
        public Gaze_GrabManager GrabbingManager;

        /// <summary>
        /// This flag defines when an object has been grabbed.
        /// </summary>
        private bool isBeingGrabbed = false;

        public bool IsBeingGrabbed
        {
            get { return isBeingGrabbed; }
        }

        /// <summary>
        /// This bool is used to check if the object is being manipulated
        /// </summary>
        private bool isBeingManipulated = false;

        public bool IsBeingManipulated
        {
            get { return isBeingManipulated; }
        }

        /// <summary>
        /// It's the collider used to manipulate the object.
        /// </summary>
        public Collider ManipulabeHandle;

        /// <summary>
        /// Grab Point Used in ManipualtionF
        /// </summary>
        private GameObject actualGrabPoint = null;

        /// <summary>
        /// Stores if the object was kinematic before tacking it in order to revert its state when the
        /// object is dropped.
        /// </summary>
        private readonly bool isKinematicByDefault;

        private readonly Rigidbody rigidBody;

        /// <summary>
        /// USefull to check if the object hasn't move since the last frame (Manipulation)
        /// </summary>
        private Vector3 lastPosition;

        /// <summary>
        /// If the object is far of the grab point this var will decrease if it reaches 0 it will be detached.
        /// </summary>
        private float remainingTimeUntilDetach = DEFAULT_TIME_UNTIL_DETACH;

        /// <summary>
        /// Determiens where is the grab point of the actual grabbing controller
        /// </summary>
        private Transform controllerGrabLocation;

        bool IsBeingReleasedBecauseOfDistance = false;

        /// <summary>
        /// Determines where the object will be hold if it has not a grab positioner.
        /// </summary>
        private Gaze_Manipulation defaultHandle;

        public Gaze_Manipulation DefaultHandle
        {
            get
            {
                if (defaultHandle == null) defaultHandle = owner.GetComponentInChildren<Gaze_Manipulation>();
                return defaultHandle;
            }
        }

        protected Vector3 ExternalVelocity;
        protected Vector3 ExternalAngularVelocity;

        private bool firstCatch = true;

        #endregion vars

        public Gaze_GrabLogic(Gaze_InteractiveObject _owner)
        {
            owner = _owner;
            rigidBody = owner.GetRigitBodyOrError();
            if (rigidBody == null)
                return;
            originalCenterOfMass = rigidBody.centerOfMass;
            isKinematicByDefault = rigidBody.isKinematic;
            Time.fixedDeltaTime = EXPECTED_DELTA_TIME;
            rigidBody.drag = 0;
            rigidBody.angularDrag = 0.05f;
            rigidBody.maxAngularVelocity = MAX_ANGULAR_VELOCITY_CHANGE;
        }

        public void SubscribeToEvents()
        {
            Gaze_InputManager.OnControllerGrabEvent += OnControllerGrabEvent;
            Gaze_EventManager.OnTeleportEvent += OnTeleportEvent;
        }

        public void UnsubscribeToEvents()
        {
            Gaze_InputManager.OnControllerGrabEvent -= OnControllerGrabEvent;
            Gaze_EventManager.OnTeleportEvent -= OnTeleportEvent;
        }

        public void Update()
        {
            if (isBeingGrabbed && owner.ManipulationMode != Gaze_ManipulationModes.LEVITATE)
            {
                FollowHand();
                AddExternalVelocities();
            }

            lastPosition = owner.transform.position;
        }

        /// <summary>
        /// The IO will follow the grabbing controller
        /// </summary>
        public void FollowHand()
        {
            FollowPoint(controllerGrabLocation);
        }

        /// <summary>
        /// Sets the tolerance of distance between the following point and the object
        /// like that if the follow point goes through a wall the object won't begin the detach process
        /// until this ditance is reached
        /// </summary>
        /// <param name="_distance"></param>
        public void SetDistanceTolerance(float _distance)
        {
            actualDetachDistance = _distance;
        }

        /// <summary>
        /// Set the detach time since the distance has been superated.
        /// </summary>
        /// <param name="_timeTolerance"></param>
        public void SetTimeTolerance(float _timeTolerance)
        {
            actualTimeUntilDetach = _timeTolerance;
        }

        /// <summary>
        /// The IO will follow the specified transform
        /// </summary>
        /// <param name="_transformToFollow">The transform that the IO will follow (Ex: A controller)</param>
        /// <param name="_followOriginTransform"> Optional parameter to choose a custom follow origin point </param>
        public void FollowPoint(Transform _transformToFollow, Transform _followOriginTransform = null)
        {
            FollowPhysicPoint(_transformToFollow, _followOriginTransform);
            AddExternalVelocities();
        }

        /// <summary>
        /// TODO: Spit this method i little ones
        /// </summary>
        /// <param name="_transformToFollow"></param>
        /// <param name="_followOriginTransform"></param>
        private void FollowPhysicPoint(Transform _transformToFollow, Transform _followOriginTransform = null)
        {
            Quaternion rotationDelta;
            Vector3 positionDelta;
            Vector3 axis;
            Vector3 desiredPosition;

            float angle;
            float velocityMagic = VELOCITY_CONST / (Time.fixedDeltaTime / EXPECTED_DELTA_TIME);
            float angularVelocityMagic = ANGULAR_VELOCITY_CONST / (Time.fixedDeltaTime / EXPECTED_DELTA_TIME);

            if (_followOriginTransform != null)
            {
                rotationDelta = _transformToFollow.transform.rotation *
                                Quaternion.Inverse(_followOriginTransform.transform.rotation);
                desiredPosition = _transformToFollow.transform.position - _followOriginTransform.transform.position;
                positionDelta = desiredPosition;
            }
            else if (owner.SnapOnGrab && owner.GrabPositionnerCollider == null)
            {
                rotationDelta = _transformToFollow.transform.rotation *
                                Quaternion.Inverse(GetSnapPointForHand(GrabbingManager.isLeftHand).transform.rotation);
                desiredPosition = _transformToFollow.transform.position -
                                  GetSnapPointForHand(GrabbingManager.isLeftHand).transform.position;
                positionDelta = desiredPosition;
            }
            else if (owner.GrabPositionnerCollider != null)
            {
                rotationDelta = _transformToFollow.transform.rotation *
                                Quaternion.Inverse(owner.GrabPositionnerCollider.transform.rotation);
                desiredPosition = _transformToFollow.transform.position -
                                  owner.GrabPositionnerCollider.transform.position;
                positionDelta = desiredPosition;
            }
            else
            {
                rotationDelta = _transformToFollow.transform.rotation *
                                Quaternion.Inverse(DefaultHandle.transform.rotation);
                desiredPosition = _transformToFollow.transform.position -
                                  DefaultHandle.transform.position;
                positionDelta = desiredPosition;
            }

            rotationDelta.ToAngleAxis(out angle, out axis);

            if (angle > 180)
                angle -= 360;

            if (Math.Abs(angle) > 0.01f)
            {
                Vector3 angularTarget = angle * axis;
                if (float.IsNaN(angularTarget.x) == false)
                {
                    angularTarget = (angularTarget * angularVelocityMagic) * Time.fixedDeltaTime;
                    rigidBody.angularVelocity = Vector3.MoveTowards(rigidBody.angularVelocity, angularTarget,
                        MAX_ANGULAR_VELOCITY_CHANGE);
                }
            }

            Vector3 velocityTarget = (positionDelta * velocityMagic) * Time.fixedDeltaTime;
            if (float.IsNaN(velocityTarget.x) == false)
            {
                rigidBody.velocity = Vector3.MoveTowards(rigidBody.velocity, velocityTarget, MAX_VELOCITY_CHANGE);
            }

            if (PositionHistory != null)
            {
                angularVelocityTrackingIndex++;

                if (angularVelocityTrackingIndex >= AngularVelocityHistory.Length)
                    angularVelocityTrackingIndex = 0;

                AngularVelocityHistory[angularVelocityTrackingIndex] = rigidBody.angularVelocity;

                if (PositionHistory.Count > SAMPLES)
                    PositionHistory.RemoveAt(PositionHistory.Count() - 1);
                PositionHistory.Insert(0, _transformToFollow.transform.position);
            }

            if (owner.ManipulationMode != Gaze_ManipulationModes.LEVITATE)
            {
                if (owner.SnapOnGrab)
                {
                    if (owner.GrabPositionnerCollider == null)
                    {
                        if (Vector3.Distance(controllerGrabLocation.position,
                                GetSnapPointForHand(GrabbingManager.isLeftHand).transform.position) >
                            actualDetachDistance)
                        {
                            remainingTimeUntilDetach -= Time.fixedDeltaTime;
                            if (remainingTimeUntilDetach <= 0)
                                StopGrabbing(true);
                        }
                    }
                    else
                    {
                        if (!owner.SnapOnGrab &&
                            Vector3.Distance(controllerGrabLocation.position,
                                owner.GrabPositionnerCollider.transform.position) > actualDetachDistance)
                        {
                            remainingTimeUntilDetach -= Time.fixedDeltaTime;
                            if (remainingTimeUntilDetach <= 0)
                                StopGrabbing(true);
                        }
                    }
                }
                else if (owner.GrabPositionnerCollider != null && (!owner.SnapOnGrab &&
                                                                   Vector3.Distance(controllerGrabLocation.position,
                                                                       owner.GrabPositionnerCollider.transform.position) > actualDetachDistance))
                {
                    remainingTimeUntilDetach -= Time.fixedDeltaTime;
                    if (remainingTimeUntilDetach <= 0)
                        StopGrabbing(true);
                }
                else if (owner.GrabPositionnerCollider == null && Vector3.Distance(controllerGrabLocation.position,
                             DefaultHandle.transform.position) > actualDetachDistance)
                {
                    remainingTimeUntilDetach -= Time.fixedDeltaTime;
                    if (remainingTimeUntilDetach <= 0)
                        StopGrabbing(true);
                }
                else
                    remainingTimeUntilDetach = actualTimeUntilDetach;
            }
            else
            {
                Transform desiredPos = _followOriginTransform == null
                    ? defaultHandle.transform
                    : _followOriginTransform;
                if (Vector3.Distance(desiredPos.transform.position, _transformToFollow.transform.position) >
                    actualDetachDistance)
                {
                    remainingTimeUntilDetach -= Time.fixedDeltaTime;
                    if (remainingTimeUntilDetach <= 0)
                        StopGrabbing(true);
                }
                else
                    remainingTimeUntilDetach = actualTimeUntilDetach;
            }
        }


        /// <summary>
        /// Fires the grab stop event in order interrupt the grabbing of an object
        /// </summary>
        public void StopGrabbing(bool _tooFar)
        {
            if (_tooFar)
            {
                IsBeingReleasedBecauseOfDistance = true;
            }

            KeyValuePair<VRNode, GameObject> dico =
                new KeyValuePair<VRNode, GameObject>(GrabbingManager.isLeftHand ? VRNode.LeftHand : VRNode.RightHand,
                    owner.gameObject);
            Gaze_InputManager.FireControllerGrabEvent(new Gaze_ControllerGrabEventArgs(GrabbingManager, dico, false));
        }

        public void AddDynamicGrabPositioner(Vector3 _hitPoint, Gaze_GrabManager _grabManager)
        {
            if (!owner.IsManipulable)
                return;

            // If we dont have a grab position collider just create one.
            if (owner.GrabPositionnerCollider == null)
            {
                GameObject go = new GameObject("Dynamic Grab Point");
                owner.GrabPositionnerCollider = go.AddComponent<BoxCollider>();
                owner.GrabPositionnerCollider.isTrigger = true;
                go.transform.SetParent(owner.transform);
            }

            owner.GrabPositionnerCollider.transform.position = _hitPoint;
            owner.GrabPositionnerCollider.transform.forward = _hitPoint - Camera.main.transform.position;
            _grabManager.gameObject.GetComponentInChildren<Gaze_GrabPositionController>().transform.forward =
                owner.GrabPositionnerCollider.transform.forward;
        }

        /// <summary>
        /// Clears the history of data needed to trow an object. 
        /// </summary>
        protected virtual void CleanPositionsAndVelocityHistory()
        {
            angularVelocityTrackingIndex = 0;
            PositionHistory.Clear();
            AngularVelocityHistory = new Vector3?[SAMPLES];
        }

        /// <summary>
        /// Returns the grab point in order to inform from where this object should be taken
        /// </summary>
        /// <returns></returns>
        public Vector3 GetGrabPoint()
        {
            Gaze_Manipulation handle = owner.GetComponentInChildren<Gaze_Manipulation>();
            if (handle == null)
                Debug.LogAssertion("An interactive object should have a Gaze_Handle child.");
            Vector3 point = actualGrabPoint != null
                ? actualGrabPoint.transform.position
                : owner.GetComponentInChildren<Gaze_Manipulation>().transform.position;
            return point - owner.transform.position;
        }

        /// <summary>
        /// This method will be called when we need to change the grab point of the
        /// gaze interactive object.
        /// </summary>
        /// <param name="_point"></param>
        public void SetGrabPoint(Vector3 _point)
        {
            if (actualGrabPoint != null)
                GameObject.Destroy(actualGrabPoint);

            actualGrabPoint = new GameObject();
            actualGrabPoint.transform.position = _point;
            actualGrabPoint.transform.parent = owner.transform;
        }


        /// <summary>
        /// In order to be able to manipulate a gaze interactive object we need to
        /// set the manipulation mode to on.
        /// </summary>
        /// <param name="_isOn">If set to false the object wont be considered as being manipulated</param>
        /// <param name="_inmeditelly"> is this going to happen now or it will wait the default time </param>
        public void SetManipulationMode(bool _isOn, bool _inmeditelly = false)
        {
            // Don't allow to change the manipulation mode if the object is not manipulable
            if (!owner.IsManipulable)
                return;

            if (_isOn)
                isBeingManipulated = true;
            else
            {
                if (!_inmeditelly)
                    owner.CancelManipulation = owner.StartCoroutine(owner.DisableManipulationModeInTime());
                else
                    CleanManipulationData();
            }
        }

        /// <summary>
        /// Once the object stops being manipulated we need to clean all the variables
        /// </summary>
        private void CleanManipulationData()
        {
            owner.CancelManipulation = null;
            isBeingManipulated = false;

            if (actualGrabPoint != null)
                GameObject.Destroy(actualGrabPoint);

            actualGrabPoint = null;

            if (owner.GrabPositionnerCollider != null)
                owner.GrabPositionnerCollider = null;
        }

        /// <summary>
        /// The object won't be manipulable after the call of this method.
        /// </summary>
        public void DisableManipulationMode()
        {
            if (Vector3.Distance(lastPosition, owner.transform.position) <= 0.05f && !IsBeingGrabbed)
                CleanManipulationData();
            else
                owner.CancelManipulation = owner.StartCoroutine(owner.DisableManipulationModeInTime());
        }


        /// <summary>
        /// Used to throw an object.
        /// </summary>
        protected virtual void ThrowObject()
        {
            if (PositionHistory.Count < SAMPLES)
                return;

            if (IsBeingReleasedBecauseOfDistance)
            {
                rigidBody.velocity = Vector3.zero;
                rigidBody.angularVelocity = Vector3.zero;
            }
            else
            {
                //Help the user a little bit if he is not levitating the object
                float userHelp = owner.ManipulationMode != Gaze_ManipulationModes.LEVITATE ? 2.6f : 1.0f;

                // Throw Impulse
                float meanAcceleration = GetMeanVelocity() * userHelp;

                Vector3 direction;
                if (Gaze_InputManager.instance.trackPosition)
                    direction = owner.transform.position - PositionHistory[PositionHistory.Count - 1];
                else
                {
                    if (controllerGrabLocation != null)
                        direction = controllerGrabLocation.transform.forward.normalized;
                    else
                        direction = GrabbingManager.transform.forward.normalized;
                }

                rigidBody.velocity = direction * meanAcceleration * Time.deltaTime;

                // Angular Velocity
                Vector3? meanAngularVelocity = GetMeanVector(AngularVelocityHistory);
                if (meanAngularVelocity != null)
                {
                    rigidBody.angularVelocity = meanAngularVelocity.Value;
                }
            }


        }

        /// <summary>
        /// Gets the mean velocity of the io by using the history 
        /// </summary>
        /// <returns></returns>
        private float GetMeanVelocity()
        {
            List<float> velocities = new List<float>();
            for (int i = 0; i < PositionHistory.Count - 5; i++)
            {
                velocities.Add(Vector3.Distance(PositionHistory[i], PositionHistory[i + 1]) /
                               Mathf.Pow(Time.deltaTime, 2));
            }

            return velocities.Average();
        }


        /// <summary>
        /// Gets the mean of an array of vectors
        /// </summary>
        /// <param name="_positions"></param>
        /// <returns></returns>
        protected Vector3? GetMeanVector(Vector3?[] _positions)
        {
            float x = 0f;
            float y = 0f;
            float z = 0f;

            int count = 0;
            for (int index = 0; index < _positions.Length; index++)
            {
                if (_positions[index] != null)
                {
                    x += _positions[index].Value.x;
                    y += _positions[index].Value.y;
                    z += _positions[index].Value.z;

                    count++;
                }
            }

            if (count > 0)
            {
                return new Vector3(x / count, y / count, z / count);
            }

            return null;
        }


        protected virtual void AddExternalVelocities()
        {
            if (ExternalVelocity != Vector3.zero)
            {
                this.rigidBody.velocity = Vector3.Lerp(this.rigidBody.velocity, ExternalVelocity, 0.5f);
                ExternalVelocity = Vector3.zero;
            }

            if (ExternalAngularVelocity == Vector3.zero) return;
            this.rigidBody.angularVelocity = Vector3.Lerp(this.rigidBody.angularVelocity, ExternalAngularVelocity,
                0.5f);
            ExternalAngularVelocity = Vector3.zero;
        }

        public void AddExternalVelocity(Vector3 _velocity)
        {
            ExternalVelocity = ExternalVelocity == Vector3.zero
                ? _velocity
                : Vector3.Lerp(ExternalVelocity, _velocity, 0.5f);
        }

        public void AddExternalAngularVelocity(Vector3 _angularVelocity)
        {
            ExternalAngularVelocity = ExternalAngularVelocity == Vector3.zero
                ? _angularVelocity
                : Vector3.Lerp(ExternalAngularVelocity, _angularVelocity, 0.5f);
        }

        #region EventHandlers

        public void OnTeleportEvent(Gaze_TeleportEventArgs args)
        {
            if (IsBeingGrabbed)
            {
                owner.transform.position = controllerGrabLocation.transform.position;
                Camera cam = Gaze_InputManager.instance.GetComponentInChildren<Camera>();
                Ray ray = new Ray(cam.transform.position, owner.transform.position - cam.transform.position);
                RaycastHit[] hits = Physics.RaycastAll(ray,
                    Vector3.Distance(cam.transform.position, owner.transform.position));
                foreach (RaycastHit hit in hits)
                {
                    if (!hit.collider.isTrigger)
                    {
                        owner.transform.position = Gaze_InputManager.instance.gameObject
                            .GetComponentInChildren<Gaze_PlayerTorso>().transform.position;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Used to update the grab state of this game s
        /// </summary>
        /// <param name="e"></param>
        public void OnControllerGrabEvent(Gaze_ControllerGrabEventArgs _e)
        {
            // Mark the object as grabbed or ungrabbed
            if (_e.ControllerObjectPair.Value != owner.gameObject) return;

            isBeingGrabbed = _e.IsGrabbing;
            // Handle all the manipulation states
            if (isBeingGrabbed)

                GrabObject(_e);
            else
                ReleaseObject();
        }


        /// <summary>
        /// Performs all the opearations needed to grab an object after
        /// the grab event has been received
        /// </summary>
        /// <param name="_e"></param>
        private void GrabObject(Gaze_ControllerGrabEventArgs _e)
        {
            Debug.ClearDeveloperConsole();
            GrabbingManager = (Gaze_GrabManager)_e.Sender;

            if (owner.IsManipulable && !IsBeingManipulated)
                SetManipulationMode(true);

            else if (IsBeingManipulated)
                owner.ContinueManipulation();

            // This will serve the object as guide to know how to follow the controllers hand
            controllerGrabLocation = GrabbingManager.GetComponentInChildren<Gaze_GrabPositionController>().transform;
            Gaze_GravityManager.ChangeGravityState(owner, Gaze_GravityRequestType.ACTIVATE_AND_DETACH);

            rigidBody.useGravity = false;
            rigidBody.maxAngularVelocity = 100;

            SetDistanceTolerance(DEFAULT_DETACH_DISTANCE);
            SetTimeTolerance(DEFAULT_TIME_UNTIL_DETACH);

            rigidBody.drag = 0;
            rigidBody.angularDrag = 0.05f;


            originalCenterOfMass = rigidBody.centerOfMass;

            if (owner.HasGrabPositionner)
            {
                rigidBody.centerOfMass = owner.GrabPositionnerCollider.transform.localPosition;
            }
            else
            {
                rigidBody.centerOfMass = DefaultHandle.transform.localPosition;
            }


            isCenterOfMassAlreadyCorrected = false;

            IsBeingReleasedBecauseOfDistance = false;
        }

        bool isCenterOfMassAlreadyCorrected = false;

        /// <summary>
        /// Perform all the operations needed after the release of an object has been received from an 
        /// event
        /// </summary>
        private void ReleaseObject()
        {
            remainingTimeUntilDetach = actualTimeUntilDetach;

            if (owner.IsManipulable)
                SetManipulationMode(false);

            if (isKinematicByDefault)
                Gaze_GravityManager.ChangeGravityState(owner, Gaze_GravityRequestType.DEACTIVATE_AND_ATTACH);
            else
            {
                if (rigidBody != null)
                    rigidBody.useGravity = true;
                ThrowObject();
            }

            if (rigidBody)
            {
                rigidBody.centerOfMass = originalCenterOfMass;
            }

            GrabbingManager = null;
            controllerGrabLocation = null;
            Gaze_GravityManager.ChangeGravityState(owner, Gaze_GravityRequestType.RETURN_TO_DEFAULT);
            CleanPositionsAndVelocityHistory();
        }

        private Transform leftSnapPoint, rigtSnapPoint;

        public Transform GetSnapPointForHand(bool _isLeftHand)
        {
            if ((leftSnapPoint == null && Gaze_InputManager.instance.LeftHandActive) ||
                (rigtSnapPoint == null && Gaze_InputManager.instance.RightHandActive))
            {
                Gaze_SnapPosition[] positions = owner.GetComponentsInChildren<Gaze_SnapPosition>();
                foreach (Gaze_SnapPosition position in positions)
                {
                    if (position.ActualHand == Gaze_SnapPosition.HAND.LEFT)
                        leftSnapPoint = position.transform;
                    else
                        rigtSnapPoint = position.transform;
                }

                if (leftSnapPoint == null && Gaze_InputManager.instance.LeftHandActive)
                {
                    Debug.LogError(string.Format("{0} needs an snap left point to be grabbable, " +
                                                 "setting the object transform as a default left snap point",
                        owner.name));
                    leftSnapPoint = owner.transform;
                }
                if (rigtSnapPoint == null && Gaze_InputManager.instance.RightHandActive)
                {
                    Debug.LogError(string.Format("{0} needs an snap right point to be grabbable, " +
                                                 "setting the object transform as a default left snap point",
                        owner.name));
                    rigtSnapPoint = owner.transform;
                }
            }
            return _isLeftHand ? leftSnapPoint : rigtSnapPoint;
        }

        #endregion EventHandlers
    }
}
