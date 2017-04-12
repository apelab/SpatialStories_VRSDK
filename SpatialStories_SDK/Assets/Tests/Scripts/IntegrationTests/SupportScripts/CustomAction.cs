using UnityTest;
using Gaze;
using System;

public class CustomAction : Gaze_AbstractBehaviour {


    public TestCustomAction Test;

    protected override void onActive()
    {
    }

    protected override void onAfter()
    {
    }

    protected override void onBefore()
    {
    }

    protected override void onReload()
    {
    }

    protected override void onTrigger()
    {
        Test.Pass();
    }
}
