using UnityEngine;
using UnityEngine.InputSystem;

public class AttackAbility : BaseAbility
{
    [Header("Input")]
    [SerializeField] private InputActionReference attackActionRef;

    [Header("Attack Settings")]
    [SerializeField] private GameObject attackHitbox;
    [SerializeField] private float attackDuration = 0.3f;

    [Header("Audio")]
    [Tooltip("Hit no ar")]
    [SerializeField] private AudioClip airAttackSound;
    private AudioSource audioSource;

    [Header("Animation")]
    [SerializeField] private string attackAnimParameterName = "Attack";
    private int attackAnimParameterID;

    private float attackTimer;

    protected override void Inicialization()
    {
        base.Inicialization();
        attackAnimParameterID = Animator.StringToHash(attackAnimParameterName);
        attackHitbox.SetActive(false);

        // Tenta encontrar o AudioSource no Player
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        attackActionRef.action.performed += TryToAttack;
    }

    private void OnDisable()
    {
        attackActionRef.action.performed -= TryToAttack;
    }

    private void TryToAttack(InputAction.CallbackContext ctx)
    {
        if (!isPermitted || linkedStateMachine.currentState == PlayerStates.State.Dash ||
            linkedStateMachine.currentState == PlayerStates.State.KnockBack ||
            linkedStateMachine.currentState == PlayerStates.State.WallJump) return;

        linkedStateMachine.ChangeState(PlayerStates.State.Attack);
        attackTimer = attackDuration;
        attackHitbox.SetActive(true);

        // --- TOCA O SOM DO ATAQUE NO AR ---
        if (audioSource != null && airAttackSound != null)
        {
            // PlayOneShot permite tocar sons por cima uns dos outros sem cortar
            audioSource.PlayOneShot(airAttackSound);
        }
    }

    public override void EnterAbility()
    {
        linkedPhysics.rb.linearVelocity = Vector2.zero;
    }

    public override void ProcessAbility()
    {
        player.Flip();
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            attackHitbox.SetActive(false);
            linkedStateMachine.ChangeState(PlayerStates.State.Idle);
        }
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(attackAnimParameterID, linkedStateMachine.currentState == PlayerStates.State.Attack);
    }
}