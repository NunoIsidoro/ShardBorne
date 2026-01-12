using UnityEngine;

public class StateMachine
{
    public PlayerStates.State previousState;
    public PlayerStates.State currentState;
    public BaseAbility[] arrayOfAbilities;

    public void ChangeState(PlayerStates.State newState)
    {
        if (currentState == newState) return;

        bool stateFound = false;

        // 1. Verificar se a habilidade existe e se é permitida
        foreach (BaseAbility ability in arrayOfAbilities)
        {
            if (ability.thisAbilityState == newState)
            {
                stateFound = true;
                if (!ability.isPermitted)
                {
                    Debug.LogError($"<color=red>BLOQUEIO:</color> Tentaste mudar para {newState}, mas a habilidade tem 'IsPermitted' desligado!");
                    return; // Sai da função, impedindo a troca
                }
            }
        }

        // 2. Se o loop acabou e não encontrou a habilidade
        if (!stateFound)
        {
            Debug.LogError($"<color=red>ERRO CRÍTICO:</color> Tentaste mudar para {newState}, mas NENHUM script tem 'This Ability State' configurado como {newState} no Inspector!");
            return;
        }

        // 3. Troca de Estado (Exit)
        foreach (BaseAbility ability in arrayOfAbilities)
        {
            if (ability.thisAbilityState == currentState)
            {
                ability.ExitAbility();
                previousState = currentState;
            }
        }

        // 4. Troca de Estado (Enter)
        foreach (BaseAbility ability in arrayOfAbilities)
        {
            if (ability.thisAbilityState == newState)
            {
                // Sucesso!
                Debug.Log($"<color=green>SUCESSO:</color> Estado alterado de {currentState} para {newState}");
                currentState = newState;
                ability.EnterAbility();
                break;
            }
        }
    }

    public void ForceChange(PlayerStates.State newState)
    {
        previousState = currentState;
        currentState = newState;
    }
}