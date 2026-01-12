using UnityEngine;

public class EnemySimpleStateMachine : MonoBehaviour
{
    protected EnemyState previousState;
    protected EnemyState currentState;

    [SerializeField] protected Animator anim;
    public bool facingRight = true;
    public void ForceFlip()
    {
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
    }
    public enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Death
    }

    public void ChangeState(EnemyState newState)
    {
        if (newState == currentState) { return; }

        ExitState(currentState);
        previousState = currentState;
        currentState = newState;
        EnterState(currentState);
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;
            case EnemyState.Move:
                UpdateMove();
                break;
            case EnemyState.Attack:
                UpdateAttack();
                break;
            case EnemyState.Death:
                UpdateDeath();
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                FixedUpdateIdle();
                break;
            case EnemyState.Move:
                FixedUpdateMove();
                break;
            case EnemyState.Attack:
                FixedUpdateAttack();
                break;
            case EnemyState.Death:
                FixedUpdateDeath();
                break;
        }
    }
    protected void EnterState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle:
                EnterIdle();
                break;
            case EnemyState.Move:
                EnterMove();
                break;
            case EnemyState.Attack:
                EnterAttack();
                break;
            case EnemyState.Death:
                EnterDeath();
                break;
        }
    }

    protected void ExitState(EnemyState state)
    {
        switch (state)
        {
            case EnemyState.Idle:
                ExitIdle();
                break;
            case EnemyState.Move:
                ExitMove();
                break;
            case EnemyState.Attack:
                ExitAttack();
                break;
            case EnemyState.Death:
                ExitDeath();
                break;
        }
    }

    //Entrar nos estados
    public virtual void EnterIdle() // virtual para poder dar override em outro script de inimigo
    {

    }
    public virtual void EnterMove()
    {

    }

    public virtual void EnterAttack()
    {

    }

    public virtual void EnterDeath()
    {

    }

    //Sair dos estados
    public virtual void ExitIdle()
    {

    }
    public virtual void ExitMove()
    {

    }

    public virtual void ExitAttack()
    {

    }

    public virtual void ExitDeath()
    {

    }

    //Atualizar os estados
    public virtual void UpdateIdle()
    {

    }
    public virtual void UpdateMove()
    {

    }

    public virtual void UpdateAttack()
    {

    }
    public virtual void UpdateDeath()
    {

    }

    //Fixed Update dos estados
    public virtual void FixedUpdateIdle()
    {

    }
    public virtual void FixedUpdateMove()
    {

    }
    public virtual void FixedUpdateAttack()
    {

    }
    public virtual void FixedUpdateDeath()
    {

    }
}
