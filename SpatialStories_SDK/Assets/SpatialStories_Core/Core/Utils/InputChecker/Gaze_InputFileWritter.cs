using System;
using System.Collections.Generic;
using System.IO;

namespace Gaze
{
    public static class Gaze_InputFileWritter
    {
        public static string WriteFile(List<Gaze_InputConfig> _configsToWrite, string _path, bool _backup = false)
        {
            // Create a backup file with a guid
            string backupPath = String.Concat(_path, "_", Guid.NewGuid(), ".backup");
            if (_backup)
                System.IO.File.Copy(_path, String.Concat(backupPath), true);

            string textToWrite = CreateYAMLHeader();

            foreach (Gaze_InputConfig config in _configsToWrite)
            {
                textToWrite = String.Concat(textToWrite, ConfigToYAML(config));
            }

            // Delete the input manager file
            if (File.Exists(_path))
            {
                File.SetLastAccessTimeUtc(_path, DateTime.Now);
            }

            using (StreamWriter outputFile = new StreamWriter(_path))
            {
                outputFile.Write(textToWrite);
            }

            return backupPath;
        }

        public static string CreateYAMLHeader()
        {
            string header = "";
            header = String.Concat(header, "%YAML 1.1", Environment.NewLine);
            header = String.Concat(header, "%TAG !u! tag:unity3d.com,2011:", Environment.NewLine);
            header = String.Concat(header, "--- !u!13 &1", Environment.NewLine);
            header = String.Concat(header, "InputManager:", Environment.NewLine);
            header = String.Concat(header, "  m_ObjectHideFlags: 0", Environment.NewLine);
            header = String.Concat(header, "  serializedVersion: 2", Environment.NewLine);
            header = String.Concat(header, "  m_Axes:", Environment.NewLine);
            return header;
        }

        /// <summary>
        /// Converts the InputConfig attribute sinto YAML.
        /// </summary>
        /// <returns>Formated text</returns>
        public static string PropToString(string _propName, string _propValue)
        {
            return String.Format("    {0}: {1}{2}", _propName, _propValue, Environment.NewLine);
        }

        /// <summary>
        /// Converts the Object into an string ready to write into a YAML FILE
        /// </summary>
        /// <returns></returns>
        public static string ConfigToYAML(Gaze_InputConfig _config)
        {
            string toYAML = "";
            toYAML += String.Format("  - {0}: {1}{2}", Gaze_InputConfigConstants.NAME_SERIALIZED_VERSION, _config.serializedVersion, Environment.NewLine);
            toYAML += PropToString(Gaze_InputConfigConstants.NAME_NAME, _config.m_Name);
            toYAML += PropToString(Gaze_InputConfigConstants.NAME_DESCRIPTIVE_NAME, _config.descriptiveName);
            toYAML += PropToString(Gaze_InputConfigConstants.NAME_DESCRIPTIVE_NEGATIVE_NAME, _config.descriptiveNegativeName);
            toYAML += PropToString(Gaze_InputConfigConstants.NAME_NEGATIVE_BUTTON, _config.negativeButton);
            toYAML += PropToString(Gaze_InputConfigConstants.NAME_POSITIVE_BUTTON, _config.positiveButton);
            toYAML += PropToString(Gaze_InputConfigConstants.NAME_ALT_NEGATIVE_BUTTON, _config.altNegativeButton);
            toYAML += PropToString(Gaze_InputConfigConstants.NAME_ALT_POSITIVE_BUTTON, _config.positiveButton);
            toYAML += PropToString(Gaze_InputConfigConstants.NAME_GRAVITY, _config.gravity);
            toYAML += PropToString(Gaze_InputConfigConstants.NAME_DEAD, _config.dead);
            toYAML += PropToString(Gaze_InputConfigConstants.NAME_SENSITIVITY, _config.sensitivity);
            toYAML += PropToString(Gaze_InputConfigConstants.NAME_SNAP, _config.snap);
            toYAML += PropToString(Gaze_InputConfigConstants.NAME_INVERT, _config.invert);
            toYAML += PropToString(Gaze_InputConfigConstants.NAME_TYPE, _config.type);
            toYAML += PropToString(Gaze_InputConfigConstants.NAME_AXIS, _config.axis);
            toYAML += PropToString(Gaze_InputConfigConstants.NAME_JOY_NUM, _config.joyNum);
            return toYAML;
        }
    }
}
