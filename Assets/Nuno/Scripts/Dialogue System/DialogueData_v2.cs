using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "DialogueAsset", menuName = "ShardBorne/Dialogue/Dialogue Asset")]
public class DialogueAsset : ScriptableObject {
    [Tooltip("ID do primeiro nó a executar quando este asset for iniciado.")]
    public string startNodeId = "A0_Narrador";
    public List<DialogueNode> nodes = new List<DialogueNode>();
}

public enum NodeType { Line, Choice, Branch, End }

[Serializable]
public class DialogueNode {
    [Tooltip("Identificador único do nó.")]
    public string id;
    public NodeType type = NodeType.Line;

    // Linha
    public string speaker;
    [Tooltip("Raça do orador (se quiseres skinar UI).")]
    public string speakerRace; // ← NOVO (será preenchido pelo importador, mas podes ignorar)
    [TextArea(2, 6)] public string text;
    public Sprite portrait;
    public AudioClip voiceClip;
    public float voiceDelayAfter = 0f;

    // Fluxo linear
    [Tooltip("Se definido, este nó segue diretamente para este outro nó quando concluído.")]
    public string nextNodeId;

    // Variações de fluxo
    public List<DialogueChoice> choices = new();
    public List<DialogueConditionBranch> branches = new();

    // Eventos/efeitos
    public List<DialogueAction> onEnterActions = new();
    public List<DialogueAction> onExitActions = new();
}

[Serializable]
public class DialogueChoice {
    [TextArea(1, 4)] public string text;
    public string nextNodeId;
    public List<DialogueAction> onChooseActions = new();
    public List<DialogueCondition> conditions = new();
}

[Serializable]
public class DialogueConditionBranch {
    public DialogueCondition condition = new();
    public string nextNodeId;
}

public enum ConditionType { String, Number, Bool }
public enum ComparisonType { Equal, NotEqual, Greater, Less, GreaterOrEqual, LessOrEqual, Exists, NotExists }

[Serializable]
public class DialogueCondition {
    [Tooltip("Nome da variável a testar.")]
    public string variable;
    public ConditionType type = ConditionType.String;
    public ComparisonType comparison = ComparisonType.Equal;

    public string stringValue;
    public float numberValue;
    public bool boolValue;
}

public enum ActionType { Set, Add, Toggle, TriggerUnityEvent }

[Serializable]
public class DialogueAction {
    public ActionType actionType = ActionType.Set;

    [Header("Alvo (para Set/Add/Toggle)")]
    public string variableName;
    public string stringValue;
    public float numberValue;
    public bool boolValue;

    [Header("UnityEvent (para TriggerUnityEvent)")]
    public UnityEvent unityEvent;
    
    public void Execute(DialogueVariables vars) {
        if (vars == null) return;
        switch (actionType) {
            case ActionType.Set:
                vars.Set(variableName, stringValue, numberValue, boolValue);
                break;
            case ActionType.Add:
                vars.Add(variableName, numberValue);
                break;
            case ActionType.Toggle:
                vars.Toggle(variableName);
                break;
            case ActionType.TriggerUnityEvent:
                unityEvent?.Invoke();
                break;
        }
    }
    
}
