using System.Collections;
using UnityEngine;


/// <summary>
/// This action moves this IO to random targets transform position.
/// </summary>

namespace Gaze
{
    public class CA_MoveToRandomTransform : Gaze_AbstractBehaviour
    {
        /// <summary>
        /// The target position to go to
        /// </summary>
		public GameObject[] targetObjects;
		private GameObject currentTarget;
		int index;

        /// <summary>
        /// The time to travel to the target destination
        /// </summary>
        public float AnimationTime = 1f;

        /// <summary>
        /// Does the object look at the destination point?
        /// </summary>
        public bool LookAtTarget = false;

        /// <summary>
        /// The number of rotations the IO will perform during its travel to the destination.
        /// </summary>
        public int numberOfFlips = 0;

        /// <summary>
        /// The height at mid-distance the IO will go when traveling to its destination.
        /// </summary>
        public float ParabolicHeight;

        private Gaze_InteractiveObject io;

		void Start() {
			
		}

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
			index = Random.Range(0, targetObjects.Length);
			currentTarget = targetObjects [index];
			//Debug.Log ("------------------------------------\nTrying to go to " + currentTarget.transform.position);
			Transform TargetTransform = currentTarget.transform;

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
                if (LookAtTarget)
                    io.gameObject.transform.LookAt(TargetTransform);

                yield return null;
            }

            io.transform.position = TargetTransform.position;
            io.transform.rotation = Quaternion.Euler(endRotation);
        }


        public override void SetupUsingApi(GameObject _interaction)
        {
           // throw new NotImplementedException();
        }
    }
}