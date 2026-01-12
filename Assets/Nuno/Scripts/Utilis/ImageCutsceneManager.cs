using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ImageCutsceneManager : MonoBehaviour
{
    public static ImageCutsceneManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject cutscenePanel; // O painel pai que contem a imagem
    [SerializeField] private Image displayImage;       // O componente Image onde trocamos os sprites
    [SerializeField] private CanvasGroup canvasGroup;  // Para controlar a opacidade (Alpha)

    [Header("Settings")]
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float visibleDuration = 2f;
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private float zoomAmount = 1.2f;  // Quanto a imagem cresce (1.0 = 100%, 1.2 = 120%)

    // Referência para congelar o player
    private GatherInput playerInput;

    private void Awake()
    {
        // Singleton simples para ser fácil chamar de qualquer lado
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Garante que começa desligado
        if (cutscenePanel) cutscenePanel.SetActive(false);
    }

    private void Start()
    {
        // Tenta encontrar o input do player para bloquear movimento
        if (Player.Instance != null)
        {
            playerInput = Player.Instance.gatherInput;
        }
    }

    /// <summary>
    /// Inicia uma sequência de cutscene.
    /// </summary>
    /// <param name="images">Lista de sprites a mostrar.</param>
    /// <param name="onComplete">Ação a executar no final (ex: iniciar diálogo).</param>
    public void PlaySequence(Sprite[] images, System.Action onComplete)
    {
        StartCoroutine(SequenceRoutine(images, onComplete));
    }

    private IEnumerator SequenceRoutine(Sprite[] images, System.Action onComplete)
    {
        // 1. Bloquear Input do Player
        if (playerInput) playerInput.EnableUIMap();

        // 2. Preparar UI
        cutscenePanel.SetActive(true);
        canvasGroup.alpha = 0f;

        // 3. Loop pelas imagens
        foreach (Sprite img in images)
        {
            displayImage.sprite = img;

            // Reset da escala
            displayImage.rectTransform.localScale = Vector3.one;

            float timer = 0f;
            float totalStepTime = fadeInDuration + visibleDuration + fadeOutDuration;

            // Animação de uma imagem (Fade In -> Wait -> Fade Out) + Zoom constante
            while (timer < totalStepTime)
            {
                timer += Time.deltaTime;
                float progress = timer / totalStepTime;

                // --- Lógica de Zoom Suave ---
                float currentScale = Mathf.Lerp(1f, zoomAmount, progress);
                displayImage.rectTransform.localScale = Vector3.one * currentScale;

                // --- Lógica de Fade (Entrada, Espera, Saída) ---
                if (timer < fadeInDuration)
                {
                    // A entrar
                    canvasGroup.alpha = timer / fadeInDuration;
                }
                else if (timer < fadeInDuration + visibleDuration)
                {
                    // Visível
                    canvasGroup.alpha = 1f;
                }
                else
                {
                    // A sair
                    float fadeOutTimer = timer - (fadeInDuration + visibleDuration);
                    canvasGroup.alpha = 1f - (fadeOutTimer / fadeOutDuration);
                }

                yield return null;
            }
        }

        // 4. Finalizar
        cutscenePanel.SetActive(false);

        // Executa o que vier a seguir (o Diálogo)
        onComplete?.Invoke();
    }
}