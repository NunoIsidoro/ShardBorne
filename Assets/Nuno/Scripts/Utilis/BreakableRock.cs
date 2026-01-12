using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BreakableRock : MonoBehaviour
{
    public enum RequiredAttack { Any, BasicOnly, HeavyOnly }

    [Header("Configurações de Vida")]
    [Tooltip("Quantos golpes são precisos para partir totalmente.")]
    public int hitsToBreak = 3;

    [Header("Configurações de Ataque")]
    public RequiredAttack requiredAttack = RequiredAttack.Any;
    public string basicAttackTag = "BasicAttack";
    public string heavyAttackTag = "HeavyAttack";

    [Header("Animação Gradual")]
    [Tooltip("Nome do parâmetro Float no Animator (0.0 a 1.0).")]
    public string progressParamName = "BreakProgress";
    [Tooltip("Nome do Trigger para tocar a animação de quebra final.")]
    public string breakTriggerName = "Break";
    public float destroyDelay = 0.6f;

    [Header("Missão (Opcional)")]
    public DialogueVariables vars;
    public string questKey = "";
    public string countKey = "";
    [Tooltip("Total de pedras a destruir para completar o objetivo da missão.")]
    public int targetCount = 3;
    [Tooltip("Valor para definir na quest quando o alvo for atingido (ex: 'return').")]
    public string completionValue = "return";

    // Estado Interno
    private Animator animator;
    private int currentHits;
    private bool broken;

    void Awake()
    {
        animator = GetComponent<Animator>();
        // Garante que a pedra é sólida (não trigger) para bloquear o caminho até partir
        GetComponent<Collider2D>().isTrigger = false;

        // Inicializa a animação a 0
        if (animator) animator.SetFloat(progressParamName, 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (broken) return;

        if (!IsCorrectAttack(other)) return;

        RegisterHit();
    }

    bool IsCorrectAttack(Collider2D other)
    {
        switch (requiredAttack)
        {
            case RequiredAttack.BasicOnly:
                return other.CompareTag(basicAttackTag);
            case RequiredAttack.HeavyOnly:
                return other.CompareTag(heavyAttackTag);
            case RequiredAttack.Any:
            default:
                return other.CompareTag(basicAttackTag) || other.CompareTag(heavyAttackTag);
        }
    }

    public void RegisterHit()
    {
        if (broken) return;

        currentHits++;

        // --- LÓGICA GRADUAL ---
        // Calcula percentagem (ex: 1/3 = 0.33)
        float progress = Mathf.Clamp01((float)currentHits / hitsToBreak);

        if (animator)
        {
            animator.SetFloat(progressParamName, progress);
        }

        // Verifica se partiu
        if (currentHits >= hitsToBreak)
            Break();
    }

    void Break()
    {
        if (broken) return;
        broken = true;

        // Toca animação final
        if (animator && !string.IsNullOrEmpty(breakTriggerName))
            animator.SetTrigger(breakTriggerName);

        // --- DEBUG 1: Verificar se temos as referências ---
        if (vars == null)
        {
            Debug.LogError($"[ERRO CRÍTICO] A pedra '{gameObject.name}' não tem o 'DialogueVariables' arrastado no Inspector!");
        }
        else
        {
            Debug.Log($"[INFO] Pedra '{gameObject.name}' a tentar atualizar variáveis...");
        }

        // --- ATUALIZAR MISSÃO ---
        if (vars && !string.IsNullOrEmpty(countKey))
        {
            // Adiciona 1 ao contador global
            vars.Add(countKey, 1);

            // Vamos ler o valor LOGO a seguir para ter a certeza
            vars.numbers.TryGetValue(countKey, out var novoValor);
            Debug.Log($"[DEBUG] Variável '{countKey}' atualizada. Valor atual: {novoValor}. Alvo: {targetCount}");

            // Verifica se o objetivo da missão foi cumprido
            if (!string.IsNullOrEmpty(questKey) && targetCount > 0)
            {
                if (novoValor >= targetCount)
                {
                    Debug.Log($"[SUCESSO] Alvo atingido ({novoValor} >= {targetCount})! A mudar '{questKey}' para '{completionValue}'");

                    // Define a quest como "return"
                    vars.Set(questKey, completionValue, 0f, false);

                    // DEBUG EXTRA: Confirmar se a mudança ficou gravada
                    vars.texts.TryGetValue(questKey, out var valorFinalQuest);
                    Debug.Log($"[CONFIRMAÇÃO] O valor de '{questKey}' é agora: '{valorFinalQuest}'");
                }
                else
                {
                    Debug.Log($"[INFO] Ainda falta partir mais pedras. ({novoValor}/{targetCount})");
                }
            }
        }
        else
        {
            Debug.LogWarning($"[AVISO] 'CountKey' está vazia na pedra '{gameObject.name}'. Não vai contar nada.");
        }

        // Desliga colisão e destrói
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        Destroy(gameObject, destroyDelay);
    }
    /*
    void Break()
    {
        if (broken) return;
        broken = true;

        // Toca animação final
        if (animator && !string.IsNullOrEmpty(breakTriggerName))
            animator.SetTrigger(breakTriggerName);

        // --- ATUALIZAR MISSÃO ---
        if (vars && !string.IsNullOrEmpty(countKey))
        {
            // Adiciona 1 ao contador global (ex: count.rocksBroken)
            vars.Add(countKey, 1);

            // Verifica se o objetivo da missão foi cumprido
            if (!string.IsNullOrEmpty(questKey) && targetCount > 0)
            {
                vars.numbers.TryGetValue(countKey, out var c);

                // Se já partiste as 3 pedras necessárias
                if (c >= targetCount)
                {
                    // Define a quest como "return" para a UI mandar voltar ao Estranho
                    vars.Set(questKey, completionValue, 0f, false);
                }
            }
        }

        // Desliga colisão e destrói
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        Destroy(gameObject, destroyDelay);
    }
    */
}