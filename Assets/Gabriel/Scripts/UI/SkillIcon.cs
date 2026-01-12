using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Settings")]
    [SerializeField] private string skillName;
    [SerializeField] private string uniqueID;       // ID ÚNICO para o SaveSystem (ex: "Unlock_DoubleJump")

    [Header("Visuals")]
    [SerializeField] private Image skillImage;
    [SerializeField] private Color usedColor = Color.gray;
    [SerializeField] private Button button; // Opcional: Referência ao botão para o desativar

    private bool isUsed = false;
    private DialogueVariables dialogueVars; // Referência local para os dados

    private void Start()
    {
        // 1. Configurar ID automático se vazio
        if (string.IsNullOrEmpty(uniqueID)) uniqueID = gameObject.name;

        // 2. Encontrar as Variáveis Globais
        // Como o MenuActions já tem a referência pública para dialogueVars, vamos buscá-la lá.
        MenuActions menuActions = FindFirstObjectByType<MenuActions>();
        if (menuActions != null)
        {
            dialogueVars = menuActions.dialogueVars;
        }
        else
        {
            // Fallback: Tenta encontrar noutros sítios se o MenuActions não estiver na cena
            Debug.LogWarning($"SkillIcon ({gameObject.name}): MenuActions não encontrado. O estado não será salvo.");
        }

        // 3. Verificar estado inicial
        CheckIfAlreadyUsed();
    }

    private void CheckIfAlreadyUsed()
    {
        // Se não temos variáveis, não fazemos nada
        if (dialogueVars == null || dialogueVars.flags == null) return;

        // Verifica se a flag existe e é verdadeira no sistema de diálogo
        if (dialogueVars.flags.ContainsKey(uniqueID) && dialogueVars.flags[uniqueID])
        {
            SetVisualsAsUsed();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (TooltipManager.Instance != null)
            TooltipManager.Instance.ShowTooltip(skillName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipManager.Instance != null)
            TooltipManager.Instance.HideTooltip();
    }

    public void OnSkillClicked()
    {
        if (!isUsed)
        {
            SetVisualsAsUsed();

            // INTEGRAÇÃO COM SAVE SYSTEM
            if (dialogueVars != null)
            {
                // Isto funciona sempre: se a chave existir, atualiza. Se não existir, cria.
                // Resolve o erro CS1061 pois o método Add pode não existir na tua classe BoolDict.
                dialogueVars.flags[uniqueID] = true;

                // Salva o jogo imediatamente
                SaveSystem.SaveAll(dialogueVars);

                Debug.Log($"Habilidade {skillName} (ID: {uniqueID}) guardada no SaveSystem!");
            }
        }
    }

    private void SetVisualsAsUsed()
    {
        isUsed = true;

        if (skillImage)
            skillImage.color = usedColor;

        // Desativa o botão para não clicar mais
        if (button)
            button.interactable = false;
        else if (TryGetComponent(out Button btn))
            btn.interactable = false;
    }
}