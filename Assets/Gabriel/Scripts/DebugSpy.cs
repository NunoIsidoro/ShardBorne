using UnityEngine;

public class DebugSpy : MonoBehaviour
{
    private StateMachine stateMachine;
    private Rigidbody2D rb;
    private PlayerStates.State lastState;

    void Start()
    {
        // Tenta encontrar as referencias automaticamente
        stateMachine = GetComponent<Player>().stateMachine; // Assume que tens o script Player aqui
        rb = GetComponent<Rigidbody2D>();

        if (stateMachine != null) lastState = stateMachine.currentState;
    }

    void Update()
    {
        if (stateMachine == null) return;

        // Só faz print se o estado mudou
        if (stateMachine.currentState != lastState)
        {
            LogChange(lastState, stateMachine.currentState);
            lastState = stateMachine.currentState;
        }
    }

    void LogChange(PlayerStates.State from, PlayerStates.State to)
    {
        string log = $"<color=yellow>MUDANÇA DE ESTADO:</color> {from} -> <color=green>{to}</color>";
        log += $"\nVelocidade X no momento da troca: {rb.linearVelocity.x}";
        log += $"\nVelocidade Y no momento da troca: {rb.linearVelocity.y}";
        log += $"\nGravidade atual: {rb.gravityScale}";
        log += $"\nInput Horizontal: {GetComponent<Player>().gatherInput.horizontalInput}";

        Debug.Log(log);
    }
}