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
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Gaze
{
    public class Gaze_DragAndDropCondition : Gaze_AbstractCondition
    {
        public Gaze_InteractiveObject TargetObject;
        private Gaze_DragAndDropManager dragAndDropManager;
        private bool debug = false;

        public Gaze_DragAndDropCondition(Gaze_Conditions _gazeConditionsScript) : base(_gazeConditionsScript) { }

        private GameObject dropTarget;
        private GameObject dropObject;

        protected override void CustomSetup()
        {
            Gaze_EventManager.OnDragAndDropEvent += OnDragAndDropEvent;
            gazeConditionsScript.OnTriggerStateEvent += Gaze_EventManager_OnTriggerStateEvent;
            Gaze_EventManager.OnDependenciesValidated += Gaze_EventManager_OnDependenciesValidated;
            dragAndDropManager = Gaze_Utils.GetIOFromObject(gazeConditionsScript.gameObject).GetComponent<Gaze_DragAndDropManager>();
        }

        protected override void CustomDispose()
        {
            Gaze_EventManager.OnDragAndDropEvent -= OnDragAndDropEvent;
            gazeConditionsScript.OnTriggerStateEvent -= Gaze_EventManager_OnTriggerStateEvent;
            Gaze_EventManager.OnDependenciesValidated -= Gaze_EventManager_OnDependenciesValidated;
        }

        public override bool IsValidated()
        {
            return IsValid;
        }

        public override void ToEditorGUI()
        {
#if UNITY_EDITOR
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
#endif
        }

        private void IsDropTargetValid(GameObject dropObject, GameObject dropTarget, Gaze_DragAndDropStates state)
        {
            if (debug)
                Debug.Log("Hey, I'm the DnD Condition (" + gazeConditionsScript.gameObject + ") on DROP TARGET " + dropTarget + " with state =" + state);

            // if I'm the drop target
            if (dropTarget.Equals(gazeConditionsScript.Root))
            {

                // check if action is ok (DROP, CANCEL...)
                if (((int)state).Equals(gazeConditionsScript.dndEventValidator))
                {
                    if (debug)
                        Debug.Log("IsDropTargetValid OK with state =" + state);

                    IsValid = true;
                }
            }
        }

        private void IsDropObjectValid(GameObject dropObject, Gaze_DragAndDropStates state)
        {
            if (debug)
                Debug.Log("Hey, I'm the DnD Condition (" + gazeConditionsScript.gameObject + ") on DROP OBJECT " + dropObject + " with state =" + state);

            // if I'm the drop object
            if (dropObject.Equals(gazeConditionsScript.Root))
            {

                // check if action is ok (DROP, CANCEL...)
                if (((int)state).Equals(gazeConditionsScript.dndEventValidator))
                {
                    // if targets condition is ANY
                    if (gazeConditionsScript.dndTargetModesIndex.Equals((int)apelab_DnDTargetsModes.ANY))
                    {
                        // get the count of all IO's targets
                        int count = gazeConditionsScript.RootIO.DnD_Targets.Count;

                        // for all the targets in the IO's list
                        for (int i = 0; i < count; i++)
                        {
                            // if it contains the drop target
                            if (gazeConditionsScript.RootIO.DnD_Targets[i].Equals(dropTarget))
                            {
                                if (debug)
                                    Debug.Log("IsDropObjectValid OK with state =" + state);

                                IsValid = true;
                            }
                        }
                    }
                    // if targets condition is CUSTOM
                    else
                    {
                        // get the count of CUSTOM IO's targets
                        int count = gazeConditionsScript.dndTargets.Count;

                        // for all the targets in the Gaze_Conditions' list
                        for (int i = 0; i < count; i++)
                        {
                            // if the target is the good one
                            if (dropTarget.Equals(gazeConditionsScript.dndTargets[i]))
                            {
                                if (debug)
                                    Debug.Log("IsDropObjectValid OK with state =" + state);

                                IsValid = true;
                            }
                        }
                    }
                }
            }
        }

        private void HandleDnDCondition(Gaze_DragAndDropEventArgs e)
        {
            if (gazeConditionsScript.triggerStateIndex == (int)Gaze_TriggerState.BEFORE)
                return;

            // get the drop object and drop target 
            dropTarget = (GameObject)e.DropTarget;
            dropObject = (GameObject)e.DropObject;

            // if I'm the drop object
            if (dropObject.Equals(gazeConditionsScript.Root))
            {
                IsDropObjectValid(dropObject, e.State);
            }
            // if I'm the dropped target
            else if (gazeConditionsScript.Root.Equals(dropTarget))
            {
                IsDropTargetValid(dropObject, dropTarget, e.State);
            }
        }

        private void OnDragAndDropEvent(Gaze_DragAndDropEventArgs e)
        {
            HandleDnDCondition(e);
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