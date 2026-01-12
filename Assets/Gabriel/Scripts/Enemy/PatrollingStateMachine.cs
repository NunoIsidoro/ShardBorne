using UnityEngine;

public class PatrollingStateMachine : EnemySimpleStateMachine
{

    [SerializeField] private PatrollPhysics patrollPhysics;

    [Header("IdleState")]
    [SerializeField] private string idleAnimationName;
    [SerializeField] private float minIdleTime;
    [SerializeField] private float maxIdleTime;
    private float idleStateTimer;

    [Header("MoveState")]
    [SerializeField] private string moveAnimationName;
    [SerializeField] private float speed;
    [SerializeField] private float minMoveTime;
    [SerializeField] private float maxMoveTime;
    [SerializeField] private float minimumTurnDelay;
    private float moveStateTimer;
    private float turnCooldown;

    [Header("AttackState")]
    [SerializeField] private string attackAnimationName;

    #region IDLE
    public override void EnterIdle()
    {
        anim.Play(idleAnimationName);
        idleStateTimer = Random.Range(minIdleTime, maxIdleTime);
        patrollPhysics.NegateForces();
    }

    public override void UpdateIdle()
    {
        idleStateTimer -= Time.deltaTime;
        if (idleStateTimer <= 0f)
        {
            ChangeState(EnemyState.Move);
        }
        if (patrollPhysics.inAttackRange)
        {
            ChangeState(EnemyState.Attack);
        }
    }

    public override void ExitIdle()
    {

    }
    #endregion

    #region MOVE
    public override void EnterMove()
    {
        anim.Play(moveAnimationName);
        moveStateTimer = Random.Range(minMoveTime, maxMoveTime);
    }
    public override void UpdateMove()
    {
        moveStateTimer -= Time.deltaTime;
        if (moveStateTimer <= 0f)
        {
            ChangeState(EnemyState.Idle);
        }
        if (patrollPhysics.inAttackRange)
        {
            ChangeState(EnemyState.Attack);
        }

        if (turnCooldown > 0f)
        {
            turnCooldown -= Time.deltaTime;
        }

        if (patrollPhysics.wallDetected || !patrollPhysics.groundDetected)
        {
            if (turnCooldown > 0) { return; }
            ForceFlip();
            speed *= -1f;
            turnCooldown = minimumTurnDelay;
        }
    }

    public override void FixedUpdateMove()
    {
        patrollPhysics.rb.linearVelocity = new Vector2(speed, patrollPhysics.rb.linearVelocityY);
    }
    public override void ExitMove()
    {

    }
    #endregion

    #region ATTACK

    public override void EnterAttack()
    {
        anim.Play(attackAnimationName);
        patrollPhysics.NegateForces();
    }
    public void EndOfAttack()
    {
        if (patrollPhysics.inAttackRange)
        {
            anim.Play(attackAnimationName, 0, 0);
        }
        else
        {
            ChangeState(previousState);
        }
    }

    #endregion
}