using UnityEngine;
using UnityEngine.InputSystem;

public class DashAbility : BaseAbility
{
    public InputActionReference dashActionRef;
    [SerializeField] private float dashForce;
    [SerializeField] private float maxDashDuration;
    private float dashTimer;

    private string dashAnimParameterName = "Dash";
    private int dashParameterID;

    protected override void Inicialization()
    {
        base.Inicialization();
        dashParameterID = Animator.StringToHash(dashAnimParameterName);
    }

    private void OnEnable()
    {
        dashActionRef.action.performed += TryToDash;
    }

    private void OnDisable()
    {
        dashActionRef.action.performed -= TryToDash;
    }

    public override void ExitAbility()
    {
        linkedPhysics.EnableGravity();

        //linkedPhysics.ResetVelocity();
    }

    private void TryToDash(InputAction.CallbackContext value)
    {
        if (!isPermitted || linkedStateMachine.currentState == PlayerStates.State.KnockBack ||
            linkedStateMachine.currentState == PlayerStates.State.Attack || // Impede cancelar ataque com dash
            linkedStateMachine.currentState == PlayerStates.State.WallJump)
        {
            return;
        }
        
        if(linkedStateMachine.currentState == PlayerStates.State.Dash || linkedPhysics.wallDetected)
        {
            return; // já está em dash
        }

        linkedStateMachine.ChangeState(PlayerStates.State.Dash);
        linkedPhysics.DisableGravity();
        linkedPhysics.ResetVelocity();
        if (player.facingRight)
        {
            linkedPhysics.rb.linearVelocityX = dashForce;
        }
        else { 
            linkedPhysics.rb.linearVelocityX = -dashForce;
        }

        dashTimer = maxDashDuration;
    }

    public override void ProcessAbility()
    {
        dashTimer -= Time.deltaTime;
        if(linkedPhysics.wallDetected)
        {
            dashTimer = -1;
        }
        if (dashTimer <= 0)
        {
            //linkedPhysics.rb.linearVelocity = Vector2.zero;
            if (linkedPhysics.grounded)
            {
                linkedStateMachine.ChangeState(PlayerStates.State.Idle);
            }
            else
            {
                linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            }
        }
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(dashParameterID, linkedStateMachine.currentState == PlayerStates.State.Dash);
    }

}
