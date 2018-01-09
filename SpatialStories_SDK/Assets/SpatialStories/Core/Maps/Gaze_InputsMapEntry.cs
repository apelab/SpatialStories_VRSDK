// <copyright file="Gaze_InputsMapEntry.cs" company="apelab sàrl">
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
    private Gaze_InputTypes inputType;
    public Gaze_InputTypes InputType
    {
        set
        {
            inputType = value;
            IsRelease = inputType.ToString().ToLower().Contains("up") || inputType.ToString().ToLower().Contains("release");
            
        }
        get
        {
            return inputType;
        }
    }

    public bool IsRelease = false;

    // has the input been used by the user
    public bool valid;

    public Gaze_InputsMapEntry()
    {
        // assign a default value for the input if none specified at construction
        InputType = Gaze_InputTypes.A_BUTTON;
        valid = false;
    }

    public Gaze_InputsMapEntry(Gaze_InputTypes _type)
    {
        InputType = _type;
    }

    public Gaze_InputsMapEntry(Gaze_InputTypes _type, bool _valid)
    {
        InputType = _type;
        valid = _valid;
    }
}