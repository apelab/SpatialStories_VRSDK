// <copyright file="Gaze_InputManager.cs" company="apelab sàrl">
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
using Gaze;
using System.Linq;
using UnityEngine;
using UnityEngine.VR;


public enum HapticForceMode
{
    FORCE_MIN,
    FORCE_MAX
}

public class Gaze_InputManager : MonoBehaviour
{
    #region members

    public static Gaze_Controllers PluggedControllerType = Gaze_Controllers.NOT_DETERMINED;

    [ReadOnly]
    public Gaze_Controllers CurrentController = Gaze_Controllers.NOT_DETERMINED;

    /// <summary>
    /// Fired when the player has found two items that creates a Totem.
    /// </summary>
    public delegate void ControllerCollisionHandler(Gaze_ControllerCollisionEventArgs e);
    public static event ControllerCollisionHandler OnControllerCollisionEvent;
    public static void FireControllerCollisionEvent(Gaze_ControllerCollisionEventArgs e)
    {
        if (OnControllerCollisionEvent != null)
            OnControllerCollisionEvent(e);
    }

    /// <summary>
    /// Fired when the player is grabbing an object with the controller.
    /// </summary>
    public delegate void ControllerGrabHandler(Gaze_ControllerGrabEventArgs e);
    public static event ControllerGrabHandler OnControllerGrabEvent;
    public static void FireControllerGrabEvent(Gaze_ControllerGrabEventArgs e)
    {
        if (OnControllerGrabEvent != null)
            OnControllerGrabEvent(e);
    }

    /// <summary>
    /// Fired when the player is touching an object with the controller.
    /// </summary>
    public delegate void ControllerTouchHandler(Gaze_ControllerTouchEventArgs e);
    public static event ControllerTouchHandler OnControllerTouchEvent;
    public static void FireControllerTouchEvent(Gaze_ControllerTouchEventArgs e)
    {
        if (OnControllerTouchEvent != null)
            OnControllerTouchEvent(e);
    }

    public const float TRIGGER_SENSIBILITY = 0.6f;

    //	public float DoubleClickTime{ get { return m_DoubleClickTime; } }
    public static Gaze_InputManager instance = null;
    public bool LeftHandActive = true;
    public bool RightHandActive = true;
    public bool trackPosition = true;
    public bool trackOrientation = true;
    public AudioClip hapticAudioClipMin, hapticAudioClipMax;
    public float padTouchDirectionThreshold = .5f;
    public bool debug = false;

    public delegate void InputEvent(Gaze_InputEventArgs e);

    public static event InputEvent OnInputEvent;

    public static event InputEvent OnStartEvent;

    public static event InputEvent OnButtonAEvent;
    public static void FireOnButtonAEvent(Gaze_InputEventArgs args)
    {
        if (OnButtonAEvent != null)
            OnButtonAEvent(args);
    }

    public static event InputEvent OnButtonADownEvent;
    public static void FireOnButtonADownEvent(Gaze_InputEventArgs args)
    {
        if (OnButtonADownEvent != null)
            OnButtonADownEvent(args);
    }
    public static event InputEvent OnButtonAUpEvent;
    public static void FireOnOnButtonAUpEvent(Gaze_InputEventArgs args)
    {
        if (OnButtonAUpEvent != null)
            OnButtonAUpEvent(args);
    }


    public static event InputEvent OnButtonBEvent;
    public static event InputEvent OnButtonBDownEvent;
    public static event InputEvent OnButtonBUpEvent;
    public static event InputEvent OnButtonXEvent;
    public static event InputEvent OnButtonXDownEvent;
    public static event InputEvent OnButtonXUpEvent;
    public static event InputEvent OnButtonYEvent;
    public static event InputEvent OnButtonYDownEvent;
    public static event InputEvent OnButtonYUpEvent;

    public static event InputEvent OnIndexLeftEvent;
    public static event InputEvent OnIndexLeftDownEvent;
    public static event InputEvent OnIndexLeftUpEvent;
    public static event InputEvent OnIndexRightEvent;
    public static event InputEvent OnIndexRightDownEvent;
    public static event InputEvent OnIndexRightUpEvent;

    public static event InputEvent OnHandLeftEvent;
    public static event InputEvent OnHandLeftDownEvent;
    public static event InputEvent OnHandLeftUpEvent;

    public static event InputEvent OnHandRightEvent;
    public static void FireOnHandRightEvent(Gaze_InputEventArgs args)
    {
        if (OnHandRightEvent != null)
            OnHandRightEvent(args);
    }
    public static event InputEvent OnHandRightDownEvent;
    public static void FireOnHandRightDownEvent(Gaze_InputEventArgs args)
    {
        if (OnHandRightDownEvent != null)
            OnHandRightDownEvent(args);
    }

    public static event InputEvent OnHandRightUpEvent;
    public static void FireOnHandRightUpEvent(Gaze_InputEventArgs args)
    {
        if (OnHandRightUpEvent != null)
            OnHandRightUpEvent(args);
    }

    public static event InputEvent OnStickLeftAxisEvent;
    public static void FireStickLeftAxisEvent(Gaze_InputEventArgs args)
    {
        if (OnStickLeftAxisEvent != null)
            OnStickLeftAxisEvent(args);
    }
    public static event InputEvent OnStickLeftDownEvent;
    public static event InputEvent OnStickLeftUpEvent;
    public static event InputEvent OnStickRightAxisEvent;
    public static void FireStickRightAxisEvent(Gaze_InputEventArgs args)
    {
        if (OnStickRightAxisEvent != null)
            OnStickRightAxisEvent(args);
    }
    public static event InputEvent OnStickRightDownEvent;
    public static event InputEvent OnStickRightUpEvent;

    public static event InputEvent OnLeftTouchpadEvent;
    public static event InputEvent OnPadLeftTouchLeftEvent;
    public static event InputEvent OnPadLeftTouchRightEvent;
    public static event InputEvent OnPadLeftTouchUpEvent;
    public static event InputEvent OnPadLeftTouchDownEvent;

    public static event InputEvent OnRightTouchpadEvent;
    public static void FireRightTouchpadEvent(Gaze_InputEventArgs args)
    {
        if (OnRightTouchpadEvent != null)
            OnRightTouchpadEvent(args);
    }
    public static event InputEvent OnPadRightTouchLeftEvent;
    public static event InputEvent OnPadRightTouchRightEvent;
    public static event InputEvent OnPadRightTouchUpEvent;
    public static event InputEvent OnPadRightTouchDownEvent;

    public GameObject LeftController { get { return leftHandIO; } }

    public GameObject RightController { get { return rightHandIO; } }

    public bool ControllersConnected { get { return controllersConnected; } }

    /*
    //The max time allowed between double clicks
    [SerializeField] private float m_DoubleClickTime = 0.3f;
    //The width of a swipe
    [SerializeField] private float m_SwipeWidth = 0.3f;
        
    // The screen position of the mouse when Fire1 is pressed.
    private Vector2 m_MouseDownPosition;
    // The screen position of the mouse when Fire1 is released.
    private Vector2 m_MouseUpPosition;
    // The time when Fire1 was last released.
    private float m_LastMouseUpTime;
    // The previous value of the horizontal axis used to detect keyboard swipes.
    private float m_LastHorizontalValue;
    // The previous value of the vertical axis used to detect keyboard swipes.
    private float m_LastVerticalValue;
    */

    private GameObject leftHandIO, rightHandIO;
    private AudioClip currentHapticAudioClip;
    private bool controllersConnected;
    // the names of the connected controllers (HTC, Oculus Touch...)
    private string[] controllersNames;
    private bool isHandRightDown = false, isHandLeftDown = false;
    private bool isIndexLeftDown = false, isIndexRightDown = false;
    private Vector2 axisValueLeft, axisValueRight;
    private OVRHaptics.OVRHapticsChannel m_hapticsChannel;
    private OVRHapticsClip hapticAudioClipConverted;
    // is left or right handed
    private uint m_handedness;
    private bool rightStickActive = false, leftStickActive = false;

    private OVRHaptics.OVRHapticsChannel m_hapticsChannelL = null;
    private OVRHaptics.OVRHapticsChannel m_hapticsChannelR = null;
    private bool isHapticFeedbackActive = false;
    private GameObject[] hapticFeedbackControllers;



    // Input type determination setup
    public delegate void OnSetupController(Gaze_Controllers controllerType);
    private static event OnSetupController setupEvent;




    public Gaze_InputLogic SpecialInputLogic;

    public GameObject UnpluggedControllerMessage;

    float localHandsHeigth;

    public static event OnSetupController OnControlerSetup
    {
        add
        {
            setupEvent += value;
            if (PluggedControllerType != Gaze_Controllers.NOT_DETERMINED)
                setupEvent(PluggedControllerType);
        }
        remove { setupEvent -= value; }
    }
    #endregion

    void Awake()
    {
        instance = this;

        axisValueLeft = new Vector2();
        axisValueLeft = Vector2.zero;

        axisValueRight = new Vector2();
        axisValueRight = Vector2.zero;

        UnpluggedControllerMessage.SetActive(false);

        if (Application.isEditor)
            RepairInputManagerIfNeeded();

    }

    void Start()
    {
        GetControllers();

        m_hapticsChannelL = OVRHaptics.LeftChannel;
        m_hapticsChannelR = OVRHaptics.RightChannel;
        hapticFeedbackControllers = new GameObject[2];

        // if position tracking is disabled, parent the controllers to the camera
        if (!trackPosition)
            ParentControllersToCamera();

        // CPU / GPU Throttling
        OVRPlugin.cpuLevel = 3;
        OVRPlugin.gpuLevel = 3;
    }

    void OnEnable()
    {
        OnStickRightAxisEvent += StickRightAxisEvent;
        OnStickLeftAxisEvent += StickLeftAxisEvent;
    }

    void OnDisable()
    {
        OnStickRightAxisEvent -= StickRightAxisEvent;
        OnStickLeftAxisEvent -= StickLeftAxisEvent;
    }

    private void IdentifyInputType()
    {
        // Check if GearVR_Controller is conected if dont just add the generic controller
        if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
        {
            SpecialInputLogic = new Gaze_GearVR_InputLogic();
            PluggedControllerType = Gaze_Controllers.GEARVR_CONTROLLER;
        }
        else
        {
            SpecialInputLogic = null;

            if (Input.GetJoystickNames().Where(name => name.Contains("Oculus")).Count() > 0)
            {
                PluggedControllerType = Gaze_Controllers.OCULUS_RIFT;
            }
            else if (Input.GetJoystickNames().Where(name => name.Contains("OpenVR")).Count() > 0)
            {
                PluggedControllerType = Gaze_Controllers.HTC_VIVE;
            }
        }

        if (setupEvent != null)
        {
            setupEvent(PluggedControllerType);
            CurrentController = PluggedControllerType;
        }
    }

    private void OnDestroy()
    {
        // Ensure that all events are unsubscribed when this is destroyed.
        OnInputEvent = null;
        OnControllerCollisionEvent = null;
        OnControllerGrabEvent = null;
    }


    private void Update()
    {
        // Hack: Right now we are not able to know what is the current input type until update method 
        // In the new update of the gear vr controller we should remove this if the problem is solved.
        IdentifyInputType();

        if (SpecialInputLogic != null)
            SpecialInputLogic.Update();
        else
        {
            UpdateGenericInputs();
        }

        if (trackPosition)
            SetPosition();

        if (isHapticFeedbackActive)
            Pulse();
    }

    void FixedUpdate()
    {
        CheckControllers();

        if (!trackPosition)
            FixedPositionLogic();

        if (trackOrientation)
            SetOrientation();
    }

    private void GetControllers()
    {
        Gaze_HandController[] controllers = GetComponentsInChildren<Gaze_HandController>();
        if (controllers != null && controllers.Length > 0)
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                if (controllers[i].leftHand)
                    leftHandIO = controllers[i].GetComponentInParent<Gaze_InteractiveObject>().gameObject;
                else
                    rightHandIO = controllers[i].GetComponentInParent<Gaze_InteractiveObject>().gameObject;
            }
        }

        if (!LeftHandActive)
            leftHandIO.SetActive(false);

        if (!RightHandActive)
            rightHandIO.SetActive(false);
    }

    Transform FixesLeftPosition;
    Transform FixedRightPosition;

    // if position tracking is disabled, parent the controllers to the camera
    private void ParentControllersToCamera()
    {
        FixesLeftPosition = GetComponentInChildren<Gaze_PlayerTorso>().transform.Find("LeftHandFixedPosition").transform;
        FixedRightPosition = GetComponentInChildren<Gaze_PlayerTorso>().transform.Find("RightHandFixedPosition").transform;
        leftHandIO.transform.localPosition = Vector3.zero;
        rightHandIO.transform.localPosition = Vector3.zero;
        localHandsHeigth = transform.position.y - GetComponentInChildren<Gaze_PlayerTorso>().transform.position.y;
        rightHandIO.transform.SetParent(transform);
    }

    private void SetPosition()
    {
        leftHandIO.transform.localPosition = InputTracking.GetLocalPosition(VRNode.LeftHand);
        rightHandIO.transform.localPosition = InputTracking.GetLocalPosition(VRNode.RightHand);
    }

    private void SetOrientation()
    {
        leftHandIO.transform.localRotation = InputTracking.GetLocalRotation(VRNode.LeftHand);
        rightHandIO.transform.localRotation = InputTracking.GetLocalRotation(VRNode.RightHand);

        // take camera rotation into account if tracking position is disabled
        if (!trackPosition)
        {
            //leftHandIO.transform.localEulerAngles -= Camera.main.transform.localEulerAngles;
            //rightHandIO.transform.localEulerAngles -= Camera.main.transform.localEulerAngles;
        }
    }

    private void FixedPositionLogic()
    {
        leftHandIO.transform.position = FixesLeftPosition.position;
        rightHandIO.transform.position = FixedRightPosition.position;

        leftHandIO.transform.position = new Vector3(leftHandIO.transform.position.x, transform.position.y - localHandsHeigth, leftHandIO.transform.position.z);
        rightHandIO.transform.position = new Vector3(rightHandIO.transform.position.x, transform.position.y - localHandsHeigth, rightHandIO.transform.position.z);
    }

    /// <summary>
    /// Starts the pulse on both controllers or on the specified one
    /// </summary>
    /// <param name="_controller">Controller.</param>
    /// <param name="force">Force.</param>
    public void HapticFeedback(bool _activateHapticFeedback, GameObject _controller = null, HapticForceMode _mode = HapticForceMode.FORCE_MIN)
    {
        isHapticFeedbackActive = _activateHapticFeedback;
        if (_activateHapticFeedback)
        {

            currentHapticAudioClip = _mode.Equals(HapticForceMode.FORCE_MIN) ? hapticAudioClipMin : hapticAudioClipMax;

            if (_controller != null)
            {
                if (_controller.Equals(Gaze_InputManager.instance.LeftController))
                {
                    hapticFeedbackControllers[0] = _controller;
                }
                else
                {
                    hapticFeedbackControllers[1] = _controller;
                }
            }

        }
        else if (!_activateHapticFeedback)
        {
            if (_controller != null)
            {
                if (_controller.Equals(Gaze_InputManager.instance.LeftController))
                {
                    // if feedback is deactivated, remove controllers
                    hapticFeedbackControllers[0] = null;

                    // stop pulse
                    m_hapticsChannelL.Clear();

                }
                else
                {
                    // if feedback is deactivated, remove controllers
                    hapticFeedbackControllers[1] = null;

                    // stop pulse
                    m_hapticsChannelR.Clear();
                }

            }
            else
            {
                // if feedback is deactivated, remove controllers
                hapticFeedbackControllers[0] = null;

                // stop pulse
                m_hapticsChannelL.Clear();

                // if feedback is deactivated, remove controllers
                hapticFeedbackControllers[1] = null;

                // stop pulse
                m_hapticsChannelR.Clear();
            }
        }
    }

    private void PlayHapticAudioClip(bool isLeft, AudioClip aClip, bool preempt = false, HapticForceMode _mode = HapticForceMode.FORCE_MIN)
    {
        //No Error Checking So Be sure your indexes are Correct;
        OVRHapticsClip clip = new OVRHapticsClip(aClip);

        ClipPlayerNow(isLeft, clip);
    }

    // Internal Clip Player that Always Preempted;
    private void ClipPlayerNow(bool isLeft, OVRHapticsClip clip)
    {
        if (isLeft)
            m_hapticsChannelL.Preempt(clip);
        else
            m_hapticsChannelR.Preempt(clip);

        OVRHaptics.Process();
    }

    private void Pulse()
    {
        if (hapticFeedbackControllers[0] != null)
            PlayHapticAudioClip(true, currentHapticAudioClip, true);

        if (hapticFeedbackControllers[1] != null)
            PlayHapticAudioClip(false, currentHapticAudioClip, true);

        if (hapticFeedbackControllers[0] == null && hapticFeedbackControllers[1] == null)
        {
            PlayHapticAudioClip(true, currentHapticAudioClip, true);
            PlayHapticAudioClip(false, currentHapticAudioClip, true);
        }
    }

    private void CheckControllers()
    {
        controllersConnected = false;

        string[] names = Input.GetJoystickNames();

        for (int i = 0; i < names.Length; i++)
        {
            if (names[i].Contains("Oculus"))
            {
                controllersConnected = true;
                break;
            }
            else if (names[i].Contains("OpenVR"))
            {
                controllersConnected = true;
                break;
            }
        }

        // HACK: This works fine with the samsung gear vr but not with 
        // Oculus or HTC vive, we need to find a better way to generalize that
        // See task Gaze-477.
        if (SpecialInputLogic != null)
            controllersConnected = SpecialInputLogic.CheckIfControllerConnected();

        if (!controllersConnected)
        {
            if (!UnpluggedControllerMessage.activeSelf)
                UnpluggedControllerMessage.SetActive(true);
        }
        else
        {
            if (UnpluggedControllerMessage.activeSelf)
                UnpluggedControllerMessage.SetActive(false);
        }
    }

    private void UpdateGenericInputs()
    {

        #region buttons input
        if (Input.GetButton(Gaze_InputConstants.APELAB_INPUT_START))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_START);

            if (OnStartEvent != null)
            {
                //gaze_InputEventArgs.InputType = Gaze_InputTypes.START_BUTTON;
                OnStartEvent(new Gaze_InputEventArgs(this.gameObject, Gaze_InputTypes.START_BUTTON));
            }
        }

        if (Input.GetButton(Gaze_InputConstants.APELAB_INPUT_A))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_A);
            FireOnButtonAEvent(new Gaze_InputEventArgs(this.gameObject, Gaze_InputTypes.A_BUTTON));
        }
        if (Input.GetButtonDown(Gaze_InputConstants.APELAB_INPUT_A))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_A + " Down");
            FireOnButtonADownEvent(new Gaze_InputEventArgs(this.gameObject, Gaze_InputTypes.A_BUTTON_DOWN));
        }
        if (Input.GetButtonUp(Gaze_InputConstants.APELAB_INPUT_A))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_A + " Up");
            FireOnOnButtonAUpEvent(new Gaze_InputEventArgs(this.gameObject, Gaze_InputTypes.A_BUTTON_UP));
        }

        if (Input.GetButton(Gaze_InputConstants.APELAB_INPUT_B))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_B);

            if (OnButtonBEvent != null)
                OnButtonBEvent(new Gaze_InputEventArgs(this.gameObject, Gaze_InputTypes.B_BUTTON));
        }
        if (Input.GetButtonDown(Gaze_InputConstants.APELAB_INPUT_B))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_B + " Down");

            if (OnButtonBDownEvent != null)
                OnButtonBDownEvent(new Gaze_InputEventArgs(this.gameObject, Gaze_InputTypes.B_BUTTON_DOWN));
        }
        if (Input.GetButtonUp(Gaze_InputConstants.APELAB_INPUT_B))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_B + " Up");

            if (OnButtonBUpEvent != null)
                OnButtonBUpEvent(new Gaze_InputEventArgs(this.gameObject, Gaze_InputTypes.B_BUTTON_UP));
        }

        if (Input.GetButton(Gaze_InputConstants.APELAB_INPUT_X))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_X);

            if (OnButtonXEvent != null)
                OnButtonXEvent(new Gaze_InputEventArgs(this.gameObject, Gaze_InputTypes.X_BUTTON));
        }
        if (Input.GetButtonDown(Gaze_InputConstants.APELAB_INPUT_X))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_X + " Down");

            if (OnButtonXDownEvent != null)
                OnButtonXDownEvent(new Gaze_InputEventArgs(this.gameObject, Gaze_InputTypes.X_BUTTON_DOWN));
        }
        if (Input.GetButtonUp(Gaze_InputConstants.APELAB_INPUT_X))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_X + " Up");

            if (OnButtonXUpEvent != null)
                OnButtonXUpEvent(new Gaze_InputEventArgs(this.gameObject, Gaze_InputTypes.X_BUTTON_UP));
        }

        if (Input.GetButton(Gaze_InputConstants.APELAB_INPUT_Y))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_Y);

            if (OnButtonYEvent != null)
                OnButtonYEvent(new Gaze_InputEventArgs(this.gameObject, Gaze_InputTypes.Y_BUTTON));
        }
        if (Input.GetButtonDown(Gaze_InputConstants.APELAB_INPUT_Y))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_Y + " Down");

            if (OnButtonYDownEvent != null)
                OnButtonYDownEvent(new Gaze_InputEventArgs(this.gameObject, Gaze_InputTypes.Y_BUTTON_DOWN));
        }
        if (Input.GetButtonUp(Gaze_InputConstants.APELAB_INPUT_Y))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_Y + " Up");

            if (OnButtonYUpEvent != null)
                OnButtonYUpEvent(new Gaze_InputEventArgs(this.gameObject, Gaze_InputTypes.Y_BUTTON_UP));
        }
        #endregion

        #region sticks/pads input
        if (Input.GetButtonDown(Gaze_InputConstants.APELAB_INPUT_STICK_LEFT))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_STICK_LEFT + " Down");

            if (OnStickLeftDownEvent != null)
                OnStickLeftDownEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.LeftHand, Gaze_InputTypes.STICK_LEFT_DOWN));
        }
        if (Input.GetButtonUp(Gaze_InputConstants.APELAB_INPUT_STICK_LEFT))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_STICK_LEFT + " Up");

            if (OnStickLeftUpEvent != null)
                OnStickLeftUpEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.LeftHand, Gaze_InputTypes.STICK_LEFT_DOWN));
        }

        if (Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_STICK_LEFT_VERTICAL) != 0 || Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_STICK_LEFT_HORIZONTAL) != 0)
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_STICK_LEFT + Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_STICK_LEFT_VERTICAL) + ":" + Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_STICK_LEFT_HORIZONTAL));

            leftStickActive = true;

            if (OnStickLeftAxisEvent != null)
            {
                axisValueLeft.x = Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_STICK_LEFT_HORIZONTAL);
                axisValueLeft.y = Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_STICK_LEFT_VERTICAL);
                OnStickLeftAxisEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.LeftHand, Gaze_InputTypes.STICK_LEFT, axisValueLeft));
            }
        }
        if (leftStickActive && (Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_STICK_LEFT_VERTICAL) == 0) && (Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_STICK_LEFT_HORIZONTAL) == 0))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_STICK_LEFT + " released");

            leftStickActive = false;

            if (OnStickLeftAxisEvent != null)
                OnStickLeftAxisEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.LeftHand, Gaze_InputTypes.STICK_LEFT, Vector2.zero));
        }

        if (Input.GetButtonDown(Gaze_InputConstants.APELAB_INPUT_STICK_RIGHT))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_STICK_RIGHT + " Down");

            if (OnStickRightDownEvent != null)
                OnStickRightDownEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.STICK_RIGHT_DOWN));
        }
        if (Input.GetButtonUp(Gaze_InputConstants.APELAB_INPUT_STICK_RIGHT))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_STICK_RIGHT + " Up");

            if (OnStickRightUpEvent != null)
                OnStickRightUpEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.STICK_RIGHT_UP));
        }
        if (Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_STICK_RIGHT_HORIZONTAL) != 0 || Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_STICK_RIGHT_VERTICAL) != 0)
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_STICK_RIGHT + Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_STICK_RIGHT_VERTICAL) + ":" + Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_STICK_RIGHT_HORIZONTAL));

            rightStickActive = true;

            if (OnStickRightAxisEvent != null)
            {
                axisValueLeft.x = Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_STICK_RIGHT_HORIZONTAL);
                axisValueLeft.y = Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_STICK_RIGHT_VERTICAL);
                OnStickRightAxisEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.STICK_RIGHT, axisValueLeft));
            }
        }
        if (rightStickActive && (Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_STICK_RIGHT_VERTICAL) == 0) && (Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_STICK_RIGHT_HORIZONTAL) == 0))
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_STICK_RIGHT + " released");

            rightStickActive = false;

            if (OnStickRightAxisEvent != null)
                OnStickRightAxisEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.STICK_RIGHT, Vector2.zero));
        }
        #endregion

        #region indexes/trigger input
        if (Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_RIGHT) != 0)
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_INDEX_RIGHT + Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_RIGHT));

            if (OnIndexRightEvent != null)
                OnIndexRightEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.INDEX_RIGHT, Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_RIGHT)));
        }
        if (!isIndexRightDown && Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_RIGHT) > .8f)
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_INDEX_RIGHT + " Down" + Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_RIGHT));
            isIndexRightDown = true;
            if (OnIndexRightDownEvent != null)
                OnIndexRightDownEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.LeftHand, Gaze_InputTypes.INDEX_RIGHT_DOWN, Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_RIGHT)));

            //HACK: This is done for the Samsung VR controller
            if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
                OnHandRightDownEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.HAND_RIGHT_DOWN, Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_RIGHT)));

        }
        if (isIndexRightDown && Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_RIGHT) <= .8f)
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_INDEX_RIGHT + " Up" + Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_RIGHT));
            isIndexRightDown = false;
            if (OnIndexRightUpEvent != null)
                OnIndexRightUpEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.LeftHand, Gaze_InputTypes.INDEX_RIGHT_UP, Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_RIGHT)));

            //HACK: This is done for the Samsung VR controller
            if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
                OnHandRightDownEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.HAND_RIGHT_UP, Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_RIGHT)));
        }

        if (Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_LEFT) != 0)
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_INDEX_LEFT + Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_LEFT));

            if (OnIndexLeftEvent != null)
                OnIndexLeftEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.LeftHand, Gaze_InputTypes.INDEX_RIGHT, Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_LEFT)));
        }
        if (!isIndexLeftDown && Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_LEFT) > .8f)
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_INDEX_LEFT + " Down" + Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_LEFT));
            isIndexLeftDown = true;
            if (OnIndexLeftDownEvent != null)
                OnIndexLeftDownEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.LeftHand, Gaze_InputTypes.INDEX_LEFT_DOWN, Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_LEFT)));
        }
        if (isIndexLeftDown && Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_LEFT) <= .8f)
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_INDEX_LEFT + " Up" + Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_LEFT));
            isIndexLeftDown = false;
            if (OnIndexLeftUpEvent != null)
                OnIndexLeftUpEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.LeftHand, Gaze_InputTypes.INDEX_LEFT_UP, Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_INDEX_LEFT)));
        }
        #endregion

        #region hands/grip input
        if (Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_HAND_RIGHT) != 0)
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_HAND_RIGHT + Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_HAND_RIGHT));
            FireOnHandRightEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.HAND_RIGHT, Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_HAND_RIGHT)));
        }
        if (!isHandRightDown && Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_HAND_RIGHT) > TRIGGER_SENSIBILITY)
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_HAND_RIGHT + " DOWN");
            isHandRightDown = true;
            FireOnHandRightDownEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.HAND_RIGHT_DOWN));
        }
        if (isHandRightDown && Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_HAND_RIGHT) <= TRIGGER_SENSIBILITY)
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_HAND_RIGHT + " UP");
            isHandRightDown = false;
            FireOnHandRightUpEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.HAND_RIGHT_UP));
        }

        if (Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_HAND_LEFT) != 0)
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_HAND_LEFT + Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_HAND_LEFT));
            if (OnHandLeftEvent != null)
                OnHandLeftEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.LeftHand, Gaze_InputTypes.HAND_LEFT, Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_HAND_LEFT)));
        }
        if (!isHandLeftDown && Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_HAND_LEFT) > TRIGGER_SENSIBILITY)
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_HAND_LEFT + " DOWN");
            isHandLeftDown = true;
            if (OnHandLeftDownEvent != null)
                OnHandLeftDownEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.LeftHand, Gaze_InputTypes.HAND_LEFT_DOWN));
        }
        if (isHandLeftDown && Input.GetAxis(Gaze_InputConstants.APELAB_INPUT_HAND_LEFT) <= TRIGGER_SENSIBILITY)
        {
            if (debug)
                Debug.Log(Gaze_InputConstants.APELAB_INPUT_HAND_LEFT + " UP");
            isHandLeftDown = false;
            if (OnHandLeftUpEvent != null)
                OnHandLeftUpEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.LeftHand, Gaze_InputTypes.HAND_LEFT_UP));
        }
        #endregion

    }

    /// <summary>
    /// Notification for Right touchpad direction (i.e. Gear VR pad)
    /// </summary>
    /// <param name="e"></param>
    private void StickRightAxisEvent(Gaze_InputEventArgs e)
    {
        // implement gesture for touchpad direction on Gear VR pad
        if (e.AxisValue.x > 0.1f)
        {
            if (debug)
                Debug.Log("Right Touchpad touched Right");

            if (OnPadRightTouchRightEvent != null)
                OnPadRightTouchRightEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.PAD_RIGHT_TOUCH_RIGHT));

            if (OnRightTouchpadEvent != null)
                OnRightTouchpadEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.PAD_RIGHT_TOUCH_RIGHT, e.AxisValue));
        }
        else if (e.AxisValue.x < .1f)
        {
            if (debug)
                Debug.Log("Right Touchpad touched Left");

            if (OnPadRightTouchLeftEvent != null)
                OnPadRightTouchLeftEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.PAD_RIGHT_TOUCH_LEFT));

            if (OnRightTouchpadEvent != null)
                OnRightTouchpadEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.PAD_RIGHT_TOUCH_LEFT, e.AxisValue));
        }

        if (e.AxisValue.y > 0.1f)
        {
            if (debug)
                Debug.Log("Right Touchpad touched Down");

            if (OnPadRightTouchDownEvent != null)
                OnPadRightTouchDownEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.PAD_RIGHT_TOUCH_DOWN));

            if (OnRightTouchpadEvent != null)
                OnRightTouchpadEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.PAD_RIGHT_TOUCH_DOWN, e.AxisValue));

        }
        else if (e.AxisValue.y < .1f)
        {
            if (debug)
                Debug.Log("Right Touchpad touched Up");

            if (OnPadRightTouchUpEvent != null)
                OnPadRightTouchUpEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.PAD_RIGHT_TOUCH_UP));

            if (OnRightTouchpadEvent != null)
                OnRightTouchpadEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.PAD_RIGHT_TOUCH_UP, e.AxisValue));
        }
    }

    /// <summary>
    /// Notification for Left touchpad direction (i.e. Gear VR pad)
    /// </summary>
    /// <param name="e"></param>
    private void StickLeftAxisEvent(Gaze_InputEventArgs e)
    {
        if (e.AxisValue.x > 0.1f)
        {
            if (debug)
                Debug.Log("Left Touchpad touched Right");

            if (OnPadLeftTouchRightEvent != null)
                OnPadLeftTouchRightEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.LeftHand, Gaze_InputTypes.PAD_LEFT_TOUCH_RIGHT));

            if (OnLeftTouchpadEvent != null)
                OnLeftTouchpadEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.PAD_LEFT_TOUCH_RIGHT, e.AxisValue));
        }
        else if (e.AxisValue.x < .1f)
        {
            if (debug)
                Debug.Log("Left Touchpad touched Left");

            if (OnPadLeftTouchLeftEvent != null)
                OnPadLeftTouchLeftEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.LeftHand, Gaze_InputTypes.PAD_LEFT_TOUCH_LEFT));

            if (OnLeftTouchpadEvent != null)
                OnLeftTouchpadEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.PAD_LEFT_TOUCH_LEFT, e.AxisValue));
        }

        if (e.AxisValue.y > 0.1f)
        {
            if (debug)
                Debug.Log("Left Touchpad touched Down");

            if (OnPadLeftTouchDownEvent != null)
                OnPadLeftTouchDownEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.LeftHand, Gaze_InputTypes.PAD_LEFT_TOUCH_DOWN));

            if (OnLeftTouchpadEvent != null)
                OnLeftTouchpadEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.PAD_LEFT_TOUCH_DOWN, e.AxisValue));

        }
        else if (e.AxisValue.y < .1f)
        {
            if (debug)
                Debug.Log("Left Touchpad touched Up");

            if (OnPadLeftTouchUpEvent != null)
                OnPadLeftTouchUpEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.LeftHand, Gaze_InputTypes.PAD_LEFT_TOUCH_UP));

            if (OnLeftTouchpadEvent != null)
                OnLeftTouchpadEvent(new Gaze_InputEventArgs(this.gameObject, VRNode.RightHand, Gaze_InputTypes.PAD_LEFT_TOUCH_UP, e.AxisValue));
        }
    }

    /// <summary>
    /// Checks if the user has the InputManger.asset installed correctly with all our custom inputs
    /// </summary>
    void RepairInputManagerIfNeeded()
    {
        if (!Gaze_InputManagerChecker.IsInputManagerAssetIsInstaled())
        {
            Gaze_InputManagerChecker.ShowInputNotCorrectlyConfiguredDialog();
        }
    }
}
