using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class HarvestMinigame : MonoBehaviour
{
    [Header("UI")]
    public GameObject root;     // painel do minijogo
    public Slider slider;       // 0..1

    [Header("Timing")]
    public float speed = 1.6f;  // velocidade do cursor
    [Range(0.01f, 0.5f)] public float perfectWidth = 0.12f; // metade para cada lado do 0.5
    public KeyCode confirmKey = KeyCode.E;

#if ENABLE_INPUT_SYSTEM
    public InputActionReference confirmAction;
#endif

    bool running;
    float t;

    public System.Action<bool> onResult; // true=success

    void Awake()
    {
        if (root) root.SetActive(false);
#if ENABLE_INPUT_SYSTEM
        if (confirmAction) confirmAction.action.Enable();
#endif
    }

    void Update()
    {
        if (!running || slider == null) return;

        t += Time.deltaTime * speed;
        float val = Mathf.PingPong(t, 1f);
        slider.value = val;

        bool pressed = false;
#if ENABLE_INPUT_SYSTEM
        if (confirmAction && confirmAction.action.WasPerformedThisFrame()) pressed = true;
        else if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) pressed = true;
#else
        if (Input.GetKeyDown(confirmKey)) pressed = true;
#endif
        if (pressed) Confirm();
    }

    public void Open(System.Action<bool> resultCallback)
    {
        onResult = resultCallback;
        t = 0f;
        if (root) root.SetActive(true);
        running = true;
        Time.timeScale = 0f; // pausa leve enquanto joga
    }

    public void Close()
    {
        running = false;
        if (root) root.SetActive(false);
        Time.timeScale = 1f;
    }

    void Confirm()
    {
        float v = slider.value;
        bool success = (v > 0.5f - perfectWidth) && (v < 0.5f + perfectWidth);
        onResult?.Invoke(success);
        Close();
    }
}
