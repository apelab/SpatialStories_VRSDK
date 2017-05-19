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

namespace Gaze
{
    public class Gaze_Levitable : MonoBehaviour
    {
        public GameObject visuals;
        public Transform root;
        public bool checkEnabled = false;

        void OnEnable()
        {
            Gaze_EventManager.OnGazeEvent += GazeEvent;
            checkEnabled = true;
            CheckItsAlreadyGazed();
        }

        void OnDisable()
        {
            Gaze_EventManager.OnGazeEvent -= GazeEvent;
        }

        void Update()
        {
            if (checkEnabled)
                CheckItsAlreadyGazed();

            if (root)
            {
                FollowRoot();
            }
        }

        private void FollowRoot()
        {
            transform.position = root.transform.position;
            transform.rotation = root.transform.rotation;
        }

        private void GazeEvent(Gaze_GazeEventArgs e)
        {
            // if I'm gazed
            if (e.Sender != null && ((GameObject)e.Sender).GetComponentInParent<Gaze_InteractiveObject>() && ((GameObject)e.Sender).GetComponentInParent<Gaze_InteractiveObject>().Equals(GetComponentInParent<Gaze_InteractiveObject>()))
            {
                if (e.IsGazed)
                {
                    Gaze_EventManager.FireLevitationEvent(new Gaze_LevitationEventArgs(this, visuals, Gaze_LevitationTypes.GAZED, Gaze_HandsEnum.BOTH));
                }
                else
                {
                    Gaze_EventManager.FireLevitationEvent(new Gaze_LevitationEventArgs(this, visuals, Gaze_LevitationTypes.UNGAZED, Gaze_HandsEnum.BOTH));
                }
            }
        }
        private void CheckItsAlreadyGazed()
        {
            if (visuals.GetComponentInParent<Gaze_InteractiveObject>().GetComponentInChildren<Gaze_Conditions>().IsGazed)
            {
                Gaze_EventManager.FireLevitationEvent(new Gaze_LevitationEventArgs(this, visuals, Gaze_LevitationTypes.GAZED, Gaze_HandsEnum.BOTH));
                checkEnabled = false;
            }
            else
            {
                Gaze_EventManager.FireLevitationEvent(new Gaze_LevitationEventArgs(this, visuals, Gaze_LevitationTypes.UNGAZED, Gaze_HandsEnum.BOTH));
            }

        }

    }
}