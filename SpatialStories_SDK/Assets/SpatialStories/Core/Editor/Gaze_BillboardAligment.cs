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
	
	//	[MenuItem("Gaze/Billboard Alignment/Look at Camera")]
		public static void lookAtCamera ()
		{
			foreach (Transform t in Selection.transforms) {
				Undo.RecordObject (t, "Billboard Alignment");
				t.LookAt (TargetCamera ());
			}
		}
		
		//[MenuItem("Gaze/Billboard Alignment/Look away from Camera")]
		public static void lookAwayFromCamera ()
		{
			foreach (Transform t in Selection.transforms) {
				Undo.RecordObject (t, "Billboard Alignment");
				t.rotation = Quaternion.LookRotation (t.position - TargetCamera ().position);
			}
		}
		
		//[MenuItem("Gaze/Billboard Alignment/Look at Camera", true)]
	//	[MenuItem("Gaze/Billboard Alignment/Look away from Camera", true)]
		public static bool CanAlign ()
		{
			return Selection.transforms.Length > 0 && TargetCamera () != null;
		}

	}
}