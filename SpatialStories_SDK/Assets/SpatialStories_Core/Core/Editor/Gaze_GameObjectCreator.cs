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
            return Instantiate(Resources.Load("Interactive Object") as GameObject);
        }

        public static GameObject CreateInteractiveCamera()
        {
            GameObject cam = new GameObject();
            cam.AddComponent<Camera>();
            ConvertInteractiveCamera(cam);

            return cam as GameObject;
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
            //			if (!go.GetComponent<Gaze_CameraCollider> ())
            //				go.AddComponent<Gaze_CameraCollider> ();

            if (!go.GetComponent<Gaze_CameraRaycaster>())
                go.AddComponent<Gaze_CameraRaycaster>();

            if (!go.GetComponent<Gaze_MouseLookController>())
                go.AddComponent<Gaze_MouseLookController>();


            // Load the interactivce camera
            GameObject InteractiveCamera = Instantiate(Resources.Load("Camera (IO)") as GameObject, go.transform.position, go.transform.rotation);

            InteractiveCamera.name = "Camera (IO)";

            // Get the camera in children
            Camera cam = InteractiveCamera.GetComponentInChildren<Camera>();

            go.GetComponent<Camera>().tag = "MainCamera";

            // Get the visuals game object
            Transform cameraParent = cam.transform.parent;

            // Destroy the camera
            GameObject.DestroyImmediate(cam.gameObject);

            // Add the components that we need.
            if (!go.GetComponent<Gaze_CameraRaycaster>())
                go.AddComponent<Gaze_CameraRaycaster>();

            if (!go.GetComponent<Gaze_MouseLookController>())
                go.AddComponent<Gaze_MouseLookController>();

            if (!go.GetComponent<Gaze_Camera>())
                go.AddComponent<Gaze_Camera>();

            if (!go.GetComponent<OVRManager>())
                go.AddComponent<OVRManager>();

            // Add the camera to the visuals of the interactive camera.
            go.transform.SetParent(cameraParent);

            // Add the controller disconnected message
            Gaze_ControllerDisconnectedMessage message = InteractiveCamera.GetComponentInChildren<Gaze_ControllerDisconnectedMessage>();
            message.transform.SetParent(cameraParent);
            message.gameObject.SetActive(false);
            message.transform.localPosition = new Vector3(0, 0, 0.75f);

            go.GetComponent<Camera>().nearClipPlane = 0.01f;

            // Return the game object
            return go;
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

        [MenuItem("Spatial Stories/Create/Interactive Object")]
        public static void MenuCreateInteractiveObject()
        {
            ParentAndUndo(CreateInteractiveObject());
        }

        [MenuItem("Spatial Stories/Create/Interactive Camera")]
        public static void MenuCreateInteractiveCamera()
        {
            ParentAndUndo(CreateInteractiveCamera());
        }

        [MenuItem("Spatial Stories/Convert/Into interactive Object")]
        public static void MenuConvertInteractiveObject()
        {
            Selection.activeGameObject = ConvertInteractiveObject(Selection.activeGameObject);
        }

        [MenuItem("Spatial Stories/Convert/Into interactive Object", true)]
        public static bool ValidateGameobjectSelection()
        {
            return Selection.activeGameObject != null;
        }

        [MenuItem("Spatial Stories/Convert/Into interactive Camera")]
        public static void MenuConvertInteractiveCamera()
        {
            Selection.activeGameObject = ConvertInteractiveCamera(Selection.activeGameObject);
        }

        [MenuItem("Spatial Stories/Convert/Into interactive Camera", true)]
        public static bool ValidateCameraSelection()
        {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Camera>() != null;
        }
        #endregion
    }
}