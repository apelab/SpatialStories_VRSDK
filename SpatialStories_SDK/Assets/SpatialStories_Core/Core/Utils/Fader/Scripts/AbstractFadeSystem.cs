using UnityEngine;

public class AbstractFadeSystem : MonoBehaviour {

    public float FadeTime = 2.0f;
    public Material FadeMaterial;
    protected YieldInstruction waitInstruction = new WaitForEndOfFrame();

    protected Color InitialFadeColor;
    protected bool HasFinnished = false;
    protected bool isFading = false;

    public virtual void Begin() { }
}
