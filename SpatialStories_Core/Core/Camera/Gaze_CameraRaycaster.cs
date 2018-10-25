// <copyright file="Gaze_CameraRaycaster.cs" company="apelab sàrl">
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
using GoogleARCore;
using SpatialStories;
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    /// <summary>
    /// This script notifies Event Manager when an object is gazed or no longer gazed at.
    /// Version 2.0 : now cast on all objects (RaycastAll).
    /// </summary>
    public class Gaze_CameraRaycaster : MonoBehaviour
    {
        // delay at start of the scene before casting a ray
        public float RayLength = 100f;
        public bool DebugMode = false;
        public Camera GazeCamera;
        public bool zoom = false;
        public float zoomFOV = 10f;
        public float zoomSpeed = 4;
        public float updateInterval = .2f;

        private Collider lastZoomCollider;
        private bool currentZoom = false;
        private float fovDefault;
        private float fovFactor = 1;
        private float zoomSpeedFactor = 1;
        private float dezoomSpeedFactor = 1;
        private float zoomTime = 0;
        private AnimationCurve zoomCurve;

        private float lastUpdateTime;

        public enum CURSOR_DETECTION_STATE { OVER_PLANE, OVER_NOTHING, OVER_OBJECT }
        public static CURSOR_DETECTION_STATE ActualDetectionState = CURSOR_DETECTION_STATE.OVER_NOTHING;
        public static Ray LastRay;
        public static RaycastHit ClosestHit;
        public static RaycastHit ClosestHitOverObject;
        public static TrackableHit ClosestHitOverObjectTrackable;
        public static RaycastHit ClosestHitOverPlane;
        public static TrackableHit ClosestHitOverPlaneTrackable;
        public static Transform RaycasterTransform;
        public static bool DetectedPlaneThisFrame = false;
        public static bool DetectedObjectThisFrame = false;
        public static bool DetectedImageInThisFrame = false;

        public S_ARAbstractRaycaster actualRaycaster;


        public virtual void OnEnable()
        {
            gameObject.AddComponent<StudioPlaneManager>();
        }
        
        void Awake()
        {
            Cursor.visible = false;
            RaycasterTransform = transform;
#if UNITY_IOS
            actualRaycaster = new S_ARKitCameraRaycaster(this);
#else
            actualRaycaster = new S_ARCoreCameraRaycaster(this);
#endif
        }

        void Start()
        {
            findCamera();

            lastUpdateTime = Time.time;
            LastRay = new Ray();
        }

        private void findCamera()
        {
            foreach (Camera cam in GetComponentsInChildren<Camera>())
            {
                if (cam.isActiveAndEnabled)
                {
                    GazeCamera = cam;
                    fovDefault = GazeCamera.fieldOfView;
                    break;
                }
            }
        }

        public void SetCamera(Camera cam)
        {
            GazeCamera = cam;
            fovDefault = GazeCamera.fieldOfView;
        }

        private void LateUpdate()
        {
            actualRaycaster.LateUpdate();
        }
    }
}
