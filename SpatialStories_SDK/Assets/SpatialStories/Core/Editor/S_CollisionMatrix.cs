using System;
using System.Text;
using UnityEngine;
using Gaze;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpatialStories
{
    [Serializable]
    [InitializeOnLoad]
    public class S_CollisionMatrix : Editor
    {
        #region Members
        private static StringBuilder popupText = new StringBuilder();
        private static bool layersHaveBeenCreated = false;
        #endregion

        /// <summary>
        /// Create needed layers and setup collision matrix
        /// </summary>
        static S_CollisionMatrix()
        {
            CreateLayerIfNeeded(Gaze_HashIDs.LAYER_TELEPORT);
            CreateLayerIfNeeded(Gaze_HashIDs.LAYER_HANDHOVER);
            CreateLayerIfNeeded(Gaze_HashIDs.LAYER_PROXIMTY);
            CreateLayerIfNeeded(Gaze_HashIDs.LAYER_GAZE);
            CreateLayerIfNeeded(Gaze_HashIDs.LAYER_SOLID);

            if (layersHaveBeenCreated)
                EditorUtility.DisplayDialog("New layers created !", popupText.ToString(), "Ok");

            SetupIgnoreCollisionMatrix();
        }

        /// <summary>
        /// Checks if all the required layers exist and create them if not.
        /// </summary>
        private static void CreateLayerIfNeeded(string _name)
        {
            if (LayerMask.NameToLayer(_name) == -1)
                if (CreateLayer(_name) != -1)
                {
                    popupText.Append("\n");
                    popupText.Append(_name);
                    layersHaveBeenCreated = true;
                }
        }

        /// <summary>
        /// Creates a layer at the next available index and returns its index.
        /// Returns silently if layer already exists.
        /// </summary>
        /// <param name="name">Name of the layer to create</param>
        public static int CreateLayer(string name)
        {
            int emptyLayerInt = -1;

            if (string.IsNullOrEmpty(name))
                throw new System.ArgumentNullException("name", "New layer name string is either null or empty.");

            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");
            var layersCount = layers.arraySize;

            SerializedProperty firstEmptyProp = null;

            for (var i = 0; i < layersCount; i++)
            {
                SerializedProperty layerProp = layers.GetArrayElementAtIndex(i);

                string stringValue = layerProp.stringValue;

                if (stringValue == name) return i;

                if (i < 8 || stringValue != string.Empty) continue;

                if (firstEmptyProp == null)
                {
                    emptyLayerInt = i;
                    firstEmptyProp = layerProp;
                }
            }

            if (firstEmptyProp == null)
            {
                UnityEngine.Debug.LogError("Maximum limit of " + layersCount + " layers exceeded. Layer \"" + name + "\" not created.");
                return -1;
            }

            firstEmptyProp.stringValue = name;
            tagManager.UpdateIfRequiredOrScript();
            tagManager.ApplyModifiedProperties();
            return emptyLayerInt;
        }

        /// <summary>
        /// Ignore collisions between newly created layers
        /// </summary>
        private static void SetupIgnoreCollisionMatrix()
        {
            // ignore collisions between new layers and any of the 8 first reserved layers
            for (int i = 0; i <= 7; i++)
            {
                Physics.IgnoreLayerCollision(i, LayerMask.NameToLayer(Gaze_HashIDs.LAYER_GAZE), true);
                Physics.IgnoreLayerCollision(i, LayerMask.NameToLayer(Gaze_HashIDs.LAYER_HANDHOVER), true);
                Physics.IgnoreLayerCollision(i, LayerMask.NameToLayer(Gaze_HashIDs.LAYER_PROXIMTY), true);
                Physics.IgnoreLayerCollision(i, LayerMask.NameToLayer(Gaze_HashIDs.LAYER_SOLID), true);
                Physics.IgnoreLayerCollision(i, LayerMask.NameToLayer(Gaze_HashIDs.LAYER_TELEPORT), false);
            }

            // ignore collisions between newly created layers
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Gaze_HashIDs.LAYER_GAZE), LayerMask.NameToLayer(Gaze_HashIDs.LAYER_HANDHOVER), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Gaze_HashIDs.LAYER_GAZE), LayerMask.NameToLayer(Gaze_HashIDs.LAYER_PROXIMTY), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Gaze_HashIDs.LAYER_GAZE), LayerMask.NameToLayer(Gaze_HashIDs.LAYER_SOLID), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Gaze_HashIDs.LAYER_GAZE), LayerMask.NameToLayer(Gaze_HashIDs.LAYER_TELEPORT), false);


            Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Gaze_HashIDs.LAYER_HANDHOVER), LayerMask.NameToLayer(Gaze_HashIDs.LAYER_PROXIMTY), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Gaze_HashIDs.LAYER_HANDHOVER), LayerMask.NameToLayer(Gaze_HashIDs.LAYER_SOLID), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Gaze_HashIDs.LAYER_HANDHOVER), LayerMask.NameToLayer(Gaze_HashIDs.LAYER_TELEPORT), false);


            Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Gaze_HashIDs.LAYER_PROXIMTY), LayerMask.NameToLayer(Gaze_HashIDs.LAYER_SOLID), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Gaze_HashIDs.LAYER_PROXIMTY), LayerMask.NameToLayer(Gaze_HashIDs.LAYER_TELEPORT), false);

            Physics.IgnoreLayerCollision(LayerMask.NameToLayer(Gaze_HashIDs.LAYER_TELEPORT), LayerMask.NameToLayer(Gaze_HashIDs.LAYER_SOLID), false);
        }
    }
}