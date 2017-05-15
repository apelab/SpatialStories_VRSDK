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
using System.Collections;
using Gaze;

namespace Gaze
{
	public class Gaze_CameraFader : MonoBehaviour
	{
		public GameObject box;
		public bool StartTransparent = true;
		public Color fadeColor;
		public float speed = .01f;
		private Gaze_CameraSwitcher[] cameraSwitcherScripts;
		private Transform currentSceneCameraTransform;
		private float _alpha;
		private GameObject root;

		void Start ()
		{
			_alpha = StartTransparent ? 0f : 1f;
			box.GetComponent<Renderer> ().sharedMaterial.SetColor ("_Color", new Color (fadeColor.r, fadeColor.g, fadeColor.b, _alpha));
			box.GetComponent<Renderer> ().sortingOrder = 1000;

			cameraSwitcherScripts = Object.FindObjectsOfType (typeof(Gaze_CameraSwitcher)) as Gaze_CameraSwitcher[];
			foreach (Gaze_CameraSwitcher c in cameraSwitcherScripts) {
				currentSceneCameraTransform = c.transform;
			}

			root = this.gameObject.GetComponent<Gaze_Conditions> ().Root;
		}

		void Update ()
		{
			root.transform.position = currentSceneCameraTransform.position;
			root.transform.rotation = currentSceneCameraTransform.rotation;
		}

		public void Fade ()
		{
//			Debug.Log (this.gameObject.name + " StartTransparent = "+StartTransparent);
			StartCoroutine (FadeCoroutine ());
		}

		private IEnumerator FadeCoroutine ()
		{
			for (float i = 0f; i <= 1f; i += speed) {
				_alpha = this.StartTransparent ? i : 1-i;
				box.GetComponent<Renderer> ().sharedMaterial.SetColor ("_Color", new Color (fadeColor.r, fadeColor.g, fadeColor.b, _alpha));
				yield return new WaitForSeconds (.01f);
			}
			box.GetComponent<Renderer> ().sharedMaterial.SetColor ("_Color", new Color (fadeColor.r, fadeColor.g, fadeColor.b, StartTransparent ? 1f: 0f));
		}
	}
}
