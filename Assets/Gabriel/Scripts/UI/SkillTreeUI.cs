using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class SkillTreeUI : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference openAction;

    [SerializeField] private InputActionReference closeAction;

    [SerializeField] private GatherInput gatherInput;

    [Header("UI References")]
    [SerializeField] private GameObject skillTreePanel;
    [SerializeField] private Light2D globalLight;

    [Header("Light Settings")]
    [SerializeField] private float darkIntensity = 0.3f;
    private float originalIntensity;

    private bool isOpen = false;

    private void Awake()
    {
        if (globalLight != null)
        {
            originalIntensity = globalLight.intensity;
        }

        // Garante que começa fechado
        CloseTree();
    }

    private void OnEnable()
    {
        // Subscreve apenas ao Open inicialmente (porque estamos no jogo)
        if (openAction != null)
        {
            openAction.action.Enable();
            openAction.action.performed += OnOpenPerformed;
        }

        if (closeAction != null)
        {
            // O close começa desligado ou ligado dependendo do mapa, 
            // mas subscrevemos já o evento
            closeAction.action.performed += OnClosePerformed;
        }
    }

    private void OnDisable()
    {
        if (openAction != null) openAction.action.performed -= OnOpenPerformed;
        if (closeAction != null) closeAction.action.performed -= OnClosePerformed;
    }

    // Chamado quando carregas na tecla de ABRIR (estando no mapa Player)
    private void OnOpenPerformed(InputAction.CallbackContext ctx)
    {
        if (!isOpen)
        {
            OpenTree();
        }
    }

    // Chamado quando carregas na tecla de FECHAR (estando no mapa UI)
    private void OnClosePerformed(InputAction.CallbackContext ctx)
    {
        if (isOpen)
        {
            CloseTree();
        }
    }

    private void OpenTree()
    {
        isOpen = true;
        skillTreePanel.SetActive(true);

        // Escurece a luz
        if (globalLight != null) globalLight.intensity = darkIntensity;

        // Muda para o mapa de UI (Isto desativa o openAction e ativa o closeAction automaticamente pelo Input System)
        if (gatherInput != null) gatherInput.EnableUIMap();

        // Garante que a ação de fechar está ativa (redundância de segurança)
        if (closeAction != null) closeAction.action.Enable();

        Debug.Log("Skill Tree Aberta");
    }

    private void CloseTree()
    {
        isOpen = false;
        skillTreePanel.SetActive(false);

        // Restaura a luz
        if (globalLight != null) globalLight.intensity = originalIntensity;

        // Muda para o mapa de Player
        if (gatherInput != null) gatherInput.EnablePlayerMap();

        // Reativa a ação de abrir
        if (openAction != null) openAction.action.Enable();

        Debug.Log("Skill Tree Fechada");
    }
}