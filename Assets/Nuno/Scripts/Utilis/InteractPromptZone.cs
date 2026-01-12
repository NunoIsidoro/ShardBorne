using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Collider2D))]
public class InteractPromptZone : MonoBehaviour
{
    [Header("Prompt (apenas visual)")]
    [Tooltip("GameObject do ícone/letra 'E' (filho da Zona). Este objeto será ligado/desligado.")]
    public GameObject prompt;

    [Header("Configuração")]
    [Tooltip("Tag do Player que ativa o prompt.")]
    public string playerTag = "Player";

    // Fallback para o Input antigo (só usado se não houver ENABLE_INPUT_SYSTEM)
    public KeyCode legacyInteractKey = KeyCode.E;

#if ENABLE_INPUT_SYSTEM
    [Header("Input System (novo)")]
    [Tooltip("Opcional: InputAction de Interact. Se vazio, usa <Keyboard>/e.")]
    public InputActionReference interactAction;
#endif

    [Header("Eventos (opcional)")]
    [Tooltip("Chamado quando o jogador prime a tecla de interação dentro da zona.")]
    public UnityEvent onInteract;

    private bool _playerInside;

    void OnValidate()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;

        if (prompt == gameObject)
            Debug.LogWarning("[InteractPromptZone] O 'prompt' não pode ser o MESMO GameObject da zona. Cria um filho visual (Sprite/Canvas).");
    }

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;

        if (!prompt)
        {
            var t = transform.Find("Prompt");
            if (t) prompt = t.gameObject;
        }
    }

    void Awake()
    {
        SetPrompt(false); // começa invisível mas a ZONA continua ativa
    }

#if ENABLE_INPUT_SYSTEM
    void OnEnable()
    {
        if (interactAction) interactAction.action.Enable();
    }
    void OnDisable()
    {
        if (interactAction) interactAction.action.Disable();
        _playerInside = false;
        SetPrompt(false);
    }
#else
    void OnDisable()
    {
        _playerInside = false;
        SetPrompt(false);
    }
#endif

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        _playerInside = true;
        SetPrompt(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        _playerInside = false;
        SetPrompt(false);
    }

    void Update()
    {
        if (!_playerInside) return;
        if (InteractPressed())
            onInteract?.Invoke();
    }

    bool InteractPressed()
    {
#if ENABLE_INPUT_SYSTEM
        // 1) Se ligaste uma ação no Inspector (ex.: Player/Interact)
        if (interactAction && interactAction.action.WasPressedThisFrame())
            return true;

        // 2) Fallback: teclado <E>
        var kb = Keyboard.current;
        return kb != null && kb.eKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(legacyInteractKey);
#endif
    }

    void SetPrompt(bool show)
    {
        if (prompt && prompt != gameObject)
            prompt.SetActive(show);
    }
}
