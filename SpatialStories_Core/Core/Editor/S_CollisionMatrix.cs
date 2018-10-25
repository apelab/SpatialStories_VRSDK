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
        private static bool layersHaveBeenCreated = false;
        #endregion

        /// <summary>
        /// Create needed layers and setup collision matrix
        /// </summary>
        static S_CollisionMatrix()
        {
            CreateLayerIfNeeded("ARKitPlane");
        }

        /// <summary>
        /// Checks if all the required layers exist and create them if not.
        /// </summary>
        private static void CreateLayerIfNeeded(string _name)
        {
            if (LayerMask.NameToLayer(_name) == -1)
                if (CreateLayer(_name) != -1)
                {

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


            SerializedProperty layerProp = layers.GetArrayElementAtIndex(10);
            emptyLayerInt = 10;
            firstEmptyProp = layerProp;
        
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
    }
}
