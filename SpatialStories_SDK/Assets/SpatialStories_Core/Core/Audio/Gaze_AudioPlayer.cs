// <copyright file="Gaze_AudioPlayer.cs" company="apelab sàrl">
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
using System.Collections.Generic;

namespace Gaze
{
	[RequireComponent (typeof(AudioSource))]
	public class Gaze_AudioPlayer : MonoBehaviour
	{
		public List<AudioClip> clips;
		public bool ducking = true;
		public float audioVolumeMin = .2f;
		public float audioVolumeMax = 1f;
		public float fadeSpeed = .15f;
		private bool audioPlaying;
		private bool uninterrupted = false;
		private AudioSource audioSource;
		private int clipIndex;
		
		void OnEnable ()
		{
			Gaze_EventManager.OnGazeEvent += onGazeEvent;
		}
		
		void OnDisable ()
		{
			Gaze_EventManager.OnGazeEvent -= onGazeEvent;
		}
		
		void Awake ()
		{
			audioSource = GetComponent<AudioSource> ();
			audioSource.volume = ducking ? audioVolumeMin : audioVolumeMax;
			audioPlaying = false;
			clipIndex = 0;
		}
		
		private IEnumerator playAudioCoroutine ()
		{
			audioPlaying = true;
			audioSource.Play ();
			yield return new WaitForSeconds (audioSource.clip.length);
			audioPlaying = false;
			uninterrupted = false;
		}
		
		public void playAudioTrack (int _trackNumber, bool _loop, bool _priority)
		{
			if ((!_priority && !audioPlaying) || _priority) {
				clipIndex = _trackNumber;
				audioSource.clip = clips [clipIndex];
				audioSource.loop = _loop;
				StartCoroutine (playAudioCoroutine ());
			}
		}
		
		public void playAudioTrack (int _trackNumber, bool _loop)
		{
			playAudioTrack (_trackNumber, _loop, false);
		}
		
		public void playAudioTrack (int _trackNumber)
		{
			playAudioTrack (_trackNumber, false, false);
		}
		
		public void stopAllAndPlayUninterupted (int _index)
		{
			audioSource.Stop ();
			audioSource.clip = clips [_index];
			audioSource.loop = false;
			uninterrupted = true;
			StartCoroutine (playAudioCoroutine ());
		}
		
		public void stopAudio ()
		{
			if (!uninterrupted) {
				audioSource.Stop ();
				audioPlaying = false;
			}
		}
		
		private IEnumerator lowerAudioVolume ()
		{
			while (audioSource.volume > audioVolumeMin) {
				audioSource.volume = Mathf.Min (audioSource.volume - fadeSpeed, audioVolumeMin);
				yield return new WaitForSeconds (.1f);
			}
		}
		
		private IEnumerator raiseAudioVolume ()
		{
			while (audioSource.volume < audioVolumeMax) {
				audioSource.volume = Mathf.Max (audioSource.volume + fadeSpeed, audioVolumeMax);
				yield return new WaitForSeconds (.1f);
			}
		}
		
		private void onGazeEvent (Gaze_GazeEventArgs e)
		{
			// if this gameobject is gazed
			if ((GameObject)e.Sender == this.gameObject && ducking) {
				Debug.Log ("gazingh");
				if (e.IsGazed) {
					StartCoroutine (raiseAudioVolume ());
				} else {
					StartCoroutine (lowerAudioVolume ());
				}
			}
		}
	}
}