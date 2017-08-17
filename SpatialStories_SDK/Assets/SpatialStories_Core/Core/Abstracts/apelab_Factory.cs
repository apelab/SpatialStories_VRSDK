// <copyright file="apelab_Factory.cs" company="apelab sàrl">
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

using UnityEngine;

public class apelab_Factory
{
    public static apelab_Factory Instance { get; private set; }

    private apelab_Factory()
    {
        Instance = this;
    }

    /// <summary>
    /// Creates a new InteractiveObject and returns it
    /// </summary>
    /// <returns></returns>
    public static GameObject CreateInteractiveObject()
    {
        return new GameObject();
    }

    /// <summary>
    /// Duplicates the given InteractiveObject and returns it.
    /// </summary>
    /// <param name="_objectToDuplicate"></param>
    /// <returns></returns>
    public static GameObject DuplicateInteractiveObject(GameObject _objectToDuplicate)
    {
        return new GameObject();
    }

    /// <summary>
    /// Adds an Interaction in the given InteractiveObject
    /// </summary>
    public static void AddInteraction(GameObject _InteractiveObject)
    {

    }

    /// <summary>
    /// Removes all interactions within an InteractiveObject GameObject
    /// </summary>
    /// <param name="_interactiveObjectGameObject"></param>
    public static void RemoveInteractions(GameObject _interactiveObjectGameObject) { }
}