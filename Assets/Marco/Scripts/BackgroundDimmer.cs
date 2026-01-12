using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class BackgroundDimmer : MonoBehaviour
{
    public static BackgroundDimmer Instance;
    public Light2D globalLight;
    public float targetIntensity = 0.4f; // Quão escuro queres que fique
    public float duration = 1.5f;        // Tempo da transição

    void Awake() => Instance = this;

    public void DimBackground()
    {
        if (globalLight != null)
            StartCoroutine(FadeLight());
    }

    IEnumerator FadeLight()
    {
        float startIntensity = globalLight.intensity;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            globalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsed / duration);
            yield return null;
        }
        globalLight.intensity = targetIntensity;
    }
}