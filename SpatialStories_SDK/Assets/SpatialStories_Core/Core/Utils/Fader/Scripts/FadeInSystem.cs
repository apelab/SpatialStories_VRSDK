using System.Collections;
using UnityEngine;

public class FadeInSystem : AbstractFadeSystem
{
    private void Start()
    {
        StartCoroutine(FadeInCorroutine());
    }

    IEnumerator FadeInCorroutine()
    {
        Camera camera = GetComponent<Camera>();
        float initialClipPlane = camera.farClipPlane;
        camera.farClipPlane = 0.1f;
        isFading = true;
        InitialFadeColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        float elapsedTime = 0.0f;
        FadeMaterial.color = InitialFadeColor;
        Color color = InitialFadeColor;
        while (elapsedTime < FadeTime)
        {
            yield return waitInstruction;
            elapsedTime += Time.deltaTime;
            float alpha = 1.0f - Mathf.Clamp01(elapsedTime / FadeTime);
            if(alpha < 0.8f)
            {
                camera.farClipPlane = initialClipPlane;
            }
            color.a = alpha;
            FadeMaterial.color = color;
        }
        isFading = false;
        HasFinnished = true;
        FadeMaterial.color = InitialFadeColor;
        yield return null;
    }

    private void Update()
    {
        if(HasFinnished)
            Destroy(this);
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

    private void OnApplicationQuit()
    {
        FadeMaterial.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    }


}
