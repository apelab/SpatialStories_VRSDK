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
using SpatialStories;
using System.Collections.Generic;
using UnityEngine;

namespace Gaze
{
    public abstract partial class Gaze_AbstractBehaviour : S_AbstractInteractionDataMonoBehaviour
    {
        // delay actions
        [HideInInspector]
        public bool isDelayed;
        [HideInInspector]
        public float delayTime;
        [HideInInspector]
        public bool isDelayRandom;
        [HideInInspector]
        public float[] delayRange = { 0.0f, 0.0f };
        [HideInInspector]
        public bool multipleActionsInTime;
        private delegate void BehaviorHandler();
        private List<Request> requests = new List<Request>();


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

        void Update()
        {
            for (int i = requests.Count - 1; i >= 0; i--)
            {
                if (Time.time > requests[i].GetTime())
                {
                    requests[i].CallHandler();
                    requests.RemoveAt(i);
                }
            }
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
                        if (isDelayed)
                            HandleActionsInTime(() => OnBefore(), TriggerEventsAndStates.OnBefore);
                        else
                            OnBefore();
                        break;

                    case Gaze_TriggerState.ACTIVE:
                        if (isDelayed)
                            HandleActionsInTime(() => OnActive(), TriggerEventsAndStates.OnActive);
                        else
                            OnActive();
                        break;

                    case Gaze_TriggerState.AFTER:
                        if (isDelayed)
                            HandleActionsInTime(() => OnAfter(), TriggerEventsAndStates.OnAfter);
                        else
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

                    //check if the action should be delayed
                    if (isDelayed)
                        HandleActionsInTime(() => OnTrigger(), TriggerEventsAndStates.OnTrigger);
                    else
                        OnTrigger();
                }
                else if (e.IsReload)
                {
                    // execute reload
                    ReloadCount = e.Count;
                    TimeLastReloaded = e.Time;

                    //check if the action should be delayed
                    if (isDelayed)
                        HandleActionsInTime(() => OnReload(), TriggerEventsAndStates.OnReload);
                    else
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

        private void HandleActionsInTime(BehaviorHandler _handler, TriggerEventsAndStates _type)
        {
            if (multipleActionsInTime)
                requests.Add(new Request(Time.time + delayTime, _handler, _type));
            else
            {
                for (int i = 0; i < requests.Count; i++)
                {
                    if (requests[i].GetRequestType() == (_type))
                        return;
                }
                requests.Add(new Request(Time.time + delayTime, _handler, _type));
            }
        }

        /// <summary>
        /// This is the method to implement to describe the behaviour once triggered.
        /// </summary>
        protected abstract void OnTrigger();

        protected virtual void OnReload(){}

        /// <summary>
        /// Implements here the actions to take on active, before and after states.
        /// </summary>
        protected virtual void OnBefore(){}

        protected virtual void OnActive(){}

        protected virtual void OnAfter(){}


        private struct Request
        {
            private float time;
            public float GetTime() { return time; }

            private BehaviorHandler handler;
            public void CallHandler() { handler(); }

            private TriggerEventsAndStates requestType;
            public TriggerEventsAndStates GetRequestType() { return requestType; }

            public Request(float tm, BehaviorHandler h, TriggerEventsAndStates tp)
            {
                time = tm;
                handler = h;
                requestType = tp;
            }
        }

    }
}