using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class NpcInteract : MonoBehaviour
{
    public string startNodeId = "A1_Tharion_Pensamento";
    public GameObject promptE;

    DialogueManager manager;
    bool inside;

    void Start()
    {
        // Unity 2023+
#if UNITY_2023_1_OR_NEWER
        manager = FindFirstObjectByType<DialogueManager>();
#else
        manager = FindObjectOfType<DialogueManager>();
#endif

        if (promptE) promptE.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        inside = true;
        if (promptE) promptE.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        inside = false;
        if (promptE) promptE.SetActive(false);
    }

    void Update()
    {
        if (!inside || manager == null) return;

        bool pressed = false;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) pressed = true;
#else
        if (Input.GetKeyDown(KeyCode.E)) pressed = true;
#endif

        if (pressed)
        {
            if (promptE) promptE.SetActive(false);
            manager.StartDialogue(startNodeId);
        }
    }
}
