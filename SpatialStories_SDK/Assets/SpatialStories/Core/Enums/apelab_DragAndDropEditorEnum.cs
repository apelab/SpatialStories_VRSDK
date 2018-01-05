//-----------------------------------------------------------------------
// <copyright file="apelab_DragAndDropEditorEnum.cs" company="apelab sàrl">
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
// <author>Michaël Martin</author>
// <email>dev@apelab.ch</email>
// <web>https://twitter.com/apelab_ch</web>
// <web>http://www.apelab.ch</web>
// <date>2016-01-25</date>
// </copyright>
//-----------------------------------------------------------------------
namespace Gaze
{
    public enum apelab_DnDStatesEditorEnum
    {
        Drop_Ready = Gaze_DragAndDropStates.DROPREADY,  // in place but not dropped yet
        Dropped = Gaze_DragAndDropStates.DROP,       // drop in the right place
        Drop_Canceled = Gaze_DragAndDropStates.DROPREADYCANCELED,  // in place but not dropped yet
        Removed = Gaze_DragAndDropStates.REMOVE      // pick up and removed from the target place
    }
}