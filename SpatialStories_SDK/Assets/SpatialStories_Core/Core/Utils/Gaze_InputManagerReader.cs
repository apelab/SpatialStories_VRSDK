// <copyright file="Gaze_InputManagerReader.cs" company="apelab sàrl">
// © apelab. All Rights Reserved.
//
// This source is subject to the apelab license.
// All other rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// </copyright>
// <author>Michaël Martin</author>
// <email>dev@apelab.ch</email>
// <web>https://twitter.com/apelab_ch</web>
// <web>http://www.apelab.ch</web>
// <date>2014-06-01</date>
using Gaze;
using UnityEditor;

public class Gaze_InputManagerReader
{
    [MenuItem("Assets/Repair Input Manager")]
    public static void RepairInputsIfNeeded()
    {
        if (!Gaze_InputManagerChecker.IsInputManagerAssetIsInstaled())
        {
            Gaze_InputManagerChecker.AddMissingInputsToPoject();
        }
        else
        {
            EditorUtility.DisplayDialog("Spatial Stories SDK", "InputManager.asset is OK, nothing to change.", "Ok");
        }
    }

}