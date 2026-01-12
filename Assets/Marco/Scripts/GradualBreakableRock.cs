using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GradualBreakableRock : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Quantos golpes são precisos para partir totalmente.")]
    public int hitsToBreak = 3;

    [Tooltip("Nome do parâmetro Float no Animator.")]
    public string progressParamName = "BreakProgress";

    [Tooltip("Tag do ataque que parte a pedra.")]
    public string heavyAttackTag = "HeavyAttack";

    [Header("Feedback")]
    public float destroyDelay = 0.5f; // Tempo para destruir o objeto após o último hit

    // Referências
    private Animator anim;
    private Collider2D col;
    private int currentHits = 0;
    private bool isBroken = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();

        // Garante que a animação começa no frame 0
        if (anim) anim.SetFloat(progressParamName, 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isBroken) return;

        // Só aceita o Ataque Pesado (ou o que definires)
        if (other.CompareTag(heavyAttackTag))
        {
            RegisterHit();
        }
    }

    void RegisterHit()
    {
        currentHits++;

        // Calcula a percentagem (0.0 a 1.0) baseada nos hits
        // Ex: 1/3 = 0.33, 2/3 = 0.66, 3/3 = 1.0
        float progress = (float)currentHits / hitsToBreak;

        // Atualiza o Animator para avançar a animação para o ponto certo
        if (anim)
        {
            anim.SetFloat(progressParamName, progress);
        }

        // Se atingiu o total, parte a pedra
        if (currentHits >= hitsToBreak)
        {
            Break();
        }
    }

    void Break()
    {
        isBroken = true;
        Debug.Log("Pedra Quebrada!");

        // Desliga o collider para o jogador passar
        if (col) col.enabled = false;

        // Destrói o objeto após a animação terminar (opcional)
        Destroy(gameObject, destroyDelay);
    }
}