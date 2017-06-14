//-----------------------------------------------------------------------
// <copyright file="Gaze_DragAndDropCondition.cs" company="apelab sàrl">
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
using UnityEditor;
using UnityEngine;

namespace Gaze
{
    public class Gaze_DragAndDropCondition : Gaze_AbstractCondition
    {

        public Gaze_CustomConditionActionEnum onDropReady = Gaze_CustomConditionActionEnum.NONE;
        public Gaze_CustomConditionActionEnum onDrop = Gaze_CustomConditionActionEnum.NONE;
        public Gaze_CustomConditionActionEnum onPickup = Gaze_CustomConditionActionEnum.NONE;
        public Gaze_CustomConditionActionEnum onRemove = Gaze_CustomConditionActionEnum.NONE;

        public Gaze_InteractiveObject TargetObject;
        private Gaze_DragAndDropManager dragAndDropManager, receivedDnDManager;

        /// <summary>
        /// If TRUE, once dropped, the object can't be grabbed again.
        /// </summary>
        public bool attached = false;

        public Gaze_DragAndDropCondition(Gaze_Conditions _gazeConditionsScript) : base(_gazeConditionsScript) { }

        private GameObject dropObject, dropTarget;

        protected override void CustomSetup()
        {
            Gaze_EventManager.OnDragAndDropEvent += OnDragAndDropEvent;
            Gaze_EventManager.OnTriggerStateEvent += Gaze_EventManager_OnTriggerStateEvent;
            Gaze_EventManager.OnDependenciesValidated += Gaze_EventManager_OnDependenciesValidated;
            dragAndDropManager = Gaze_Utils.GetIOFromObject(gazeConditionsScript.gameObject).GetComponent<Gaze_DragAndDropManager>();
        }

        protected override void CustomDispose()
        {
            Gaze_EventManager.OnDragAndDropEvent -= OnDragAndDropEvent;
            Gaze_EventManager.OnTriggerStateEvent -= Gaze_EventManager_OnTriggerStateEvent;
            Gaze_EventManager.OnDependenciesValidated -= Gaze_EventManager_OnDependenciesValidated;
        }

        public override bool IsValidated()
        {
            return IsValid;
        }

        public override void ToEditorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (IsValid)
            {
                RenderSatisfiedLabel("Drag And Drop:");
                RenderSatisfiedLabel("True");
            }
            else
            {
                RenderNonSatisfiedLabel("Drag And Drop:");
                RenderNonSatisfiedLabel("False");
            }
            EditorGUILayout.EndHorizontal();
        }

        private void HandleConditionAction(Gaze_CustomConditionActionEnum action)
        {
            if (!IsTargetValid())
                return;

            switch (action)
            {
                case Gaze_CustomConditionActionEnum.SEND_TRUE:
                    IsValid = true;
                    break;

                case Gaze_CustomConditionActionEnum.SEND_FALSE:
                    IsValid = false;
                    break;

                case Gaze_CustomConditionActionEnum.RELOAD:
                    gazeConditionsScript.GetComponent<Gaze_Conditions>().ManualReload();
                    break;

                default:
                    break;
            }
        }

        // check if the target is in the list
        private bool IsTargetValid()
        {
            Gaze_InteractiveObject rootIO = gazeConditionsScript.RootIO;

            // if any target validates the condition
            if (gazeConditionsScript.dndTargetModesIndex.Equals((int)apelab_DnDTargetsModes.ANY))
            {
                // get targets count
                int targetsCount = rootIO.DnD_TargetsIndexes.Count;

                // iterate and find one valid
                for (int i = 0; i < targetsCount; i++)
                {
                    if (Gaze_SceneInventory.Instance.InteractiveObjects[rootIO.DnD_TargetsIndexes[i]].Equals(dropTarget))
                    {
                        Debug.Log("valid drop target " + dropTarget);
                        return true;
                    }
                }
            }
            else
            {
                // iterate through selected targets
                int targetsCount = gazeConditionsScript.dndTargets.Count;
                for (int i = 0; i < targetsCount; i++)
                {
                    Debug.Log(gazeConditionsScript.RootIO + " dnd targets " + Gaze_SceneInventory.Instance.InteractiveObjects[gazeConditionsScript.dndTargets[i]]);
                }
                //for (int i = 0; i < targetsCount; i++)
                //{
                //    if (Gaze_SceneInventory.Instance.InteractiveObjects[gazeConditionsScript.dndTargets[i]].Equals(dropTarget))
                //    {
                //        Debug.Log("valid drop target " + dropTarget);
                //        return true;
                //    }
                //}
            }

            Debug.Log("invalid drop target " + dropTarget);
            return false;
        }

        private void OnDragAndDropEvent(Gaze_DragAndDropEventArgs e)
        {
            // get the DragAndDrop 
            receivedDnDManager = ((GameObject)e.Sender).GetComponent<Gaze_DragAndDropManager>();
            dropObject = (GameObject)e.Sender;
            dropTarget = (GameObject)e.TargetObject;

            if (receivedDnDManager && (GameObject)e.Sender == receivedDnDManager.gameObject)
            {
                // check if state is valid
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
            // exit if I have no DnD manager
            if (dragAndDropManager == null)
                return;

            // exit if the sender is not my manager
            if (!Gaze_Utils.AreUnderSameGameObject(gazeConditionsScript.gameObject, e.Sender))
                return;

            // exit if my IO is not active
            if (e.TriggerState != Gaze_TriggerState.ACTIVE)
                return;

            // exit if my dependencies are not satisfied
            if (!gazeConditionsScript.GetComponent<Gaze_Conditions>().ActivateOnDependencyMap.AreDependenciesSatisfied)
                return;

            dragAndDropManager.SetupDragAndDropProcess(gazeConditionsScript);
        }

        private void Gaze_EventManager_OnDependenciesValidated(Gaze_DependenciesValidatedEventArgs e)
        {
            // exit if I have no DnD manager
            if (dragAndDropManager == null)
                return;

            // exit if the sender is not my manager
            if (!Gaze_Utils.AreUnderSameGameObject(e.Sender, this))
                return;

            dragAndDropManager.SetupDragAndDropProcess(gazeConditionsScript);
        }
    }
}