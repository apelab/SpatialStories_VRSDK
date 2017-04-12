using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Gaze
{
	public class Gaze_HierarchyFilterEditor : MonoBehaviour
	{
		static List<GameObject> AllObjects = new List<GameObject> ();
		static List<GameObject> GazeObjects = new List<GameObject> ();
		static bool filter = false;

		private static void getAllObjects ()
		{
			AllObjects.Clear ();
			AllObjects.AddRange (Object.FindObjectsOfType (typeof(GameObject)) as GameObject[]);
		}

		private static void getGazeObjects ()
		{
			GazeObjects.Clear ();

			List<Gaze_Conditions> gs = new List<Gaze_Conditions> ();
			gs.AddRange (Object.FindObjectsOfType (typeof(Gaze_Conditions)) as Gaze_Conditions[]);

			foreach (Gaze_Conditions g in gs) {
				addGameObjectAndParents (GazeObjects, g.gameObject);
			}
		}

		private static void addGameObjectAndParents (List<GameObject> l, GameObject go)
		{
			l.Add (go);

			if (go.transform.parent != null) {
				Transform t = go.transform.parent;

				while (t != null && !l.Contains(t.gameObject)) {
					l.Add (t.gameObject);
					t = t.parent;
				}
			}
		}

		[MenuItem("Gaze/Filter Hierarchy/Filter Gaze Objects")]
		public static void filterGazeObjects ()
		{
			filter = true;
			getAllObjects ();
			getGazeObjects ();

			foreach (GameObject go in AllObjects) {
				if (!GazeObjects.Contains (go)) {
					go.hideFlags = HideFlags.HideInHierarchy;
				}
			}
		}

		[MenuItem("Gaze/Filter Hierarchy/Clear Filter")]
		public 	static void clearFilter ()
		{
			filter = false;
			getAllObjects ();
			foreach (Object o in AllObjects) {
				if (o.hideFlags != HideFlags.HideAndDontSave &&
					o.hideFlags != HideFlags.DontSave && o.hideFlags != HideFlags.NotEditable)
					o.hideFlags = o.hideFlags & ~HideFlags.HideInHierarchy;
			}
		}
		
		[MenuItem("Gaze/Filter Hierarchy/Filter Gaze Objects", true)]
		public static bool CanFilterGazeObjects ()
		{
			return filter == false;
		}
		
		[MenuItem("Gaze/Filter Hierarchy/Clear Filter", true)]
		public static bool CanClearFilter ()
		{
			return filter == true;
		}

	}

}