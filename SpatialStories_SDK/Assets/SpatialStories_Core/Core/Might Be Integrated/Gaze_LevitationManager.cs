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
    public class Gaze_LevitationManager : MonoBehaviour
    {
        public float DetachDistance = 0.5f;
        public float DetachTime = 3f;

        #region members
        [Tooltip("The duration in seconds to charge the levitation")]
        public float chargeDuration = .1f;
        [Tooltip("The amount of force (speed) used to center the levitated object at gaze position")]
        public float attractionForce = 3f;

        [Tooltip("The amount of force (speed) used to pull/push the levitated object")]
        public float levitatingObjectAttractingSpeed = 5f;

        // Used for the ease in / out of the object
        public float TimePressingToReachMaxSpeed = 1.0f;
        private float actualTimePressingTrigger = 0.0f;
        float timeStoppedPushAndPull = 0.0f;


        [Tooltip("The closest distance the object can be from the controller")]
        public float closestDistance = 1f;
        public float beamSplineSmoothness = .1f;
        public AudioClip beamSoundClip;
        [Range(0f, 1f)]
        public float audioFXVolume = 1f;
        public int beamNumberOfControlPoints = 5;
        public float beamWidth = .005f;
        public float beamVerticalOffsetPosition = .02f;
        public float beamOffsetSpeed = 10f, beamOffsetAmplitude = .02f;
        public Material beamMaterial;
        public float beamTextureSpeed = .4f;
        public Color dropOffStartColor = Color.white, dropOffEndColor = Color.white;
        public Color dropOnStartColor = Color.white, dropOnEndColor = Color.green;
        public float pointerDiameter = .05f;
        public Color pointerColor = Color.black;
        public Material pointerMaterial;
        public float dropReadyFeedbackDuration = .3f;
        public bool active = true;
        public bool debug = false;

        private bool isLeftHand;
        private GameObject controllerLeft, controllerRight;
        private Gaze_HandsEnum actualHand;

        public GameObject TargetLocation { get { return targetLocation; } }
        private GameObject targetLocation;

        private GameObject objectToLevitate;
        private float chargeStartTime;
        private bool isCharged;
        private bool isCharging;
        private bool isControllerTrigger;
        private LineRenderer beam;
        private Vector3 oldPos, newPos;
        private Vector3 objectToLevitateVelocity;
        private float objectDistance;
        private Vector3 beamEndPosition;
        private float chargeProgress;
        private Vector3[] beamControlPoints, beamSplinePoints;
        private Renderer[] originalLevitateRenderers;
        private Gaze_Conditions gazable;
        private GameObject visuals;
        private Renderer[] visualsRenderers;
        private float startCameraHandsDistance;
        private float levitatingObjectDistance;
        private float pushPullDelta;
        private float distanceHeadHands;
        /// <summary>
        /// The distance between the hands' midpoint and the torso (attached below the camera)
        /// </summary>
        private float torsoDistance;
        private bool controllersFound;
        private Vector3 beamStartPosition;
        private Gaze_LevitationStates levitationState;
        private Vector3 hitPosition;

        public GameObject AttachPoint { get { return attachPoint; } }
        private GameObject attachPoint;

        private Color attachPointColor;
        private Transform handLocation;
        private bool dropReady;
        private IEnumerator updateBeamColorRoutine;

        // Interaction physX vars
        private Gaze_InteractiveObject IOToLevitate;
        private GameObject DynamicLevitationPoint;

        // The current drag and drop manager
        private Gaze_DragAndDropManager currentDragAndDropManager;

        // Is this object being controlled by the drag and drop manager (Snap on Drop)
        private bool snappedByDragAndDrop = false;

        // This is the offset distance between the Dynamic levitaion point and the object position
        private float snappedTolerance = 0f;

        public static List<Gaze_LevitationManager> LevitationManagers = new List<Gaze_LevitationManager>();
        private GameObject OriginPoint;
        public GameObject OriginParticlesPrefab;
        public GameObject EndPointPrefab;

        #endregion

        void OnEnable()
        {
            actualHand = GetComponentInChildren<Gaze_GrabManager>().isLeftHand ? Gaze_HandsEnum.LEFT : Gaze_HandsEnum.RIGHT;

            Gaze_EventManager.OnLevitationEvent += OnLevitationEvent;
            Gaze_EventManager.OnDragAndDropEvent += OnDragAndDropEvent;

            // TODO: Discriminate the hand that we are using
            Gaze_InputManager.OnControllerGrabEvent += OnControllerGrabEvent;

            if (actualHand == Gaze_HandsEnum.LEFT)
            {
                Gaze_InputManager.OnPadLeftTouchDownEvent += OnPadRightTouchDownEvent;
                Gaze_InputManager.OnPadLeftTouchUpEvent += OnPadRightTouchUpEvent;
                Gaze_InputManager.OnLeftTouchpadEvent += OnRightTouchpadEvent;
            }
            else
            {
                Gaze_InputManager.OnPadRightTouchDownEvent += OnPadRightTouchDownEvent;
                Gaze_InputManager.OnPadRightTouchUpEvent += OnPadRightTouchUpEvent;
                Gaze_InputManager.OnRightTouchpadEvent += OnRightTouchpadEvent;
            }

            Gaze_HandsReplacer.OnHandsReplaced += Gaze_HandsReplacer_OnHandsReplaced;

            transform.gameObject.AddComponent<AudioSource>();
            GetComponent<AudioSource>().playOnAwake = false;
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().clip = beamSoundClip;
            GetComponent<AudioSource>().volume = audioFXVolume;
            LevitationManagers.Add(this);
        }

        private void Gaze_HandsReplacer_OnHandsReplaced(Gaze_GrabManager grabManager, Transform GrabTarget, GameObject DistantGrabObject)
        {
            if ((actualHand == Gaze_HandsEnum.LEFT) == grabManager.isLeftHand)
            {
                handLocation = GrabTarget;
            }
        }

        void OnDisable()
        {
            Gaze_EventManager.OnLevitationEvent -= OnLevitationEvent;
            Gaze_EventManager.OnDragAndDropEvent -= OnDragAndDropEvent;

            Gaze_InputManager.OnControllerGrabEvent -= OnControllerGrabEvent;

            if (actualHand == Gaze_HandsEnum.LEFT)
            {
                Gaze_InputManager.OnPadLeftTouchDownEvent -= OnPadRightTouchDownEvent;
                Gaze_InputManager.OnPadLeftTouchUpEvent -= OnPadRightTouchUpEvent;
                Gaze_InputManager.OnLeftTouchpadEvent -= OnRightTouchpadEvent;
            }
            else
            {
                Gaze_InputManager.OnPadRightTouchDownEvent -= OnPadRightTouchDownEvent;
                Gaze_InputManager.OnPadRightTouchUpEvent -= OnPadRightTouchUpEvent;
                Gaze_InputManager.OnRightTouchpadEvent -= OnRightTouchpadEvent;
            }
            Gaze_HandsReplacer.OnHandsReplaced -= Gaze_HandsReplacer_OnHandsReplaced;
            LevitationManagers.Remove(this);
        }

        private void OnDestroy()
        {
            if (beamMaterial)
                beamMaterial.SetTextureOffset("_MainTex", new Vector2(0f, 0f));
        }

        void Start()
        {
            isLeftHand = GetComponentInChildren<Gaze_GrabManager>().isLeftHand;
            actualHand = GetComponentInChildren<Gaze_GrabManager>().isLeftHand ? Gaze_HandsEnum.LEFT : Gaze_HandsEnum.RIGHT;
            isControllerTrigger = false;
            targetLocation = transform.Find("Levitation Target").gameObject;

            beam = gameObject.AddComponent<LineRenderer>();
            beam.receiveShadows = false;
            beam.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            beam.material = beamMaterial;
            beam.startColor = dropOffStartColor;
            beam.endColor = dropOffEndColor;
            beam.startWidth = .001f;
            beam.endWidth = beamWidth;

            ResetCharge();

            // the number of points for the spline is total number of control points (minus 3 to avoid first and 2 lasts) divided by step distance
            beamSplinePoints = new Vector3[(int)((beamNumberOfControlPoints - 3) / beamSplineSmoothness + 1)];
            Gaze_CatmullRomSpline.StepDistance = beamSplineSmoothness;
            Gaze_CatmullRomSpline.SplinePoints = beamSplinePoints;

            levitationState = Gaze_LevitationStates.NEUTRAL;

            StartCoroutine(FindControllers());

            CreateAttachPoint();

            handLocation = GetComponentInChildren<Gaze_DistantGrabPointer>().transform;

            active = true;
        }


        void FixedUpdate()
        {

            if (IOToLevitate != null && IOToLevitate.GrabLogic.GrabbingManager != null && IOToLevitate.GrabLogic.GrabbingManager.isLeftHand != isLeftHand)
                return;

            if ((!isCharged && !isCharging) && isControllerTrigger)
            {
                StartCoroutine(Charge());
            }

            if (!isCharged) return;
            if (!isControllerTrigger)
                ResetCharge();
            else
            {
                if (IOToLevitate != null && IOToLevitate.ManipulationMode == Gaze_ManipulationModes.LEVITATE)
                    Levitate();
                else
                {
                    Gaze_EventManager.FireLevitationEvent(new Gaze_LevitationEventArgs(this, IOToLevitate.gameObject, Gaze_LevitationTypes.LEVITATE_STOP, actualHand));
                    isControllerTrigger = false;
                }
            }
        }

        private IEnumerator UpdateBeamFeedback(bool _dropReady)
        {
            //Color attachPointColor = attachPoint.GetComponent<Renderer>().material.color;

            if (_dropReady)
            {
                beam.startColor = dropOnStartColor;
                beam.endColor = dropOnEndColor;
                attachPoint.GetComponent<Renderer>().material.color = dropOnEndColor;
            }
            else
            {
                beam.startColor = dropOffStartColor;
                beam.endColor = dropOffEndColor;
                attachPoint.GetComponent<Renderer>().material.color = dropOffEndColor;
            }
            yield return null;
        }

        private IEnumerator FindControllers()
        {
            yield return new WaitForSeconds(1f);
            //controllerLeft = GameObject.FindWithTag("Controller (left)");
            //controllerRight = GameObject.FindWithTag("Controller (right)");
            Gaze_HandController[] handControllers = Camera.main.GetComponentInParent<Gaze_InputManager>().GetComponentsInChildren<Gaze_HandController>();
            for (int i = 0; i < handControllers.Length; i++)
            {
                if (handControllers[i].leftHand)
                {
                    controllerLeft = handControllers[i].gameObject;

                }
                else
                {
                    controllerRight = handControllers[i].gameObject;
                }
            }
        }

        private void CreateAttachPoint()
        {
            if (attachPoint != null)
                Destroy(attachPoint);


            attachPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(attachPoint.GetComponent<Collider>());
            attachPoint.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            attachPoint.name = " - Target";
            attachPoint.GetComponent<Renderer>().enabled = false;

            if (EndPointPrefab == null)
            {
                attachPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                
                attachPoint.transform.localScale = new Vector3(pointerDiameter, pointerDiameter, pointerDiameter);
                attachPoint.GetComponent<SphereCollider>().isTrigger = true;
            }
            else
            {
                attachPoint = GameObject.Instantiate(EndPointPrefab);
            }

            if (OriginParticlesPrefab == null)
            {
                OriginPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                OriginPoint.GetComponent<SphereCollider>().isTrigger = true;
                OriginPoint.transform.localScale = new Vector3(pointerDiameter, pointerDiameter, pointerDiameter);
            }
            else
            {
                OriginPoint = GameObject.Instantiate(OriginParticlesPrefab);
                OriginPoint.name = "Origin Point";
            }
            OriginPoint.SetActive(false);
            attachPoint.SetActive(false);

        }

        private IEnumerator Charge()
        {
            #region INIT
            beam.positionCount = 2;
            isCharging = true;
            chargeStartTime = Time.time;
            #endregion

            CreateAttachPoint();
            OriginPoint.SetActive(true);
            attachPoint.SetActive(true);

            #region CHARGING
            GetComponent<AudioSource>().Play();
            while (Time.time - chargeStartTime < chargeDuration)
            {
                UpdateBeamLength();
                yield return null;
            }
            #endregion

            #region CHARGE FINISHED
            isCharged = true;
            isCharging = false;

            // the 5 control points for the straight line
            beamControlPoints = new Vector3[beamNumberOfControlPoints];

            // add vertices to be able to curve the line
            beam.positionCount = beamSplinePoints.Length;

            // Set the beam points correctly
            for (int i = 0; i < beamSplinePoints.Length; i++)
            {
                beam.SetPosition(i, beamSplinePoints[i]);
            }

            // set pointer position
            SetAttachPoint();

            // set the distance from controller to object to levitate to the target levitation object
            SetTargetLocation();

            // initialize start position for velocity computations
            oldPos = attachPoint.transform.position;

            IOToLevitate.GrabLogic.SetDistanceTolerance(DetachDistance);
            IOToLevitate.GrabLogic.SetTimeTolerance(DetachTime);

            Gaze_EventManager.FireLevitationEvent(new Gaze_LevitationEventArgs(this, IOToLevitate.gameObject, Gaze_LevitationTypes.LEVITATE_START, actualHand));
            #endregion


        }

        private void SetAttachPoint()
        {
            // set position of the pointer to the hit point
            attachPoint.transform.position = hitPosition;

            // show attach point
            attachPoint.GetComponent<Renderer>().enabled = true;
        }

        private void ClearAttachPoint()
        {
            // reset pointer when levitation finished
            if (attachPoint)
                attachPoint.GetComponent<Renderer>().enabled = false;
        }

        private void SetTargetLocation()
        {
            // position at hand's location
            targetLocation.transform.localPosition = handLocation.localPosition;

            // find delta between object and hand
            float distance = Vector3.Distance(handLocation.position, attachPoint.transform.position);

            // add distance between object and hand in forward direction of the target location
            targetLocation.transform.localPosition = new Vector3(targetLocation.transform.localPosition.x, targetLocation.transform.localPosition.y, targetLocation.transform.localPosition.z + distance);
        }

        private void TrySwitchGravity(GameObject _object, bool _activateGravity)
        {
            if (_object == null || _object.GetComponentInParent<Gaze_InteractiveObject>() == null)
                return;

            Gaze_InteractiveObject IO = _object.GetComponentInParent<Gaze_InteractiveObject>();

            if (_activateGravity)
                Gaze_GravityManager.ChangeGravityState(IO, Gaze_GravityRequestType.RETURN_TO_DEFAULT);
            else
                Gaze_GravityManager.ChangeGravityState(IO, Gaze_GravityRequestType.DEACTIVATE_AND_ATTACH);
        }

        private Vector3 getVectorPointAtDistance(Vector3 _vectorA, Vector3 _vectorB, float _distanceFromVectorAPercentage)
        {
            return _vectorA + (_distanceFromVectorAPercentage * (_vectorB - _vectorA));
        }

        private void UpdateBeamLength()
        {
            if (objectToLevitate != null)
            {
                if (!handLocation)
                    handLocation = GetComponentInChildren<Gaze_DistantGrabPointer>().transform;

                objectDistance = Vector3.Distance(attachPoint.transform.localPosition, handLocation.position);
                chargeProgress = (Time.time - chargeStartTime) / chargeDuration;
                beamEndPosition = handLocation.position + ((hitPosition - handLocation.position) * chargeProgress);

                beam.SetPosition(0, handLocation.position);
                beam.SetPosition(1, beamEndPosition);
            }
        }

        private float GetChargeProgression()
        {
            objectDistance = Vector3.Distance(attachPoint.transform.localPosition, handLocation.position);
            return (Time.time - chargeStartTime) / chargeDuration;
        }

        private void Levitate()
        {
            if (IOToLevitate == null) return;

            beam.enabled = true;

            // beam update
            UpdateBeamControlPoints();
            beamSplinePoints = Gaze_CatmullRomSpline.getSplinePoints(beamControlPoints);
            for (int i = 0; i < beamSplinePoints.Length; i++)
            {
                beam.SetPosition(i, beamSplinePoints[i]);
            }

            AnimateBeamTexture();

            // push / pull levitating object if needed
            PullPush();

            // move object to target's position
            attachPoint.transform.position = Vector3.Lerp(attachPoint.transform.position, targetLocation.transform.position, Time.fixedDeltaTime * attractionForce);

            // rotate attach point towards the controller
            attachPoint.transform.LookAt(handLocation);

            // set beam's length
            if(OriginPoint != null)
                OriginPoint.transform.position = handLocation.position;

            // get object's velocity
            newPos = attachPoint.transform.position;
            objectToLevitateVelocity = (newPos - oldPos) / Time.deltaTime;
            oldPos = newPos;
            newPos = attachPoint.transform.position;

            // rotate attach point according to controller's 'Z' rotation
            attachPoint.transform.eulerAngles = new Vector3(attachPoint.transform.eulerAngles.x, attachPoint.transform.eulerAngles.y, handLocation.transform.eulerAngles.z * -1);
            if (!snappedByDragAndDrop)
                IOToLevitate.GrabLogic.FollowPoint(attachPoint.transform, DynamicLevitationPoint.transform);
            else
            {
                if (!currentDragAndDropManager.IsInDistance(attachPoint.transform.position, snappedTolerance))
                {
                    IOToLevitate.transform.position = attachPoint.transform.position;
                    Gaze_GravityManager.ChangeGravityState(IOToLevitate, Gaze_GravityRequestType.UNLOCK);
                    Gaze_GravityManager.ChangeGravityState(IOToLevitate, Gaze_GravityRequestType.ACTIVATE_AND_DETACH);
                    snappedByDragAndDrop = false;
                }
            }

        }

        private Vector3 GetPullPushDirection()
        {
            return targetLocation.transform.position - handLocation.transform.position;
        }

        private void PullPush()
        {
            // To play with the touch :
            // spacebar = NEUTRAL and Up/Down arrows to pull/push
            if (debug)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                    levitationState = Gaze_LevitationStates.PUSH;
                else if (Input.GetKey(KeyCode.DownArrow))
                    levitationState = Gaze_LevitationStates.PULL;
                else if (Input.GetKey(KeyCode.Space))
                    levitationState = Gaze_LevitationStates.NEUTRAL;
            }

            CalcPullPushSpeed();

            float speedFactor = actualTimePressingTrigger / TimePressingToReachMaxSpeed;

            speedFactor = Mathf.Max(0.2f, speedFactor);

            Vector3 pullPushDir = GetPullPushDirection();
            float pullPushMagnitude = attractionForce * Time.deltaTime * speedFactor;

            // if pushing
            switch (levitationState)
            {
                case Gaze_LevitationStates.PULL:
                    targetLocation.transform.position += pullPushDir * pullPushMagnitude;
                    Gaze_EventManager.FireLevitationEvent(new Gaze_LevitationEventArgs(this, objectToLevitate, Gaze_LevitationTypes.PULL, actualHand));
                    break;
                case Gaze_LevitationStates.PUSH:
                    if (Vector3.Distance(targetLocation.transform.position, handLocation.transform.position) > closestDistance)
                    {
                        targetLocation.transform.position -= pullPushDir * pullPushMagnitude;
                        Gaze_EventManager.FireLevitationEvent(new Gaze_LevitationEventArgs(this, objectToLevitate, Gaze_LevitationTypes.PUSH, actualHand));
                    }
                    break;
                default:
                    break;
            }
        }

        private void CalcPullPushSpeed()
        {
            if (levitationState == Gaze_LevitationStates.NEUTRAL)
            {
                actualTimePressingTrigger = 0.0f;
            }

            actualTimePressingTrigger += Time.deltaTime;
            actualTimePressingTrigger = Mathf.Min(actualTimePressingTrigger, TimePressingToReachMaxSpeed);
        }

        private void AnimateBeamTexture()
        {
            if (beamMaterial)
                beamMaterial.SetTextureOffset("_MainTex", new Vector2(Time.time * beamTextureSpeed * -1, 0f));
        }

        private void UpdateBeamControlPoints()
        {
            beamStartPosition = handLocation.position;
            beamControlPoints[0] = beamStartPosition;
            beamControlPoints[1] = beamStartPosition;

            for (int i = 2; i < beamControlPoints.Length - 2; i++)
            {
                beamControlPoints[i] = PerturbateBeam(getVectorPointAtDistance(handLocation.position, targetLocation.transform.position, (float)(1f / (beamNumberOfControlPoints - 3) * (i - 1))), i);
            }

            beamControlPoints[beamControlPoints.Length - 2] = attachPoint.transform.position;
            beamControlPoints[beamControlPoints.Length - 1] = attachPoint.transform.position;
        }

        private Vector3 PerturbateBeam(Vector3 _point, int _index)
        {
            return _point + Mathf.Sin((Time.time + _index) * beamOffsetSpeed) * beamOffsetAmplitude * Vector3.right;
        }

        private void ResetCharge()
        {
            StopAllCoroutines();
            isCharged = false;
            isCharging = false;
            beam.positionCount = 1;

            if (OriginPoint != null)
                OriginPoint.SetActive(false);

            GetComponent<AudioSource>().Stop();

            // notify the levitation has stopped
            if (IOToLevitate != null)
                Gaze_EventManager.FireLevitationEvent(new Gaze_LevitationEventArgs(this, IOToLevitate.gameObject, Gaze_LevitationTypes.LEVITATE_STOP, actualHand));

            Gaze_GrabManager.EnableAllGrabManagers();
            UpdateBeamFeedback(false);
        }


        private void tryActivateGravity(bool _enableGravity)
        {
            // deactivate gravity if exists
            if (objectToLevitate != null && objectToLevitate.GetComponent<Rigidbody>() != null)
            {
                if (_enableGravity)
                    Gaze_GravityManager.ChangeGravityState(objectToLevitate.GetComponent<Gaze_InteractiveObject>(), Gaze_GravityRequestType.RETURN_TO_DEFAULT);
                else
                    Gaze_GravityManager.ChangeGravityState(objectToLevitate.GetComponent<Gaze_InteractiveObject>(), Gaze_GravityRequestType.DEACTIVATE_AND_ATTACH);

                objectToLevitate.GetComponent<Rigidbody>().velocity = objectToLevitateVelocity;
            }
        }

        private void OnLevitationEvent(Gaze_LevitationEventArgs e)
        {
            if (e.Hand != actualHand && e.Hand != Gaze_HandsEnum.BOTH)
                return;

            if (e.Type.Equals(Gaze_LevitationTypes.LEVITATE_START))
            {
                IOToLevitate = Gaze_Utils.GetIOFromGameObject(e.ObjectToLevitate);
                Gaze_GravityManager.ChangeGravityState(IOToLevitate, Gaze_GravityRequestType.UNLOCK);
                Gaze_GravityManager.ChangeGravityState(IOToLevitate, Gaze_GravityRequestType.ACTIVATE_AND_DETACH);
                IOToLevitate.SetActualGravityStateAsDefault();
                targetLocation.transform.position = IOToLevitate.transform.position;
                UpdateBeamControlPoints();
                Gaze_Teleporter.IsTeleportAllowed = false;
                snappedByDragAndDrop = false;

                currentDragAndDropManager = IOToLevitate.GetComponent<Gaze_DragAndDropManager>();

                if (currentDragAndDropManager.IsSnapped)
                {
                    snappedByDragAndDrop = true;
                    snappedTolerance = Vector3.Distance(attachPoint.transform.position, IOToLevitate.transform.position);
                }
            }
            else if (e.Type.Equals(Gaze_LevitationTypes.LEVITATE_STOP))
            {
                beam.enabled = false;
                Destroy(DynamicLevitationPoint);
                Gaze_GravityManager.ChangeGravityState(IOToLevitate, Gaze_GravityRequestType.RETURN_TO_DEFAULT);
                IOToLevitate = null;
                Gaze_GrabManager.EnableAllGrabManagers();
                ClearAttachPoint();
                Gaze_Teleporter.IsTeleportAllowed = true;
            }
        }

        private void OnControllerGrabEvent(Gaze_ControllerGrabEventArgs e)
        {
            // else, if the I'm the concerned controller
            if ((e.ControllerObjectPair.Key.Equals(VRNode.LeftHand) && isLeftHand) || (e.ControllerObjectPair.Key.Equals(VRNode.RightHand) && !isLeftHand))
            {
                // and this object is in LEVITATE mode
                if (e.ControllerObjectPair.Value.GetComponent<Gaze_InteractiveObject>().ManipulationMode == Gaze_ManipulationModes.LEVITATE)
                {
                    isControllerTrigger = e.IsGrabbing;
                    IOToLevitate = e.ControllerObjectPair.Value.GetComponent<Gaze_InteractiveObject>();
                    hitPosition = e.HitPosition;

                    // Ensure 1 dynamic levitation point on the manager.
                    Destroy(DynamicLevitationPoint);
                    DynamicLevitationPoint = new GameObject();
                    DynamicLevitationPoint.transform.localScale = Vector3.zero;

                    DynamicLevitationPoint.transform.position = e.HitPosition;
                    DynamicLevitationPoint.transform.SetParent(IOToLevitate.transform);
                    DynamicLevitationPoint.transform.rotation =
                        IOToLevitate.transform.rotation;
                }

            }
        }

        private void OnDragAndDropEvent(Gaze_DragAndDropEventArgs e)
        {
            if (IOToLevitate == null)
                return;

            Gaze_DragAndDropManager dndManager = Gaze_Utils.ConvertIntoGameObject(e.DropObject).GetComponent<Gaze_DragAndDropManager>();

            if (IOToLevitate.gameObject != dndManager.gameObject)
                return;


            // Check if the position control is in drag and drop
            if (e.State.Equals(Gaze_DragAndDropStates.REMOVE))
            {
                TintBeam(false);
            }
            else if (e.State.Equals(Gaze_DragAndDropStates.DROPREADY))
            {
                if (IOToLevitate.DnD_snapBeforeDrop)
                {
                    snappedByDragAndDrop = true;
                    currentDragAndDropManager = dndManager;
                    snappedTolerance = Vector3.Distance(attachPoint.transform.position, IOToLevitate.transform.position);
                }

                TintBeam(true);
            }
            else if (e.State.Equals(Gaze_DragAndDropStates.DROPREADYCANCELED))
            {
                if (IOToLevitate.DnD_snapBeforeDrop)
                    snappedByDragAndDrop = false;
            }

            if (e.State.Equals(Gaze_DragAndDropStates.DROPREADY) && !dropReady)
            {
                dropReady = !dropReady;
                TintBeam(true);

            }
            else if (e.State.Equals(Gaze_DragAndDropStates.DROPREADYCANCELED) && dropReady)
            {
                dropReady = !dropReady;
                TintBeam(false);
            }
            else if (e.State.Equals(Gaze_DragAndDropStates.DROP))
            {
                ResetBeamColor();

                // Trying to hide everything 
                beam.enabled = false;
                attachPoint.GetComponent<Renderer>().enabled = false;
            }
        }

        private void TintBeam(bool _dropReady)
        {
            if (updateBeamColorRoutine != null)
                StopCoroutine(updateBeamColorRoutine);
            updateBeamColorRoutine = UpdateBeamFeedback(_dropReady);
            StartCoroutine(updateBeamColorRoutine);
        }

        public void ResetBeamColor()
        {
            beam.startColor = dropOffStartColor;
            beam.endColor = dropOffEndColor;
            attachPoint.GetComponent<Renderer>().material.color = dropOffEndColor;
        }

        #region GearVR Controller
        private void OnPadRightTouchDownEvent(Gaze_InputEventArgs e)
        {
            levitationState = Gaze_InputManager.instance.CurrentController == Gaze_Controllers.GEARVR_CONTROLLER ? Gaze_LevitationStates.PULL : Gaze_LevitationStates.PUSH;
        }

        private void OnPadRightTouchUpEvent(Gaze_InputEventArgs e)
        {
            levitationState = Gaze_InputManager.instance.CurrentController == Gaze_Controllers.GEARVR_CONTROLLER ? Gaze_LevitationStates.PUSH : Gaze_LevitationStates.PULL;
        }

        private void OnRightTouchpadEvent(Gaze_InputEventArgs e)
        {
            if (Vector2.Distance(e.AxisValue, Vector2.zero) < 0.1f && levitationState != Gaze_LevitationStates.NEUTRAL)
            {
                levitationState = Gaze_LevitationStates.NEUTRAL;
                float angleBetweenVectors = Vector3.Angle(attachPoint.transform.position - handLocation.transform.position, targetLocation.transform.position - handLocation.transform.position);

                if (angleBetweenVectors < 10f)
                {
                    targetLocation.transform.position = attachPoint.transform.position;
                }

                timeStoppedPushAndPull = Time.time;
            }
        }

        #endregion

        public static bool IsIOBeingLevitatedByHand(Gaze_InteractiveObject io, bool isLefHand)
        {
            foreach (Gaze_LevitationManager manager in LevitationManagers)
            {
                if (manager.IOToLevitate == io && manager.actualHand == (isLefHand ? Gaze_HandsEnum.LEFT : Gaze_HandsEnum.RIGHT))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
