using UnityEngine;
using Gaze;
using System;

public class CA_ActivateGameObject : Gaze_AbstractBehaviour 
{
	public GameObject GameObjectToActivate;

    public override void SetupUsingApi(GameObject _interaction)
    {
        throw new NotImplementedException();
    }

    protected override void OnTrigger()
	{
        if(GameObjectToActivate != null)
		    GameObjectToActivate.SetActive(true);
	}
}
