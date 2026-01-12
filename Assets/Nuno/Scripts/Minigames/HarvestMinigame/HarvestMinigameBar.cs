using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HarvestMinigameBar : MonoBehaviour
{
    [Header("UI")]
    public GameObject root;         // Painel (desativado por omissão)
    public RectTransform track;     // barra branca
    public RectTransform zone;      // retângulo verde
    public RectTransform pointer;   // linha vermelha

    [Header("Input")]
    public InputActionReference confirmAction; // Player/Confirm (mapa F no teu asset)
    public InputActionReference cancelAction;  // opcional: Player/Cancel (Esc)

    [Header("Movimento")]
    public float speedPixelsPerSec = 420f;

    public System.Action<bool> onResult;
    bool running;
    float dir = 1f, minX, maxX;

    public bool Running => running;

    void Awake() { if (root) root.SetActive(false); }

    void OnEnable()
    {
        if (confirmAction && confirmAction.action != null) { confirmAction.action.performed += OnConfirm; confirmAction.action.Enable(); }
        if (cancelAction && cancelAction.action != null) { cancelAction.action.performed += OnCancel; cancelAction.action.Enable(); }
    }
    void OnDisable()
    {
        if (confirmAction && confirmAction.action != null) { confirmAction.action.performed -= OnConfirm; confirmAction.action.Disable(); }
        if (cancelAction && cancelAction.action != null) { cancelAction.action.performed -= OnCancel; cancelAction.action.Disable(); }
        // fail-safe: nunca deixar o jogo pausado
        if (running) { Time.timeScale = 1f; running = false; }
    }

    bool ValidateUI()
    {
        if (!root || !track || !zone || !pointer)
        {
            Debug.LogError("[Harvest] UI refs em falta (root/track/zone/pointer).");
            return false;
        }
        return true;
    }

    void Update()
    {
        if (!running) return;
        float x = pointer.anchoredPosition.x + dir * speedPixelsPerSec * Time.unscaledDeltaTime;
        if (x > maxX) { x = maxX; dir = -1; }
        if (x < minX) { x = minX; dir = 1; }
        pointer.anchoredPosition = new Vector2(x, pointer.anchoredPosition.y);
    }

    public void Open(System.Action<bool> result)
    {
        // só entra se a UI estiver mesmo ligada
        if (!ValidateUI()) { onResult = null; return; }

        onResult = result;

        var w = track.rect.width;
        minX = -w * 0.5f; maxX = w * 0.5f;
        pointer.anchoredPosition = new Vector2(minX, pointer.anchoredPosition.y);
        dir = 1;

        root.SetActive(true);
        running = true;
        Time.timeScale = 0f; // pausa leve
    }

    void Close()
    {
        running = false;
        if (root) root.SetActive(false);
        Time.timeScale = 1f;
    }

    void OnConfirm(InputAction.CallbackContext _) { if (!running) return; bool ok = IsInsideZone(); Close(); onResult?.Invoke(ok); }
    void OnCancel(InputAction.CallbackContext _) { if (!running) return; Close(); onResult?.Invoke(false); }

    bool IsInsideZone()
    {
        float px = pointer.anchoredPosition.x;
        float left = zone.anchoredPosition.x - zone.rect.width * 0.5f;
        float right = zone.anchoredPosition.x + zone.rect.width * 0.5f;
        return px >= left && px <= right;
    }
}
