using Gaze;

public class TriggerCounterAction : Gaze_AbstractBehaviour {

    public TriggerCounterAbstractTest Test;

    protected override void onTrigger()
    {
        Test.UpdateTriggerCounter();
    }

    protected override void onActive(){}
    protected override void onAfter(){}
    protected override void onBefore(){}
    protected override void onReload(){}
}
