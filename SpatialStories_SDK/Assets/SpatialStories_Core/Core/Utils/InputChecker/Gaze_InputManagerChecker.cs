using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gaze
{
    /// <summary>
    /// Checks if the current project has all the necessary inputs required for using
    /// Spatial Stories SDK and repairs it if not.
    /// </summary>
    public static class Gaze_InputManagerChecker
    {
        public static string PathToAssetsFolder;
        public static string ActualInputManagerPath;
        public static string SpatialStoriesInputMannagerPath;

        private static bool AreInputFixed = false;

        public static void ShowExplorer(string itemPath)
        {
            EditorUtility.RevealInFinder(itemPath);
        }

        public static void ShowInputNotCorrectlyConfiguredDialog()
        {
            PathToAssetsFolder = Gaze_InputFileManager.GetAssetsFolderPath();
            string path = Gaze_InputFileManager.GetActualInputManagerPath();

            if (EditorUtility.DisplayDialog("Missing Inputs", "It seems that you don't have the Spatial Stories SDK input manager correctly configured", "Solve It Automatically", "Add them by hand"))
            {
                AddMissingInputsToPoject();
            }
            else
            {
                if (EditorUtility.DisplayDialog("Missing Inputs", "Replace the InputManager.asset file with our own (can be downloaded on www.spatialstories.net).", "Show me the file to replace", "Ok"))
                {
                    ShowExplorer(path);
                }
            }
            EditorApplication.ExecuteMenuItem("Edit/Play");

        }

        public static bool IsInputManagerAssetIsInstaled()
        {
            try
            {
                Input.GetButtonDown("Oculus Stick Left");
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }

        public static void AddMissingInputsToPoject()
        {
            if (AreInputFixed)
                return;

            // Set that the inputs has been fixed (Multiple input managers can cause problems)
            AreInputFixed = true;

            // Find the actual assets folder on the machine
            PathToAssetsFolder = Gaze_InputFileManager.GetAssetsFolderPath();

            // Get the actual Input Manager.
            ActualInputManagerPath = Gaze_InputFileManager.GetActualInputManagerPath();

            // Get the folder with our input manager.
            SpatialStoriesInputMannagerPath = Gaze_InputFileManager.GetSpatialStoriesSDKInputManager();

            if (SpatialStoriesInputMannagerPath == null)
            {
                UnityEngine.Debug.LogError("Could not " + Gaze_InputConfigConstants.SSDK_INPUT_MANAGER + " file import the project again and import it.");
                return;
            }

            // Get the user's current input
            SerializationMode actualSerializationMode = Gaze_InputParser.IsInBinaryFormat(ActualInputManagerPath) ? SerializationMode.ForceBinary : SerializationMode.ForceText;
            List<Gaze_InputConfig> actualInputConfig = GetActualInputConfig(actualSerializationMode);

            // Get SSDK inputs
            List<Gaze_InputConfig> spatialStoriesInputs = Gaze_InputParser.ParseInputFile(SpatialStoriesInputMannagerPath);

            // Compare what inputs are missing
            List<Gaze_InputConfig> missingInputs = FindMissingInputsFromTo(actualInputConfig, spatialStoriesInputs);

            // Combine input lists
            List<Gaze_InputConfig> newInputConfig = new List<Gaze_InputConfig>();
            newInputConfig.AddRange(actualInputConfig);
            newInputConfig.AddRange(missingInputs);

            // Create the new input file
            string backupFile = Gaze_InputFileWritter.WriteFile(newInputConfig, ActualInputManagerPath, true);

            EditorUtility.DisplayDialog("Spatial Stories SDK", "Inputs Fixed," + Environment.NewLine + Environment.NewLine + "(As we have modified your input settings contents," +
                " after clicking \"Ok\" we are going to reveal the location of the old input file contents in case you want to keep them)", "Ok");
            ShowExplorer(backupFile);
        }

        public static List<Gaze_InputConfig> GetActualInputConfig(SerializationMode _serializationMode)
        {
            return _serializationMode == SerializationMode.ForceText ?
                Gaze_InputParser.ParseInputFile(ActualInputManagerPath) :
                Gaze_InputParser.ReadBinaryInputs(ActualInputManagerPath);
        }

        public static List<Gaze_InputConfig> FindMissingInputsFromTo(List<Gaze_InputConfig> _actualInputConfig, List<Gaze_InputConfig> _ssdkInputs)
        {
            return _ssdkInputs.Where(inp => !_actualInputConfig.Any(inp2 => inp.m_Name == inp2.m_Name)).ToList();
        }
    }
}
