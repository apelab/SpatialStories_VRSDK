using Gaze;

public class TriggerCounterAction : Gaze_AbstractBehaviour {

    public TriggerCounterAbstractTest Test;

    protected override void OnTrigger()
    {
        Test.UpdateTriggerCounter();
    }

    protected override void OnActive(){}
    protected override void OnAfter(){}
    protected override void OnBefore(){}
    protected override void OnReload(){}
}
