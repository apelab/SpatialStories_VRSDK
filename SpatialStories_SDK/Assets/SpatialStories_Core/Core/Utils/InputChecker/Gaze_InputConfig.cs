using System;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaze
{
    /// <summary>
    /// Represents one input config stored on the InputManager.asset file of Unity3D.
    /// </summary>
    public class Gaze_InputConfig
    {
        public const int NUM_PARAMETERS = 16;

        public string axis;
        public string type;
        public string serializedVersion;
        public string m_Name;
        public string descriptiveName;
        public string descriptiveNegativeName;
        public string negativeButton;
        public string positiveButton;
        public string altNegativeButton;
        public string altPositiveButton;
        public string gravity;
        public string dead;
        public string sensitivity;
        public string snap;
        public string invert;
        public string joyNum;

        /// <summary>
        /// Creates a new Instance from a list all the lines that
        /// represents an object object on InputManager.asset
        /// </summary>
        /// <param name="_lines"></param>
        public Gaze_InputConfig(string[] _lines)
        {
            foreach (string line in _lines)
            {
                if (GetParamInLine(ref axis, Gaze_InputConfigConstants.NAME_AXIS, line)) continue;
                else if (GetParamInLine(ref type, Gaze_InputConfigConstants.NAME_TYPE, line)) continue;
                else if (GetParamInLine(ref serializedVersion, Gaze_InputConfigConstants.NAME_SERIALIZED_VERSION, line)) continue;
                else if (GetParamInLine(ref m_Name, Gaze_InputConfigConstants.NAME_NAME, line)) continue;
                else if (GetParamInLine(ref descriptiveName, Gaze_InputConfigConstants.NAME_DESCRIPTIVE_NAME, line)) continue;
                else if (GetParamInLine(ref descriptiveNegativeName, Gaze_InputConfigConstants.NAME_DESCRIPTIVE_NEGATIVE_NAME, line)) continue;
                else if (GetParamInLine(ref negativeButton, Gaze_InputConfigConstants.NAME_NEGATIVE_BUTTON, line)) continue;
                else if (GetParamInLine(ref positiveButton, Gaze_InputConfigConstants.NAME_POSITIVE_BUTTON, line)) continue;
                else if (GetParamInLine(ref altNegativeButton, Gaze_InputConfigConstants.NAME_ALT_NEGATIVE_BUTTON, line)) continue;
                else if (GetParamInLine(ref altPositiveButton, Gaze_InputConfigConstants.NAME_ALT_POSITIVE_BUTTON, line)) continue;
                else if (GetParamInLine(ref gravity, Gaze_InputConfigConstants.NAME_GRAVITY, line)) continue;
                else if (GetParamInLine(ref dead, Gaze_InputConfigConstants.NAME_DEAD, line)) continue;
                else if (GetParamInLine(ref sensitivity, Gaze_InputConfigConstants.NAME_SENSITIVITY, line)) continue;
                else if (GetParamInLine(ref snap, Gaze_InputConfigConstants.NAME_SNAP, line)) continue;
                else if (GetParamInLine(ref invert, Gaze_InputConfigConstants.NAME_INVERT, line)) continue;
                else if (GetParamInLine(ref joyNum, Gaze_InputConfigConstants.NAME_JOY_NUM, line)) continue;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Creates an input config from a binary object
        /// </summary>
        /// <param name="_inputConfig"></param>
        public Gaze_InputConfig(SerializedProperty _inputConfig)
        {
            axis = GetPropertyFromSerialized<int>(Gaze_InputConfigConstants.NAME_AXIS, _inputConfig);

            InputType newType = (InputType)_inputConfig.FindPropertyRelative(Gaze_InputConfigConstants.NAME_TYPE).intValue;
            type = ((int)newType).ToString();

            serializedVersion = GetPropertyFromSerialized<int>(Gaze_InputConfigConstants.NAME_SERIALIZED_VERSION, _inputConfig, "3");
            m_Name = GetPropertyFromSerialized<string>(Gaze_InputConfigConstants.NAME_NAME, _inputConfig);
            descriptiveName = GetPropertyFromSerialized<string>(Gaze_InputConfigConstants.NAME_DESCRIPTIVE_NAME, _inputConfig);
            descriptiveNegativeName = GetPropertyFromSerialized<string>(Gaze_InputConfigConstants.NAME_DESCRIPTIVE_NEGATIVE_NAME, _inputConfig);
            negativeButton = GetPropertyFromSerialized<string>(Gaze_InputConfigConstants.NAME_NEGATIVE_BUTTON, _inputConfig);
            positiveButton = GetPropertyFromSerialized<string>(Gaze_InputConfigConstants.NAME_POSITIVE_BUTTON, _inputConfig);
            altNegativeButton = GetPropertyFromSerialized<string>(Gaze_InputConfigConstants.NAME_ALT_NEGATIVE_BUTTON, _inputConfig);
            altPositiveButton = GetPropertyFromSerialized<string>(Gaze_InputConfigConstants.NAME_ALT_POSITIVE_BUTTON, _inputConfig);
            gravity = GetPropertyFromSerialized<float>(Gaze_InputConfigConstants.NAME_GRAVITY, _inputConfig);
            dead = GetPropertyFromSerialized<float>(Gaze_InputConfigConstants.NAME_DEAD, _inputConfig);
            sensitivity = GetPropertyFromSerialized<float>(Gaze_InputConfigConstants.NAME_SENSITIVITY, _inputConfig);
            snap = GetPropertyFromSerialized<float>(Gaze_InputConfigConstants.NAME_SNAP, _inputConfig);
            invert = GetPropertyFromSerialized<float>(Gaze_InputConfigConstants.NAME_INVERT, _inputConfig);
            joyNum = GetPropertyFromSerialized<float>(Gaze_InputConfigConstants.NAME_JOY_NUM, _inputConfig);
        }
#endif

        /// <summary>
        /// Gets a parameter of a line and modifies the attribute passed as a parameter.
        /// </summary>
        /// <param name="_attribute">A reference to an input config attribute</param>
        /// <param name="_param">The name of the parameter that we want to assing to the attribute</param>
        /// <param name="_line">The string containing the line extracted from the file</param>
        /// <returns></returns>
        public bool GetParamInLine(ref string _attribute, string _param, string _line)
        {
            Match match = Regex.Match(_line, @"(\s" + _param + ")", RegexOptions.ExplicitCapture);
            if (match.Success)
            {
                _attribute = _line.Split(':')[1].Trim();
                return true;
            }
            return false;
        }


#if UNITY_EDITOR
        /// <summary>
        /// Gets a property of a serialized object and returns its value or a default one
        /// </summary>
        /// <param name="_propName"></param>
        /// <param name="_inputConfig">The binary object</param>
        /// <param name="_nullValue">The default value that will be returned if the prop is null</param>
        /// <returns></returns>
        public string GetPropertyFromSerialized<T>(string _propName, SerializedProperty _inputConfig, string _nullValue = "")
        {
            SerializedProperty serProp = _inputConfig.FindPropertyRelative(_propName);
            try
            {
                if (typeof(T).IsAssignableFrom(typeof(int)))
                    return serProp != null ? serProp.intValue.ToString() : _nullValue;
                else if (typeof(T).IsAssignableFrom(typeof(float)))
                    return serProp != null ? serProp.floatValue.ToString() : _nullValue;
                else if (typeof(T).IsAssignableFrom(typeof(string)))
                    return serProp != null ? serProp.stringValue.ToString() : _nullValue;
                return "";
            }
            catch (Exception)
            {
                return _nullValue;
            }
        }
#endif

    }
}