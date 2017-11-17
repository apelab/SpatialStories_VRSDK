#if UNITY_EDITOR
#endif


using System;
using UnityEditor;
using UnityEngine;

namespace Gaze
{
    /// <summary>
    /// Checks if the current project has all the necessary layers and matrix options correctly setup
    /// Spatial Stories SDK and repairs it if not.
    /// </summary>
    public static class Gaze_PhysicsChecker
    {

        public static void CreateNecessaryLayersIfNeeded()
        {
            GameObject go = new GameObject();

            try
            {
                go.layer = LayerMask.NameToLayer(Gaze_HashIDs.LAYER_PROXIMTY);
            }
            catch (Exception ex)
            {
                CreateLayer(Gaze_HashIDs.LAYER_GAZE);
                CreateLayer(Gaze_HashIDs.LAYER_SOLID);
                CreateLayer(Gaze_HashIDs.LAYER_PROXIMTY);
                CreateLayer(Gaze_HashIDs.LAYER_HANDHOVER);
            }
            GameObject.Destroy(go);

        }

        static void CreateLayer(string _name)
        {
            if (EditorUtility.DisplayDialog("Missing Physics Layers", "Please add the {0},{1},{2} and {3} on the Edit > Project Settings > Tangs & Layers ", "Add them by hand"))
            {
                EditorApplication.ExecuteMenuItem("Edit/Play");
            }
        }
    }
}