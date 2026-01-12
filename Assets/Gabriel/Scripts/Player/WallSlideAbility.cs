using UnityEngine;

/// <summary>
/// Habilidade de deslizar na parede do jogador.
/// </summary>
public class WallSlideAbility : BaseAbility
{
    [SerializeField] private float maxSlideSpeed; // Velocidade máxima de descida na parede

    // Parâmetro de animação
    private string wallSlideAnimParameterName = "WallSlide";
    private int wallSlideParameterID;

    /// <summary>
    /// Inicializa variáveis e parâmetros de animação.
    /// </summary>
    protected override void Inicialization()
    {
        base.Inicialization();
        wallSlideParameterID = Animator.StringToHash(wallSlideAnimParameterName);
    }

    /// <summary>
    /// Executado ao entrar no estado de deslizar na parede.
    /// </summary>
    public override void EnterAbility()
    {
        // Zera a velocidade ao começar a deslizar
        linkedPhysics.rb.linearVelocity = Vector2.zero;
    }

    /// <summary>
    /// Processa a lógica do deslize na parede a cada frame.
    /// </summary>
    public override void ProcessAbility()
    {
        // Se encostou no chão, volta para Idle
        if (linkedPhysics.grounded)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Idle);
            return;
        }

        // Se o jogador está virado para a direita e tenta ir para a esquerda, sai do deslize
        if (player.facingRight && linkedInput.horizontalInput < 0)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            linkedPhysics.wallDetected = false;
            linkedAnimator.SetBool("WallSlide", false);
            return;
        }

        // Se o jogador está virado para a esquerda e tenta ir para a direita, sai do deslize
        if (!player.facingRight && linkedInput.horizontalInput > 0)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            linkedPhysics.wallDetected = false;
            linkedAnimator.SetBool("WallSlide", false);
            return;
        }

        // Se não está mais encostado na parede, sai do deslize
        if (!linkedPhysics.wallDetected)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            return;
        }
    }

    /// <summary>
    /// Limita a velocidade de descida na parede a cada FixedUpdate.
    /// </summary>
    public override void ProcessFixedAbility()
    {
        linkedPhysics.rb.linearVelocityY = Mathf.Clamp(linkedPhysics.rb.linearVelocityY, -maxSlideSpeed, 1);
    }

    /// <summary>
    /// Atualiza o parâmetro de animação do deslize na parede.
    /// </summary>
    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(wallSlideParameterID, linkedStateMachine.currentState == PlayerStates.State.WallSlide);
    }
}
