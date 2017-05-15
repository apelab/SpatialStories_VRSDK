// <copyright file="Gaze_CameraSwitcher.cs" company="apelab sàrl">
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
	public class Gaze_CameraSwitcher : MonoBehaviour
	{
		private static Gaze_CameraSwitcher instance;

		public static Gaze_CameraSwitcher Instance {
			get {
				if (instance == null) {
					instance = FindObjectOfType<Gaze_CameraSwitcher> ();
				}
				return instance;
			}
		}

		/// <summary>
		/// Currently selected camera type index as defined in Gaze_CameraTypes
		/// </summary>
		public int cameraTypeIndex;

		public Gaze_CameraType CameraType {
			get {
				return (Gaze_CameraType)cameraTypeIndex;
			}
		}

		public bool enableUI;
		public Canvas userCanvasPrefab;
		public Transform camerasNode;
		public Transform uiManagerNode;
		public Transform previewCamera;
		public Camera activeCamera;

		void Awake ()
		{
			camerasNode = transform.GetChild (0);
			uiManagerNode = transform.GetChild (1);
			previewCamera = transform.GetChild (2);
			updateCameras ();
		}

		void Start ()
		{
			camerasNode.gameObject.SetActive (true);
			uiManagerNode.gameObject.SetActive (enableUI);
			previewCamera.gameObject.SetActive (false);
		}
		
		private void updateCameras ()
		{
			// activate / deactivate cameras accordingly
			for (int i=0; i<camerasNode.childCount; i++) {
				camerasNode.GetChild (i).gameObject.SetActive (cameraTypeIndex.Equals (i));
			}

			activeCamera = camerasNode.GetChild (cameraTypeIndex).GetComponentInChildren<Camera> ();

			if (((Gaze_CameraType)cameraTypeIndex).Equals (Gaze.Gaze_CameraType.STEAM_VR)) {
				// use last camera for Steam VR and Cardboard
				Camera[] cms = camerasNode.GetChild (cameraTypeIndex).GetComponentsInChildren<Camera> ();
				activeCamera = cms [cms.Length - 1];

			} else if (((Gaze_CameraType)cameraTypeIndex).Equals (Gaze.Gaze_CameraType.CARDBOARD)) {
				// use last camera for Steam VR and Cardboard
				Camera[] cms = camerasNode.GetChild (cameraTypeIndex).GetComponentsInChildren<Camera> ();
				activeCamera = cms [0];
			}

			GetComponent<Gaze_CameraRaycaster> ().SetCamera (activeCamera);
		}

		void OnApplicationQuit ()
		{
			camerasNode.gameObject.SetActive (false);
			uiManagerNode.gameObject.SetActive (false);
			previewCamera.gameObject.SetActive (true);
		}
	}
}