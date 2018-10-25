using UnityEngine;
using Gaze;
using System;

public class CA_WebLink : Gaze_AbstractBehaviour 
{
    public String WebLink;

    public override void SetupUsingApi(GameObject _interaction)
    {
        throw new NotImplementedException();
    }

    protected override void OnTrigger()
	{
        Debug.Log(WebLink);
        Application.OpenURL(WebLink);
	}
}
