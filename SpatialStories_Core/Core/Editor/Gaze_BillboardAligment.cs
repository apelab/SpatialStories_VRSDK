using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Gaze
{
	public class Gaze_BillboardAligment : MonoBehaviour
	{
		private static Transform targetCamera;

		public static Transform TargetCamera ()
		{
			if (targetCamera != null) {
				return targetCamera;
			}

			if (Gaze_CameraSwitcher.Instance != null) {
				targetCamera = Gaze_CameraSwitcher.Instance.transform;
			} else if (Camera.main != null) {
				targetCamera = Camera.main.transform;
			}

			return targetCamera;
		}
	
		public static void lookAtCamera ()
		{
			foreach (Transform t in Selection.transforms) {
				Undo.RecordObject (t, "Billboard Alignment");
				t.LookAt (TargetCamera ());
			}
		}
		
		public static void lookAwayFromCamera ()
		{
			foreach (Transform t in Selection.transforms) {
				Undo.RecordObject (t, "Billboard Alignment");
				t.rotation = Quaternion.LookRotation (t.position - TargetCamera ().position);
			}
		}
		
		public static bool CanAlign ()
		{
			return Selection.transforms.Length > 0 && TargetCamera () != null;
		}

	}
}