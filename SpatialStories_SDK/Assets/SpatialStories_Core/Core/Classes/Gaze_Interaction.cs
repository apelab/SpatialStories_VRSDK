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
        private bool hasActions = false;
        private bool hasConditions = false;

        public bool HasConditions
        {
            get { return hasConditions; }
            set
            {
                hasConditions = value;
                if (value)
                {
                    AddConditions();
                }
                else
                {
                    RemoveConditions();
                }
            }
        }

        public bool HasActions
        {
            get { return hasActions; }
            set
            {
                hasActions = value;
                if (value)
                {
                    AddActions();
                }
                else
                {
                    RemoveActions();
                }
            }
        }
        #endregion

        public void AddActions()
        {
            // display Activation component
            if (!GetComponent<Gaze_Actions>())
            {
                // add the component
                gameObject.AddComponent<Gaze_Actions>();
            }

            // tell the script it's active
            gameObject.GetComponent<Gaze_Actions>().isActive = true;
        }

        public void RemoveActions()
        {
            // remove Activation component
            if (gameObject.GetComponent<Gaze_Actions>())
            {
                // tell the script to deactive itself
                gameObject.GetComponent<Gaze_Actions>().isActive = false;
            }
        }

        public void AddConditions()
        {
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
            // remove Activation component
            if (gameObject.GetComponent<Gaze_Conditions>())
            {
                // tell the script to deactive itself
                gameObject.GetComponent<Gaze_Conditions>().isActive = false;
            }
        }
    }
}