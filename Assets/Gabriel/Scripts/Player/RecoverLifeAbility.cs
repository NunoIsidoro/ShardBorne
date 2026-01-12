using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class RecoverLifeAbility : BaseAbility
{
    public InputActionReference recoverActionRef;

    [Header("Hold Settings")]
    [SerializeField] private float holdTimeToRecover = 2f;

    [Header("Eco Cost")]
    [SerializeField] private float ecoCost;

    [Header("Shake via Impulse")]
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private float impulseAmplitude = 1f;
    [SerializeField] private float impulseInterval = 0.06f;

    private bool isHolding;
    private float holdTimer;
    private float impulseTimer;

    private PlayerStats playerStats;
    private readonly string recoverAnimParameterName = "RecoverHealth";
    private int recoverParameterID;

    protected override void Inicialization()
    {
        base.Inicialization();
        playerStats = PlayerStats.Instance;
        recoverParameterID = Animator.StringToHash(recoverAnimParameterName);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // --- CORREÇÃO: Forçar a ligação das referências ---
        // Se a BaseAbility falhou em encontrar o Player, nós procuramos manualmente aqui.
        if (linkedPhysics == null || linkedStateMachine == null)
        {
            Player playerRef = GetComponent<Player>();
            if (playerRef != null)
            {
                if (linkedPhysics == null) linkedPhysics = playerRef.physicsControl;
                if (linkedStateMachine == null) linkedStateMachine = playerRef.stateMachine;

                // Garante que a inicialização corre se ainda não correu
                if (recoverParameterID == 0) Inicialization();
            }
            else
            {
                // Se não encontrar no próprio objeto, tenta no pai
                playerRef = GetComponentInParent<Player>();
                if (playerRef != null)
                {
                    if (linkedPhysics == null) linkedPhysics = playerRef.physicsControl;
                    if (linkedStateMachine == null) linkedStateMachine = playerRef.stateMachine;
                    if (recoverParameterID == 0) Inicialization();
                }
            }
        }
        // ------------------------------------------------

        if (playerStats == null)
        {
            playerStats = PlayerStats.Instance;
            if (playerStats == null)
                playerStats = FindFirstObjectByType<PlayerStats>();
        }
    }

    private void OnEnable()
    {
        if (recoverActionRef != null)
        {
            recoverActionRef.action.Enable();
            recoverActionRef.action.started += OnHoldStarted;
            recoverActionRef.action.canceled += OnHoldCanceled;
            Debug.Log("[Recover] Action ativada e subscrevida.");
        }
        else
        {
            Debug.LogError("[Recover] ERRO CRÍTICO: recoverActionRef está vazio no Inspector!");
        }
    }

    private void OnDisable()
    {
        if (recoverActionRef != null)
        {
            recoverActionRef.action.started -= OnHoldStarted;
            recoverActionRef.action.canceled -= OnHoldCanceled;
            recoverActionRef.action.Disable();
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (impulseSource == null)
            impulseSource = FindFirstObjectByType<CinemachineImpulseSource>();
    }

    private void OnHoldStarted(InputAction.CallbackContext ctx)
    {
        // DEBUG 1: Input Básico
        Debug.Log("[Recover] Input recebido! A verificar condições...");

        if (linkedPhysics == null || linkedStateMachine == null)
        {
            // Tenta recuperar de emergência se o Start falhou
            Start();
            if (linkedPhysics == null || linkedStateMachine == null)
            {
                Debug.LogError("[Recover] FALHA FINAL: Physics ou StateMachine continuam desligados. Verifica se o script está no objeto do Player!");
                return;
            }
        }

        if (!isPermitted)
        {
            Debug.Log($"[Recover] FALHA: Habilidade não permitida (isPermitted = false).");
            return;
        }

        // Proteções de Estado
        if (!linkedPhysics.grounded ||
            linkedStateMachine.currentState == PlayerStates.State.Jump ||
            linkedStateMachine.currentState == PlayerStates.State.WallJump ||
            linkedStateMachine.currentState == PlayerStates.State.Attack ||
            linkedStateMachine.currentState == PlayerStates.State.Dash ||
            linkedStateMachine.currentState == PlayerStates.State.KnockBack)
        {
            Debug.Log($"[Recover] FALHA: Estado inválido ou no ar. Estado atual: {linkedStateMachine.currentState} | Grounded: {linkedPhysics.grounded}");
            return;
        }

        // Verifica vida cheia
        if (playerStats != null && playerStats.GetCurrentHealth() >= playerStats.GetMaxHealth())
        {
            Debug.Log($"[Recover] FALHA: Vida já está cheia ({playerStats.GetCurrentHealth()}/{playerStats.GetMaxHealth()}).");
            return;
        }

        // Verifica Eco
        if (playerStats != null && playerStats.GetCurrentEco() < ecoCost)
        {
            Debug.Log($"[Recover] FALHA: Sem Eco suficiente ({playerStats.GetCurrentEco()} < {ecoCost}).");
            return;
        }

        // INICIA O PROCESSO
        Debug.Log("[Recover] SUCESSO: A começar a carregar cura...");
        isHolding = true;
        holdTimer = 0f;
        impulseTimer = 0f;

        linkedPhysics.ResetVelocity();
        linkedStateMachine.ChangeState(PlayerStates.State.RecoverHealth);
        UpdateAnimator();

        if (impulseSource != null)
            impulseSource.GenerateImpulse(impulseAmplitude);
    }

    public override void ProcessAbility()
    {
        // Se deixar de estar no chão ou sair do estado, cancela
        if (!linkedPhysics.grounded || linkedStateMachine.currentState != PlayerStates.State.RecoverHealth)
        {
            if (isHolding)
            {
                Debug.Log($"[Recover] CANCELADO: Saiu do estado ou saltou. Estado: {linkedStateMachine.currentState}");
                CancelRecovery();
            }
            return;
        }

        if (!isPermitted || !isHolding) return;

        linkedPhysics.ResetVelocity();

        holdTimer += Time.deltaTime;
        impulseTimer += Time.deltaTime;

        // Debug visual do progresso (opcional, descomenta se precisares)
        // Debug.Log($"[Recover] A carregar: {holdTimer}/{holdTimeToRecover}");

        if (impulseTimer >= impulseInterval)
        {
            impulseTimer = 0f;
            if (impulseSource != null)
                impulseSource.GenerateImpulse(impulseAmplitude);
        }

        if (holdTimer >= holdTimeToRecover)
        {
            DoRecover();
        }
    }

    private void OnHoldCanceled(InputAction.CallbackContext ctx)
    {
        if (isHolding)
        {
            Debug.Log("[Recover] Input cancelado (botão largado).");
            CancelRecovery();
        }
    }

    private void DoRecover()
    {
        Debug.Log("[Recover] Tempo completado! A tentar curar...");

        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();

        if (playerStats != null)
        {
            if (playerStats.GetCurrentHealth() < playerStats.GetMaxHealth())
            {
                if (playerStats.SpendEco(ecoCost))
                {
                    playerStats.AddHealth(1f);
                    Debug.Log("<color=green>Vida Recuperada com Eco!</color>");
                }
                else
                {
                    Debug.Log("<color=red>Falha na cura: Eco insuficiente.</color>");
                }
            }
        }

        CancelRecovery();
    }

    private void CancelRecovery()
    {
        isHolding = false;
        holdTimer = 0f;
        impulseTimer = 0f;

        if (linkedStateMachine != null && linkedStateMachine.currentState == PlayerStates.State.RecoverHealth)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Idle);
        }

        UpdateAnimator();
    }

    public override void UpdateAnimator()
    {
        if (linkedAnimator == null || linkedStateMachine == null) return;

        if (linkedStateMachine.currentState == PlayerStates.State.RecoverHealth)
            linkedAnimator.SetBool(recoverParameterID, isHolding);
        else
            linkedAnimator.SetBool(recoverParameterID, false);
    }
}