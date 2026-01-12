using UnityEngine;
using UnityEngine.UI;

public class EcoHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image ecoBarImage; // O componente Image que vai mudar de sprite

    [Header("Sprites")]
    // Arrasta aqui as tuas 5 imagens (Vazia, 25%, 50%, 75%, Cheia)
    [SerializeField] private Sprite[] ecoBarSprites;

    private void Start()
    {
        // Subscreve ao evento do PlayerStats
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnEcoChanged += UpdateEcoUI;
            // Força atualização inicial
            UpdateEcoUI(PlayerStats.Instance.GetCurrentEco(), PlayerStats.Instance.GetMaxEco());
        }
    }

    private void OnDestroy()
    {
        // Limpa a subscrição para evitar erros se o objeto for destruído
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.OnEcoChanged -= UpdateEcoUI;
        }
    }

    private void UpdateEcoUI(float currentEco, float maxEco)
    {
        if (ecoBarImage == null || ecoBarSprites.Length == 0) return;

        // Calcula a percentagem (0 a 1)
        float percentage = currentEco / maxEco;

        // Calcula o índice do sprite com base na quantidade de sprites disponíveis
        // Ex: Se tens 5 sprites e a percentagem é 0.5 (50%), index = 2 (terceiro sprite)
        int spriteIndex = Mathf.FloorToInt(percentage * (ecoBarSprites.Length - 1));

        // Garante que não ultrapassa os limites
        spriteIndex = Mathf.Clamp(spriteIndex, 0, ecoBarSprites.Length - 1);

        // Se tivermos 100% de eco, garante que mostra o último sprite (mesmo que o cálculo dê erro)
        if (percentage >= 1f)
        {
            spriteIndex = ecoBarSprites.Length - 1;
        }

        // Atualiza a imagem
        ecoBarImage.sprite = ecoBarSprites[spriteIndex];
    }
}