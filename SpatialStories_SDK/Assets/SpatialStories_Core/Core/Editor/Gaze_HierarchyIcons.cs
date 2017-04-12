using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Gaze
{
	[InitializeOnLoad]
	class Gaze_HierarchyIcons
	{
		static Texture2D gazableTexture;
		static List<int> gazableObjects;

		static Gaze_HierarchyIcons ()
		{
			gazableTexture = AssetDatabase.LoadAssetAtPath ("Assets/Gaze SDK/Graphics/Gaze_Gazable/Gaze_Gazable_Hierarchy.png", typeof(Texture2D)) as Texture2D;
			EditorApplication.update += UpdateCallback;
			EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCallback;
		}

		static void UpdateCallback ()
		{
			GameObject[] go = Object.FindObjectsOfType (typeof(GameObject)) as GameObject[];

			gazableObjects = new List<int> ();
			foreach (GameObject g in go) {
				if (g.GetComponent<Gaze_Conditions> () != null) {
					gazableObjects.Add (g.GetInstanceID ());
				}
			}

		}

		static void HierarchyItemCallback (int instanceID, Rect selectionRect)
		{
			if (gazableObjects != null && gazableObjects.Contains (instanceID)) {
				// place the icoon to the right of the list
				Rect r = new Rect (selectionRect);
				r.x = r.width;
				r.width = 20;

				// Draw the texture
				GUI.Label (r, gazableTexture);
			}
		}
	}
}