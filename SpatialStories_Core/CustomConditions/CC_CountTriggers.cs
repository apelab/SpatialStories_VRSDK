using UnityEngine;
using Gaze;

public class CC_CountTriggers : Gaze_AbstractConditions
{
    public Gaze_Conditions Interaction;
    public int NumTimes = 1;

    private void OnEnable()
    {
        Gaze_EventManager.OnTriggerEvent += Gaze_EventManager_OnTriggerEvent;
    }

    private void Gaze_EventManager_OnTriggerEvent(Gaze_TriggerEventArgs e)
    {
        if(e.IsTrigger && ((GameObject)e.Sender).GetInstanceID().Equals(Interaction.gameObject.GetInstanceID()))
        {
            NumTimes--;

            if (NumTimes == 0)
                ValidateCustomCondition(true);
        }
    }
}
