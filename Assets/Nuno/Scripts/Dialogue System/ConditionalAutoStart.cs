using UnityEngine;

public class ConditionalAutoStart : MonoBehaviour
{
    [Header("Configurações")]
    public DialogueManager dialogueManager;
    public DialogueVariables variables;

    [Tooltip("ID do nó de diálogo que deve correr (ex: A0_Narrador)")]
    public string dialogueNodeId = "A0_Narrador";

    [Tooltip("Nome da flag que impede o diálogo de repetir (ex: hasIntroPlayed)")]
    public string blockingFlag = "hasIntroPlayed";

    void Start()
    {
        // 1. Validar referências
        if (dialogueManager == null) dialogueManager = FindFirstObjectByType<DialogueManager>();

        // Se não houver variáveis ou manager, não faz nada para evitar erros
        if (dialogueManager == null || variables == null) return;

        // 2. Verificar se a Intro já foi tocada
        // Tenta obter o valor da flag. Se ela existir E for verdadeira, saímos.
        if (variables.flags.TryGetValue(blockingFlag, out bool played) && played)
        {
            // A intro já foi jogada, por isso NÃO iniciamos o diálogo.
            return;
        }

        // 3. Se chegámos aqui, é a primeira vez. Iniciar diálogo.
        dialogueManager.StartDialogue(dialogueNodeId);
    }
}