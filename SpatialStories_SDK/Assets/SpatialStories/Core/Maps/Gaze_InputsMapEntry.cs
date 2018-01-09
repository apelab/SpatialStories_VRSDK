﻿// <copyright file="Gaze_InputsMapEntry.cs" company="apelab sàrl">
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

[System.Serializable]
public class Gaze_InputsMapEntry
{
    /// <summary>
    /// UI vars used to make the InputCondition Editor more friendly
    /// </summary>
    public Gaze_Controllers UISelectedPlatform;
    public int UIControllerSpecificInput;

    // input name

    public Gaze_InputTypes InputType;

    public bool IsRelease = false;

    // has the input been used by the user
    public bool Valid;

    public Gaze_InputsMapEntry()
    {
        // assign a default value for the input if none specified at construction
        InputType = Gaze_InputTypes.A_BUTTON;
        Valid = false;
    }

    public Gaze_InputsMapEntry(Gaze_InputTypes _type)
    {
        InputType = _type;
    }

    public Gaze_InputsMapEntry(Gaze_InputTypes _type, bool _valid)
    {
        InputType = _type;
        Valid = _valid;
    }

    public void CheckIfIsRelease()
    {
        string s = InputType.ToString().ToLower();
        IsRelease = s.Contains("up") || s.Contains("release") || s.Contains("untouch"); 
    }
}