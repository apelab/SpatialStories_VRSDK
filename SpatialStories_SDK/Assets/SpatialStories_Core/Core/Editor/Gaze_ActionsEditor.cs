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
        private Gaze_Actions actionsScript;
        private List<Animator> hierarchyAnimators;
        private List<string> hierarchyAnimatorNames;
        private List<AudioSource> hierarchyAudioSources;
        private List<string> hierarchyAudioSourceNames;
        private List<string> selectedAnimatorTriggers;

        private string[] grabModes;
        private string[] allVisuals;
        private List<Renderer> allRenderers;

        private List<string> selectedAnimatorStatesNames;
        private List<AnimationClip> selectedAnimatorStates;

        void OnEnable()
        {
            actionsScript = (Gaze_Actions)target;

            EditorApplication.update += OnEditorApplicationUpdate;

            hierarchyAnimators = new List<Animator>();
            hierarchyAnimatorNames = new List<string>();
            hierarchyAudioSources = new List<AudioSource>();
            hierarchyAudioSourceNames = new List<string>();
            selectedAnimatorStatesNames = new List<string>();
            selectedAnimatorStates = new List<AnimationClip>();
            selectedAnimatorTriggers = new List<string>();

            grabModes = Enum.GetNames(typeof(Gaze_GrabMode));

            FindAnimatorsInHierarchy();
            FindAudioSourcesInHierarchy();
        }

        void OnDisable()
        {
            EditorApplication.update -= OnEditorApplicationUpdate;
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
                    actionsScript.audio_stopOthers = EditorGUILayout.ToggleLeft("Stop Other Audios", actionsScript.audio_stopOthers);

                    actionsScript.audio_ForceStop = EditorGUILayout.ToggleLeft("Immediate play", actionsScript.audio_ForceStop);
                    if (!actionsScript.audio_ForceStop)
                    {
                        actionsScript.audio_AllowMultiple = EditorGUILayout.ToggleLeft("Cumulate audios", actionsScript.audio_AllowMultiple);

                        if (actionsScript.audio_AllowMultiple)
                        {
                            actionsScript.audio_MaxConcurrentSound = EditorGUILayout.IntField("Max concurrent audios", actionsScript.audio_MaxConcurrentSound);
                        }
                    }

                    EditorGUILayout.Space();

                    actionsScript.audio_randomizePitch = EditorGUILayout.ToggleLeft("Randomize pitch", actionsScript.audio_randomizePitch);
                    if (actionsScript.audio_randomizePitch)
                    {
                        actionsScript.audio_minPitch = EditorGUILayout.FloatField("Min pitch", actionsScript.audio_minPitch, GUILayout.Width(300));
                        Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.audio_minPitch);
                        actionsScript.audio_maxPitch = EditorGUILayout.FloatField("Max pitch", actionsScript.audio_maxPitch, GUILayout.Width(300));
                        Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.audio_maxPitch);
                    }

                    EditorGUILayout.Space();

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
                if (actionsScript.ActionAudio == Gaze_Actions.ACTIVABLE_OPTION.DEACTIVATE)
                {
                    #region Fade Out
                    EditorGUILayout.Space();
                    GUILayout.BeginHorizontal();
                    actionsScript.fadeOutDeactEnabled = EditorGUILayout.ToggleLeft("Fade Out", actionsScript.fadeOutDeactEnabled);

                    if (actionsScript.fadeOutDeactEnabled)
                    {
                        actionsScript.fadeOutDeactTime = EditorGUILayout.FloatField("", actionsScript.fadeOutDeactTime, GUILayout.Width(200));
                        Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.fadeOutDeactTime);
                        EditorGUILayout.LabelField("[s]");
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();

                        actionsScript.fadeOutDeactCurve = EditorGUILayout.CurveField(actionsScript.fadeOutDeactCurve, Color.green, new Rect(0, 0, actionsScript.fadeOutDeactTime, 1), GUILayout.Width(400));

                    }
                    GUILayout.EndHorizontal();
                    #endregion

                }
                else if (actionsScript.ActionAudio == Gaze_Actions.ACTIVABLE_OPTION.ACTIVATE)
                {
                    #region Fade In
                    EditorGUILayout.Space();
                    GUILayout.BeginHorizontal();
                    actionsScript.fadeInEnabled = EditorGUILayout.ToggleLeft("Fade In", actionsScript.fadeInEnabled);
                    if (actionsScript.fadeInEnabled)
                    {
                        actionsScript.fadeInTime = EditorGUILayout.FloatField("", actionsScript.fadeInTime, GUILayout.Width(100));
                        Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.fadeInTime);
                        EditorGUILayout.LabelField("[s]");
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();

                        actionsScript.fadeInCurve = EditorGUILayout.CurveField(actionsScript.fadeInCurve, Color.green, new Rect(0, 0, actionsScript.fadeInTime, 1), GUILayout.Width(400));
                    }
                    GUILayout.EndHorizontal();
                    if (actionsScript.DontHaveAudioLoop())
                    {
                        GUILayout.BeginHorizontal();
                        actionsScript.fadeOutEnabled = EditorGUILayout.ToggleLeft("Fade Out", actionsScript.fadeOutEnabled);
                        if (actionsScript.fadeOutEnabled)
                        {
                            actionsScript.fadeOutTime = EditorGUILayout.FloatField("", actionsScript.fadeOutTime, GUILayout.Width(100));
                            Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.fadeOutTime);
                            EditorGUILayout.LabelField("[s]");
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();

                            actionsScript.fadeOutCurve = EditorGUILayout.CurveField(actionsScript.fadeOutCurve, Color.green, new Rect(0, 0, actionsScript.fadeOutTime, 1), GUILayout.Width(400));

                        }
                        GUILayout.EndHorizontal();
                    }
                    #endregion

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
            actionsScript.ActionAnimation = (Gaze_Actions.ANIMATION_OPTION)EditorGUILayout.EnumPopup("Animation", actionsScript.ActionAnimation);
            #endregion Animations

            #region MECANIM
            #region Animator
            if (actionsScript.ActionAnimation != Gaze_Actions.ANIMATION_OPTION.NOTHING)
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
            if (actionsScript.ActionAnimation == Gaze_Actions.ANIMATION_OPTION.MECANIM)
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
            #endregion MECANIM

            #region CLIP           
            #region AnimationTriggers
            if (actionsScript.ActionAnimation == Gaze_Actions.ANIMATION_OPTION.CLIP)
            {
                for (int i = 0; i < Gaze_HashIDs.TriggerEventsAndStates.Length; i++)
                {
                    actionsScript.activeTriggerStatesAnim[i] = EditorGUILayout.ToggleLeft(Gaze_HashIDs.TriggerEventsAndStates[i], actionsScript.activeTriggerStatesAnim[i]);

                    if (actionsScript.activeTriggerStatesAnim[i])
                    {
                        DisplayAnimBlock(i);
                    }
                }
                EditorGUILayout.Space();
            }
            #endregion AnimationTriggers
            #endregion CLIP
        }

        public override void Gaze_OnInspectorGUI()
        {
            base.BeginChangeComparision();

            // display conditions
            if (actionsScript.isActive)
            {
                if (!Application.isPlaying)
                {
                    EditorGUILayout.Space();
                    if (actionsScript && !actionsScript.DestroyOnTrigger)
                    {
                        ShowVisualsOption();
                        ShowGrabOption();
                        ShowGrabDistanceOption();
                        ShowGrabModeOption();
                        ShowTouchAbilityOption();
                        ShowTouchDistanceOption();
                        ShowDragAndDropOptions();

                        ShowCollidersOption();
                        ShowGravityOption();
                        ShowAudioOptions();
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

        private void ShowTouchAbilityOption()
        {
            actionsScript.ActionTouch = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("Touch Ability", actionsScript.ActionTouch);
        }

        private void ShowGrabModeOption()
        {
            actionsScript.ModifyGrabMode = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup("Manipulation Mode", actionsScript.ModifyGrabMode);
            if (actionsScript.ModifyGrabMode == Gaze_Actions.ALTERABLE_OPTION.MODIFY)
            {
                EditorGUILayout.BeginHorizontal();
                actionsScript.grabModeIndex = EditorGUILayout.Popup("", actionsScript.grabModeIndex, grabModes);
                EditorGUILayout.EndHorizontal();

            }
        }

        private void ShowGrabDistanceOption()
        {
            actionsScript.ModifyGrabDistance = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup("Manipulation Distance", actionsScript.ModifyGrabDistance);
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
            actionsScript.ActionGrab = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("Manipulation Ability", actionsScript.ActionGrab);
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

        private void ShowDragAndDropOptions()
        {
            actionsScript.ModifyDragAndDrop = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup("Drag And Drop", actionsScript.ModifyDragAndDrop);
            if (actionsScript.ModifyDragAndDrop == Gaze_Actions.ALTERABLE_OPTION.MODIFY)
            {
                EditorGUILayout.BeginHorizontal();
                actionsScript.ActionDragAndDrop = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup(actionsScript.ActionDragAndDrop);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                actionsScript.ModifyDnDMinDistance = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup("Min Distance", actionsScript.ModifyDnDMinDistance);
                EditorGUILayout.EndHorizontal();
                if (actionsScript.ModifyDnDMinDistance == Gaze_Actions.ALTERABLE_OPTION.MODIFY)
                {
                    EditorGUILayout.BeginHorizontal();
                    actionsScript.dnDMinDistance = EditorGUILayout.FloatField("", actionsScript.dnDMinDistance);
                    Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.dnDMinDistance);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                actionsScript.ModifyDnDSnapBeforeDrop = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("Snap Before Drop", actionsScript.ModifyDnDSnapBeforeDrop);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                actionsScript.ModifyDnDAttached = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("Attached", actionsScript.ModifyDnDAttached);
                EditorGUILayout.EndHorizontal();

                DisplayDnDAxisConstraints();


                EditorGUILayout.BeginHorizontal();
                actionsScript.ModifyDnDAngleThreshold = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup("Angle Threshold", actionsScript.ModifyDnDAngleThreshold);
                EditorGUILayout.EndHorizontal();
                if (actionsScript.ModifyDnDAngleThreshold == Gaze_Actions.ALTERABLE_OPTION.MODIFY)
                {
                    EditorGUILayout.BeginHorizontal();
                    actionsScript.dnDAngleThreshold = EditorGUILayout.FloatField("", actionsScript.dnDAngleThreshold);
                    Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.dnDAngleThreshold);
                    EditorGUILayout.EndHorizontal();
                }


                DisplayDnDTargets();

                EditorGUILayout.Space();
            }
        }

        private void DisplayDnDTargets()
        {
            /*
            EditorGUILayout.BeginHorizontal();
            actionsScript.ModifyDragAndDropTargets = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup("Targets", actionsScript.ModifyDragAndDropTargets);
            EditorGUILayout.EndHorizontal();
            if (actionsScript.ModifyDragAndDropTargets == Gaze_Actions.ALTERABLE_OPTION.MODIFY)
            {
                for (int i = 0; i < actionsScript.DnD_Targets.Count; i++)
                    EditorGUILayout.BeginHorizontal();
                actionsScript.DnD_Targets = EditorGUILayout.FloatField("", actionsScript.dnDMinDistance);
                EditorGUILayout.EndHorizontal();
            }
            */
        }

        private void DisplayDnDAxisConstraints()
        {
            EditorGUILayout.BeginHorizontal();
            actionsScript.ModifyDnDRespectAxis = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup("Respect Axis", actionsScript.ModifyDnDRespectAxis);
            EditorGUILayout.EndHorizontal();
            if (actionsScript.ModifyDnDRespectAxis == Gaze_Actions.ALTERABLE_OPTION.MODIFY)
            {
                EditorGUILayout.BeginHorizontal();
                actionsScript.dnDRespectXAxis = EditorGUILayout.ToggleLeft("Respect X Axis", actionsScript.dnDRespectXAxis);
                if (actionsScript.dnDRespectXAxis)
                    actionsScript.dnDRespectXAxisMirror = EditorGUILayout.ToggleLeft("Mirrored", actionsScript.dnDRespectXAxisMirror);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                actionsScript.dnDRespectYAxis = EditorGUILayout.ToggleLeft("Respect Y Axis", actionsScript.dnDRespectYAxis);
                if (actionsScript.dnDRespectYAxis)
                    actionsScript.dnDRespectYAxisMirror = EditorGUILayout.ToggleLeft("Mirrored", actionsScript.dnDRespectYAxisMirror);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                actionsScript.dnDRespectZAxis = EditorGUILayout.ToggleLeft("Respect Z Axis", actionsScript.dnDRespectZAxis);
                if (actionsScript.dnDRespectZAxis)
                    actionsScript.dnDRespectZAxisMirror = EditorGUILayout.ToggleLeft("Mirrored", actionsScript.dnDRespectZAxisMirror);
                EditorGUILayout.EndHorizontal();
            }
        }


        private void DisplayAnimBlock(int k)
        {
            if (actionsScript.ActionAnimation == Gaze_Actions.ANIMATION_OPTION.MECANIM && FindAnimatorTriggers())
            {
                if (!selectedAnimatorTriggers.Contains(actionsScript.animatorTriggers[k]))
                {
                    actionsScript.animatorTriggers[k] = selectedAnimatorTriggers[0];
                }

                actionsScript.animatorTriggers[k] = selectedAnimatorTriggers[EditorGUILayout.Popup(selectedAnimatorTriggers.IndexOf(actionsScript.animatorTriggers[k]), selectedAnimatorTriggers.ToArray())];
            }
            else if (actionsScript.ActionAnimation == Gaze_Actions.ANIMATION_OPTION.CLIP)
            {
                // help message if no aniamtion are found
                if (!FindAnimatorStates())
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("There are no animation in your animator", MessageType.Warning);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    if (actionsScript.animationClip.Count(k) < 1)
                    {
                        AnimationClip d = actionsScript.animationClip.Add(k);
                        d = selectedAnimatorStates[0];
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        int index = EditorGUILayout.Popup(selectedAnimatorStates.IndexOf(actionsScript.animationClip.Get(k, 0)), selectedAnimatorStatesNames.ToArray());
                        index = Mathf.Max(index, 0);
                        actionsScript.animationClip.Set(k, 0, selectedAnimatorStates[index]);
                        EditorGUILayout.EndHorizontal();

                        // update inputs list in Gaze_Interactions (Gaze_Interactions may have been removed in the hierarchy)
                        for (int i = 1; i < actionsScript.animationClip.Count(k); i++)
                        {
                            if (!selectedAnimatorStates.Contains(actionsScript.animationClip.Get(k, i)))
                            {
                                actionsScript.animationClip.Set(k, i, selectedAnimatorStates[0]);
                            }

                            // display the entry
                            EditorGUILayout.BeginHorizontal();
                            AnimationClip clip = selectedAnimatorStates[EditorGUILayout.Popup(selectedAnimatorStates.IndexOf(actionsScript.animationClip.Get(k, i)), selectedAnimatorStatesNames.ToArray())];
                            actionsScript.animationClip.Set(k, i, clip);

                            // and a '-' button to remove it if needed
                            if (GUILayout.Button("-", GUILayout.Width(100)))
                            {
                                actionsScript.animationClip.Remove(k, i);
                            }

                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    // display 'add' button
                    if (GUILayout.Button("+", GUILayout.Width(400)))
                    {
                        AnimationClip d = actionsScript.animationClip.Add(k);

                        EditorGUILayout.BeginHorizontal();

                        if (GUILayout.Button("-"))
                        {
                            actionsScript.animationClip.Remove(k, d);

                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.BeginHorizontal();
                    actionsScript.loopAnim[k] = (Gaze_Actions.LOOP_MODES)EditorGUILayout.EnumPopup("Loop", actionsScript.loopAnim[k]);

                    if (actionsScript.loopAnim[k] == Gaze_Actions.LOOP_MODES.Single)
                    {
                        actionsScript.loopAnimType[k] = (Gaze_Actions.ANIMATION_LOOP)EditorGUILayout.EnumPopup("", actionsScript.loopAnimType[k]);
                    }
                    EditorGUILayout.EndHorizontal();

                    if (actionsScript.animationClip.Count(k) > 1 && actionsScript.loopAnim[k] != Gaze_Actions.LOOP_MODES.PlaylistOnce)
                    {
                        actionsScript.animationSequence[k] = (Gaze_Actions.AUDIO_SEQUENCE)EditorGUILayout.EnumPopup("Sequence", actionsScript.animationSequence[k]);
                    }

                    if (actionsScript.loopAnim[k] == Gaze_Actions.LOOP_MODES.PlaylistOnce)
                    {
                        EditorGUILayout.BeginHorizontal();
                        actionsScript.loopOnLast[k] = EditorGUILayout.ToggleLeft("Loop on last", actionsScript.loopOnLast[k]);
                        if (actionsScript.loopOnLast[k])
                        {
                            actionsScript.loopAnimType[k] = (Gaze_Actions.ANIMATION_LOOP)EditorGUILayout.EnumPopup("", actionsScript.loopAnimType[k]);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }

        private void DisplayAudioBlock(int k)
        {
            // help message if no input is specified
            if (actionsScript.audioClips.Count(k) < 1)
            {
                AudioClip d = actionsScript.audioClips.Add(k);
            }
            else
            {

                EditorGUILayout.BeginHorizontal();
                actionsScript.audioClips.Set(k, 0, (AudioClip)EditorGUILayout.ObjectField("", actionsScript.audioClips.Get(k, 0), typeof(AudioClip), false, GUILayout.Width(300)));
                EditorGUILayout.EndHorizontal();

                // update inputs list in Gaze_Interactions (Gaze_Interactions may have been removed in the hierarchy)
                for (int i = 1; i < actionsScript.audioClips.Count(k); i++)
                {
                    // display the entry
                    EditorGUILayout.BeginHorizontal();
                    actionsScript.audioClips.Set(k, i, (AudioClip)EditorGUILayout.ObjectField("", actionsScript.audioClips.Get(k, i), typeof(AudioClip), false, GUILayout.Width(300)));

                    // and a '-' button to remove it if needed
                    if (GUILayout.Button("-", GUILayout.Width(100)))
                        actionsScript.audioClips.Remove(k, i);

                    EditorGUILayout.EndHorizontal();
                }
            }

            // display 'add' button
            if (GUILayout.Button("+", GUILayout.Width(400)))
            {
                AudioClip d = actionsScript.audioClips.Add(k);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("-"))
                {
                    actionsScript.audioClips.Remove(k, d);

                    // TODO @apelab remove event subscription


                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            actionsScript.loopAudio[k] = (Gaze_Actions.LOOP_MODES)EditorGUILayout.EnumPopup("Loop", actionsScript.loopAudio[k]);

            if (actionsScript.audioClips.Count(k) > 1 && actionsScript.loopAudio[k] == Gaze_Actions.LOOP_MODES.Playlist)
            {
                actionsScript.fadeInBetween[k] = EditorGUILayout.ToggleLeft("Fade in between", actionsScript.fadeInBetween[k]);

            }
            EditorGUILayout.EndHorizontal();

            if (actionsScript.audioClips.Count(k) > 1 && actionsScript.loopAudio[k] != Gaze_Actions.LOOP_MODES.PlaylistOnce)
            {
                actionsScript.audio_sequence[k] = (Gaze_Actions.AUDIO_SEQUENCE)EditorGUILayout.EnumPopup("Sequence", actionsScript.audio_sequence[k]);
            }

            if (actionsScript.loopAudio[k] == Gaze_Actions.LOOP_MODES.PlaylistOnce)
            {
                actionsScript.audioLoopOnLast[k] = EditorGUILayout.ToggleLeft("Loop on last", actionsScript.audioLoopOnLast[k]);
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

        private bool FindAnimatorStates()
        {
            selectedAnimatorStates.Clear();

            if (actionsScript.targetAnimator != null && actionsScript.targetAnimator.isInitialized)
            {
                for (int i = 0; i < actionsScript.targetAnimator.runtimeAnimatorController.animationClips.Length; i++)
                {
                    AnimationClip p = actionsScript.targetAnimator.runtimeAnimatorController.animationClips[i];

                    selectedAnimatorStates.Add(p);
                    selectedAnimatorStatesNames.Add(p.name);

                }
            }

            return selectedAnimatorStates.Count > 0;
        }
    }
}