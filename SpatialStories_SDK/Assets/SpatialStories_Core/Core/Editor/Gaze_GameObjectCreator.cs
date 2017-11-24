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

        public static GameObject CovertCameraUsingPrefab(GameObject _go, string _prefabName)
        {
            if (!_go.GetComponent<Gaze_CameraRaycaster>())
                _go.AddComponent<Gaze_CameraRaycaster>();

            if (!_go.GetComponent<Gaze_MouseLookController>())
                _go.AddComponent<Gaze_MouseLookController>();


            // Load the interactivce camera
            GameObject InteractiveCamera = Instantiate(Resources.Load(_prefabName) as GameObject, _go.transform.position, _go.transform.rotation);

            InteractiveCamera.name = _prefabName;

            // Get the camera in children
            Camera cam = InteractiveCamera.GetComponentInChildren<Camera>();

            _go.GetComponent<Camera>().tag = "MainCamera";

            // Get the visuals game object
            Transform cameraParent = cam.transform.parent;

            // Destroy the camera
            GameObject.DestroyImmediate(cam.gameObject);

            // Add the components that we need.
            if (!_go.GetComponent<Gaze_CameraRaycaster>())
                _go.AddComponent<Gaze_CameraRaycaster>();

            if (!_go.GetComponent<Gaze_MouseLookController>())
                _go.AddComponent<Gaze_MouseLookController>();

            if (!_go.GetComponent<Gaze_Camera>())
                _go.AddComponent<Gaze_Camera>();

            if (!_go.GetComponent<OVRManager>())
                _go.AddComponent<OVRManager>();

            // Add the camera to the visuals of the interactive camera.
            _go.transform.SetParent(cameraParent);

            // Add the controller disconnected message
            Gaze_ControllerDisconnectedMessage message = InteractiveCamera.GetComponentInChildren<Gaze_ControllerDisconnectedMessage>();
            message.transform.SetParent(cameraParent);
            message.gameObject.SetActive(false);
            message.transform.localPosition = new Vector3(0, 0, 0.75f);

            _go.GetComponent<Camera>().nearClipPlane = 0.01f;

            // Return the game object
            return _go;
        }

        /// <summary>
        /// Converts a Standard Camera into a SpatialStories one.
        /// </summary>
        /// <returns>The Gazable Root.</returns>
        /// <param name="go">The reference GameObject</param>
        public static GameObject ConvertInteractiveCamera(GameObject go)
        {
            return CovertCameraUsingPrefab(go, "Camera (IO)");
        }

        public static GameObject CreateGearVrInteractiveCamera()
        {
            GameObject cam = new GameObject();
            cam.AddComponent<Camera>();
            ConvertGearVrInteractiveCamera(cam);
            return cam as GameObject;
        }

        /// <summary>
        /// Converts a Standard Camera into a Gear Vr one.
        /// </summary>
        /// <returns>The Gazable Root.</returns>
        /// <param name="go">The reference GameObject</param>
        public static GameObject ConvertGearVrInteractiveCamera(GameObject go)
        {
            return CovertCameraUsingPrefab(go, "Camera GearVR (IO)");
        }

        /// <summary>
        /// Converts a Standard Camera into a Gear Vr one.
        /// </summary>
        /// <returns>The Gazable Root.</returns>
        /// <param name="go">The reference GameObject</param>
        public static GameObject ConvertOculusInteractiveCamera(GameObject go)
        {
            return CovertCameraUsingPrefab(go, "Camera Oculus (IO)");
        }

        /// <summary>
        /// Converts a Standard Camera into a Gear Vr one.
        /// </summary>
        /// <returns>The Gazable Root.</returns>
        /// <param name="go">The reference GameObject</param>
        public static GameObject ConvertHTCInteractiveCamera(GameObject go)
        {
            return CovertCameraUsingPrefab(go, "Camera HTCVive (IO)");
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

        public static void AdjustColliders(GameObject _selectedGameObject)
        {
            Gaze_InteractiveObjectVisuals vis = _selectedGameObject.GetComponentInChildren<Gaze_InteractiveObjectVisuals>();
            Transform t = vis.transform.GetChild(0);
            BoxCollider collider = null;
            if (t != null)
            {
                collider = t.gameObject.AddComponent<BoxCollider>();
            }
            BoxCollider[] colliders = _selectedGameObject.GetComponentsInChildren<BoxCollider>();
            foreach (BoxCollider c in colliders)
            {
                c.center = collider.center;
                c.size = collider.size;
            }
            GameObject.DestroyImmediate(collider);
        }

        [MenuItem("GameObject/Tools/AdjustColliders", false, 10)]
        public static void AdjustCollidersOfObject(MenuCommand menuCommand)
        {
            if (Selection.activeGameObject != null)
                AdjustColliders(Selection.activeGameObject);
        }

        [MenuItem("GameObject/SpatialStories/Convert into interactive object", false, 10)]
        public static void GameObjectConvertIntoObject(MenuCommand menuCommand)
        {
            if (Selection.activeGameObject != null)
                ConvertInteractiveObject(Selection.activeGameObject);
        }

        [MenuItem("GameObject/SpatialStories/Convert into Oculus Rift camera", false, 10)]
        public static void GameObjectConvertIntoCamera(MenuCommand menuCommand)
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Camera>() != null)
                ConvertOculusInteractiveCamera(Selection.activeGameObject);
        }

        [MenuItem("GameObject/SpatialStories/Convert into HTC Vive camera", false, 10)]
        public static void GameObjectConvertIntoHTCVive(MenuCommand menuCommand)
        {
            EditorUtility.DisplayDialog("HTC Vive tip", "Remember to place the camera at the same height than your scene's floor", "Understood");
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Camera>() != null)
                ConvertHTCInteractiveCamera(Selection.activeGameObject);
        }

        [MenuItem("GameObject/SpatialStories/Convert into GearVr interactive camera", false, 10)]
        public static void GameObjectToGearVrCamera(MenuCommand menuCommand)
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Camera>() != null)
                ConvertGearVrInteractiveCamera(Selection.activeGameObject);
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

        [MenuItem("SpatialStories/Convert/Into Interactive Object")]
        public static void MenuConvertInteractiveObject()
        {
            Selection.activeGameObject = ConvertInteractiveObject(Selection.activeGameObject);
        }

        [MenuItem("SpatialStories/Convert/Into Interactive Object", true)]
        public static bool ValidateGameobjectSelection()
        {
            return Selection.activeGameObject != null;
        }

        [MenuItem("SpatialStories/Convert/Into Interactive Camera")]
        public static void MenuConvertInteractiveCamera()
        {
            Selection.activeGameObject = ConvertInteractiveCamera(Selection.activeGameObject);
        }

        [MenuItem("SpatialStories/Convert/into Interactive Camera", true)]
        public static bool ValidateCameraSelection()
        {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Camera>() != null;
        }

        [MenuItem("SpatialStories/Create/GearVr Interactive Camera")]
        public static void GearVrCameraCreation()
        {
            ParentAndUndo(CreateGearVrInteractiveCamera());
        }
        #endregion
    }
}
