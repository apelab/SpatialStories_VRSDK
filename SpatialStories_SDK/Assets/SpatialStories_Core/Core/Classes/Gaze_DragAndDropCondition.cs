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
using UnityEngine;

namespace Gaze
{
    public class Gaze_DragAndDropCondition : Gaze_AbstractConditions
    {

        public Gaze_CustomConditionActionEnum onDropReady = Gaze_CustomConditionActionEnum.NONE;
        public Gaze_CustomConditionActionEnum onDrop = Gaze_CustomConditionActionEnum.NONE;
        public Gaze_CustomConditionActionEnum onPickup = Gaze_CustomConditionActionEnum.NONE;
        public Gaze_CustomConditionActionEnum onRemove = Gaze_CustomConditionActionEnum.NONE;

        public Gaze_InteractiveObject TargetObject;
        private Gaze_DragAndDropManager DragAndDropManager;

        /// <summary>
        /// If TRUE, once dropped, the object can't be grabbed again.
        /// </summary>
        public bool attached = false;

        private void Awake()
        {
            DragAndDropManager = Gaze_Utils.GetIOFromObject(gameObject).GetComponent<Gaze_DragAndDropManager>();
        }

        void OnEnable()
        {
            Gaze_EventManager.OnDragAndDropEvent += OnDragAndDropEvent;
            Gaze_EventManager.OnTriggerStateEvent += Gaze_EventManager_OnTriggerStateEvent;
            Gaze_EventManager.OnDependenciesValidated += Gaze_EventManager_OnDependenciesValidated;
        }

        void OnDisable()
        {
            Gaze_EventManager.OnDragAndDropEvent -= OnDragAndDropEvent;
            Gaze_EventManager.OnTriggerStateEvent -= Gaze_EventManager_OnTriggerStateEvent;
            Gaze_EventManager.OnDependenciesValidated -= Gaze_EventManager_OnDependenciesValidated;

        }

        void HandleConditionAction(Gaze_CustomConditionActionEnum action)
        {
            switch (action)
            {
                case Gaze_CustomConditionActionEnum.SEND_TRUE:
                    ValidateCustomCondition(true);
                    break;
                case Gaze_CustomConditionActionEnum.SEND_FALSE:
                    ValidateCustomCondition(false);
                    break;
                case Gaze_CustomConditionActionEnum.RELOAD:
                    GetComponent<Gaze_Conditions>().ManualReload();
                    break;
                default:
                    break;
            }
            //GetComponentInParent<Gaze_InteractiveObject> ().isCatchable = !attached;
        }

        void OnDragAndDropEvent(Gaze_DragAndDropEventArgs e)
        {
            Gaze_DragAndDropManager manager = GetComponentInParent<Gaze_DragAndDropManager>();

            if (((GameObject)e.Sender).GetComponent<Gaze_DragAndDropManager>().CurrentDragAndDropCondition != this)
                return;

            if (manager && (GameObject)e.Sender == manager.gameObject)
            {
                switch (e.State)
                {
                    case Gaze_DragAndDropStates.DROPREADY:
                        HandleConditionAction(onDropReady);
                        break;
                    case Gaze_DragAndDropStates.DROP:
                        HandleConditionAction(onDrop);
                        break;
                    case Gaze_DragAndDropStates.PICKUP:
                        HandleConditionAction(onPickup);
                        break;
                    case Gaze_DragAndDropStates.REMOVE:
                        HandleConditionAction(onRemove);
                        break;
                    default:
                        break;
                }
            }

        }

        private void Gaze_EventManager_OnTriggerStateEvent(Gaze_TriggerStateEventArgs e)
        {
            if (DragAndDropManager == null)
                return;

            if (!Gaze_Utils.AreUnderSameGameObject(gameObject, e.Sender) ||
                e.TriggerState != Gaze_TriggerState.ACTIVE ||
                !GetComponent<Gaze_Conditions>().ActivateOnDependencyMap.AreDependenciesSatisfied)
                return;

            DragAndDropManager.SetupDragAndDropProcess(this);
        }

        private void Gaze_EventManager_OnDependenciesValidated(Gaze_DependenciesValidatedEventArgs e)
        {
            if (DragAndDropManager == null)
                return;

            if (!Gaze_Utils.AreUnderSameGameObject(e.Sender, this))
                return;

            DragAndDropManager.SetupDragAndDropProcess(this);
        }
    }
}