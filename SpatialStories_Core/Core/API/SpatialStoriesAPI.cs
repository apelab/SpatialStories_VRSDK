using Gaze;
using UnityEngine;

namespace SpatialStories
{
    public static class SpatialStoriesAPI
    {
        /// <summary>
        /// Creates a new IO definition, usefull to begin the process of
        /// creation of an interactive object.
        /// </summary>
        /// <param name="_name">Name of the io that will be created with this definition</param>
        /// <returns></returns>
        public static S_IODefinition CreateIODefinition(string _name)
        {
            return new S_IODefinition(_name);
        }

        /// <summary>
        /// Creates a new IO definition using a GameObject as starting point (it will be
        /// the interactive object visuals and use its name)
        /// </summary>
        /// <param name="_visuals"> The visuals of the new interactive object </param>
        /// <param name="_copyPositionAndRotation"> Use the visuals position as rotation as starting point for the io</param>
        /// <returns></returns>
        public static S_IODefinition CreateIODefinition(GameObject _visuals, bool _copyPositionAndRotation=true)
        {
            return new S_IODefinition(_visuals, _copyPositionAndRotation);
        }

        /// <summary>
        /// Creates a new Interactive Object using the definition.
        /// </summary>
        /// <param name="_definition">Definition of the game object to create</param>
        /// <param name="_wireDependencies"> Do the dependencies will be wired now or are you going to call WirePendingDependencies? (set that to false if you have circular dependencies that need
        /// to be post processed when all the ios and interactions have been created) </param>
        /// <returns></returns>
        public static Gaze_InteractiveObject CreateInteractiveObject(S_IODefinition _definition, bool _wireDependencies=true)
        {
            return SpatialStoriesFactory.CreateInteractiveObject(_definition, _wireDependencies);
        }

        /// <summary>
        /// Connects all the pending dependencies created by the api, when _wireDependencies is set to false
        /// </summary>
        public static void WirePendingDependencies()
        {
            SpatialStoriesFactory.WirePendingDependencies();
        }

        /// <summary>
        /// Finds an already created object by GUID.
        /// </summary>
        /// <param name="_guid">The guid of the object</param>
        /// <returns></returns>
        public static GameObject GetObjectOfTypeWithGUID(string _guid)
        {
            S_Guid[] guids = GameObject.FindObjectsOfType<S_Guid>();

            for (int i = 0; i < guids.Length; i++)
            {
                S_Guid guid = guids[i];
                if (guid.GUID.Equals(_guid))
                {
                    return guid.gameObject;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds an interactive object using it's GUID
        /// </summary>
        /// <param name="_guid">Guid of the Interactive Object</param>
        /// <returns></returns>
        public static Gaze_InteractiveObject GetInteractiveObjectWithGUID(string _guid)
        {
            // Get the object to touch
            GameObject ioObj = SpatialStoriesAPI.GetObjectOfTypeWithGUID(_guid);
            Gaze_InteractiveObject io = null;
            if (ioObj != null)
            {
                io = ioObj.GetComponent<Gaze_InteractiveObject>();
            }
            else
            {
                Debug.LogError(string.Format("IO with GUID {0} Not found", _guid));
            }
            return io;
        }
    }
}

