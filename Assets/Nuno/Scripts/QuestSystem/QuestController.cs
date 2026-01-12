using TMPro;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    [Header("Refs")]
    public DialogueVariables vars;
    public GameObject questPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText; // NOVO: Texto descritivo do objetivo
    public TextMeshProUGUI progressText;

    [Header("Config")]
    public string questKey = "quest_FarmVeggies";
    public string countKey = "count.potato";
    public int targetCount = 5; // Atualizado para 5
    [TextArea] public string title = "Missão";

    void Reset()
    {
        questPanel = gameObject;
    }

    void Update()
    {
        if (!vars) return;
        if (string.IsNullOrEmpty(questKey)) return;

        // Ler estado atual
        vars.texts.TryGetValue(questKey, out var stage);
        vars.numbers.TryGetValue(countKey, out var countFloat);
        vars.flags.TryGetValue("spoke_Weaver", out var spokeWeaver);

        int count = Mathf.RoundToInt(countFloat);
        bool show = false;
        string currentDesc = "";
        string currentProgress = "";

        // --- MISSÃO 1: LEGUMES ---
        if (questKey == "quest_FarmVeggies")
        {
            if (stage == "collect")
            {
                show = true;
                currentDesc = "Vai ao campo para colher os legumes";
                currentProgress = $"{count} / {targetCount}";

                if (count >= targetCount)
                {
                    vars.Set(questKey, "return", 0f, false);
                }
            }
            else if (stage == "return")
            {
                show = true;
                currentDesc = "Volta para a Vila para fazer a Sopa";
                currentProgress = "";
            }
            else if (stage == "done")
            {
                // Se já falou com a Tecelã (spokeWeaver == true), escondemos esta missão
                if (!spokeWeaver)
                {
                    show = true;
                    currentDesc = "Volta à Vila e fala com a Tecelã";
                    currentProgress = "";
                }
                else
                {
                    show = false;
                }
            }
        }
        // --- NOVA MISSÃO: ESTRANHO (Adiciona isto) ---
        else if (questKey == "quest_FindStranger")
        {
            // Esta missão ativa-se quando a Tecelã te manda ir lá (ver JSON nó C5)
            if (stage == "active")
            {
                show = true;
                currentDesc = "Encontra a casa do Estranho nos Ermos";
                currentProgress = "";
            }
            // Quando entrares na casa dele (Cena D), podes mudar esta var para "done" para esconder
            else
            {
                show = false;
            }
        }
        // --- MISSÃO 3: TREINO HEAVY ATTACK ---
        else if (questKey == "quest_HeavyTraining")
        {
            vars.flags.TryGetValue("ability_HeavyStrike", out bool hasHeavySkill);

            // FASE 1: Destruir Pedras (Active)
            if (stage == "active")
            {
                show = true;
                if (titleText) titleText.text = "Despertar da Força"; // Título Sugestivo
                currentDesc = "Destrói 3 rochas de treino no exterior.";
                currentProgress = $"{count} / {targetCount}";
            }
            // FASE 2: Voltar ao Estranho (Return)
            // Isto acontece quando o script da pedra mete a quest em "return"
            else if (stage == "return")
            {
                show = true;
                if (titleText) titleText.text = "Despertar da Força";
                currentDesc = "As rochas cederam. Volta a falar com o Estranho.";
                currentProgress = ""; // Esconde contador
            }
            // FASE 3: Desbloquear Skill (Unlock)
            // O Estranho vai mudar a quest para "unlock_skill" depois da conversa
            else if (stage == "unlock_skill" && !hasHeavySkill)
            {
                show = true;
                if (titleText) titleText.text = "Canalizar Poder";
                currentDesc = "Abre o menu (M), clica no X e desbloqueia o Heavy Attack.";
                currentProgress = "";
            }
            // FASE 4: Ir para o Underground (Done + Skill)
            else if (hasHeavySkill)
            {
                show = true;
                if (titleText) titleText.text = "Rumo às Profundezas";
                currentDesc = "Vai até à Vila e destrói a pedra para entrares no underground.";
                currentProgress = "";
            }
            else
            {
                show = false;
            }
        }

        // Aplicar à UI
        if (questPanel) questPanel.SetActive(show);

        if (show)
        {
            if (titleText) titleText.text = title;
            if (descriptionText) descriptionText.text = currentDesc;
            if (progressText) progressText.text = currentProgress;
        }
    }
}