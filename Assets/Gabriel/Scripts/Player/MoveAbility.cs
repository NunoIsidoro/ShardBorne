using UnityEngine;
using System.Collections; // Necessário para Coroutines

public class MoveAbility : BaseAbility
{
    [SerializeField] private float speed;
    private string runAnimParameterName = "Run";
    private int runParameterID;

    // Variáveis novas para o Slow
    private float defaultSpeed;
    private Coroutine slowCoroutine;

    protected override void Inicialization()
    {
        base.Inicialization();
        runParameterID = Animator.StringToHash(runAnimParameterName);

        // Guarda a velocidade original assim que o jogo começa
        defaultSpeed = speed;
    }

    public override void EnterAbility()
    {
        player.Flip();
    }

    public override void ProcessAbility()
    {
        if (linkedPhysics.grounded && linkedInput.horizontalInput == 0)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Idle);
        }
        if (!linkedPhysics.grounded)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
        }
    }

    public override void ProcessFixedAbility()
    {
        linkedPhysics.rb.linearVelocity = new Vector2(speed * linkedInput.horizontalInput, linkedPhysics.rb.linearVelocity.y);
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(runParameterID, linkedStateMachine.currentState == PlayerStates.State.Run);
    }

    // --- NOVA FUNCIONALIDADE: SLOW ---
    public void ApplySlow(float multiplier, float duration)
    {
        // Se já estiver lento, paramos a contagem anterior para reiniciar o tempo
        if (slowCoroutine != null) StopCoroutine(slowCoroutine);

        slowCoroutine = StartCoroutine(SlowRoutine(multiplier, duration));
    }

    private IEnumerator SlowRoutine(float multiplier, float duration)
    {
        // Aplica a lentidão (Ex: 10 * 0.5 = 5)
        speed = defaultSpeed * multiplier;

        // Opcional: Podes mudar a cor do sprite para azul aqui para dar feedback visual
        if (GetComponent<SpriteRenderer>()) GetComponent<SpriteRenderer>().color = Color.cyan;

        yield return new WaitForSeconds(duration);

        // Restaura a velocidade original
        speed = defaultSpeed;

        // Restaura a cor
        if (GetComponent<SpriteRenderer>()) GetComponent<SpriteRenderer>().color = Color.white;

        slowCoroutine = null;
    }
}