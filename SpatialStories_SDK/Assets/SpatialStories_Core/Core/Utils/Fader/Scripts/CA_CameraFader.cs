// <copyright file="Gaze_CameraFader.cs" company="apelab sàrl">
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

namespace Gaze
{
    [AddComponentMenu("SpatialStories/Custom Actions/CA_CameraFader")]
    public class CA_CameraFader : Gaze_AbstractBehaviour
    {
        public enum FadeType { FADE_IN, FADE_OUT }
        public FadeType fadeType = FadeType.FADE_IN;
        public float Duration = 2.0f;
        public Material material;
        AbstractFadeSystem fader;

        private void Awake()
        {
            //attach the apelab_ScreenFadePostProcess script to the camera
            GameObject cam = FindObjectOfType<Gaze_Camera>().gameObject;
            if (fadeType == FadeType.FADE_IN)
            {
                fader = cam.AddComponent<FadeInSystem>();
            }
            else
            {
                fader = cam.AddComponent<FadeOutSystem>();
            }

            fader.FadeTime = Duration;
            fader.FadeMaterial = material;
        }

        protected override void OnTrigger()
        {
            if (fadeType == FadeType.FADE_OUT)
                fader.Begin();
        }
    }
}
