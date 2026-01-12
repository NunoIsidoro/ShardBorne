using UnityEngine;
using UnityEngine.UI;

public class CreditsScroller : MonoBehaviour
{
    public RectTransform creditsText; // arrasta aqui o Text ou TextMeshPro
    public float scrollSpeed = 20f;   // velocidade de rolagem

    private float startY;

    void Start()
    {
        // Guarda a posição inicial para reiniciar a rolagem
        startY = creditsText.anchoredPosition.y;
    }

    void Update()
    {
        creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        // Se o texto tiver saído totalmente da tela, reinicia a rolagem
        if (creditsText.anchoredPosition.y > creditsText.rect.height + 600) // 600 ajusta dependendo da altura do painel
        {
            creditsText.anchoredPosition = new Vector2(creditsText.anchoredPosition.x, startY);
        }
    }
}
