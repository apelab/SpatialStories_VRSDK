using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gaze
{
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

        void OnEnable()
        {
            actionsScript = (Gaze_Actions)target;

            rootIO = actionsScript.GetComponentInParent<Gaze_InteractiveObject>();

            //actionsScript.isActive = true;

            EditorApplication.update += onEditorApplicationUpdate;

            hierarchyAnimators = new List<Animator>();
            hierarchyAnimatorNames = new List<string>();
            hierarchyAudioSources = new List<AudioSource>();
            hierarchyAudioSourceNames = new List<string>();
            selectedAnimatorTriggers = new List<string>();

            grabModes = Enum.GetNames(typeof(Gaze_GrabMode));

            findAnimatorsInHierarchy();
            FindAudioSourcesInHierarchy();
        }

        void OnDisable()
        {
            EditorApplication.update -= onEditorApplicationUpdate;

            //actionsScript.isActive = false;
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
                            displayAudioBlock(i);
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
                            displayAudioDuckingBlock();
                        }
                        else
                        {
                            displayAudioVolume();
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
                        displayAnimBlock(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();
            }
            #endregion AnimationTriggers
        }

        public override void Gaze_OnInspectorGUI()
        {

            // display conditions
            if (actionsScript.isActive)
            {
                EditorGUILayout.Space();

                if (actionsScript && !actionsScript.DestroyOnTrigger)
                {
                    ShowVisualsOption();
                    ShowCollidersOption();
                    ShowGrabOption();
                    ShowGrabDistanceOption();
                    ShowGrabModeOption();
                    ShowTouchAbilityOption();
                    ShowTouchDistanceOption();
                    actionsScript.ActionGravity = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("Gravity", actionsScript.ActionGravity);
                    ShowAudioOptions();
                    ShowAnimationOptions();
                }


                actionsScript.DestroyOnTrigger = EditorGUILayout.ToggleLeft("Destroy", actionsScript.DestroyOnTrigger);

            }
            // save changes
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
                EditorGUILayout.PropertyField(serializedObject.FindProperty("VisualsToAlter"), true);
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


        private void displayAnimBlock(int i)
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

        private void displayAudioBlock(int i)
        {
            if (actionsScript.ActionAudio == Gaze_Actions.ACTIVABLE_OPTION.ACTIVATE && hierarchyAudioSources.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                actionsScript.audioClips[i] = (AudioClip)EditorGUILayout.ObjectField("", actionsScript.audioClips[i], typeof(AudioClip), false);
                actionsScript.loopAudio[i] = EditorGUILayout.ToggleLeft("Loop", actionsScript.loopAudio[i]);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void displayAudioDuckingBlock()
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

        private void displayAudioVolume()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Volume");
            actionsScript.audioVolumeMax = EditorGUILayout.Slider(actionsScript.audioVolumeMax, 0, 1);
            EditorGUILayout.EndHorizontal();
        }

        private void onEditorApplicationUpdate()
        {
            if (target == null)
                return;

            findAnimatorsInHierarchy();
            FindAnimatorTriggers();
            FindAudioSourcesInHierarchy();
        }

        private void findAnimatorsInHierarchy()
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
