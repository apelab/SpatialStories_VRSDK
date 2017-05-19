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
    public class Gaze_LevitationManager : MonoBehaviour
    {
        #region members
        [Tooltip("The duration in seconds to charge the levitation")]
        public float chargeDuration = .1f;
        [Tooltip("The amount of force (speed) used to center the levitated object at gaze position")]
        public float attractionForce = 3f;
        [Tooltip("The amount of force (speed) used to pull/push the levitated object")]
        public float levitatingObjectAttractingSpeed = 5f;
        [Tooltip("The closest distance the object can be from the controller")]
        public float closestDistance = 1f;
        public float beamSplineSmoothness = .1f;
        public AudioClip beamSoundClip;
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

        private GameObject controllerLeft, controllerRight;
        private bool isLeftHand;
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
        private GameObject attachPoint;
        private Color attachPointColor;
        private Transform handLocation;
        private bool dropReady;
        private IEnumerator updateBeamColorRoutine;
        #endregion

        void OnEnable()
        {
            isLeftHand = GetComponentInChildren<Gaze_GrabManager>().isLeftHand; isLeftHand = GetComponentInChildren<Gaze_GrabManager>().isLeftHand;

            Gaze_EventManager.OnLevitationEvent += OnLevitationEvent;
            Gaze_EventManager.OnDragAndDropEvent += OnDragAndDropEvent;

            // TODO: Discriminate the hand that we are using
            Gaze_InputManager.OnControllerGrabEvent += OnControllerGrabEvent;
            if (isLeftHand)
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

            transform.gameObject.AddComponent<AudioSource>();
            GetComponent<AudioSource>().playOnAwake = false;
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().clip = beamSoundClip;
        }

        void OnDisable()
        {
            Gaze_EventManager.OnLevitationEvent -= OnLevitationEvent;
            Gaze_EventManager.OnDragAndDropEvent -= OnDragAndDropEvent;

            Gaze_InputManager.OnControllerGrabEvent -= OnControllerGrabEvent;

            if (isLeftHand)
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

        }

        void Start()
        {
            isLeftHand = GetComponentInChildren<Gaze_GrabManager>().isLeftHand;
            isControllerTrigger = false;
            targetLocation = transform.FindChild("Levitation Target").gameObject;

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

        void Update()
        {
            if (active)
            {
                if ((!isCharged && !isCharging) && isControllerTrigger)
                {
                    StartCoroutine(Charge());

                }
                else if (isCharging && !isControllerTrigger)
                    ResetCharge();

                if (isCharged)
                {
                    if (isControllerTrigger)
                    {
                        Levitate();
                    }
                    else
                        ResetCharge();
                }
            }
        }

        private IEnumerator UpdateBeamFeedback(bool _dropReady)
        {
            Color lerpingStartColor, lerpingEndColor;
            Color attachPointColor = attachPoint.GetComponent<Renderer>().material.color;

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

            /*
            if (_dropReady)
            {
                for (float i = 0f; i < dropReadyFeedbackDuration; i += Time.deltaTime)
                {
                    lerpingStartColor = Color.Lerp(attachPointColor, dropOnStartColor, i);
                    lerpingEndColor = Color.Lerp(attachPointColor, dropOnEndColor, i);

                    beam.startColor = lerpingStartColor;
                    beam.endColor = lerpingEndColor;
                    attachPoint.GetComponent<Renderer>().material.color = lerpingEndColor;
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                for (float i = 0f; i < dropReadyFeedbackDuration; i += Time.deltaTime)
                {
                    lerpingStartColor = Color.Lerp(attachPointColor, dropOffStartColor, i);
                    lerpingEndColor = Color.Lerp(attachPointColor, dropOffEndColor, i);

                    beam.startColor = lerpingStartColor;
                    beam.endColor = lerpingEndColor;
                    attachPoint.GetComponent<Renderer>().material.color = lerpingEndColor;
                    yield return new WaitForEndOfFrame();
                }
            }
            */
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
            attachPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            attachPoint.transform.localScale = new Vector3(pointerDiameter, pointerDiameter, pointerDiameter);
            if (pointerMaterial)
                attachPoint.GetComponent<Renderer>().sharedMaterial = pointerMaterial;

            attachPoint.GetComponent<Renderer>().sharedMaterial.color = Color.white;
            attachPointColor = attachPoint.GetComponent<Renderer>().material.color;
        }

        private IEnumerator Charge()
        {
            #region INIT
            beam.positionCount = 2;
            isCharging = true;
            chargeStartTime = Time.time;
            #endregion

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
            //oldPos = objectToLevitate.transform.position;
            oldPos = attachPoint.transform.position;

            // deactivate gravity if exists
            TrySwitchGravity(objectToLevitate, false);

            // notify the levitation is occuring
            Gaze_EventManager.FireLevitationEvent(new Gaze_LevitationEventArgs(this, objectToLevitate, Gaze_LevitationTypes.LEVITATE_START));

            // get distance between headset and handsMidPoint
            startCameraHandsDistance = Vector3.Distance(Camera.main.transform.position, transform.position);
            #endregion
        }

        private void SetAttachPoint()
        {
            // set position of the pointer to the hit point
            attachPoint.transform.position = hitPosition;

            // attachPoint look at controller
            attachPoint.transform.LookAt(handLocation);

            // parent levitable to attach point
            objectToLevitate.transform.parent = attachPoint.transform;

            // show attach point
            attachPoint.GetComponent<Renderer>().enabled = true;
        }

        private void ClearAttachPoint()
        {
            // reset pointer when levitation finished
            if (attachPoint)
                attachPoint.GetComponent<Renderer>().enabled = false;

            if (objectToLevitate)
                objectToLevitate.transform.parent = null;
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
                Gaze_GravityManager.ChangeGravityState(IO, Gaze_GravityRequestType.ACTIVATE_AND_DETACH);
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

                // set beam's length
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
            if (objectToLevitate != null)
            {
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
                attachPoint.transform.position = Vector3.Lerp(attachPoint.transform.position, targetLocation.transform.position, Time.deltaTime * attractionForce);

                // rotate attach point towards the controller
                attachPoint.transform.LookAt(handLocation);

                // rotate attach point according to controller's 'Z' rotation
                attachPoint.transform.eulerAngles = new Vector3(attachPoint.transform.eulerAngles.x, attachPoint.transform.eulerAngles.y, handLocation.transform.eulerAngles.z * -1);

                // get object's velocity
                newPos = attachPoint.transform.position;
                objectToLevitateVelocity = (newPos - oldPos) / Time.deltaTime;
                oldPos = newPos;
                newPos = attachPoint.transform.position;
            }
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

            // if pushing
            switch (levitationState)
            {
                case Gaze_LevitationStates.PULL:
                    if (Vector3.Distance(targetLocation.transform.position, handLocation.transform.position) > closestDistance)
                        targetLocation.transform.localPosition = new Vector3(targetLocation.transform.localPosition.x, targetLocation.transform.localPosition.y, targetLocation.transform.localPosition.z + (-levitatingObjectAttractingSpeed * attractionForce * Time.deltaTime));
                    break;

                case Gaze_LevitationStates.PUSH:
                    targetLocation.transform.localPosition = new Vector3(targetLocation.transform.localPosition.x, targetLocation.transform.localPosition.y, targetLocation.transform.localPosition.z + (levitatingObjectAttractingSpeed * attractionForce * Time.deltaTime));
                    break;

                default:
                    break;
            }
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
            if (isCharged)
                tryActivateGravity(true);
            isCharged = false;
            isCharging = false;
            beam.positionCount = 1;

            GetComponent<AudioSource>().Stop();

            // deactivate gravity if exists
            TrySwitchGravity(objectToLevitate, true);

            // notify the levitation has stopped
            Gaze_EventManager.FireLevitationEvent(new Gaze_LevitationEventArgs(this, objectToLevitate, Gaze_LevitationTypes.LEVITATE_STOP));
            Gaze_GrabManager.EnableAllGrabManagers();
        }

        private void tryActivateGravity(bool _enableGravity)
        {
            // deactivate gravity if exists
            if (objectToLevitate != null && objectToLevitate.GetComponent<Rigidbody>() != null)
            {
                if (_enableGravity)
                    Gaze_GravityManager.ChangeGravityState(objectToLevitate.GetComponent<Gaze_InteractiveObject>(), Gaze_GravityRequestType.ACTIVATE_AND_DETACH);
                else
                    Gaze_GravityManager.ChangeGravityState(objectToLevitate.GetComponent<Gaze_InteractiveObject>(), Gaze_GravityRequestType.DEACTIVATE_AND_ATTACH);

                objectToLevitate.GetComponent<Rigidbody>().velocity = objectToLevitateVelocity;
            }
        }

        private void OnLevitationEvent(Gaze_LevitationEventArgs e)
        {
            if (e.Type.Equals(Gaze_LevitationTypes.LEVITATE_START))
            {
                objectToLevitate = e.ObjectToLevitate;
            }
            else if (e.Type.Equals(Gaze_LevitationTypes.LEVITATE_STOP))
            {
                if (objectToLevitate)
                    objectToLevitate.transform.parent = null;
                objectToLevitate = null;
                Gaze_GrabManager.EnableAllGrabManagers();
                ClearAttachPoint();
            }
        }

        private void OnControllerGrabEvent(Gaze_ControllerGrabEventArgs e)
        {
            // else, if the I'm the concerned controller
            if ((e.ControllerObjectPair.Key.Equals(VRNode.LeftHand) && isLeftHand) || (e.ControllerObjectPair.Key.Equals(VRNode.RightHand) && !isLeftHand))
            {
                // and this object is in LEVITATE mode
                if (e.ControllerObjectPair.Value.GetComponent<Gaze_InteractiveObject>().GrabModeIndex.Equals((int)Gaze_GrabMode.LEVITATE))
                {
                    isControllerTrigger = e.IsGrabbing;
                    objectToLevitate = e.ControllerObjectPair.Value.GetComponent<Gaze_InteractiveObject>().gameObject;
                    hitPosition = e.HitPosition;
                }
            }
        }

        private void OnDragAndDropEvent(Gaze_DragAndDropEventArgs e)
        {
            Gaze_DragAndDropManager dndManager = Gaze_Utils.ConvertIntoGameObject(e.Sender).GetComponent<Gaze_DragAndDropManager>();

            if (objectToLevitate != dndManager.gameObject)
                return;

            if (e.State.Equals(Gaze_DragAndDropStates.DROPREADY) && !dropReady)
            {
                dropReady = !dropReady;
                if (updateBeamColorRoutine != null)
                    StopCoroutine(updateBeamColorRoutine);
                updateBeamColorRoutine = UpdateBeamFeedback(true);
                StartCoroutine(updateBeamColorRoutine);
            }
            else if (e.State.Equals(Gaze_DragAndDropStates.DROPREADYCANCELED) && dropReady)
            {
                dropReady = !dropReady;
                if (updateBeamColorRoutine != null)
                    StopCoroutine(updateBeamColorRoutine);
                updateBeamColorRoutine = UpdateBeamFeedback(false);
                StartCoroutine(updateBeamColorRoutine);
            }
            else if (e.State.Equals(Gaze_DragAndDropStates.DROP))
            {
                ResetBeamColor();
            }
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
            levitationState = Gaze_LevitationStates.PUSH;
        }

        private void OnPadRightTouchUpEvent(Gaze_InputEventArgs e)
        {
            levitationState = Gaze_LevitationStates.PULL;
        }

        private void OnRightTouchpadEvent(Gaze_InputEventArgs e)
        {
            if (e.AxisValue.Equals(Vector2.zero))
                levitationState = Gaze_LevitationStates.NEUTRAL;
        }
        #endregion
    }
}