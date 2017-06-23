

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gaze
{
    [CanEditMultipleObjects]
    [InitializeOnLoad]
    [CustomEditor(typeof(Gaze_Actions))]
    public class Gaze_ActionsEditor : Gaze_Editor
    {
        private Gaze_InteractiveObject rootIO;

        private Gaze_Actions actionsScript;
        private List<Animator> hierarchyAnimators;
        private List<string> hierarchyAnimatorNames;
        private List<AudioSource> hierarchyAudioSources;
        private List<string> hierarchyAudioSourceNames;
        private List<string> selectedAnimatorTriggers;
        private string[] grabModes;
        private string[] allVisuals;
        private List<Renderer> allRenderers;
        private List<string> dnd_dropTargetsNames;
        private string[] dndEventValidatorEnum;

        void OnEnable()
        {
            actionsScript = (Gaze_Actions)target;
            rootIO = actionsScript.GetComponentInParent<Gaze_InteractiveObject>();

            //actionsScript.isActive = true;

            EditorApplication.update += OnEditorApplicationUpdate;

            hierarchyAnimators = new List<Animator>();
            hierarchyAnimatorNames = new List<string>();
            hierarchyAudioSources = new List<AudioSource>();
            hierarchyAudioSourceNames = new List<string>();
            selectedAnimatorTriggers = new List<string>();
            dnd_dropTargetsNames = new List<string>();
            dndEventValidatorEnum = Enum.GetNames(typeof(Gaze_DragAndDropStates));

            grabModes = Enum.GetNames(typeof(Gaze_GrabMode));

            FindAnimatorsInHierarchy();
            FindAudioSourcesInHierarchy();
        }

        void OnDisable()
        {
            EditorApplication.update -= OnEditorApplicationUpdate;

            //actionsScript.isActive = false;
        }

        private void UpdateDropTargetsNames()
        {
            dnd_dropTargetsNames.Clear();

            // rebuild them
            if (Gaze_SceneInventory.Instance != null)
            {
                for (int i = 0; i < Gaze_SceneInventory.Instance.InteractiveObjectsCount; i++)
                {
                    dnd_dropTargetsNames.Add(Gaze_SceneInventory.Instance.InteractiveObjects[i].gameObject.name);
                }
            }
        }

        public void ShowAudioOptions()
        {
            #region AudioSource

            actionsScript.ActionAudio = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("Audio", actionsScript.ActionAudio);

            if (actionsScript.ActionAudio != Gaze_Actions.ACTIVABLE_OPTION.NOTHING)
            {
                EditorGUILayout.BeginVertical();
                if (hierarchyAudioSources.Count > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Audio Source");

                    if (!hierarchyAudioSources.Contains(actionsScript.targetAudioSource))
                    {
                        actionsScript.targetAudioSource = hierarchyAudioSources[0];
                    }

                    actionsScript.targetAudioSource = hierarchyAudioSources[EditorGUILayout.Popup(hierarchyAudioSources.IndexOf(actionsScript.targetAudioSource), hierarchyAudioSourceNames.ToArray())];

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.HelpBox("No audio sources found.", MessageType.Warning);
                }
                EditorGUILayout.EndVertical();

                #endregion AudioSource

                #region AudioClips
                if (actionsScript.ActionAudio == Gaze_Actions.ACTIVABLE_OPTION.ACTIVATE)
                {
                    for (int i = 0; i < Gaze_HashIDs.TriggerEventsAndStates.Length; i++)
                    {
                        actionsScript.activeTriggerStatesAudio[i] = EditorGUILayout.ToggleLeft(Gaze_HashIDs.TriggerEventsAndStates[i], actionsScript.activeTriggerStatesAudio[i]);

                        if (actionsScript.activeTriggerStatesAudio[i])
                        {
                            DisplayAudioBlock(i);
                        }
                    }
                }
                #endregion onAfter 

                if (actionsScript.ActionAudio == Gaze_Actions.ACTIVABLE_OPTION.ACTIVATE)
                {
                    #region Ducking
                    if (actionsScript.ActionAudio == Gaze_Actions.ACTIVABLE_OPTION.ACTIVATE)
                    {
                        EditorGUILayout.Space();
                        actionsScript.duckingEnabled = EditorGUILayout.ToggleLeft("Ducking", actionsScript.duckingEnabled);
                        if (actionsScript.duckingEnabled)
                        {
                            DisplayAudioDuckingBlock();
                        }
                        else
                        {
                            DisplayAudioVolume();
                        }
                    }
                    #endregion
                }
            }
        }

        public void ShowAnimationOptions()
        {
            #region Animations
            actionsScript.triggerAnimation = EditorGUILayout.ToggleLeft("Animation", actionsScript.triggerAnimation);
            #endregion Animations

            #region Animator
            if (actionsScript.triggerAnimation)
            {
                EditorGUILayout.BeginHorizontal();
                if (hierarchyAnimators.Count > 0)
                {

                    EditorGUILayout.LabelField("Animator");

                    if (!hierarchyAnimators.Contains(actionsScript.targetAnimator))
                    {
                        actionsScript.targetAnimator = hierarchyAnimators[0];
                    }

                    actionsScript.targetAnimator = hierarchyAnimators[EditorGUILayout.Popup(hierarchyAnimators.IndexOf(actionsScript.targetAnimator), hierarchyAnimatorNames.ToArray())];
                }
                else
                {
                    EditorGUILayout.HelpBox("No animators found.", MessageType.Warning);
                }
                EditorGUILayout.EndHorizontal();
            }
            #endregion Animator

            #region AnimationTriggers
            if (actionsScript.triggerAnimation)
            {
                for (int i = 0; i < Gaze_HashIDs.TriggerEventsAndStates.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    actionsScript.activeTriggerStatesAnim[i] = EditorGUILayout.ToggleLeft(Gaze_HashIDs.TriggerEventsAndStates[i], actionsScript.activeTriggerStatesAnim[i]);

                    if (actionsScript.activeTriggerStatesAnim[i])
                    {
                        DisplayAnimBlock(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();
            }
            #endregion AnimationTriggers
        }

        public override void Gaze_OnInspectorGUI()
        {
            base.BeginChangeComparision();

            // display conditions
            if (actionsScript.isActive)
            {
                if (!Application.isPlaying)
                {
                    UpdateDropTargetsNames();

                    EditorGUILayout.Space();
                    if (actionsScript && !actionsScript.DestroyOnTrigger)
                    {
                        ShowVisualsOption();
                        EditorGUILayout.Space();
                        ShowCollidersOption();
                        EditorGUILayout.Space();
                        ShowGrabOption();
                        EditorGUILayout.Space();
                        ShowGrabDistanceOption();
                        EditorGUILayout.Space();
                        ShowGrabModeOption();
                        EditorGUILayout.Space();
                        ShowTouchAbilityOption();
                        EditorGUILayout.Space();
                        ShowTouchDistanceOption();
                        EditorGUILayout.Space();
                        actionsScript.ActionGravity = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("Gravity", actionsScript.ActionGravity);
                        EditorGUILayout.Space();
                        ShowDragAndDropOption();
                        EditorGUILayout.Space();
                        ShowAudioOptions();
                        EditorGUILayout.Space();
                        ShowAnimationOptions();
                    }
                    actionsScript.DestroyOnTrigger = EditorGUILayout.ToggleLeft("Destroy", actionsScript.DestroyOnTrigger);

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Delay", EditorStyles.boldLabel);
                    ShowDelayBlock();
                }
            }

            // save changes
            base.EndChangeComparision();
            EditorUtility.SetDirty(actionsScript);
        }

        private void ShowTouchDistanceOption()
        {
            actionsScript.ModifyTouchDistance = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup("Touch Distance", actionsScript.ModifyTouchDistance);
            if (actionsScript.ModifyTouchDistance == Gaze_Actions.ALTERABLE_OPTION.MODIFY)
            {
                EditorGUILayout.BeginHorizontal();
                actionsScript.touchDistance = EditorGUILayout.FloatField("", actionsScript.touchDistance);
                Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.touchDistance);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void ShowDragAndDropOption()
        {
            actionsScript.ModifyDragAndDrop = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup("Drag And Drop", actionsScript.ModifyDragAndDrop);

            if (actionsScript.ModifyDragAndDrop.Equals(Gaze_Actions.ALTERABLE_OPTION.MODIFY))
            {
                actionsScript.ActionDragAndDrop = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("DnD Enable", actionsScript.ActionDragAndDrop);
                DisplayDnDMinDistance();
                DisplayDnDTargets();
                DisplayDnDRespectAxis();
                DisplayDnDAngleThreshold();
                DisplayDnDSnapBeforeDrop();
            }
        }

        private void DisplayDnDSnapBeforeDrop()
        {
            GUILayout.BeginHorizontal();
            actionsScript.ModifyDnDSnapBeforeDrop = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("DnD Snap Before Drop", actionsScript.ModifyDnDSnapBeforeDrop);
            GUILayout.EndHorizontal();
        }

        private void DisplayDnDMinDistance()
        {
            actionsScript.ModifyDnDMinDistance = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup("DnD Min Distance", actionsScript.ModifyDnDMinDistance);
            if (actionsScript.ModifyDnDMinDistance == Gaze_Actions.ALTERABLE_OPTION.MODIFY)
            {
                GUILayout.BeginHorizontal();
                actionsScript.dnDMinDistance = EditorGUILayout.FloatField(actionsScript.dnDMinDistance);
                GUILayout.EndHorizontal();
            }
        }

        private void DisplayDnDAngleThreshold()
        {
            actionsScript.ModifyDnDAngleThreshold = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup("DnD Angle Threshold", actionsScript.ModifyDnDAngleThreshold);

            if (actionsScript.ModifyDnDAngleThreshold.Equals(Gaze_Actions.ALTERABLE_OPTION.MODIFY))
            {
                EditorGUILayout.BeginHorizontal();
                actionsScript.dnDAngleThreshold = EditorGUILayout.Slider(actionsScript.dnDAngleThreshold, 1, 100);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DisplayDnDRespectAxis()
        {
            actionsScript.ModifyDnDRespectAxis = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup("Respect Axis", actionsScript.ModifyDnDRespectAxis);
            if (actionsScript.ModifyDnDRespectAxis == Gaze_Actions.ALTERABLE_OPTION.MODIFY)
            {
                GUILayout.BeginHorizontal();
                actionsScript.dnDRespectXAxis = EditorGUILayout.ToggleLeft("X Axis", actionsScript.dnDRespectXAxis);
                if (actionsScript.dnDRespectXAxis)
                    actionsScript.dnDRespectXAxisMirror = EditorGUILayout.ToggleLeft("Mirrored", actionsScript.dnDRespectXAxisMirror);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                actionsScript.dnDRespectYAxis = EditorGUILayout.ToggleLeft("Y Axis", actionsScript.dnDRespectYAxis);
                if (actionsScript.dnDRespectYAxis)
                    actionsScript.dnDRespectYAxisMirror = EditorGUILayout.ToggleLeft("Mirrored", actionsScript.dnDRespectYAxisMirror);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                actionsScript.dnDRespectZAxis = EditorGUILayout.ToggleLeft("Z Axis", actionsScript.dnDRespectZAxis);
                if (actionsScript.dnDRespectZAxis)
                    actionsScript.dnDRespectZAxisMirror = EditorGUILayout.ToggleLeft("Mirrored", actionsScript.dnDRespectZAxisMirror);
                GUILayout.EndHorizontal();
            }
        }

        private void DisplayDnDTargets()
        {
            actionsScript.ModifyDragAndDropTargets = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup("DnD Target(s)", actionsScript.ModifyDragAndDropTargets);
            if (actionsScript.ModifyDragAndDropTargets == Gaze_Actions.ALTERABLE_OPTION.MODIFY)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("New Drop Targets");
                GUILayout.EndHorizontal();

                // help message if no input is specified
                if (actionsScript.DnD_Targets.Count < 1)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("Add at least one drop target or deactivate this condition if not needed.", MessageType.Warning);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    // for each DnD target
                    for (int i = 0; i < actionsScript.DnD_Targets.Count; i++)
                    {
                        // display it in a popup
                        EditorGUILayout.BeginHorizontal();
                        actionsScript.DnD_Targets[i] = Gaze_SceneInventory.Instance.InteractiveObjects[EditorGUILayout.Popup(Gaze_SceneInventory.Instance.InteractiveObjects.IndexOf(actionsScript.DnD_Targets[i]), dnd_dropTargetsNames.ToArray())];

                        // and a '-' button to remove it if needed
                        if (GUILayout.Button("-"))
                            actionsScript.DnD_Targets.Remove(actionsScript.DnD_Targets[i]);

                        EditorGUILayout.EndHorizontal();
                    }
                }
                // display 'add' button
                if (GUILayout.Button("+"))
                {
                    // exit if there are no Interactive Object in the scene
                    if (Gaze_SceneInventory.Instance.InteractiveObjectsCount < 1)
                        return;

                    // add the first Interactive Object by default
                    actionsScript.DnD_Targets.Add(Gaze_SceneInventory.Instance.InteractiveObjects[0]);

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("-"))
                    {
                        //targetConditions.InputsMap.Delete(d);
                        actionsScript.DnD_Targets.Remove(Gaze_SceneInventory.Instance.InteractiveObjects[0]);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        private void ShowTouchAbilityOption()
        {
            actionsScript.ActionTouch = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("Touch Ability", actionsScript.ActionTouch);
        }

        private void ShowGrabModeOption()
        {
            actionsScript.ModifyGrabMode = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup("Grab Mode", actionsScript.ModifyGrabMode);
            if (actionsScript.ModifyGrabMode == Gaze_Actions.ALTERABLE_OPTION.MODIFY)
            {
                EditorGUILayout.BeginHorizontal();
                actionsScript.grabModeIndex = EditorGUILayout.Popup("", actionsScript.grabModeIndex, grabModes);
                EditorGUILayout.EndHorizontal();

            }
        }

        private void ShowGrabDistanceOption()
        {
            actionsScript.ModifyGrabDistance = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup("Grab Distance", actionsScript.ModifyGrabDistance);
            if (actionsScript.ModifyGrabDistance == Gaze_Actions.ALTERABLE_OPTION.MODIFY)
            {
                EditorGUILayout.BeginHorizontal();
                actionsScript.grabDistance = EditorGUILayout.FloatField("", actionsScript.grabDistance);
                Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.grabDistance);
                EditorGUILayout.EndHorizontal();

            }
        }

        private void ShowGrabOption()
        {
            actionsScript.ActionGrab = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("Grab Ability", actionsScript.ActionGrab);
        }

        private void ShowVisualsOption()
        {
            actionsScript.ActionVisuals = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("Visuals", actionsScript.ActionVisuals);

            if (actionsScript.ActionVisuals == Gaze_Actions.ACTIVABLE_OPTION.NOTHING)
                return;

            actionsScript.AlterAllVisuals = EditorGUILayout.Toggle("Alter All Visuals", actionsScript.AlterAllVisuals);


            if (!actionsScript.AlterAllVisuals)
            {
                // Get All the renderers on this IO
                allRenderers = actionsScript.visualsScript.GetAllRenderers();
                actionsScript.UpdateSelectedRenderers(allRenderers.Count);
                if (allRenderers.Count < 1)
                {
                    // If no visuals on this object, dont show anything else than warning
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("No visual found.", MessageType.Warning);
                    EditorGUILayout.EndHorizontal();
                }

                else
                {
                    // storing all this object visuals name
                    allVisuals = new string[allRenderers.Count];
                    for (int i = 0; i < allVisuals.Length; i++)
                        allVisuals[i] = allRenderers[i].gameObject.name;

                    if (actionsScript.selectedRenderers.Count < 1)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.HelpBox("Add at least one visual or deactivate this condition if not needed.", MessageType.Warning);
                        EditorGUILayout.EndHorizontal();
                    }

                    // for all selected renderers
                    for (int i = 0; i < actionsScript.selectedRenderers.Count; i++)
                    {
                        // get the corresponding Renderer from the AllRenderers List
                        int selectedEntryIndex = actionsScript.selectedRenderers[i];

                        //display it
                        EditorGUILayout.BeginHorizontal();
                        actionsScript.selectedRenderers[i] = EditorGUILayout.Popup(selectedEntryIndex, allVisuals);

                        // add a remove button
                        if (GUILayout.Button("-"))
                        {
                            actionsScript.RemoveSelectedRenderer(actionsScript.selectedRenderers[i]);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    // display 'add' button
                    if (GUILayout.Button("+"))
                    {
                        actionsScript.AddSelectedRenderer(allRenderers.Count);
                    }

                    EditorGUILayout.Space();
                }
            }
        }

        private void ShowCollidersOption()
        {
            actionsScript.ActionColliders = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("Colliders", actionsScript.ActionColliders);
            if (actionsScript.ActionColliders == Gaze_Actions.ACTIVABLE_OPTION.DEACTIVATE)
            {
                if (actionsScript.GetIO().IsAffectedByGravity() && actionsScript.ActionGravity != Gaze_Actions.ACTIVABLE_OPTION.DEACTIVATE)
                {
                    // This avoids the false positive problem created by the fact that the obejct is already attached by the dnd manager. 
                    Gaze_DragAndDropCondition dndCondition = actionsScript.GetComponent<Gaze_DragAndDropCondition>();
                    if (dndCondition == null || !dndCondition.attached)
                        EditorGUILayout.HelpBox("Object will fall off the map (deactivate gravity to prevent it).", MessageType.Warning);
                }
            }
        }

        /// <summary>
        /// Shows the alter gravity option and warns the user if he desn't have a rigidbody
        /// </summary>
        private void ShowGravityOption()
        {
            actionsScript.ActionGravity = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("Gravity:", actionsScript.ActionGravity);

            // Warn the user if he is trying to change the gravity of an IO that does not have a rigibody on the root
            if (actionsScript.ActionGravity != Gaze_Actions.ACTIVABLE_OPTION.NOTHING)
            {
                Gaze_InteractiveObject IO = actionsScript.GetIO();
                if (IO.GetComponentInParent<Rigidbody>() == null)
                {
                    if (GUILayout.Button("+ Add Rigidbody"))
                    {
                        Gaze_GravityManager.AddRigidBodyToIO(IO);
                    }
                    EditorGUILayout.HelpBox("No rigidbody found on " + IO.name, MessageType.Error);
                }
            }
        }

        private void ShowDelayBlock()
        {
            actionsScript.isDelayed = EditorGUILayout.ToggleLeft("Delayed", actionsScript.isDelayed);

            if (actionsScript.isDelayed)
            {
                if (!actionsScript.isDelayRandom)
                {
                    GUILayout.BeginHorizontal();
                    actionsScript.delayTime = EditorGUILayout.FloatField(actionsScript.delayTime);
                    Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.delayTime);
                    EditorGUILayout.LabelField("[s]");
                    GUILayout.EndHorizontal();
                }

                else
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Min", GUILayout.MaxWidth(50));
                    actionsScript.delayRange[0] = EditorGUILayout.FloatField(actionsScript.delayRange[0]);
                    Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.delayRange[0]);
                    EditorGUILayout.LabelField("[s]");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Max", GUILayout.MaxWidth(50));
                    actionsScript.delayRange[1] = EditorGUILayout.FloatField(actionsScript.delayRange[1]);
                    Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.delayRange[1]);
                    EditorGUILayout.LabelField("[s]");
                    GUILayout.EndHorizontal();

                    if (actionsScript.delayRange[1] < actionsScript.delayRange[0])
                        actionsScript.delayRange[1] = actionsScript.delayRange[0] + 1.0f;
                }

                GUILayout.BeginHorizontal();
                actionsScript.isDelayRandom = EditorGUILayout.ToggleLeft("Random", actionsScript.isDelayRandom);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                actionsScript.multipleActionsInTime = EditorGUILayout.ToggleLeft("Fire All Interactions Recorded During Delay", actionsScript.multipleActionsInTime);
                GUILayout.EndHorizontal();
            }
        }

        private void DisplayAnimBlock(int i)
        {
            if (actionsScript.triggerAnimation && FindAnimatorTriggers())
            {
                if (!selectedAnimatorTriggers.Contains(actionsScript.animatorTriggers[i]))
                {
                    actionsScript.animatorTriggers[i] = selectedAnimatorTriggers[0];
                }

                actionsScript.animatorTriggers[i] = selectedAnimatorTriggers[EditorGUILayout.Popup(selectedAnimatorTriggers.IndexOf(actionsScript.animatorTriggers[i]), selectedAnimatorTriggers.ToArray())];
            }
        }

        private void DisplayAudioBlock(int i)
        {
            if (actionsScript.ActionAudio == Gaze_Actions.ACTIVABLE_OPTION.ACTIVATE && hierarchyAudioSources.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                actionsScript.audioClips[i] = (AudioClip)EditorGUILayout.ObjectField("", actionsScript.audioClips[i], typeof(AudioClip), false);
                actionsScript.loopAudio[i] = EditorGUILayout.ToggleLeft("Loop", actionsScript.loopAudio[i]);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DisplayAudioDuckingBlock()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Volume Min.");
            actionsScript.audioVolumeMin = EditorGUILayout.Slider(actionsScript.audioVolumeMin, 0, 1);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Volume Max.");
            actionsScript.audioVolumeMax = EditorGUILayout.Slider(actionsScript.audioVolumeMax, 0, 1);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Fader Speed");
            actionsScript.fadeSpeed = EditorGUILayout.Slider(actionsScript.fadeSpeed, 0, 1);
            EditorGUILayout.EndHorizontal();
        }

        private void DisplayAudioVolume()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Volume");
            actionsScript.audioVolumeMax = EditorGUILayout.Slider(actionsScript.audioVolumeMax, 0, 1);
            EditorGUILayout.EndHorizontal();
        }

        private void OnEditorApplicationUpdate()
        {
            if (target == null)
                return;

            FindAnimatorsInHierarchy();
            FindAnimatorTriggers();
            FindAudioSourcesInHierarchy();
        }

        private void FindAnimatorsInHierarchy()
        {
            Animator[] ac = actionsScript.GetComponentInParent<Gaze_InteractiveObject>().GetComponentsInChildren<Animator>();

            hierarchyAnimators.Clear();
            hierarchyAnimatorNames.Clear();

            foreach (Animator a in ac)
            {
                if (a.runtimeAnimatorController != null)
                {
                    hierarchyAnimators.Add(a);
                    hierarchyAnimatorNames.Add(a.runtimeAnimatorController.name + " (" + a.gameObject.name + ")");
                }
            }
        }

        private void FindAudioSourcesInHierarchy()
        {
            AudioSource[] asrc = actionsScript.GetComponentInParent<Gaze_InteractiveObject>().GetComponentsInChildren<AudioSource>();

            hierarchyAudioSources.Clear();
            hierarchyAudioSourceNames.Clear();

            foreach (AudioSource a in asrc)
            {
                hierarchyAudioSources.Add(a);
                hierarchyAudioSourceNames.Add(a.gameObject.name);
            }
        }

        private bool FindAnimatorTriggers()
        {
            selectedAnimatorTriggers.Clear();

            if (actionsScript.targetAnimator != null && actionsScript.targetAnimator.isInitialized)
            {
                for (int i = 0; i < actionsScript.targetAnimator.parameters.Length; i++)
                {
                    AnimatorControllerParameter p = actionsScript.targetAnimator.parameters[i];
                    // TODO integrate other types of parameters ?
                    if (p.type == AnimatorControllerParameterType.Trigger)
                    {
                        selectedAnimatorTriggers.Add(p.name);
                    }
                }
            }
            // TODO no triggers found when Animator view changes

            return selectedAnimatorTriggers.Count > 0;
        }
    }
}