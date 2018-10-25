using UnityEngine;
using Gaze;
using System;

public class CA_ActivateGameObjects : Gaze_AbstractBehaviour 
{
	public GameObject[] GameObjectsToActivate;

    public override void SetupUsingApi(GameObject _interaction)
    {
        throw new NotImplementedException();
    }

    protected override void OnTrigger()
	{
        foreach (GameObject go in GameObjectsToActivate)
        {
            if (go != null)
                go.SetActive(true);
        }
    }
}
