// <copyright file="Gaze_Interaction.cs" company="apelab sàrl">
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
using System;
using UnityEngine;

namespace Gaze
{
    /// <summary>
    /// Gazable
    /// This component should be attached to any GameObject who may be gazed.
    /// </summary>
    [Serializable]
    [ExecuteInEditMode]
    public class Gaze_Interaction : MonoBehaviour
    {
        #region Members
        public bool HasActions = false;
        public bool HasConditions = false;
        #endregion

        public void AddActions()
        {
            HasActions = true;
            // display Activation component
            if (!GetComponent<Gaze_Actions>())
            {
                // add the component
                gameObject.AddComponent<Gaze_Actions>();
            }

            // tell the script it's active
            gameObject.GetComponent<Gaze_Actions>().isActive = true;

            AddDeactivatedConditionsIfNeeded();
        }

        /// <summary>
        /// This method is used when the user adds actions but without addding conditions
        /// right now gaze_sdk neeeds that the object has conditions in order to trigger
        /// an action, thats why we add condtions and the we deactivate them is needed.
        /// </summary>
        public void AddDeactivatedConditionsIfNeeded()
        {
            if (GetComponent<Gaze_Conditions>() == null)
            {
                AddConditions();
                RemoveConditions();
            }
        }

        public void RemoveActions()
        {
            HasActions = false;
            // remove Activation component
            if (gameObject.GetComponent<Gaze_Actions>())
            {
                // tell the script to deactive itself
                gameObject.GetComponent<Gaze_Actions>().isActive = false;
            }
        }

        public void AddConditions()
        {
            HasConditions = true;
            // display Conditions component
            if (!gameObject.GetComponent<Gaze_Conditions>())
            {
                gameObject.AddComponent<Gaze_Conditions>();
            }

            // tell the script it's active
            gameObject.GetComponent<Gaze_Conditions>().isActive = true;
        }

        public void RemoveConditions()
        {
            HasConditions = false;
            // remove Activation component
            if (gameObject.GetComponent<Gaze_Conditions>())
            {
                // tell the script to deactive itself
                gameObject.GetComponent<Gaze_Conditions>().isActive = false;
            }
        }
    }
}