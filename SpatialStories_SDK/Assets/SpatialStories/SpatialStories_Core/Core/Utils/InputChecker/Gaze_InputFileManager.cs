using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Gaze
{
    /// <summary>
    /// Used to get paths, and files related with the input manager assets configs.
    /// </summary>
    public static class Gaze_InputFileManager
    {
        public static string GetAssetsFolderPath()
        {
            return Application.dataPath;
        }

        public static string GetActualInputManagerPath()
        {
            List<string> files = FindFilesInFolder(Path.Combine(Gaze_InputManagerChecker.PathToAssetsFolder, ".."), Gaze_InputConfigConstants.GENERIC_INPUT_MANAGER);
            if (files.Count > 0)
                return files[0];
            return null;
        }

        public static string GetSpatialStoriesSDKInputManager()
        {
            List<string> files = FindFilesInFolder(Gaze_InputManagerChecker.PathToAssetsFolder, Gaze_InputConfigConstants.SSDK_INPUT_MANAGER);
            if (files.Count > 0)
                return files[0];
            return null;
        }

        public static List<string> FindFilesInFolder(string _path, string _fileName)
        {
            List<string> pathsToReturn = new List<string>();
            foreach (string file in Directory.GetFiles(_path, _fileName, SearchOption.AllDirectories))
            {
                pathsToReturn.Add(file);
            }
            return pathsToReturn;
        }
    }
}
