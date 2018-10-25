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

using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;

namespace Gaze
{
    public class Gaze_InputManager : MonoBehaviour
    {
        public static event Action<Touch> OnScreenTouched;
        public static event Action<Gaze_InteractiveObject> OnObjectTouchedDown;
        public static event Action<Gaze_InteractiveObject> OnObjectTouchUp;

        public static Gaze_InputManager instance = null;
        private bool m_WasTouching = false;

        private List<Gaze_InteractiveObject> m_RaycastedGameobjects = new List<Gaze_InteractiveObject>();
        private List<Gaze_InteractiveObject> m_CurrentRaycastedObjects = new List<Gaze_InteractiveObject>();

        private Touch m_Touch;

        void Awake()
        {
            instance = this;
        }

        private void Update()
        {
#if UNITY_EDITOR
            /* For testing in Unity */
            if (Input.GetMouseButtonDown(0))
            {
                m_Touch = new Touch();
                m_Touch.phase = TouchPhase.Began;
                m_Touch.position = Input.mousePosition;
                if (OnScreenTouched != null)
                    OnScreenTouched(m_Touch);
                m_WasTouching = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                m_Touch = new Touch();
                m_Touch.phase = TouchPhase.Ended;
                m_Touch.position = Input.mousePosition;
                if (OnScreenTouched != null)
                    OnScreenTouched(m_Touch);
                m_WasTouching = false;
            }

#else

            if (Input.touchCount > 0 )
            {
                if (!m_WasTouching && OnScreenTouched != null)
                {
                    m_Touch = Input.touches[0];
                    m_Touch.phase = TouchPhase.Began;
                    OnScreenTouched(m_Touch);
                }
                m_WasTouching = true;
            }
            else if (m_WasTouching)
            {
                if (OnScreenTouched != null)
                {
                    m_Touch = new Touch();
                    m_Touch.phase = TouchPhase.Ended;
                    OnScreenTouched(m_Touch);
                }
                m_WasTouching = false;
            }
#endif


            if (m_WasTouching)
            {

                CheckRaycastedObjects();
            }
            else
            {
                NotifyObjectsTouchUp();
            }
        }


        
        private void CheckRaycastedObjects()
        {
            if (OnObjectTouchedDown == null)
                return;
            m_Touch.position = GetTouchPosition();
            var screenPosition = S_ArUtilitiesManager.SelectedCamera.ScreenToViewportPoint(m_Touch.position);
            Ray ray = S_ArUtilitiesManager.SelectedCamera.ViewportPointToRay(screenPosition);
        
            // Raycast against the IOs layer
            LayerMask lm = 1 << 8;

            RaycastHit[] hits = Physics.RaycastAll (ray);
            foreach(RaycastHit hit in hits)
            {
                Gaze_InteractiveObject collider = hit.collider.GetComponentInParent<Gaze_InteractiveObject>();
                if (collider != null)
                {
                    // All the raycasted ios goes here
                    m_CurrentRaycastedObjects.Add(collider);
                    
                    // The new ones are computed here (Those will be notified)
                    if (!m_RaycastedGameobjects.Contains(collider))
                        OnObjectTouchedDown(collider);
                }
            }
            
            // We need to compute also the ones that are no longer raycasted
            Gaze_InteractiveObject io;
            for (int i = m_RaycastedGameobjects.Count -1; i >= 0; --i)
            {
                io = m_RaycastedGameobjects[i];
                if (!m_CurrentRaycastedObjects.Contains(io))
                {
                    OnObjectTouchUp(io);
                    m_RaycastedGameobjects.RemoveAt(i);
                }
            }
            
            m_RaycastedGameobjects = new List<Gaze_InteractiveObject>(m_CurrentRaycastedObjects);
            m_CurrentRaycastedObjects.Clear();
        }

        private Vector2 GetTouchPosition()
        {
#if UNITY_EDITOR
            return Input.mousePosition;
#else
            return Input.touches[0].position;
#endif
        }
        
        private void NotifyObjectsTouchUp()
        {
            if (OnObjectTouchUp != null)
            {       
                foreach (Gaze_InteractiveObject io in m_RaycastedGameobjects)
                {
                    OnObjectTouchUp(io);
                }
            }
            
            m_RaycastedGameobjects.Clear();
        }
    }
}