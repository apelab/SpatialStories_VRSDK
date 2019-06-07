using Gaze;
using UnityEngine.Playables;

public class CA_TimelinePlay : Gaze_AbstractBehaviour
{

    public PlayableDirector playableDirector;
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
        playableDirector.Play();
    }

}
