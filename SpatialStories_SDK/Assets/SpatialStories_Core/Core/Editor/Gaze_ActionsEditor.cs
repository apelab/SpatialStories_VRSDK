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
        private List<string> selectedAnimatorStatesNames;
        private List<AnimationClip> selectedAnimatorStates;


        private string[] grabModes;
        private string[] allVisuals;
        private List<Renderer> allRenderers;

        void OnEnable()
        {
            actionsScript = (Gaze_Actions)target;

            EditorApplication.update += OnEditorApplicationUpdate;

            hierarchyAnimators = new List<Animator>();
            hierarchyAnimatorNames = new List<string>();
            hierarchyAudioSources = new List<AudioSource>();
            hierarchyAudioSourceNames = new List<string>();
            selectedAnimatorTriggers = new List<string>();
            selectedAnimatorStatesNames = new List<string>();
            selectedAnimatorStates = new List<AnimationClip>();

            grabModes = Enum.GetNames(typeof(Gaze_GrabMode));

            FindAnimatorsInHierarchy();
            FindAudioSourcesInHierarchy();
        }

        void OnDisable()
        {
            EditorApplication.update -= OnEditorApplicationUpdate;
        }

        bool ShowOtherAudioOptions = false;
        public void ShowAudioOptions()
        {
            #region AudioSource

            actionsScript.ActionAudio = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup(new GUIContent("Audio", "Launch a sound using this action."), actionsScript.ActionAudio);

            if (actionsScript.ActionAudio != Gaze_Actions.ACTIVABLE_OPTION.NOTHING)
            {
                EditorGUILayout.BeginVertical();
                if (hierarchyAudioSources.Count > 0)
                {
                    Gaze_EditorUtils.DrawSectionTitle("Audio Setup");
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Audio Source", "Select the audiosource you want to use to play your audio if you have many on your IO"));

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
                    actionsScript.activeTriggerStatesAudio[0] = EditorGUILayout.ToggleLeft(Gaze_HashIDs.TriggerEventsAndStates[0], actionsScript.activeTriggerStatesAudio[0]);

                    if (actionsScript.activeTriggerStatesAudio[0])
                    {
                        DisplayAudioBlock(0);
                    }

                    EditorGUI.indentLevel++;
                    ShowOtherAudioOptions = EditorGUILayout.Foldout(ShowOtherAudioOptions, "More Options:");

                    if (ShowOtherAudioOptions)
                    {
                        for (int i = 1; i < Gaze_HashIDs.TriggerEventsAndStates.Length; i++)
                        {
                            actionsScript.activeTriggerStatesAudio[i] = EditorGUILayout.ToggleLeft(new GUIContent(Gaze_HashIDs.TriggerEventsAndStates[i], AudioTriggerEvensAndStatesHints[i]), actionsScript.activeTriggerStatesAudio[i]);
                            if (actionsScript.activeTriggerStatesAudio[i])
                            {
                                DisplayAudioBlock(i);
                            }
                        }
                    }

                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();

                    Gaze_EditorUtils.DrawSectionTitle("Audio Parameters");

                    actionsScript.audio_stopOthers = EditorGUILayout.ToggleLeft(new GUIContent("Stop Other Audios", "Stops any others audios launched by this IO only"), actionsScript.audio_stopOthers);

                    // Randomize Pitch
                    actionsScript.audio_randomizePitch = EditorGUILayout.ToggleLeft(new GUIContent("Randomize pitch", "Change the pitch of this interaction only"), actionsScript.audio_randomizePitch);
                    if (actionsScript.audio_randomizePitch)
                    {
                        actionsScript.audio_minPitch = EditorGUILayout.FloatField("Min pitch", actionsScript.audio_minPitch, GUILayout.Width(300));
                        Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.audio_minPitch);
                        actionsScript.audio_maxPitch = EditorGUILayout.FloatField("Max pitch", actionsScript.audio_maxPitch, GUILayout.Width(300));
                        Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.audio_maxPitch);
                    }
                    // End of Randomize Pitch

                    #endregion onAfter 


                    #region Fade In
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

                        #endregion

                        #region Ducking
                        if (actionsScript.ActionAudio == Gaze_Actions.ACTIVABLE_OPTION.ACTIVATE)
                        {
                            actionsScript.duckingEnabled = EditorGUILayout.ToggleLeft(new GUIContent("Ducking", "The volume rises when this IO is Gazed"), actionsScript.duckingEnabled);
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

                    EditorGUILayout.Space();
                    Gaze_EditorUtils.DrawSectionTitle("Audio Reload Parameters");
                    Gaze_EditorUtils.DrawEditorHint("By default audios will wait until the last one is finished when reloaded");
                    EditorGUILayout.Space();

                    actionsScript.audio_ForceStop = EditorGUILayout.ToggleLeft(new GUIContent("Immediate play", "Stops others audios launched with this interaction when reloaded."), actionsScript.audio_ForceStop);
                    if (!actionsScript.audio_ForceStop)
                    {
                        actionsScript.audio_AllowMultiple = EditorGUILayout.ToggleLeft(new GUIContent("Cumulate audios", "Cumulates audios launched with this interaction when reloaded."), actionsScript.audio_AllowMultiple);

                        if (actionsScript.audio_AllowMultiple)
                        {
                            actionsScript.audio_MaxConcurrentSound = EditorGUILayout.IntField("Max concurrent audios", actionsScript.audio_MaxConcurrentSound);
                        }
                    }

                }

                if (actionsScript.ActionAudio == Gaze_Actions.ACTIVABLE_OPTION.DEACTIVATE)
                {
                    #region Fade Out
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
            }
        }

        bool isOtherAnimationTriggerOptionsOpen = false;
        public void ShowAnimationOptions()
        {
            #region Animations
            actionsScript.ActionAnimation = (Gaze_Actions.ANIMATION_OPTION)EditorGUILayout.EnumPopup(new GUIContent("Animation", "Launch animations directly from your Visual or Unity animations."), actionsScript.ActionAnimation);
            #endregion Animations

            #region MECANIM
            #region Animator
            if (actionsScript.ActionAnimation != Gaze_Actions.ANIMATION_OPTION.NOTHING)
            {
                EditorGUILayout.BeginHorizontal();
                if (hierarchyAnimators.Count > 0)
                {

                    EditorGUILayout.LabelField(new GUIContent("Animator", "Lists all the animators available in your IO"));

                    if (!hierarchyAnimators.Contains(actionsScript.targetAnimator))
                    {
                        actionsScript.targetAnimator = hierarchyAnimators[0];
                    }

                    actionsScript.targetAnimator = hierarchyAnimators[EditorGUILayout.Popup(hierarchyAnimators.IndexOf(actionsScript.targetAnimator), hierarchyAnimatorNames.ToArray())];
                }
                else
                {
                    EditorGUILayout.HelpBox("No animator found in the structure of your IO.", MessageType.Warning);
                }
                EditorGUILayout.EndHorizontal();
            }
            #endregion Animator

            #region AnimationTriggers
            if (actionsScript.ActionAnimation == Gaze_Actions.ANIMATION_OPTION.MECANIM)
            {
                EditorGUILayout.BeginHorizontal();
                actionsScript.activeTriggerStatesAnim[0] = EditorGUILayout.ToggleLeft(Gaze_HashIDs.TriggerEventsAndStates[0], actionsScript.activeTriggerStatesAnim[0]);

                if (actionsScript.activeTriggerStatesAnim[0])
                {
                    DisplayAnimBlock(0);
                }
                EditorGUILayout.EndHorizontal();

                isOtherAnimationTriggerOptionsOpen = EditorGUILayout.Foldout(isOtherAnimationTriggerOptionsOpen, "Other Trigger Options");
                if (isOtherAnimationTriggerOptionsOpen)
                {
                    EditorGUI.indentLevel++;
                    for (int i = 1; i < Gaze_HashIDs.TriggerEventsAndStates.Length; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        actionsScript.activeTriggerStatesAnim[i] = EditorGUILayout.ToggleLeft(Gaze_HashIDs.TriggerEventsAndStates[i], actionsScript.activeTriggerStatesAnim[i]);

                        if (actionsScript.activeTriggerStatesAnim[i])
                        {
                            DisplayAnimBlock(i);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                }


                EditorGUILayout.Space();
            }
            #endregion AnimationTriggers
            #endregion MECANIM

            #region CLIP           
            #region AnimationTriggers
            if (actionsScript.ActionAnimation == Gaze_Actions.ANIMATION_OPTION.CLIP)
            {

                actionsScript.activeTriggerStatesAnim[0] = EditorGUILayout.ToggleLeft(Gaze_HashIDs.TriggerEventsAndStates[0], actionsScript.activeTriggerStatesAnim[0]);

                if (actionsScript.activeTriggerStatesAnim[0])
                {
                    DisplayAnimBlock(0);
                }

                isOtherAnimationTriggerOptionsOpen = EditorGUILayout.Foldout(isOtherAnimationTriggerOptionsOpen, "Other Trigger Options");
                if (isOtherAnimationTriggerOptionsOpen)
                {
                    EditorGUI.indentLevel++;
                    for (int i = 1; i < Gaze_HashIDs.TriggerEventsAndStates.Length; i++)
                    {
                        actionsScript.activeTriggerStatesAnim[i] = EditorGUILayout.ToggleLeft(Gaze_HashIDs.TriggerEventsAndStates[i], actionsScript.activeTriggerStatesAnim[i]);

                        if (actionsScript.activeTriggerStatesAnim[i])
                        {
                            DisplayAnimBlock(i);
                        }
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
                    Gaze_EditorUtils.DrawSectionTitle("DELAY");
                    ShowDelayBlock();

                    EditorGUILayout.Space();

                    if (actionsScript && !actionsScript.DestroyOnTrigger)
                    {
                        EditorGUILayout.Space();
                        Gaze_EditorUtils.DrawSectionTitle("ACTIONS");
                        Gaze_EditorUtils.DrawEditorHint("Actions will be fired once all conditions are met.");

                        Gaze_EditorUtils.DrawSectionTitle("Manipulation");
                        ShowGrabOption();
                        ShowGrabDistanceOption();
                        ShowGrabModeOption();
                        ShowTouchAbilityOption();
                        ShowTouchDistanceOption();
                        ShowDragAndDropOptions();

                        EditorGUILayout.Space();
                        Gaze_EditorUtils.DrawSectionTitle("Object");
                        ShowVisualsOption();
                        ShowCollidersOption();
                        ShowGravityOption();
                    }
                    actionsScript.DestroyOnTrigger = EditorGUILayout.ToggleLeft("Destroy", actionsScript.DestroyOnTrigger);

                    if (actionsScript && !actionsScript.DestroyOnTrigger)
                    {
                        EditorGUILayout.Space();

                        EditorGUILayout.Space();
                        Gaze_EditorUtils.DrawSectionTitle("Animation & Audio");
                        ShowAudioOptions();
                        ShowAnimationOptions();
                    }

                    EditorGUILayout.Space();

                }
            }

            // save changes
            base.EndChangeComparision();
            EditorUtility.SetDirty(actionsScript);
        }

        private void ShowTouchDistanceOption()
        {
            actionsScript.ModifyTouchDistance = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup(new GUIContent("Touch Distance", "Modify the touch distance of this IO at runtime."), actionsScript.ModifyTouchDistance);
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
            actionsScript.ActionTouch = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup(new GUIContent("Touch Ability", "Activate or deactivate the touch ability of this IO at runtime."), actionsScript.ActionTouch);
        }

        private void ShowGrabModeOption()
        {
            actionsScript.ModifyGrabMode = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup(new GUIContent("Manipulation Mode", "Change the way your IO is being manipulated at runtime."), actionsScript.ModifyGrabMode);
            if (actionsScript.ModifyGrabMode == Gaze_Actions.ALTERABLE_OPTION.MODIFY)
            {
                EditorGUILayout.BeginHorizontal();
                actionsScript.grabModeIndex = EditorGUILayout.Popup("", actionsScript.grabModeIndex, grabModes);
                EditorGUILayout.EndHorizontal();

            }
        }

        private void ShowGrabDistanceOption()
        {
            actionsScript.ModifyGrabDistance = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup(new GUIContent("Manipulation Distance", "Modify Grab & Levitation distances of this IO at runtime."), actionsScript.ModifyGrabDistance);
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
            actionsScript.ActionVisuals = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup(new GUIContent("Visuals", "Activate or deactivate one are all visuals of this IO at runtime."), actionsScript.ActionVisuals);

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
            actionsScript.ActionColliders = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup(new GUIContent("Colliders", "Deactivate or Activate all colliders of an IO."), actionsScript.ActionColliders);
        }


        /// <summary>
        /// Shows the alter gravity option and warns the user if he desn't have a rigidbody
        /// </summary>
        private void ShowGravityOption()
        {
            actionsScript.ActionGravity = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup(new GUIContent("Gravity:", "Deactivate or Activate gravity of this IO. "), actionsScript.ActionGravity);

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

            EditorGUI.indentLevel++;
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
            EditorGUI.indentLevel--;
        }

        private void ShowDragAndDropOptions()
        {
            actionsScript.ModifyDragAndDrop = (Gaze_Actions.ALTERABLE_OPTION)EditorGUILayout.EnumPopup(new GUIContent("Drag And Drop", "Activate, Deactivate and Modify all parameters of the Drag & Drop of this IO at runtime. "), actionsScript.ModifyDragAndDrop);
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
            EditorGUI.indentLevel++;
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
            EditorGUI.indentLevel--;
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
            FindAnimatorStates();
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

            if (actionsScript.targetAnimator != null && actionsScript.targetAnimator.isInitialized && actionsScript.targetAnimator.isActiveAndEnabled)
            {
                for (int i = 0; i < actionsScript.targetAnimator.parameters.Length; i++)
                {
                    AnimatorControllerParameter p = actionsScript.targetAnimator.parameters[i];
                    if (p.type == AnimatorControllerParameterType.Trigger)
                    {
                        selectedAnimatorTriggers.Add(p.name);
                    }
                }
            }

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

        public static string[] AudioTriggerEvensAndStatesHints =
        {
            "",
            "Choose the clips you want to launch when the interaction is reloaded",
            "",
            "Choose the clips you want to launch when the interaction in it's Active ",
            "Choose the clips you want to launch when the interaction in it's After State"
        };
    }
}