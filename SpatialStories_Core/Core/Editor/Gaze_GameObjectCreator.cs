using SpatialStories;
using UnityEditor;
using UnityEngine;

namespace Gaze
{
    public class Gaze_GameObjectCreator : MonoBehaviour
    {
        private static void ParentAndUndo(GameObject go, GameObject parent = null)
        {
            GameObjectUtility.SetParentAndAlign(go, parent);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeGameObject = go;
        }

        /// <summary>
        /// Creates the base structure of a TriggerAnimAudio.
        /// </summary>
        public static GameObject CreateInteractiveObject()
        {
            GameObject instance = Instantiate(Resources.Load("Interactive Object") as GameObject);
            instance.name = "Interactive Object (IO)";
            return instance;
        }

        public static GameObject CreateInteractiveCamera()
        {
            GameObject instance = Instantiate(Resources.Load("Camera (IO)") as GameObject);
            instance.name = "Camera (IO)";
            return instance;
        }

        /// <summary>
        /// Converts a GameObject into a Gazable.
        /// </summary>
        /// <returns>The Gazable Root.</returns>
        /// <param name="go">The reference GameObject</param>
        public static GameObject ConvertInteractiveObject(GameObject go)
        {
            GameObject root = Instantiate(Resources.Load("Interactive Object") as GameObject);
            root.name = go.name + " (IO)";
            Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

            root.transform.position = go.transform.position;
            root.transform.rotation = go.transform.rotation;

            // store the original parent for re-attach later on
            Transform goParent = go.transform.parent;

            Undo.SetTransformParent(go.transform, root.transform.Find("Visuals"), "Convert");

            // re-attach to the original parent
            root.transform.parent = goParent;

            Selection.activeGameObject = root;

            return root;
        }

        /// <summary>
        /// Converts a Standard Camera into a SpatialStories one.
        /// </summary>
        /// <returns>The Gazable Root.</returns>
        /// <param name="go">The reference GameObject</param>
        public static GameObject ConvertInteractiveCamera(GameObject go)
        {
            // Load the interactivce camera
            GameObject InteractiveCamera = Instantiate(Resources.Load("Camera (IO)") as GameObject, go.transform.position, go.transform.rotation);

            GameObject arkitUtilities = Instantiate(Resources.Load("ARUtilities") as GameObject, go.transform.position, go.transform.rotation);
            arkitUtilities.name = "ARUtilities";

            InteractiveCamera.transform.position = Vector3.zero;
            InteractiveCamera.transform.rotation = Quaternion.identity;
            InteractiveCamera.transform.localScale = go.transform.localScale;
            InteractiveCamera.name = "Camera (IO)";
            DestroyImmediate(go);
            return InteractiveCamera;
        }

        /// <summary>
        /// Creates the base structure of a SceneLoader.
        /// </summary>
        public static GameObject CreateSceneLoader()
        {
            GameObject root = Instantiate(Resources.Load("Interactive Object") as GameObject);
            GameObject sceneLoader = root.GetComponentInChildren<Gaze_Actions>().gameObject;

            sceneLoader.name = "SceneLoader";
            sceneLoader.AddComponent<Gaze_SceneLoader>();

            return root;
        }

        #region Main Menu

        [MenuItem("GameObject/Spatial Stories/Convert into interactive object", false, 10)]
        public static void GameObjectConvertIntoObject(MenuCommand menuCommand)
        {
            if (Selection.activeGameObject != null)
                ConvertInteractiveObject(Selection.activeGameObject);
        }

        [MenuItem("GameObject/Spatial Stories/Convert into interactive camera", false, 10)]
        public static void GameObjectConvertIntoCamera(MenuCommand menuCommand)
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Camera>() != null)
                ConvertInteractiveCamera(Selection.activeGameObject);
        }

        #endregion

        #region Contextual Menu

        [MenuItem("SpatialStories/Create/Interactive Object")]
        public static void MenuCreateInteractiveObject()
        {
            ParentAndUndo(CreateInteractiveObject());
        }

        [MenuItem("SpatialStories/Create/Interactive Camera")]
        public static void MenuCreateInteractiveCamera()
        {
            ParentAndUndo(CreateInteractiveCamera());
        }

        [MenuItem("SpatialStories/Convert/Into interactive Object")]
        public static void MenuConvertInteractiveObject()
        {
            Selection.activeGameObject = ConvertInteractiveObject(Selection.activeGameObject);
        }

        [MenuItem("SpatialStories/Convert/Into interactive Object", true)]
        public static bool ValidateGameobjectSelection()
        {
            return Selection.activeGameObject != null;
        }

        [MenuItem("SpatialStories/Convert/Into interactive Camera")]
        public static void MenuConvertInteractiveCamera()
        {
            Selection.activeGameObject = ConvertInteractiveCamera(Selection.activeGameObject);
        }

        [MenuItem("SpatialStories/Convert/Into interactive Camera", true)]
        public static bool ValidateCameraSelection()
        {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Camera>() != null;
        }
        #endregion

        [MenuItem("Utils/Delete All PlayerPrefs")]
	    static public void DeleteAllPlayerPrefs() {
		    PlayerPrefs.DeleteAll();
	    }

        [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
        public class ReadOnlyDrawer : PropertyDrawer
        {
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
                GUI.enabled = true;
            }
        }
    }
}