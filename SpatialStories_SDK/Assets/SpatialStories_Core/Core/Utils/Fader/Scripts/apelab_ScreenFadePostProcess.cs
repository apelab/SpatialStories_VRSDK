// <copyright file="apelab_ScreenFadePostProcess.cs" company="apelab sàrl">
// © apelab. All Rights Reserved.
//
// This source is subject to the apelab license.
// All other rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// </copyright>
// <author>Michaël Martin</author>
// <email>dev@apelab.ch</email>
// <web>https://twitter.com/apelab_ch</web>
// <web>http://www.apelab.ch</web>
// <date>2014-06-01</date>
using System.Collections;
using UnityEngine;

/// <summary>
/// Fades the screen from black after a new scene is loaded.
/// </summary>
public class apelab_ScreenFadePostProcess : MonoBehaviour
{
    /// <summary>
    /// How long it takes to fade.
    /// </summary>
    public float fadeTime = 2.0f;
    public bool startTransparent = true;

    /// <summary>
    /// The initial screen color.
    /// </summary>
    public Color fadeColor = new Color(0.01f, 0.01f, 0.01f, 1.0f);

    public Material fadeMaterial;
    private bool isFading = false;
    private YieldInstruction fadeInstruction = new WaitForEndOfFrame();

	void Start(){
		if (!startTransparent) {
			Color color = fadeColor;
			color.a = 1f;
			fadeMaterial.color = color;
			fadeMaterial.SetPass (0);
			GL.PushMatrix ();
			GL.LoadOrtho ();
			GL.Color (fadeMaterial.color);
			GL.Begin (GL.QUADS);
			GL.Vertex3 (0f, 0f, -12f);
			GL.Vertex3 (0f, 1f, -12f);
			GL.Vertex3 (1f, 1f, -12f);
			GL.Vertex3 (1f, 0f, -12f);
			GL.End ();
			GL.PopMatrix ();
		}
	}
	
    /// <summary>
    /// Fades alpha from 1.0 to 0.0
    /// </summary>
    IEnumerator FadeIn()
    {
        float elapsedTime = 0.0f;
        fadeMaterial.color = fadeColor;
        Color color = fadeColor;
        isFading = true;
        while (elapsedTime < fadeTime)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime;

            if (startTransparent)
            {
                color.a = Mathf.Clamp01(elapsedTime / fadeTime);
            }
            else
            {
                color.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
            }
            fadeMaterial.color = color;
        }
        isFading = false;
    }


    public void Fade()
    {
        StartCoroutine(FadeIn());
    }
}