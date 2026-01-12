using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class ScreenFader : MonoBehaviour
{
    [Header("Refs")]
    public Image fadeImage;
    public TextMeshProUGUI sceneTitleText;

    [Header("Config")]
    public float fadeDuration = 1f;      // duração total
    [Range(0f, 1f)]
    public float holdPercent = 0.6f;     // percentagem do tempo que fica preto

    // --- NOVO: Campo para escreveres no Inspector ---
    [Header("Conteúdo")]
    [Tooltip("Escreve aqui o texto que queres que apareça. Se deixares vazio, pode ser definido por código.")]
    public string titleToDisplay;
    // ------------------------------------------------

    private CanvasGroup cg;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();

        if (fadeImage == null) fadeImage = GetComponentInChildren<Image>(true);
        if (sceneTitleText == null) sceneTitleText = GetComponentInChildren<TextMeshProUGUI>(true);

        cg.alpha = 1f;

        if (fadeImage != null)
        {
            var c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
            fadeImage.raycastTarget = true;
        }

        if (sceneTitleText != null)
        {
            sceneTitleText.text = titleToDisplay;   
            sceneTitleText.gameObject.SetActive(true);
        }
    }

    /*
    public void SetSceneName(string name)
    {
        // Esta função continua a funcionar caso queiras mudar o nome via código noutras situações
        if (sceneTitleText != null)
        {
            sceneTitleText.text = name;
            sceneTitleText.gameObject.SetActive(true);
        }
    }
    */
    // Chamado pelo PlayerSpawnManager2D quando tudo está pronto
    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        float holdTime = fadeDuration * holdPercent;
        float fadeTime = fadeDuration * (1f - holdPercent);

        // 1) PRETO TOTAL durante a fase de hold
        cg.alpha = 1f;
        cg.blocksRaycasts = true;
        cg.interactable = true;

        yield return new WaitForSecondsRealtime(holdTime);

        // 2) FADE para transparente
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            float v = Mathf.Lerp(1f, 0f, t / fadeTime);
            cg.alpha = v;

            yield return null;
        }

        // Garantir estado final (alpha 0)
        cg.alpha = 0f;

        // Aqui está a parte crucial:
        cg.blocksRaycasts = false;
        cg.interactable = false;

        // Esconde o texto
        if (sceneTitleText != null)
            sceneTitleText.gameObject.SetActive(false);
    }
}