using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gaze
{
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

        private List<AudioClip> hierarchyClips;
        private List<Animation> hierarchyAnimationSources;
        private List<string> hierarchyAnimationSourcesNames;

        void OnEnable()
        {
            actionsScript = (Gaze_Actions)target;

            EditorApplication.update += onEditorApplicationUpdate;

            hierarchyAnimators = new List<Animator>();
            hierarchyAnimatorNames = new List<string>();
            hierarchyAudioSources = new List<AudioSource>();
            hierarchyAudioSourceNames = new List<string>();
            selectedAnimatorTriggers = new List<string>();
            hierarchyClips = new List<AudioClip>();
            hierarchyAnimationSources = new List<Animation>();
            hierarchyAnimationSourcesNames = new List<string>();

            findAnimatorsInHierarchy();
            findAudioSourcesInHierarchy();
            FindAnimationSourcesInHierarchy();

        }

        void OnDisable()
        {
            EditorApplication.update -= onEditorApplicationUpdate;
        }

        public void ShowAudioOptions()
        {

            actionsScript.ActionAudio = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("Audio:", actionsScript.ActionAudio);
            #region AudioSource

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

                //Choose between old Audio block vs new one
                actionsScript.OldAudio = EditorGUILayout.ToggleLeft("Old SDK Audio", actionsScript.OldAudio);

                #endregion AudioSource

                #region AudioClipsOLD
                if (actionsScript.OldAudio) //Audio put with old SDK
                {
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
                }
                #endregion AudioClipsOLD

                #region AudioClipsNew
                else
                {
                    if (actionsScript.ActionAudio == Gaze_Actions.ACTIVABLE_OPTION.ACTIVATE)
                    {
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
                            // Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.audio_minPitch);
                            actionsScript.audio_maxPitch = EditorGUILayout.FloatField("Max pitch", actionsScript.audio_maxPitch, GUILayout.Width(300));
                            // Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.audio_maxPitch);
                        }

                        EditorGUILayout.Space();

                        for (int i = 0; i < Gaze_HashIDs.TriggerEventsAndStates.Length; i++)
                        {
                            actionsScript.activeTriggerStatesAudio[i] = EditorGUILayout.ToggleLeft(Gaze_HashIDs.TriggerEventsAndStates[i], actionsScript.activeTriggerStatesAudio[i]);

                            if (actionsScript.activeTriggerStatesAudio[i])
                            {
                                displayAudioBlock(i);
                            }
                        }
                    }
                    if (actionsScript.ActionAudio == Gaze_Actions.ACTIVABLE_OPTION.DEACTIVATE)
                    {
                        #region FadeOut
                        EditorGUILayout.Space();
                        GUILayout.BeginHorizontal();
                        actionsScript.fadeOutDeactEnabled = EditorGUILayout.ToggleLeft("Fade Out", actionsScript.fadeOutDeactEnabled);

                        if (actionsScript.fadeOutDeactEnabled)
                        {
                            actionsScript.fadeOutDeactTime = EditorGUILayout.FloatField("", actionsScript.fadeOutDeactTime, GUILayout.Width(200));
                            //Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.fadeOutDeactTime);
                            EditorGUILayout.LabelField("[s]");
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();

                            actionsScript.fadeOutDeactCurve = EditorGUILayout.CurveField(actionsScript.fadeOutDeactCurve, Color.green, new Rect(0, 0, actionsScript.fadeOutDeactTime, 1), GUILayout.Width(400));

                        }
                        GUILayout.EndHorizontal();
                        #endregion FadeOut

                    }
                    else if (actionsScript.ActionAudio == Gaze_Actions.ACTIVABLE_OPTION.ACTIVATE)
                    {
                        #region FadeIn
                        EditorGUILayout.Space();
                        GUILayout.BeginHorizontal();
                        actionsScript.fadeInEnabled = EditorGUILayout.ToggleLeft("Fade In", actionsScript.fadeInEnabled);
                        if (actionsScript.fadeInEnabled)
                        {
                            actionsScript.fadeInTime = EditorGUILayout.FloatField("", actionsScript.fadeInTime, GUILayout.Width(100));
                            //Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.fadeInTime);
                            EditorGUILayout.LabelField("[s]");
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();

                            actionsScript.fadeInCurve = EditorGUILayout.CurveField(actionsScript.fadeInCurve, Color.green, new Rect(0, 0, actionsScript.fadeInTime, 1), GUILayout.Width(400));
                        }
                        #endregion FadeIn
                        #region FadeOut2
                        GUILayout.EndHorizontal();
                        if (actionsScript.DontHaveAudioLoop())
                        {
                            GUILayout.BeginHorizontal();
                            actionsScript.fadeOutEnabled = EditorGUILayout.ToggleLeft("Fade Out", actionsScript.fadeOutEnabled);
                            if (actionsScript.fadeOutEnabled)
                            {
                                actionsScript.fadeOutTime = EditorGUILayout.FloatField("", actionsScript.fadeOutTime, GUILayout.Width(100));
                                //Gaze_Utils.EnsureFieldIsPositiveOrZero(ref actionsScript.fadeOutTime);
                                EditorGUILayout.LabelField("[s]");
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();

                                actionsScript.fadeOutCurve = EditorGUILayout.CurveField(actionsScript.fadeOutCurve, Color.green, new Rect(0, 0, actionsScript.fadeOutTime, 1), GUILayout.Width(400));

                            }
                            GUILayout.EndHorizontal();
                        }
                        #endregion FadeOut2
                    }
                    #endregion AudioClipsNew 
                }

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
                    #endregion Ducking
                }
            }

        }

        public void ShowAnimationOptions()
        {
            #region Animations
            actionsScript.triggerAnimation = EditorGUILayout.ToggleLeft("Old SDK Animation", actionsScript.triggerAnimation);
            #endregion Animations

            if (actionsScript.triggerAnimation)
            {
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
            else
            {
                #region Animations
                actionsScript.ActionAnimation = (Gaze_Actions.ANIMATION_OPTION)EditorGUILayout.EnumPopup("Animation", actionsScript.ActionAnimation);
                #endregion Animations

                #region MECANIM
                if (actionsScript.ActionAnimation == Gaze_Actions.ANIMATION_OPTION.MECANIM)
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
                #endregion MECANIM

                #region CLIP           
                else if (actionsScript.ActionAnimation == Gaze_Actions.ANIMATION_OPTION.CLIP)
                {
                    EditorGUILayout.BeginVertical();
                    if (hierarchyAnimationSources.Count > 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Animation Source");

                        if (!hierarchyAnimationSources.Contains(actionsScript.targetAnimationSource))
                        {
                            actionsScript.targetAnimationSource = hierarchyAnimationSources[0];
                        }

                        actionsScript.targetAnimationSource = hierarchyAnimationSources[EditorGUILayout.Popup(hierarchyAnimationSources.IndexOf(actionsScript.targetAnimationSource), hierarchyAnimationSourcesNames.ToArray())];

                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No animation found.", MessageType.Warning);
                    }
                    EditorGUILayout.EndVertical();

                    for (int i = 0; i < Gaze_HashIDs.TriggerEventsAndStates.Length; i++)
                    {
                        actionsScript.activeTriggerStatesAnim[i] = EditorGUILayout.ToggleLeft(Gaze_HashIDs.TriggerEventsAndStates[i], actionsScript.activeTriggerStatesAnim[i]);

                        if (actionsScript.activeTriggerStatesAnim[i])
                        {
                            displayAnimBlock(i);
                        }
                    }
                    EditorGUILayout.Space();
                }
                #endregion CLIP

                #region DEACTIVATE
                else if (actionsScript.ActionAnimation == Gaze_Actions.ANIMATION_OPTION.DEACTIVATE)
                {
                    EditorGUILayout.BeginVertical();
                    if (hierarchyAnimationSources.Count > 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Animation Source");

                        if (!hierarchyAnimationSources.Contains(actionsScript.targetAnimationSource))
                        {
                            actionsScript.targetAnimationSource = hierarchyAnimationSources[0];
                        }

                        actionsScript.targetAnimationSource = hierarchyAnimationSources[EditorGUILayout.Popup(hierarchyAnimationSources.IndexOf(actionsScript.targetAnimationSource), hierarchyAnimationSourcesNames.ToArray())];

                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No animation found.", MessageType.Warning);
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
                #endregion DEACTIVATE

            }
        }

        public override void Gaze_OnInspectorGUI()
        {
            if (!actionsScript.DestroyOnTrigger)
            {
                actionsScript.ActionReset = EditorGUILayout.Toggle("Reset Transform", actionsScript.ActionReset);

                actionsScript.ActionVisuals = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("Visuals:", actionsScript.ActionVisuals);

                actionsScript.ActionGrab = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("Grab:", actionsScript.ActionGrab);

                actionsScript.ActionTouch = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("Touch:", actionsScript.ActionTouch);

                ShowCollidersOption();

                ShowGravityOption();

                ShowAudioOptions();

                ShowAnimationOptions();
            }

            actionsScript.DestroyOnTrigger = EditorGUILayout.ToggleLeft("Destroy", actionsScript.DestroyOnTrigger);

            // save changes
            EditorUtility.SetDirty(actionsScript);
        }

        private void ShowCollidersOption()
        {
            actionsScript.ActionColliders = (Gaze_Actions.ACTIVABLE_OPTION)EditorGUILayout.EnumPopup("Colliders:", actionsScript.ActionColliders);
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

        private void displayAnimBlock(int k)
        {
            if (actionsScript.triggerAnimation) // Anim put with old SDK
            {
                if (actionsScript.triggerAnimation && findAnimatorTriggers())
                {
                    if (!selectedAnimatorTriggers.Contains(actionsScript.animatorTriggers[k]))
                    {
                        actionsScript.animatorTriggers[k] = selectedAnimatorTriggers[0];
                    }

                    actionsScript.animatorTriggers[k] = selectedAnimatorTriggers[EditorGUILayout.Popup(selectedAnimatorTriggers.IndexOf(actionsScript.animatorTriggers[k]), selectedAnimatorTriggers.ToArray())];
                }
            }
            else
            {
                if (actionsScript.ActionAnimation == Gaze_Actions.ANIMATION_OPTION.MECANIM && findAnimatorTriggers())
                {
                    if (!selectedAnimatorTriggers.Contains(actionsScript.animatorTriggers[k]))
                    {
                        actionsScript.animatorTriggers[k] = selectedAnimatorTriggers[0];
                    }

                    actionsScript.animatorTriggers[k] = selectedAnimatorTriggers[EditorGUILayout.Popup(selectedAnimatorTriggers.IndexOf(actionsScript.animatorTriggers[k]), selectedAnimatorTriggers.ToArray())];
                }
                else if (actionsScript.ActionAnimation == Gaze_Actions.ANIMATION_OPTION.CLIP)
                {
                    // help message if no input is specified
                    if (actionsScript.animationClip.Count(k) < 1)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.HelpBox("Add at least one animation or deactivate this action if not needed.", MessageType.Warning);
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        // update inputs list in Gaze_Interactions (Gaze_Interactions may have been removed in the hierarchy)
                        for (int i = 0; i < actionsScript.animationClip.Count(k); i++)
                        {
                            // display the entry
                            EditorGUILayout.BeginHorizontal();
                            AnimationClip clip = (AnimationClip)EditorGUILayout.ObjectField("", actionsScript.animationClip.Get(k, i), typeof(AnimationClip), false, GUILayout.Width(300));
                            if (clip != null)
                            {
                                clip.legacy = true;
                                actionsScript.targetAnimationSource.AddClip(clip, clip.name);
                            }
                            actionsScript.animationClip.Set(k, i, clip);

                            // and a '-' button to remove it if needed
                            if (GUILayout.Button("-", GUILayout.Width(100)))
                            {
                                actionsScript.targetAnimationSource.RemoveClip(actionsScript.animationClip.Get(k, i));
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
                    actionsScript.loopAnim[k] = (Gaze_Actions.AUDIO_LOOP)EditorGUILayout.EnumPopup("Loop", actionsScript.loopAnim[k]);

                    if (actionsScript.loopAnim[k] == Gaze_Actions.AUDIO_LOOP.Single)
                    {
                        actionsScript.loopAnimType[k] = (Gaze_Actions.ANIMATION_LOOP)EditorGUILayout.EnumPopup("", actionsScript.loopAnimType[k]);

                    }
                    EditorGUILayout.EndHorizontal();

                    if (actionsScript.animationClip.Count(k) > 1)
                    {
                        actionsScript.animationSequence[k] = (Gaze_Actions.AUDIO_SEQUENCE)EditorGUILayout.EnumPopup("Sequence", actionsScript.animationSequence[k]);
                    }

                }
            }
        }

        private void displayAudioBlock(int k)
        {
            if (actionsScript.OldAudio) // Audio put with old SDK
            {
                if (actionsScript.ActionAudio == Gaze_Actions.ACTIVABLE_OPTION.ACTIVATE && hierarchyAudioSources.Count > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    actionsScript.audioClips[k] = (AudioClip)EditorGUILayout.ObjectField("", actionsScript.audioClips[k], typeof(AudioClip), false);
                    actionsScript.loopAudio[k] = EditorGUILayout.ToggleLeft("Loop", actionsScript.loopAudio[k]);
                    EditorGUILayout.EndHorizontal();
                }
            }

            else //New SDK
            {
                if (actionsScript.audioClipsNew.Count(k) < 1)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.HelpBox("Add at least one audio or deactivate this action if not needed.", MessageType.Warning);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    // update inputs list in Gaze_Interactions (Gaze_Interactions may have been removed in the hierarchy)
                    for (int i = 0; i < actionsScript.audioClipsNew.Count(k); i++)
                    {
                        // display the entry
                        EditorGUILayout.BeginHorizontal();
                        actionsScript.audioClipsNew.Set(k, i, (AudioClip)EditorGUILayout.ObjectField("", actionsScript.audioClipsNew.Get(k, i), typeof(AudioClip), false, GUILayout.Width(300)));


                        // TODO @apelab add/remove event subscription with the new popup value

                        // and a '-' button to remove it if needed
                        if (GUILayout.Button("-", GUILayout.Width(100)))
                            actionsScript.audioClipsNew.Remove(k, i);

                        EditorGUILayout.EndHorizontal();
                    }
                }

                // display 'add' button
                if (GUILayout.Button("+", GUILayout.Width(400)))
                {
                    AudioClip d = actionsScript.audioClipsNew.Add(k);


                    // TODO @apelab add event subscription with a default value
                    //Gaze_InputsCondition inputscondition = targetConditions.activeConditions.GetType(typeof(Gaze_InputsCondition));

                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("-"))
                    {
                        actionsScript.audioClipsNew.Remove(k, d);

                        // TODO @apelab remove event subscription


                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                actionsScript.loopAudioNew[k] = (Gaze_Actions.AUDIO_LOOP)EditorGUILayout.EnumPopup("Loop", actionsScript.loopAudioNew[k]);

                if (actionsScript.audioClipsNew.Count(k) > 1 && actionsScript.loopAudioNew[k] == Gaze_Actions.AUDIO_LOOP.Playlist)
                {
                    actionsScript.fadeInBetween[k] = EditorGUILayout.ToggleLeft("Fade in between", actionsScript.fadeInBetween[k]);

                }
                EditorGUILayout.EndHorizontal();

                if (actionsScript.audioClipsNew.Count(k) > 1)
                {
                    actionsScript.audio_sequence[k] = (Gaze_Actions.AUDIO_SEQUENCE)EditorGUILayout.EnumPopup("Sequence", actionsScript.audio_sequence[k]);
                }
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
            findAnimatorsInHierarchy();
            findAnimatorTriggers();
            findAudioSourcesInHierarchy();
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

        private void findAudioSourcesInHierarchy()
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

        private void FindAnimationSourcesInHierarchy()
        {
            Animation[] asrc = actionsScript.GetComponentInParent<Gaze_InteractiveObject>().GetComponentsInChildren<Animation>();

            hierarchyAnimationSources.Clear();
            hierarchyAnimationSourcesNames.Clear();

            foreach (Animation a in asrc)
            {
                hierarchyAnimationSources.Add(a);
                hierarchyAnimationSourcesNames.Add(a.gameObject.name);
            }
        }

        private bool findAnimatorTriggers()
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
