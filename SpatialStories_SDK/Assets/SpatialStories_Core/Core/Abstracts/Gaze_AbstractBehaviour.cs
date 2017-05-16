// <copyright file="Gaze_AbstractBehaviour.cs" company="apelab sàrl">
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
using UnityEngine;

namespace Gaze
{
    public abstract class Gaze_AbstractBehaviour : MonoBehaviour
    {
        protected Gaze_Conditions gazable;
        protected Gaze_TriggerState triggerState;
        protected int TriggerCount;
        protected float TimeLastTriggered;
        protected int ReloadCount;
        protected float TimeLastReloaded;
        protected Gaze_AutoTriggerMode autoTriggerMode;
        protected Gaze_ReloadMode ReloadMode;
        protected int ReloadMaxRepetitions;
        // is the camera in the collider proximity zone
        protected bool isInProximity;
        // is the proximity zone enabled
        //		protected bool proximityEnabled;

        public virtual void OnEnable()
        {
            gazable = GetComponent<Gaze_Conditions>();
            Gaze_EventManager.OnTriggerStateEvent += onTriggerStateEvent;
            Gaze_EventManager.OnTriggerEvent += onTriggerEvent;
            Gaze_EventManager.OnProximityEvent += onProximityEvent;
        }

        public virtual void OnDisable()
        {
            Gaze_EventManager.OnTriggerStateEvent -= onTriggerStateEvent;
            Gaze_EventManager.OnTriggerEvent -= onTriggerEvent;
            Gaze_EventManager.OnProximityEvent -= onProximityEvent;
        }

        /// <summary>
        /// When the camea enters the proximity's collider zone
        /// </summary>
        /// <param name="e">The Gaze_ProximityEventArgs</param>
        private void onProximityEvent(Gaze_ProximityEventArgs e)
        {
            if (gazable != null && e.Sender.Equals(gazable.Root))
            {
                isInProximity = e.IsInProximity;
            }
        }

        private void onTriggerStateEvent(Gaze_TriggerStateEventArgs e)
        {
            // if sender is the this Behaviour gameObject
            if ((GameObject)e.Sender == gameObject)
            {
                triggerState = e.TriggerState;

                switch (triggerState)
                {
                    case Gaze_TriggerState.BEFORE:
                        OnBefore();
                        break;
                    case Gaze_TriggerState.ACTIVE:
                        OnActive();
                        break;
                    case Gaze_TriggerState.AFTER:
                        OnAfter();
                        break;
                }
            }
        }

        private void onTriggerEvent(Gaze_TriggerEventArgs e)
        {
            // if sender is the this Behaviour gameObject
            if ((GameObject)e.Sender == gameObject)
            {
                if (e.IsTrigger)
                {
                    // execute trigger 
                    TriggerCount = e.Count;
                    TimeLastTriggered = e.Time;

                    OnTrigger();
                }
                else if (e.IsReload)
                {
                    // execute reload
                    ReloadCount = e.Count;
                    TimeLastReloaded = e.Time;

                    OnReload();
                }
                else
                {
                    // initialize trigger
                    autoTriggerMode = e.AutoTriggerMode;
                    ReloadMode = e.ReloadMode;
                    ReloadMaxRepetitions = e.ReloadMaxRepetitions;
                }
            }
        }

        /// <summary>
        /// This is the method to implement to describe the behaviour once triggered.
        /// </summary>
        protected abstract void OnTrigger();

        protected abstract void OnReload();

        /// <summary>
        /// Implements here the actions to take on active, before and after states.
        /// </summary>
        protected abstract void OnBefore();

        protected abstract void OnActive();

        protected abstract void OnAfter();

    }
}