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

[System.Serializable]
public class Gaze_InputsMapEntry
{
    // input name
    public Gaze_InputTypes inputType;

    // has the input been used by the user
    public bool valid;

    public Gaze_InputsMapEntry()
    {
        // assign a default value for the input if none specified at construction
        inputType = Gaze_InputTypes.A_BUTTON;
        valid = false;
    }

    public Gaze_InputsMapEntry(Gaze_InputTypes _type)
    {
        inputType = _type;
    }

    public Gaze_InputsMapEntry(Gaze_InputTypes _type, bool _valid)
    {
        inputType = _type;
        valid = _valid;
    }
}