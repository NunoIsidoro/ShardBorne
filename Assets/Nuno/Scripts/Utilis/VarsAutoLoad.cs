using UnityEngine;

public class VarsAutoLoad : MonoBehaviour
{
    public DialogueVariables vars;
    void Awake()
    {
        SaveSystem.LoadAll(vars);
        SaveSystem.EnsureDefaultVars(vars); // <- cria chaves se faltarem
    }
    void OnDestroy() => SaveSystem.SaveAll(vars);
}
