using System.Collections;
using UnityEngine;

public class FadeOutSystem : AbstractFadeSystem
{
    public override void Begin()
    {
        InitialFadeColor = new Color(0f, 0f, 0f, 0.0f);
        StartCoroutine(FadeOutCourroutine());
    }

    IEnumerator FadeOutCourroutine()
    {
        isFading = true;
        float elapsedTime = 0.0f;
        FadeMaterial.color = InitialFadeColor;
        Color color = InitialFadeColor;
        while (elapsedTime < FadeTime)
        {
            yield return waitInstruction;
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / FadeTime);
            FadeMaterial.color = color;
        }
        HasFinnished = true;
        yield return null;
    }


    void OnPostRender()
    {
        if (!isFading)
            return;

        FadeMaterial.SetPass(0);
        GL.PushMatrix();
        GL.LoadOrtho();
        GL.Color(FadeMaterial.color);
        GL.Begin(GL.QUADS);
        GL.Vertex3(0f, 0f, -12f);
        GL.Vertex3(0f, 1f, -12f);
        GL.Vertex3(1f, 1f, -12f);
        GL.Vertex3(1f, 0f, -12f);
        GL.End();
        GL.PopMatrix();
    }
}
