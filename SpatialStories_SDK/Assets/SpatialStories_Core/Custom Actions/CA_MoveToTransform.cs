using System;
using UnityEngine;
using System.Collections;

namespace Gaze
{
    public class CA_MoveToTransform : Gaze_AbstractBehaviour
    {
        public Transform TargetTransform;
        public float AnimationTime = 1f;
        public int numberOfFlips = 0;
        public float ParabolicHeight;

        private Gaze_InteractiveObject io;

        private void Awake()
        {
            io = GetComponentInParent<Gaze_InteractiveObject>();
        }

        protected override void OnTrigger()
        {
            StartCoroutine(LerpPosition());
        }

        public IEnumerator LerpPosition(){

            Vector3 startPosition = io.transform.position;
			Vector3 startRotation = io.transform.rotation.eulerAngles;
            Vector3 endRotation = io.transform.rotation.eulerAngles + Vector3.forward * 360 * numberOfFlips;

            float startTime = Time.time;
            float endTime = Time.time + AnimationTime;

            float remainingTime = endTime - startTime;

			while (remainingTime > 0)
			{
                remainingTime = endTime - Time.time;
                float t = 1 - remainingTime / AnimationTime;
                var height = Mathf.Sin(Mathf.PI * t) * ParabolicHeight;
                io.transform.position = Vector3.Lerp(startPosition, TargetTransform.position, t) + Vector3.up * height;
                io.transform.rotation = Quaternion.Euler(Vector3.Lerp(startRotation, endRotation, t));
				yield return null;
			}
			
            io.transform.position = TargetTransform.position;
            io.transform.rotation = Quaternion.Euler(endRotation);
	   }

        protected override void OnReload()
        {
        }

        protected override void OnBefore()
        {
        }

        protected override void OnActive()
        {
        }

        protected override void OnAfter()
        {
        }
    }   
}