using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AutoDialogueTrigger : MonoBehaviour
{
    [Header("Diálogo")]
    public string nodeId = "B0_Trigger_Colheita";

    // Deixamos público para debug, mas o script vai preencher sozinho se faltar
    public DialogueManager manager;

    [Header("Condição (opcional)")]
    public DialogueVariables vars;
    public enum Cond { None, QuestEquals, BoolTrue, BoolFalse }
    public Cond condition = Cond.QuestEquals;
    public string questKey = "quest_FarmVeggies";
    public string questValue = "collect";
    public string boolKey;

    [Header("Trigger")]
    public string playerTag = "Player";
    public bool onlyOnce = true;

    bool used;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (onlyOnce && used) return;
        if (!IsPlayer(other)) return;

        // --- CORREÇÃO: Encontrar Manager Dinamicamente ---
        if (manager == null)
        {
            manager = FindFirstObjectByType<DialogueManager>();
            if (manager == null)
            {
                Debug.LogWarning($"[AutoDialogueTrigger] DialogueManager não encontrado na cena para o nó {nodeId}.");
                return;
            }
        }
        // ------------------------------------------------

        // Verificar condições DEPOIS de ter o manager (se precisares das vars do manager)
        // Se usares 'vars' local do asset, tudo bem. Se usares 'manager.variables', tens de garantir que manager existe.
        if (!Check()) return;

        used = true;
        Debug.Log($"[AutoDialogueTrigger] A disparar nó: {nodeId}");
        manager.StartDialogue(nodeId);
    }

    bool IsPlayer(Collider2D other)
    {
        if (other.CompareTag(playerTag)) return true;
        var root = other.transform.root;
        return root && root.CompareTag(playerTag);
    }

    bool Check()
    {
        if (condition == Cond.None || vars == null) return true;

        switch (condition)
        {
            case Cond.QuestEquals:
                bool match = vars.texts.TryGetValue(questKey, out var s) && s == questValue;
                // Debug opcional
                // if (!match) Debug.Log($"[AutoTrigger] Falha condição Quest: {questKey} é '{s}', espera-se '{questValue}'");
                return match;
            case Cond.BoolTrue:
                return vars.flags.TryGetValue(boolKey, out var b) && b;
            case Cond.BoolFalse:
                return vars.flags.TryGetValue(boolKey, out var b2) && !b2;
            default:
                return true;
        }
    }
}

/*
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class AutoDialogueTrigger : MonoBehaviour
{
    [Header("Diálogo")]
    public string nodeId = "B0_Trigger_Colheita";
    public DialogueManager manager;

    [Header("Condição (opcional)")]
    public DialogueVariables vars;
    public enum Cond { None, QuestEquals, BoolTrue, BoolFalse }
    public Cond condition = Cond.QuestEquals;
    public string questKey = "quest_FarmVeggies";
    public string questValue = "collect";
    public string boolKey;

    [Header("Trigger")]
    public string playerTag = "Player";
    public bool onlyOnce = true;

    bool used;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    void Awake()
    {
        if (!manager)
            manager = FindFirstObjectByType<DialogueManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (onlyOnce && used) return;
        if (!IsPlayer(other)) return;
        if (!Check()) return;

        if (!manager)
        {
            manager = FindFirstObjectByType<DialogueManager>();
            if (!manager)
            {
                Debug.LogError("[AutoDialogueTrigger] DialogueManager não encontrado na cena.", this);
                return;
            }
        }

        used = true;
        manager.StartDialogue(nodeId);
    }

    bool IsPlayer(Collider2D other)
    {
        // aceita collider do player OU de um filho do player
        if (other.CompareTag(playerTag)) return true;
        var root = other.transform.root;
        return root && root.CompareTag(playerTag);
    }

    bool Check()
    {
        if (condition == Cond.None || vars == null) return true;

        switch (condition)
        {
            case Cond.QuestEquals:
                return vars.texts.TryGetValue(questKey, out var s) && s == questValue;
            case Cond.BoolTrue:
                return vars.flags.TryGetValue(boolKey, out var b) && b;
            case Cond.BoolFalse:
                return vars.flags.TryGetValue(boolKey, out var b2) && !b2;
            default:
                return true;
        }
    }
}
*/