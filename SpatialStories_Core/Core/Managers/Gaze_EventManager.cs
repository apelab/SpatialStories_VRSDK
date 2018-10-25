// <copyright file="Gaze_EventManager.cs" company="apelab sàrl">
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

namespace Gaze
{
    public static class Gaze_EventManager
    {
        /// <summary>
        /// Fired whenever an object is being gazed and ungazed.
        /// </summary>
        public delegate void GazeHandler(Gaze_GazeEventArgs e);
        public static event GazeHandler OnGazeEvent;
        public static void FireGazeEvent(Gaze_GazeEventArgs e)
        {
            if (OnGazeEvent != null)
                OnGazeEvent(e);
        }

        /// <summary>
        /// Fired when a Trigger changes states.
        /// </summary>
        public delegate void TriggerStateHandler(Gaze_TriggerStateEventArgs e);
        public static event TriggerStateHandler OnTriggerStateEvent;
        public static void FireTriggerStateEvent(Gaze_TriggerStateEventArgs e)
        {
            if (OnTriggerStateEvent != null)
                OnTriggerStateEvent(e);
        }

        /// <summary>
        /// Fired when a Trigger is triggered.
        /// </summary>
        public delegate void TriggerHandler(Gaze_TriggerEventArgs e);
        public static event TriggerHandler OnTriggerEvent;
        public static void FireTriggerEvent(Gaze_TriggerEventArgs e)
        {
            if (OnTriggerEvent != null)
                OnTriggerEvent(e);
        }

        /// <summary>
        /// Fired on Trigger's audio related event occurs.
        /// </summary>
        public delegate void AudioPlayerHandler(Gaze_AudioPlayerEventArgs e);
        public static event AudioPlayerHandler OnAudioPlayerEvent;
        public static void FireAudioPlayerEvent(Gaze_AudioPlayerEventArgs e)
        {
            if (OnAudioPlayerEvent != null)
                OnAudioPlayerEvent(e);
        }

        /// <summary>
        /// Fired when two Triggers collide.
        /// </summary>
        public delegate void CollisionHandler(Gaze_CollisionEventArgs e);
        public static event CollisionHandler OnCollisionEvent;
        public static void FireCollisionEvent(Gaze_CollisionEventArgs e)
        {
            if (OnCollisionEvent != null)
                OnCollisionEvent(e);
        }

        /// <summary>
        /// Fired when the camera enters the object's proximity zone.
        /// </summary>
        public delegate void ProximityHandler(Gaze_ProximityEventArgs e);
        public static event ProximityHandler OnProximityEvent;
        public static void FireProximityEvent(Gaze_ProximityEventArgs e)
        {
            if (OnProximityEvent != null)
            {
                OnProximityEvent(e);
            }

        }

        /// <summary>
        /// Fired when an object with zoom enabled is gazed.
        /// </summary>
        public delegate void ZoomHandler(Gaze_ZoomEventArgs e);
        public static event ZoomHandler OnZoomEvent;
        public static void FireZoomEvent(Gaze_ZoomEventArgs e)
        {
            if (OnZoomEvent != null)
                OnZoomEvent(e);
        }

        /// <summary>
        /// Fired when the custom condition notifies the Gaze_Conditions
        /// </summary>
        public delegate void CustomConditionHandler(Gaze_CustomConditionEventArgs e);
        public static event CustomConditionHandler OnCustomConditionEvent;
        public static void FireCustomConditionEvent(Gaze_CustomConditionEventArgs e)
        {
            if (OnCustomConditionEvent != null)
                OnCustomConditionEvent(e);
        }

        /// <summary>
        /// Fired when all the dependencies of a trigger have been validated
        /// </summary>
        public delegate void DependenciesValidatedHandler(Gaze_DependenciesValidatedEventArgs e);
        public static event DependenciesValidatedHandler OnDependenciesValidated;
        public static void FireOnDependenciesValidated(Gaze_DependenciesValidatedEventArgs e)
        {
            if (OnDependenciesValidated != null)
                OnDependenciesValidated(e);
        }

        /// Fired when the controller is triggering a laser raycast
        /// </summary>
        public delegate void LaserHandler(Gaze_LaserEventArgs e);
        public static event LaserHandler OnLaserEvent;
        public static void FireLaserEvent(Gaze_LaserEventArgs e)
        {
            if (OnLaserEvent != null)
                OnLaserEvent(e);
        }

        /// Fired when the controller is teleporting
        /// </summary>
        public delegate void IODestroyHandler(Gaze_IODestroyEventArgs e);
        public static event IODestroyHandler OnIODestroyed;
        public static void FireOnIODestroyed(Gaze_IODestroyEventArgs e)
        {
            if (OnIODestroyed != null)
                OnIODestroyed(e);
        }

        /// <summary>
        /// FIred when an AR detected plane is on the center of the screen (Like Gaze)
        /// </summary>
        public delegate void PointingToAPlaneHandler(bool _overAPlane);
        public static event PointingToAPlaneHandler OnPointingOverPlane;
        public static void FireOnPointingToAPlaneEvent(bool _pointingIntoPlane)
        {
            if (OnPointingOverPlane != null)
                OnPointingOverPlane(_pointingIntoPlane);
        }
    }
}