using UnityEngine;
using UnityEngine.InputSystem;

public class GatherInput : MonoBehaviour
{
    public InputActionReference jumpActionRef;
    public InputActionReference moveActionRef;

    public PlayerInput playerInput;
    private InputActionMap playerMap;
    private InputActionMap uiMap;

    [HideInInspector]
    public float horizontalInput;

    // MUDANÇA CRÍTICA: Inicializar no Awake para garantir que existe antes de ser chamado
    void Awake()
    {
        if (playerInput != null)
        {
            playerMap = playerInput.actions.FindActionMap("Player");
            uiMap = playerInput.actions.FindActionMap("UI");
        }
        else
        {
            Debug.LogError("GatherInput: PlayerInput não foi atribuído no Inspector!");
        }
    }

    void Start()
    {
        EnablePlayerMap();
    }

    public void EnableUIMap()
    {
        // Proteção contra NullReferenceException
        if (playerMap != null) playerMap.Disable();

        if (uiMap != null)
        {
            uiMap.Enable();
            Debug.Log("UI Map Enabled");
        }
    }

    public void EnablePlayerMap()
    {
        // Proteção contra NullReferenceException
        if (uiMap != null) uiMap.Disable();

        if (playerMap != null)
        {
            playerMap.Enable();
            Debug.Log("Player Map Enabled");
        }
    }

    void Update()
    {
        if (moveActionRef != null && moveActionRef.action != null)
        {
            horizontalInput = moveActionRef.action.ReadValue<float>();
        }
    }
}