using UnityEngine;
using UnityEngine.UI;

public class UIHealthDisplay : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Image[] lifeImages;

    void Start()
    {
        // Se a referência do Player não está definida no Inspector, tenta encontrá-lo na cena
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();

            if (playerStats == null)
            {
                Debug.LogError("UIHealthDisplay: Não encontrei o PlayerStats na cena!");
            }
        }
    }

    void Update()
    {
        AtualizarVidas();
    }

    private void AtualizarVidas()
    {
        if (playerStats == null || lifeImages == null || lifeImages.Length == 0)
            return;

        int currentLives = Mathf.RoundToInt(playerStats.GetCurrentHealth());
        currentLives = Mathf.Clamp(currentLives, 0, lifeImages.Length);

        for (int i = 0; i < lifeImages.Length; i++)
        {
            lifeImages[i].enabled = (i < currentLives);
        }
    }
}
