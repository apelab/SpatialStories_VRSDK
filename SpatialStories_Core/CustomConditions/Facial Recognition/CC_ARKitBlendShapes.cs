using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.XR.iOS;
#endif

namespace Gaze
{
    [Serializable]
    [ExecuteInEditMode]
    public class CC_ARKitBlendShapes : Gaze_AbstractConditions
    {
        [SerializeField]
        public string[] ARBlendShapes =
        {
        "browDown_L",
        "browDown_R",
        "browInnerUp",
        "browOuterUp_L",
        "browOuterUp_R",
        "cheekPuff",
        "cheekSquint_L",
        "cheekSquint_R",
        "eyeBlink_L",
        "eyeBlink_R",
        "eyeLookDown_L",
        "eyeLookDown_R",
        "eyeLookIn_L",
        "eyeLookIn_R",
        "eyeLookOut_L",
        "eyeLookOut_R",
        "eyeLookUp_L",
        "eyeLookUp_R",
        "eyeSquint_L",
        "eyeSquint_R",
        "eyeWide_L",
        "eyeWide_R",
        "jawForward",
        "jawLeft",
        "jawOpen",
        "jawRight",
        "mouthClose",
        "mouthDimple_L",
        "mouthDimple_R",
        "mouthFrown_L",
        "mouthFrown_R",
        "mouthFunnel",
        "mouthLeft",
        "mouthLowerDown_L",
        "mouthLowerDown_R",
        "mouthPress_L",
        "mouthPress_R",
        "mouthPucker",
        "mouthRight",
        "mouthRollLower",
        "mouthRollUpper",
        "mouthShrugLower",
        "mouthShrugUpper",
        "mouthSmile_L",
        "mouthSmile_R",
        "mouthStretch_L",
        "mouthStretch_R",
        "mouthUpperUp_L",
        "mouthUpperUp_R",
        "noseSneer_L",
        "noseSneer_R"
    };

        public bool RequireAll = true;

        [SerializeField]
        public SelectedBlendShapesContainer SelectedBlendShapes = new SelectedBlendShapesContainer();
        public bool[] blendShapesStates;

        private Gaze_Conditions conditions;

        bool isActive = false;

        private void Start()
        {
            conditions = GetComponent<Gaze_Conditions>();
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
                return;

            blendShapesStates = new bool[SelectedBlendShapes.ShapesToTrack.Count];
            for (int i = 0; i < blendShapesStates.Length; i++)
                blendShapesStates[i] = false;

#if UNITY_IOS
            UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdded;
            UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdated;
            UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent += FaceRemoved;
#endif
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
                return;

#if UNITY_IOS
            UnityARSessionNativeInterface.ARFaceAnchorAddedEvent -= FaceAdded;
            UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent -= FaceUpdated;
            UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent -= FaceRemoved;
#endif
        }

#if UNITY_IOS
        private void FaceRemoved(ARFaceAnchor anchorData)
        {
            isActive = false;
        }

        private void FaceUpdated(ARFaceAnchor anchorData)
        {
            // Check if we have dependencies before beginning to check
            if (conditions.dependent && !conditions.ActivateOnDependencyMap.AreDependenciesSatisfied)
                return;

            if (!isActive)
                return;

            int count = SelectedBlendShapes.ShapesToTrack.Count;
            for (int i = 0; i < count; i++)
            {
                ShapeToTrack shape = SelectedBlendShapes.ShapesToTrack[i];
                if (anchorData.blendShapes.ContainsKey(ARBlendShapes[shape.PositionInList]))
                {
                    // Get the value
                    float value = anchorData.blendShapes[ARBlendShapes[shape.PositionInList]];

                    bool validity = false;
                    switch (shape.comparisonType)
                    {
                        case BlendValueType.EQUALS:
                            validity = shape.Value == value;
                            break;
                        case BlendValueType.LOWER_THAN:
                            validity = shape.Value > value;
                            break;
                        case BlendValueType.GREATER_THAN:
                            validity = shape.Value < value;
                            break;
                    }
                    blendShapesStates[i] = validity;
                    if (!RequireAll)
                    {
                        if (blendShapesStates[i])
                        {
                            ValidateCustomCondition(true);
                            return;
                        }
                    }
                }

                if (RequireAll)
                {
                    for (int z = 0; z < count; z++)
                    {
                        shape = SelectedBlendShapes.ShapesToTrack[z];
                        if (!blendShapesStates[i])
                            return;
                    }
                    ValidateCustomCondition(true);
                }
            }
        }

        private void FaceAdded(ARFaceAnchor anchorData)
        {
            isActive = true;
        }
#endif
        public void Validate()
        {
            ValidateCustomCondition(true);
        }
    }

    [Serializable]
    public class ShapeToTrack
    {
        [SerializeField]
        public int PositionInList;
        [SerializeField]
        public BlendValueType comparisonType;
        [SerializeField]
        public float Value;
    }

    [Serializable]
    public class SelectedBlendShapesContainer
    {
        [SerializeField]
        public List<ShapeToTrack> ShapesToTrack;

        public SelectedBlendShapesContainer()
        {
            ShapesToTrack = new List<ShapeToTrack>();
        }
    }

    [Serializable]
    public enum BlendValueType
    {
        GREATER_THAN,
        LOWER_THAN,
        EQUALS
    }
}