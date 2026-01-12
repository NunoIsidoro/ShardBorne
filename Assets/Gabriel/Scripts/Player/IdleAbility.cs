using UnityEngine;

public class IdleAbility : BaseAbility
{
    private string idleAnimParameterName = "Idle";
    private int idleParameterInt;

    public override void EnterAbility()
    {
        linkedPhysics.rb.linearVelocityX = 0;
    }

    protected override void Inicialization()
    {
        base.Inicialization();
        idleParameterInt = Animator.StringToHash(idleAnimParameterName);
    }

    public override void ProcessAbility()
    {
        // 1. Detetar Queda (Cair sem saltar)
        if (!linkedPhysics.grounded)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            return;
        }

        if (linkedInput.horizontalInput != 0)
        {
            player.Flip();
            linkedStateMachine.ChangeState(PlayerStates.State.Run);
        }
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(idleParameterInt, linkedStateMachine.currentState == PlayerStates.State.Idle);
    }
}
