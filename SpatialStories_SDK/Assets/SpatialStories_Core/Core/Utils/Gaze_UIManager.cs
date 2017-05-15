// <copyright file="Gaze_UIManager.cs" company="apelab sàrl">
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
using System.Collections;
using System;

namespace Gaze
{
	[Serializable]
	public class Gaze_UIManager : MonoBehaviour
	{
		private Canvas canvas;
		private Gaze_CameraSwitcher cameraSwitcher;

		[HideInInspector]
		public Canvas userCanvasPrefab;

		void Start ()
		{
			// locate Camera Switcher script
			cameraSwitcher = transform.parent.GetComponent<Gaze_CameraSwitcher> ();

			if (cameraSwitcher == null) {
				Debug.LogError ("Camera Manager not found.");
				this.enabled = false;
				return;
			}

			if (cameraSwitcher.enableUI) {
				// locate user UI prefab
				userCanvasPrefab = cameraSwitcher.userCanvasPrefab;

				if (userCanvasPrefab == null) {
					Debug.LogError ("No UI Canvas to instantiate.");
					this.enabled = false;
					return;
				}

				// instantiate the target prefab
				canvas = Instantiate (userCanvasPrefab) as Canvas;

				// locate target Camera in Camera Manager
				Camera targetCamera = transform.parent.GetComponent<Gaze_CameraRaycaster> ().gazeCamera;

				if (targetCamera == null) {
					Debug.LogError ("Camera not found in raycaster.");
					this.enabled = false;
					return;
				}

				// output canvas to the correct camera
				canvas.worldCamera = targetCamera;
				canvas.transform.SetParent (targetCamera.transform, false);
				canvas.planeDistance = 1f;
			}
		}
	}

}