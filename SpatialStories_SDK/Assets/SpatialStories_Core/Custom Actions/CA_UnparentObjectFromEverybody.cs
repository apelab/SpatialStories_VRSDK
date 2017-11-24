using Gaze;

/// <summary>
/// This action will unparent the given object from everybody.
/// As a result, the given object will be at the rool level of the hierarchy.
/// </summary>

public class CA_UnparentObjectFromEverybody : Gaze_AbstractBehaviour
{
    public Gaze_InteractiveObject ObjectToUnparent;

    protected override void OnTrigger()
    {
        ObjectToUnparent.transform.SetParent(null);
    }
}
