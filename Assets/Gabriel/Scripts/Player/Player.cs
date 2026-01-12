using UnityEngine;
using UnityEngine.SceneManagement; // <--- Necessário para detetar mudança de cena

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public GatherInput gatherInput;
    public StateMachine stateMachine;
    public PhysicsControl physicsControl;
    public Animator anim;
    public PlayerStats playerStats;

    private BaseAbility[] playerAbilities;
    public bool facingRight = true;

    DialogueManager dialogueManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        stateMachine = new StateMachine();
        playerAbilities = GetComponents<BaseAbility>();
        stateMachine.arrayOfAbilities = playerAbilities;

        // No Awake fazemos a ligação inicial (para a primeira cena)
        ConnectToDialogueManager();
    }

    // --- NOVO: Subscrever ao evento de mudança de cena ---
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // --- NOVO: Chamado automaticamente sempre que uma cena acaba de carregar ---
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ConnectToDialogueManager();
    }

    // --- NOVO: Função auxiliar para evitar repetir código ---
    void ConnectToDialogueManager()
    {
        // 1. Desliga do antigo (se houver) para evitar erros
        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueStart -= HandleDialogueStart;
            dialogueManager.OnDialogueEnd -= HandleDialogueEnd;
        }

        // 2. Procura o NOVO manager da cena atual
        dialogueManager = FindFirstObjectByType<DialogueManager>();

        // 3. Liga os cabos ao novo manager
        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueStart += HandleDialogueStart;
            dialogueManager.OnDialogueEnd += HandleDialogueEnd;
        }
    }
    // ---------------------------------------------------------

    private void Update()
    {
        bool alguemTrabalhou = false;

        foreach (BaseAbility ability in playerAbilities)
        {
            if (ability.thisAbilityState == stateMachine.currentState)
            {
                ability.ProcessAbility();
                alguemTrabalhou = true;
            }
            ability.UpdateAnimator();
        }

        if (!alguemTrabalhou)
        {
            // Debug.LogError removido temporariamente para limpar consola, 
            // mas podes manter se for útil para ti.
        }
    }

    private void FixedUpdate()
    {
        foreach (BaseAbility ability in playerAbilities)
        {
            if (ability.thisAbilityState == stateMachine.currentState)
            {
                ability.ProcessFixedAbility();
            }
        }
    }

    public void Flip()
    {
        if (facingRight == true && gatherInput.horizontalInput < 0)
        {
            transform.Rotate(0f, 180f, 0f);
            facingRight = !facingRight;
        }
        else if (facingRight == false && gatherInput.horizontalInput > 0)
        {
            transform.Rotate(0f, 180f, 0f);
            facingRight = !facingRight;
        }
    }

    public void ForceFlip()
    {
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
    }

    void OnDestroy()
    {
        // Boa prática: limpar eventos ao destruir o objeto
        if (dialogueManager)
        {
            dialogueManager.OnDialogueStart -= HandleDialogueStart;
            dialogueManager.OnDialogueEnd -= HandleDialogueEnd;
        }
    }

    void HandleDialogueStart()
    {
        physicsControl.ResetVelocity();
        anim.SetBool("Run", false);
        gatherInput.EnableUIMap();
    }

    void HandleDialogueEnd()
    {
        gatherInput.EnablePlayerMap();
    }
}