using System; // Necessário para Action
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 4f;
    private float currentHealth;

    [Header("Eco Settings (Resource)")]
    [SerializeField] private float maxEco = 100f;
    [SerializeField] private float currentEco = 0f;

    // EVENTO: Avisa a UI quando o Eco muda (para não usar Update na UI)
    public event Action<float, float> OnEcoChanged;

    [Header("Flash")]
    [SerializeField] private float flashDuration = 0.15f;
    [SerializeField, Range(0, 1)] private float flashStrength = 0.5f;
    [SerializeField] private Color flashCol;
    [SerializeField] private Material flashMaterial;
    private Material defaultMaterial;
    private SpriteRenderer spriter;

    private bool canTakeDamage = true;

    // Singleton
    public static PlayerStats Instance;

    void Awake()
    {
        currentHealth = maxHealth;
        // currentEco = 0f; // Começa vazio ou cheio, como preferires

        spriter = player.GetComponentInParent<SpriteRenderer>();
        if (spriter) defaultMaterial = spriter.material;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Dispara o evento no inicio para a UI atualizar logo
        OnEcoChanged?.Invoke(currentEco, maxEco);
    }

    public void DamagePlayer(float damage)
    {
        if (!canTakeDamage) return;

        currentHealth -= damage;
        StartCoroutine(Flash());

        if (currentHealth <= 0)
        {
            Debug.Log("Player Died");
            canTakeDamage = false;
            SceneManager.LoadScene("Finish_TryAgain_Scene");
        }
    }

    private IEnumerator Flash()
    {
        canTakeDamage = false;

        if (spriter && flashMaterial)
        {
            spriter.material = flashMaterial;
            flashMaterial.SetColor("_FlashColor", flashCol);
            flashMaterial.SetFloat("_FlashAmount", flashStrength);
        }

        yield return new WaitForSeconds(flashDuration);

        if (spriter) spriter.material = defaultMaterial;

        if (currentHealth > 0)
            canTakeDamage = true;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        canTakeDamage = true;
        if (spriter) spriter.material = defaultMaterial;
    }

    // --- SISTEMA DE ECO ---

    public void AddEco(float amount)
    {
        currentEco += amount;
        if (currentEco > maxEco) currentEco = maxEco;

        Debug.Log($"Eco: {currentEco}/{maxEco}");

        // Avisa a UI que o valor mudou
        OnEcoChanged?.Invoke(currentEco, maxEco);
    }

    public bool SpendEco(float amount)
    {
        if (currentEco >= amount)
        {
            currentEco -= amount;
            Debug.Log($"Eco gasto: {amount}. Restante: {currentEco}");

            // Avisa a UI que o valor mudou
            OnEcoChanged?.Invoke(currentEco, maxEco);

            return true;
        }
        else
        {
            return false;
        }
    }

    public float GetCurrentEco() { return currentEco; }
    public float GetMaxEco() { return maxEco; }

    // --- FIM SISTEMA DE ECO ---

    public bool GetCanTakeDamage() { return canTakeDamage; }
    public float GetCurrentHealth() { return currentHealth; }
    public float GetMaxHealth() { return maxHealth; }

    public void AddHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }
}