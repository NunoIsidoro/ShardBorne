using System;
using System.Collections;
using UnityEngine;

public class KnockBackAbility : BaseAbility
{

    Coroutine currentKnockBack;

    public override void ExitAbility()
    {
        currentKnockBack = null;
    }

    //Para multiplos KnockBacks sem bugar
    public void StartKnockBack(float duration, Vector2 force, Transform enemyObject)
    {
        if (player.playerStats.GetCanTakeDamage() == false) return;
        if (currentKnockBack == null)
        {
            currentKnockBack = StartCoroutine(KnockBack(duration, force, enemyObject));
        }
        else
        {
            //Podemos tambem nao fazer nada aqui, mas assim fica melhor
            StopCoroutine(currentKnockBack);
            currentKnockBack = StartCoroutine(KnockBack(duration, force, enemyObject));
        }
    }
    public void StartSwingKnockBack(float duration, Vector2 force, int direction)
    {
        if (player.playerStats.GetCanTakeDamage() == false) return;
        if (currentKnockBack == null)
        {
            currentKnockBack = StartCoroutine(SwingKnockBack(duration, force, direction));
        }
        else
        {
            //Podemos tambem nao fazer nada aqui, mas assim fica melhor
            StopCoroutine(currentKnockBack);
            currentKnockBack = StartCoroutine(SwingKnockBack(duration, force, direction));
        }
    }
    public IEnumerator KnockBack(float duration, Vector2 force, Transform enemyObject)
    {
        linkedStateMachine.ChangeState(PlayerStates.State.KnockBack);
        linkedPhysics.ResetVelocity();
        if(transform.position.x >= enemyObject.transform.position.x)
        {
            linkedPhysics.rb.linearVelocity = force;
        }
        else
        {
            linkedPhysics.rb.linearVelocity = new Vector2(-force.x, force.y);
        }
        yield return new WaitForSeconds(duration);
        //retorna para os outros estados

        if (player.playerStats.GetCurrentHealth() > 0)
        {
            if (linkedPhysics.grounded)
            {
                if (linkedInput.horizontalInput != 0)
                {
                    linkedStateMachine.ChangeState(PlayerStates.State.Run);
                }
                else
                {
                    linkedStateMachine.ChangeState(PlayerStates.State.Idle);
                }
            }
            else
            {
                linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            }
        }
        else
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Death);
        }
    }
    public IEnumerator SwingKnockBack(float duration, Vector2 force, int direction)
    {
        linkedStateMachine.ChangeState(PlayerStates.State.KnockBack);
        linkedPhysics.ResetVelocity();
        
        force.x *= direction;
        linkedPhysics.rb.linearVelocity = force;

        yield return new WaitForSeconds(duration);
        //retorna para os outros estados

        if (player.playerStats.GetCurrentHealth() > 0)
        {
            if (linkedPhysics.grounded)
            {
                if (linkedInput.horizontalInput != 0)
                {
                    linkedStateMachine.ChangeState(PlayerStates.State.Run);
                }
                else
                {
                    linkedStateMachine.ChangeState(PlayerStates.State.Idle);
                }
            }
            else
            {
                linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            }
        }
        else
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Death);
        }
    }
    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool("KnockBack", linkedStateMachine.currentState == PlayerStates.State.KnockBack);
    }
}
