using Gaze;
using System.Collections;
using UnityEngine;

[AddComponentMenu("SpatialStories/Custom Actions/CA_ChangeScale")]


public class CA_ChangeScale : Gaze_AbstractBehaviour
{
    public bool Activate = true;
    public GameObject ToScale;

    public Vector3 TargetScale;
    public float TransitionTime;
    
    protected override void OnTrigger()
    {
        StartCoroutine(Reescale());
    }

    public IEnumerator Reescale()
    {
        float startTime = Time.time;
        float endTime = Time.time + TransitionTime;

        Vector3 initialScale = ToScale.transform.localScale;
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / TransitionTime;
            ToScale.transform.localScale = Vector3.Lerp(initialScale, TargetScale, t);
            yield return null;
        }
        ToScale.transform.localScale = TargetScale;
        yield return null;
    }

}
