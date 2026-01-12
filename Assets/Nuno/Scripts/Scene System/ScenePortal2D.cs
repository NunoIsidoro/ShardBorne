using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Collider2D))]
public class ScenePortal2D : MonoBehaviour
{
    [Header("Destino")]
    public string targetScene;
    public string targetSpawnId = "Start";

    [Header("Interação")]
    public bool requireButtonPress = true;
    public KeyCode interactKey = KeyCode.E;

#if ENABLE_INPUT_SYSTEM
    [Tooltip("Opcional: ação do Input System (ex.: Player/Interact). Se vazio, usa <Keyboard>/e.")]
    public InputActionReference interactAction;
#endif

    [Tooltip("Se preencher, será desativado durante a transição (ex.: 'PlayerController2D').")]
    public string playerControllerComponentName;

    bool playerInRange;
    Transform playerTf;
    Behaviour cachedCtrl;

    void OnValidate()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

#if ENABLE_INPUT_SYSTEM
    void OnEnable()
    {
        if (interactAction) interactAction.action.Enable();
    }

    void OnDisable()
    {
        if (interactAction) interactAction.action.Disable();
    }
#endif

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;
        playerTf = other.transform;

        // Portais automáticos (sem botão) → teleporta ao entrar,
        // mas só depois do cooldown do spawn terminar.
        if (!requireButtonPress &&
            Time.unscaledTime >= SceneTransit.portalCooldownUntil)
        {
            StartCoroutine(DoTransition());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        playerTf = null;
    }

    void Update()
    {
        // Portais que exigem botão
        if (!requireButtonPress) return;

        if (playerInRange &&
            Time.unscaledTime >= SceneTransit.portalCooldownUntil &&
            InteractPressed())
        {
            StartCoroutine(DoTransition());
        }
    }

    bool InteractPressed()
    {
#if ENABLE_INPUT_SYSTEM
        // 1) Se tiveres uma InputAction ligada no Inspector
        if (interactAction && interactAction.action.WasPressedThisFrame())
            return true;

        // 2) Fallback simples: <Keyboard>/e (ou alguns populares)
        var kb = Keyboard.current;
        if (kb == null) return false;
        if (interactKey == KeyCode.E) return kb.eKey.wasPressedThisFrame;
        if (interactKey == KeyCode.F) return kb.fKey.wasPressedThisFrame;
        if (interactKey == KeyCode.Space) return kb.spaceKey.wasPressedThisFrame;
        return false;
#else
        return Input.GetKeyDown(interactKey);
#endif
    }

    IEnumerator DoTransition()
    {
        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogError("[Portal2D] targetScene vazio!");
            yield break;
        }

        SceneTransit.nextSpawnId = targetSpawnId;

        cachedCtrl = null;
        if (playerTf && !string.IsNullOrEmpty(playerControllerComponentName))
        {
            cachedCtrl = playerTf.GetComponent(playerControllerComponentName) as Behaviour;
            if (cachedCtrl) cachedCtrl.enabled = false;
        }

        void OnLoaded(Scene s, LoadSceneMode m)
        {
            if (cachedCtrl != null) cachedCtrl.enabled = true;
            SceneManager.sceneLoaded -= OnLoaded;
        }
        if (cachedCtrl != null) SceneManager.sceneLoaded += OnLoaded;

       
        SaveSystem.SaveAtScene(targetScene);
        SceneManager.LoadScene(targetScene);
        
    }
}
