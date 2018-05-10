using System;
using Gaze;
using System.Collections.Generic;

public class CA_ReloadInteractions : Gaze_AbstractBehaviour
{
    public List<Gaze_Conditions> ConditionsToReload = new List<Gaze_Conditions>();

    protected override void OnTrigger()
    {
        foreach (Gaze_Conditions condition in ConditionsToReload)
            ReloadCondition(condition);
    }

    private static void ReloadCondition(Gaze_Conditions _conditionToReload)
    {
        _conditionToReload.ManualReload();
    }
}
