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
        public Gaze_InteractiveObject TargetObject;
        private Gaze_DragAndDropManager dragAndDropManager, receivedDnDManager;

        /// <summary>
        /// If TRUE, once dropped, the object can't be grabbed again.
        /// </summary>
        public bool attached = false;

        public Gaze_DragAndDropCondition(Gaze_Conditions _gazeConditionsScript) : base(_gazeConditionsScript) { }

        private GameObject dropTarget;

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

        private void HandleConditionAction(Gaze_DragAndDropStates state)
        {
            // exit if we are a drop object and target is the not a valid one (not in the list)
            if (!gazeConditionsScript.RootIO.DnD_IsTarget && !IsTargetValid())
                return;

            // compare the received state with the one specified in the dnd conditions
            if (gazeConditionsScript.dndEventValidator.Equals((int)state))
                IsValid = true;
        }

        private bool IsTargetValid()
        {
            Gaze_InteractiveObject rootIO = gazeConditionsScript.RootIO;

            // if any target validates the condition
            if (gazeConditionsScript.dndTargetModesIndex.Equals((int)apelab_DnDTargetsModes.ANY))
            {
                // get targets count
                int targetsCount = rootIO.DnD_Targets.Count;

                // iterate and find one valid
                for (int i = 0; i < targetsCount; i++)
                {
                    if (rootIO.DnD_Targets[i].Equals(dropTarget))
                    {
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
                    if (gazeConditionsScript.dndTargets[i].Equals(dropTarget))
                        return true;
                }
            }

            return false;
        }

        private void OnDragAndDropEvent(Gaze_DragAndDropEventArgs e)
        {
            // get the DragAndDrop 
            receivedDnDManager = ((GameObject)e.Sender).GetComponent<Gaze_DragAndDropManager>();
            dropTarget = (GameObject)e.TargetObject;
            if ((gazeConditionsScript.RootIO.DnD_IsTarget && gazeConditionsScript.RootIO.gameObject.Equals(dropTarget))
                || receivedDnDManager && receivedDnDManager.gameObject.Equals(Gaze_Utils.GetIOFromObject(gazeConditionsScript.gameObject).GetComponent<Gaze_DragAndDropManager>().gameObject))
            {
                // check if state is valid
                HandleConditionAction(e.State);
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