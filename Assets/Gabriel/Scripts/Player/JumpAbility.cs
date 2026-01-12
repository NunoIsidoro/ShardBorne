using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Habilidade de pulo do jogador.
/// </summary>
public class JumpAbility : BaseAbility
{
    // Referência à ação de pulo configurada no Input System
    public InputActionReference jumpActionRef;

    [SerializeField] private float jumpForce;         // Força do pulo
    [SerializeField] private float airSpeed;          // Velocidade no ar
    [SerializeField] private float minimumAirTime;    // Tempo mínimo no ar após pular
    private float startMinimumAirTime;                // Valor inicial do tempo mínimo no ar

    // Parâmetros de animação
    private string jumpAnimParameterName = "Jump";
    private string ySpeedAnimParameterName = "ySpeed";
    private int jumpParameterID;
    private int ySpeedParameterID;

    /// <summary>
    /// Inicializa variáveis e parâmetros de animação.
    /// </summary>
    protected override void Inicialization()
    {
        base.Inicialization();
        startMinimumAirTime = minimumAirTime;
        jumpParameterID = Animator.StringToHash(jumpAnimParameterName);
        ySpeedParameterID = Animator.StringToHash(ySpeedAnimParameterName);
    }

    /// <summary>
    /// Ativa os eventos de input ao habilitar o componente.
    /// </summary>
    private void OnEnable()
    {
        jumpActionRef.action.performed += TryToJump;
        jumpActionRef.action.canceled += StopJump;
    }

    /// <summary>
    /// Remove os eventos de input ao desabilitar o componente.
    /// </summary>
    private void OnDisable()
    {
        jumpActionRef.action.performed -= TryToJump;
        jumpActionRef.action.canceled -= StopJump;
    }

    /// <summary>
    /// Processa a lógica do pulo a cada frame.
    /// </summary>
    public override void ProcessAbility()
    {
        player.Flip(); // Gira o sprite conforme direção
        minimumAirTime -= Time.deltaTime;

        // Se está no chão e já cumpriu o tempo mínimo no ar, volta para Idle ou Run
        if (linkedPhysics.grounded && minimumAirTime < 0)
        {
            if (linkedInput.horizontalInput != 0)
                linkedStateMachine.ChangeState(PlayerStates.State.Run);
            else
                linkedStateMachine.ChangeState(PlayerStates.State.Idle);
        }

        // Se está no ar e encostou na parede, muda para WallSlide se estiver caindo
        if (!linkedPhysics.grounded && linkedPhysics.wallDetected)
        {
            if (linkedPhysics.rb.linearVelocityY < 0)
            {
                linkedStateMachine.ChangeState(PlayerStates.State.WallSlide);
            }
        }
    }

    /// <summary>
    /// Processa a física do pulo a cada FixedUpdate.
    /// </summary>
    public override void ProcessFixedAbility()
    {
        // Permite movimentação horizontal no ar
        if (!linkedPhysics.grounded)
        {
            linkedPhysics.rb.linearVelocity = new Vector2(airSpeed * linkedInput.horizontalInput, linkedPhysics.rb.linearVelocityY);
        }
    }

    /// <summary>
    /// Tenta executar o pulo quando o input é acionado.
    /// </summary>
    private void TryToJump(InputAction.CallbackContext value)
    {
        if (!isPermitted || linkedStateMachine.currentState == PlayerStates.State.KnockBack || linkedStateMachine.currentState == PlayerStates.State.Attack ||
        linkedStateMachine.currentState == PlayerStates.State.Dash) {
            Debug.LogWarning("Pulo falhou: Habilidade não permitida (isPermitted = false).");
            return; }

        /*
        if (linkedStateMachine.currentState == PlayerStates.State.Ladders)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            //linkedPhysics.EnableGravity();
            linkedPhysics.rb.linearVelocity = new Vector2(airSpeed * linkedInput.horizontalInput, jumpForce);
            minimumAirTime = startMinimumAirTime;
            return;
        }*/
        // 3. Debug do Coyote Timer (O Suspeito Principal)
        Debug.Log($"Tentativa de Pulo. CoyoteTimer: {linkedPhysics.coyoteTimer}. Grounded: {linkedPhysics.grounded}");

        if (linkedPhysics.coyoteTimer > 0)
        {
            Debug.Log("<color=green>PULO APROVADO!</color> Mudando estado para JUMP.");
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);

            linkedPhysics.rb.linearVelocity = new Vector2(airSpeed * linkedInput.horizontalInput, jumpForce);
            minimumAirTime = startMinimumAirTime;
            linkedPhysics.coyoteTimer = -1;
        }

        // Só pula se estiver no chão
        //if (linkedPhysics.grounded)
        //{
        //    linkedStateMachine.ChangeState(PlayerStates.State.Jump);
        //    linkedPhysics.rb.linearVelocity = new Vector2(airSpeed * linkedInput.horizontalInput, jumpForce);
        //    minimumAirTime = startMinimumAirTime;
        //}
    }

    /// <summary>
    /// Chamado quando o input de pulo é liberado.
    /// </summary>
    private void StopJump(InputAction.CallbackContext value)
    {
        Debug.Log("Jump action canceled");
    }

    /// <summary>
    /// Atualiza os parâmetros de animação relacionados ao pulo.
    /// </summary>
    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(jumpParameterID, linkedStateMachine.currentState == PlayerStates.State.Jump || linkedStateMachine.currentState == PlayerStates.State.WallJump);
        linkedAnimator.SetFloat(ySpeedParameterID, linkedPhysics.rb.linearVelocity.y);
    }
}