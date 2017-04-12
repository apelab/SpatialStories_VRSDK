using Gaze;

public class TeleportCustomCondition : Gaze_AbstractConditions
{

    void OnEnable()
    {
        Gaze_EventManager.OnTeleportEvent += OnTeleportEvent;
    }

    void OnDisable()
    {
        Gaze_EventManager.OnTeleportEvent -= OnTeleportEvent;
    }

    private void OnTeleportEvent(Gaze_TeleportEventArgs e)
    {
        ValidateCustomCondition(true);
    }
}