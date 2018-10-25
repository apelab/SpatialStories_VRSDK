using UnityEngine;
using Gaze;
using System;

public class CA_DeactivateGameObject : Gaze_AbstractBehaviour
{
    public GameObject GameObjectToDeactivate;

    public override void SetupUsingApi(GameObject _interaction)
    {
        throw new NotImplementedException();
    }

    protected override void OnTrigger()
    {
        if (GameObjectToDeactivate != null)
            GameObjectToDeactivate.SetActive(false);
    }
}
