using Gaze;
using UnityEngine;

public abstract class Gaze_SucceedOnTriggerTest : Gaze_AbstractTest
{
    protected bool isTriggerAllowed = false;
    public Gaze_Actions ActionToTrigger;

    private void OnEnable()
    {
        Gaze_EventManager.OnTriggerEvent += AutoTriggerTest;
    }

    private void OnDisable()
    {
        Gaze_EventManager.OnTriggerEvent -= AutoTriggerTest;
    }

    private void AutoTriggerTest(Gaze_TriggerEventArgs e)
    {
        if ((GameObject)e.Sender == ActionToTrigger.gameObject)
        {
            if (e.IsTrigger)
            {
                if (isTriggerAllowed)
                    PassTest();
                else
                    FailTest("Triggered before intended to be!");
            }
        }
    }
}
