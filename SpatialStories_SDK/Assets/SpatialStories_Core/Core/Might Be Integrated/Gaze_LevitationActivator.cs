//-----------------------------------------------------------------------
// <copyright file="LevitationEventArgs.cs" company="apelab sàrl">
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
using UnityEngine;
using Gaze;

namespace Gaze
{
    public class Gaze_LevitationActivator : Gaze_AbstractBehaviour
    {
        public bool isManager = false;
        public LevitationState levitationState;

        public enum LevitationState
        {
            ACTIVATE,
            DEACTIVATE
        }

        private Gaze_Levitable levitable;
        private Gaze_LevitationManager levitationManager;

        void Start()
        {
            if (!isManager)
            {
                levitable = GetComponentInParent<Gaze_InteractiveObject>().GetComponentInChildren<Gaze_Levitable>();

                if (!levitable)
                    throw new MissingComponentException("This (IO) must have a Levitation GameObject as a child with Levitable component attached !");

                levitable.enabled = false;

            }
            else
            {
                levitationManager = GetComponentInParent<Gaze_LevitationManager>();

                if (!levitationManager)
                    throw new MissingComponentException("This (IO) must have a LevitationManager component attached !");

                //GetComponentInParent<Gaze_LevitationManager>().active = false;
            }
        }

        #region implemented abstract members of Gaze_AbstractBehaviour

        protected override void onTrigger()
        {
            if (isManager)
            {
                GetComponentInParent<Gaze_LevitationManager>().active = levitationState.Equals(LevitationState.ACTIVATE) ? true : false;

            }
            else
                levitable.enabled = levitationState.Equals(LevitationState.ACTIVATE) ? true : false;
        }

        protected override void onReload()
        {
        }

        protected override void onBefore()
        {
        }

        protected override void onActive()
        {
        }

        protected override void onAfter()
        {
        }

        #endregion
    }
}