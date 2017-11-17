using System.Collections;
using UnityEngine;


/// <summary>
/// This action moves this IO to the target transform position.
/// </summary>

namespace Gaze
{
    public class CA_MoveToTransform : Gaze_AbstractBehaviour
    {
        /// <summary>
        /// The target position to go to
        /// </summary>
        public Transform TargetTransform;

        /// <summary>
        /// The time to travel to the target destination
        /// </summary>
        public float AnimationTime = 1f;

        /// <summary>
        /// The number of rotations the IO will perform during its travel to the destination.
        /// </summary>
        public int numberOfFlips = 0;

        /// <summary>
        /// The height at mid-distance the IO will go when traveling to its destination.
        /// </summary>
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

        public IEnumerator LerpPosition()
        {

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