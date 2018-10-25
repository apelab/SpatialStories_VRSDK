using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using System;

public class Gaze_CreatePlayPlaneAction : MonoBehaviour {

	private Mesh mesh;
	private List<Vector3> vertices;
	private bool m_planeCreated = false;

	public bool PlaneCreated {
		get{
			return m_planeCreated;
		}

		set{
			m_planeCreated = value;
			PlaySpaceStateChanged ();
		}
	}

	private bool allowGeneration = false;

	public event Action PlaySpaceStateChanged;

	public void playSpaceActionStart(){
		allowGeneration = true;
	}

	public void playSpaceActionDelete(){
		vertices = new List<Vector3> ();
		PlaneCreated = false;
		mesh = new Mesh ();
	}

	void Start(){
		mesh = GetComponent<MeshFilter> ().mesh;
		vertices = new List<Vector3> (4);
	}

	bool HitTestWithResultType (ARPoint point, ARHitTestResultType resultTypes)
	{
		List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface ().HitTest (point, resultTypes);
		if (hitResults.Count > 0) {
			foreach (var hitResult in hitResults) {
				if (vertices.Count == 0) {
					Debug.Log ("Adding first vertices");
					vertices.Add(UnityARMatrixOps.GetPosition (hitResult.worldTransform));

					mesh.SetVertices (vertices);
					mesh.SetIndices (new int[1]{0}, MeshTopology.Points, 0);
				}
				else if (vertices.Count == 1) {
					Debug.Log ("Adding second vertices");

					vertices.Add(UnityARMatrixOps.GetPosition (hitResult.worldTransform));

					mesh.SetVertices (vertices);
					mesh.SetIndices (new int[2]{0, 1}, MeshTopology.Lines, 0);
				}
				else if (vertices.Count == 2) {
					Debug.Log ("Adding third vertices");

					vertices.Add(UnityARMatrixOps.GetPosition (hitResult.worldTransform));

					mesh.SetVertices (vertices);
					mesh.SetIndices (new int[3]{0, 1, 2}, MeshTopology.Triangles, 0);
					mesh.uv = new Vector2[]{ new Vector2 (0, 0), new Vector2 (0, 1), new Vector2 (1, 1) };

				}
				else if (vertices.Count == 3) {
					Debug.Log ("Adding last vertices");

					vertices.Add(UnityARMatrixOps.GetPosition (hitResult.worldTransform));

					mesh.SetVertices (vertices);
					mesh.SetIndices (new int[6]{0, 1, 2, 0, 2, 3}, MeshTopology.Triangles, 0);
					mesh.uv = new Vector2[]{ new Vector2 (0, 0), new Vector2 (0, 1), new Vector2 (1, 1), new Vector2 (1, 0) };

					PlaneCreated = true;
				}
				return true;
			}
		}
		return false;
	}

	// Update is called once per frame
	void Update () {
		if (allowGeneration && !PlaneCreated && Input.touchCount > 0)
		{
			var touch = Input.GetTouch(0);
			if (/*touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved ||*/ touch.phase == TouchPhase.Ended)
			{
				var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
				ARPoint point = new ARPoint {
					x = screenPosition.x,
					y = screenPosition.y
				};

				// prioritize reults types
				ARHitTestResultType[] resultTypes = {
					ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent, 
					// if you want to use infinite planes use this:
					//ARHitTestResultType.ARHitTestResultTypeExistingPlane,
					ARHitTestResultType.ARHitTestResultTypeHorizontalPlane, 
					ARHitTestResultType.ARHitTestResultTypeFeaturePoint
				}; 

				foreach (ARHitTestResultType resultType in resultTypes)
				{
					if (HitTestWithResultType (point, resultType))
					{
						return;
					}
				}
			}
		}
	}
}
