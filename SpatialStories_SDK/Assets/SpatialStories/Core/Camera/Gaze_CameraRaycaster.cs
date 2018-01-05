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
        public float rayLength = 100f;
        public bool debugMode = false;
        public Camera gazeCamera;
        public bool zoom = false;
        public float zoomFOV = 10f;
        public float zoomSpeed = 4;
        public float updateInterval = .2f;

        private Ray ray;
        private List<RaycastHit> hits;
        private List<GameObject> previousGazedObjects;
        private List<GameObject> currentGazedObjects;
        private Collider lastZoomCollider;
        private bool currentZoom = false;
        private float fovDefault;
        private float fovFactor = 1;
        private float zoomSpeedFactor = 1;
        private float dezoomSpeedFactor = 1;
        private float zoomTime = 0;
        private AnimationCurve zoomCurve;
        private Gaze_GazeEventArgs gaze_gazeEventArgs;
        private float lastUpdateTime;
        private int gazeRaycastLayer;


        public virtual void OnEnable()
        {
            Gaze_EventManager.OnZoomEvent += OnZoomEvent;
        }

        public virtual void OnDisable()
        {
            Gaze_EventManager.OnZoomEvent -= OnZoomEvent;
        }

        void Awake()
        {
            previousGazedObjects = new List<GameObject>();
            currentGazedObjects = new List<GameObject>();
            Cursor.visible = false;
        }

        void Start()
        {
            findCamera();
            hits = new List<RaycastHit>();
            gaze_gazeEventArgs = new Gaze_GazeEventArgs();
            lastUpdateTime = Time.time;
            ray = new Ray();
            gameObject.layer = LayerMask.NameToLayer(Gaze_HashIDs.LAYER_GAZE);
            gazeRaycastLayer = 1 << gameObject.layer;
        }

        private void findCamera()
        {
            foreach (Camera cam in GetComponentsInChildren<Camera>())
            {
                if (cam.isActiveAndEnabled)
                {
                    gazeCamera = cam;
                    fovDefault = gazeCamera.fieldOfView;
                    break;
                }
            }
        }

        public void SetCamera(Camera cam)
        {
            gazeCamera = cam;
            fovDefault = gazeCamera.fieldOfView;
        }

        void FixedUpdate()
        {
            if (Time.time > lastUpdateTime + updateInterval)
            {

                // clear current gazed objects list
                currentGazedObjects.Clear();

                // cast a ray
                //ray = new Ray(gazeCamera.transform.position, gazeCamera.transform.forward);
                ray.origin = gazeCamera.transform.position;
                ray.direction = gazeCamera.transform.forward;
                hits.Clear();
                hits.AddRange(Physics.RaycastAll(ray, rayLength, gazeRaycastLayer));

                if (debugMode)
                    Debug.DrawRay(gazeCamera.transform.position, gazeCamera.transform.forward * rayLength, Color.red);

                // if camera's ray hits something
                if (hits != null && hits.Count > 0)
                {
                    // construct new current gazed objects list
                    foreach (RaycastHit h in hits)
                    {
                        // add it to the current gazed objects list
                        currentGazedObjects.Add(h.collider.gameObject);

                        // if it wasn't in the previous gazed objects list
                        if (!previousGazedObjects.Contains(h.collider.gameObject))
                        {
                            // notify every listener with the new current gazed object
                            gaze_gazeEventArgs.Sender = h.collider.gameObject;
                            gaze_gazeEventArgs.IsGazed = true;
                            Gaze_EventManager.FireGazeEvent(gaze_gazeEventArgs);
                        }
                    }

                    // if there were any previous gazed object
                    if (previousGazedObjects.Count > 0)
                    {

                        // check for each one of them
                        foreach (GameObject p in previousGazedObjects)
                        {

                            // if they don't exist anymore in the current gazed objects
                            if (!currentGazedObjects.Contains(p))
                            {
                                // notify every listener this previous gazed object is no longer gazed at
                                gaze_gazeEventArgs.Sender = p;
                                gaze_gazeEventArgs.IsGazed = false;
                                Gaze_EventManager.FireGazeEvent(gaze_gazeEventArgs);
                            }
                        }
                    }

                    // update previous gazed objects list with the new ones
                    previousGazedObjects = new List<GameObject>(currentGazedObjects);

                    // if camera's ray hits nothing
                }
                else
                {

                    // and the there were previous gazed objects
                    if (previousGazedObjects.Count > 0)
                    {

                        // for each one of the previous gazed object
                        foreach (GameObject p in previousGazedObjects)
                        {

                            // notify every listener its no longer gazed at
                            gaze_gazeEventArgs.Sender = p;
                            gaze_gazeEventArgs.IsGazed = false;
                            Gaze_EventManager.FireGazeEvent(gaze_gazeEventArgs);
                        }

                        // clear the previous gazed objects list
                        previousGazedObjects.Clear();
                    }
                }

                // zoom logic
                if (zoom)
                {
                    // if no object is gazed or the last zoomed object is no longer gazed
                    if (currentGazedObjects.Count == 0 || (lastZoomCollider && !currentGazedObjects.Contains(lastZoomCollider.gameObject)))
                    {
                        //					if (currentGazedObjects.Count == 0)
                        if (currentZoom)
                        {
                            zoomTime = 0;
                            currentZoom = false;
                        }

                        // zoom out
                        if (gazeCamera.fieldOfView < fovDefault)
                        {
                            gazeCamera.fieldOfView = fovDefault - zoomFOV * fovFactor + zoomFOV * fovFactor * zoomCurve.Evaluate(zoomTime * zoomSpeed * dezoomSpeedFactor);
                            gazeCamera.fieldOfView = Mathf.Min(gazeCamera.fieldOfView, fovDefault);
                        }
                    }
                    else
                    {
                        // if a zoomable object is gazed
                        if (currentZoom)
                        {
                            // zoom in
                            if (gazeCamera.fieldOfView > fovDefault - zoomFOV * fovFactor)
                            {
                                gazeCamera.fieldOfView = fovDefault - zoomFOV * fovFactor * zoomCurve.Evaluate(zoomTime * zoomSpeed * zoomSpeedFactor);
                                gazeCamera.fieldOfView = Mathf.Max(gazeCamera.fieldOfView, fovDefault - zoomFOV * fovFactor);
                            }
                        }
                    }

                    zoomTime += Time.deltaTime;
                }


                // update last time value
                lastUpdateTime = Time.time;
            }
        }

        private void OnZoomEvent(Gaze_ZoomEventArgs e)
        {
            if (zoom)
            {
                if (!currentZoom)
                {
                    zoomTime = 0;
                    currentZoom = true;
                }

                lastZoomCollider = e.Collider;
                fovFactor = e.FovFactor;
                zoomSpeedFactor = e.ZoomSpeedFactor;

                if (e.DezoomMode.Equals(Gaze_DezoomMode.CUSTOM))
                {
                    dezoomSpeedFactor = e.DezoomSpeedFactor;
                }
                else
                {
                    dezoomSpeedFactor = zoomSpeedFactor;
                }

                if (e.ZoomCurve != null)
                {
                    zoomCurve = e.ZoomCurve;
                }
                else
                {
                    zoomCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
                }
            }
        }
    }
}
