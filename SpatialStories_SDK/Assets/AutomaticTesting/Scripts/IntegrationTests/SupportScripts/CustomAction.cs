using UnityTest;
using Gaze;
using System;

public class CustomAction : Gaze_AbstractBehaviour {


    public TestCustomAction Test;

    protected override void OnActive()
    {
    }

    protected override void OnAfter()
    {
    }

    protected override void OnBefore()
    {
    }

    protected override void OnReload()
    {
    }

    protected override void OnTrigger()
    {
        Test.Pass();
    }
}
