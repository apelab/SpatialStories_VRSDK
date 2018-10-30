﻿using UnityEngine;
#if UNITY_IOS
using UnityEngine.XR.iOS;
#endif
using System.Collections.Generic;
using GoogleARCore;

public class StudioPlaneManager : MonoBehaviour 
{
    public static StudioPlaneManager Instance;
    private bool ArePlanesVisible = true;

    private List<GameObject> registeredPlanes = new List<GameObject>();
    private Dictionary<int, Renderer[]> registeredPlanesRenderers = new Dictionary<int, Renderer[]>();

    private void Awake()
    {
        Instance = this;

        //NOTE: Used to make sure that planes will not be visible if they shouldn't
#if UNITY_ANDROID
#elif UNITY_IOS
        UnityARGeneratePlane planeGenerator = FindObjectOfType<UnityARGeneratePlane>();
		if (planeGenerator != null && planeGenerator.planePrefab != null){
			StudioPlane plane = planeGenerator.planePrefab.GetComponent<StudioPlane>();
			if(plane == null)
				planeGenerator.planePrefab.AddComponent<StudioPlane>();
		}
#else

#endif
    }

    private void OnEnable()
    {
#if UNITY_ANDROID
#elif UNITY_IOS
        UnityARSessionNativeInterface.ARAnchorAddedEvent += PlanesUpdated;
		UnityARSessionNativeInterface.ARAnchorUpdatedEvent += PlanesUpdated;
		UnityARSessionNativeInterface.ARAnchorRemovedEvent += PlanesUpdated;
#else

#endif

    }

    private void OnDisable()
    {
#if UNITY_ANDROID
#elif UNITY_IOS
        UnityARSessionNativeInterface.ARAnchorAddedEvent -= PlanesUpdated;
        UnityARSessionNativeInterface.ARAnchorUpdatedEvent -= PlanesUpdated;
        UnityARSessionNativeInterface.ARAnchorRemovedEvent -= PlanesUpdated;
#else

#endif
    }

    private void PlanesUpdated()
    {
        UpdatePlanesVisibility();
    }

    public void UpdatePlanesVisibility()
    {
#if UNITY_ANDROID
        foreach(GameObject plane in registeredPlanes)
        {
            Renderer r = plane.GetComponent<Renderer>();
            DetectedPlane t = plane.GetComponent<DetectedPlane>();
            r.enabled = ArePlanesVisible;
            // t.enabled = ArePlanesVisible;
        }
#elif UNITY_IOS
        int counter = registeredPlanes.Count;
        for (int i = 0; i < counter; ++i)
        {
            Renderer[] renderers = registeredPlanesRenderers[registeredPlanes[i].GetInstanceID()];
            for (int j = 0; j < renderers.Length; ++j)
            {
               renderers[j].enabled = ArePlanesVisible;   
            }
        }
#else

#endif

    }

    public void SetPlanesVisibility(bool _active)
	{
        ArePlanesVisible = _active;
        UpdatePlanesVisibility();
    }

    public void RegisterPlane(GameObject _plane)
    {
        if (registeredPlanes.Contains(_plane))
            return;
        
		registeredPlanes.Add(_plane);
#if UNITY_ANDROID
#elif UNITY_IOS
        registeredPlanesRenderers.Add(_plane.GetInstanceID(), _plane.GetComponentsInChildren<Renderer>());
#else
#endif

        UpdatePlanesVisibility();
    }

    public void UnRegisterPlane(GameObject _plane)
    {
#if UNITY_ANDROID
#elif UNITY_IOS
        registeredPlanes.Remove(_plane);
#else

#endif
        registeredPlanesRenderers.Remove(_plane.GetInstanceID());
    }
}
