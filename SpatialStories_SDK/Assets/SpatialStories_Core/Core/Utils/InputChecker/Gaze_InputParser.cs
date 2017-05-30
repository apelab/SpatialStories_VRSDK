using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gaze
{
    /// <summary>
    /// Reads the InputManager files in YAML or Binary mode and converts them into
    /// Gaze_InputConfig objects (More friendly for being compared, than text or
    /// binary files)
    /// </summary>
    public static class Gaze_InputParser
    {





        /// <summary>
        /// In a YAML file looks for the keyword that points the origin of all the inputs.
        /// </summary>
        /// <param name="_inputs"></param>
        /// <returns></returns>
        public static int FindInputsOrigin(string[] _inputs)
        {
            return _inputs.ToList().FindIndex(i => i.Contains(Gaze_InputConfigConstants.AXIS_TAG)) + 1;
        }

        /// <summary>
        /// Returns a list of lists where each list contains all the parameters of a Inputconfig, like that
        /// is very easy to create the objects passing the subarrays to the costructor.
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static List<string[]> GroupLinesInObjects(string _path)
        {
            List<string[]> objects = new List<string[]>();
            string[] inputs = File.ReadAllLines(_path);
            int inputsOrigin = FindInputsOrigin(inputs);
            int numInputs = (inputs.Length - inputsOrigin) / Gaze_InputConfig.NUM_PARAMETERS;
            for (int i = 0; i < numInputs; i++)
            {
                objects.Add(inputs.SubArray((i * Gaze_InputConfig.NUM_PARAMETERS) + inputsOrigin, Gaze_InputConfig.NUM_PARAMETERS));
            }
            return objects;
        }


        /// <summary>
        /// Converts all the lines from the output of GroupLinesInObjects method into 
        /// Gaze_InputConfig.
        /// </summary>
        /// <param name="_objects"></param>
        /// <returns></returns>
        public static List<Gaze_InputConfig> LinesToInputConfigList(List<string[]> _objects)
        {
            List<Gaze_InputConfig> inputConfigs = new List<Gaze_InputConfig>();
            foreach (string[] obj in _objects)
                inputConfigs.Add(new Gaze_InputConfig(obj));
            return inputConfigs;
        }

        /// <summary>
        /// Combines  LinesToInputConfigList and The GroupILinesInObjects into one single call.
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static List<Gaze_InputConfig> ParseInputFile(string _path)
        {
            return LinesToInputConfigList(GroupLinesInObjects(_path));
        }

        /// <summary>
        /// Deserializes the InputManager.asset and returns a list of Gaze_InputConfigs
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static List<Gaze_InputConfig> ReadBinaryInputs(string _path)
        {
            List<Gaze_InputConfig> inputConfigs = new List<Gaze_InputConfig>();
            var inputManager = AssetDatabase.LoadAllAssetsAtPath(_path)[0];

            SerializedObject obj = new SerializedObject(inputManager);

            SerializedProperty axisArray = obj.FindProperty("m_Axes");

            if (axisArray.arraySize == 0)
                Debug.Log("No Axes");

            for (int i = 0; i < axisArray.arraySize; ++i)
            {
                SerializedProperty inputConfig = axisArray.GetArrayElementAtIndex(i);
                Gaze_InputConfig conf = new Gaze_InputConfig(inputConfig);
                inputConfigs.Add(conf);
            }

            return inputConfigs;
        }

        /// <summary>
        /// Checks if the object is binary (Unity3D format) or in YAML.
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public static bool IsInBinaryFormat(string _path)
        {
            string[] lines = File.ReadAllLines(_path);
            if (lines.Length == 0)
                return true;

            if (lines.Contains("%YAML 1.1"))
                return false;

            return true;
        }
    }
}