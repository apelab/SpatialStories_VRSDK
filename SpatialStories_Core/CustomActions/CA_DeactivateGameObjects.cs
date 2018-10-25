using UnityEngine;
using Gaze;
using System;

public class CA_DeactivateGameObjects : Gaze_AbstractBehaviour
{
    public GameObject[] GameObjectsToDeactivate;

    public override void SetupUsingApi(GameObject _interaction)
    {
        throw new NotImplementedException();
    }

    protected override void OnTrigger()
    {
        foreach(GameObject go in GameObjectsToDeactivate)
        {
            if (go != null)
                go.SetActive(false);
        }

    }
}
