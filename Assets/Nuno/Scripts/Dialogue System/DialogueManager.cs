using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("Data")]
    public DialogueAsset dialogue;
    public DialogueVariables variables;

    [Header("UI")]
    public DialogueUI ui;

    [Header("Speaker Data")]
    public SpeakerDB speakerDB;
    [Tooltip("Se true, tenta Resources/Portraits e Resources/Voices como fallback.")]
    public bool fallbackToResources = false;
    public string portraitsFolder = "Portraits"; // Resources/Portraits/<Speaker>
    public string voicesFolder = "Voices";    // Resources/Voices/<Speaker>

    [Header("Opções")]
    [Tooltip("Arranca sozinho no startNodeId quando a cena inicia")]
    public bool autoStart = true;

    [Tooltip("Se desligado, choices não aparecem — avança automaticamente (primeira válida).")]
    public bool allowChoices = false;

    // Estado/Events
    public event Action OnDialogueStart;
    public event Action OnDialogueEnd;
    public bool IsActive { get; private set; }

    // Internos
    readonly Dictionary<string, DialogueNode> index = new();
    DialogueNode current;

    void Awake()
    {
        BuildIndex();
    }

    void Start()
    {
        if (dialogue == null || ui == null)
        {
            Debug.LogError("[DialogueManager] Dialogue asset/UI em falta.");
            return;
        }

        if (autoStart && !string.IsNullOrEmpty(dialogue.startNodeId))
            StartDialogue(dialogue.startNodeId);
    }

    public void BuildIndex()
    {
        index.Clear();
        if (dialogue == null || dialogue.nodes == null) return;

        foreach (var n in dialogue.nodes)
        {
            if (n != null && !string.IsNullOrEmpty(n.id))
                index[n.id] = n;
        }
    }

    public void StartDialogue(string startId)
    {
        BuildIndex(); // garante índice atualizado
        if (!index.TryGetValue(startId, out var node))
        {
            Debug.LogError("[DialogueManager] Nó inicial não existe: " + startId);
            return;
        }

        IsActive = true;
        OnDialogueStart?.Invoke();
        ShowNode(node);
    }

    void ShowNode(DialogueNode node)
    {
        if (node == null)
        {
            Debug.LogError("[DialogueManager] Tentativa de mostrar um nó nulo. A encerrar diálogo.");
            EndDialogue();
            return;
        }

        current = node;

        if (node.onEnterActions != null)
            for (int i = 0; i < node.onEnterActions.Count; i++)
                node.onEnterActions[i]?.Execute(variables);

        switch (node.type)
        {
            case NodeType.Line:
            case NodeType.Choice:
                ui.DisplayLine(node.speaker, node.text, GetPortrait(node), GetVoiceClip(node),
                    () => OnLineFinished(node));
                break;

            case NodeType.Branch:
                if (node.branches != null)
                {
                    for (int i = 0; i < node.branches.Count; i++)
                    {
                        var b = node.branches[i];
                        if (variables.Evaluate(b.condition)) { ContinueToNode(b.nextNodeId); return; }
                    }
                }
                if (!string.IsNullOrEmpty(node.nextNodeId)) ContinueToNode(node.nextNodeId);
                else EndDialogue();
                break;

            case NodeType.End:
                EndDialogue();
                break;
        }
    }

    /*
    void ShowNode(DialogueNode node)
    {
        current = node;

        // onEnter
        if (node.onEnterActions != null)
            for (int i = 0; i < node.onEnterActions.Count; i++)
                node.onEnterActions[i]?.Execute(variables);

#if UNITY_EDITOR
        if (speakerDB && !string.IsNullOrEmpty(node.speaker) && speakerDB.GetPortrait(node.speaker) == null && node.portrait == null)
            Debug.LogWarning($"[SpeakerDB] Sem retrato para speaker '{node.speaker}'.", this);
#endif

        switch (node.type)
        {
            case NodeType.Line:
                ui.DisplayLine(node.speaker, node.text, GetPortrait(node), GetVoiceClip(node),
                    () => OnLineFinished(node));
                break;

            case NodeType.Choice:
                // Para fluxo linear, mostramos a linha e depois tratamos choices em OnLineFinished
                ui.DisplayLine(node.speaker, node.text, GetPortrait(node), GetVoiceClip(node),
                    () => OnLineFinished(node));
                break;

            case NodeType.Branch:
                // Vai para o primeiro branch cujo Evaluate for true; senão next; senão termina.
                if (node.branches != null)
                {
                    for (int i = 0; i < node.branches.Count; i++)
                    {
                        var b = node.branches[i];
                        if (variables.Evaluate(b.condition)) { ContinueToNode(b.nextNodeId); return; }
                    }
                }

                if (!string.IsNullOrEmpty(node.nextNodeId)) ContinueToNode(node.nextNodeId);
                else EndDialogue();
                break;

            case NodeType.End:
                EndDialogue();
                break;
        }
    }
    */
    void OnLineFinished(DialogueNode node)
    {
        variables?.Set("seen." + node.id, "", 0f, true);
        // onExit
        if (node.onExitActions != null)
            for (int i = 0; i < node.onExitActions.Count; i++)
                node.onExitActions[i]?.Execute(variables);

        // 1) Branches têm prioridade
        if (node.branches != null && node.branches.Count > 0)
        {
            for (int i = 0; i < node.branches.Count; i++)
            {
                var br = node.branches[i];
                if (variables.Evaluate(br.condition)) { ContinueToNode(br.nextNodeId); return; }
            }
        }

        // 2) Next linear
        if (!string.IsNullOrEmpty(node.nextNodeId))
        {
            ContinueToNode(node.nextNodeId);
            return;
        }

        // 3) Choices
        if (node.choices != null && node.choices.Count > 0)
        {
            if (!allowChoices)
            {
                // fluxo linear: escolhe a primeira válida
                for (int i = 0; i < node.choices.Count; i++)
                {
                    var ch = node.choices[i];
                    if (ChoicePasses(ch)) { ApplyChoiceAndGo(ch); return; }
                }
                // nenhuma válida → termina
                EndDialogue();
                return;
            }
            else
            {
                // (opcional) mostrar UI de choices
                var opts = new List<(string, Action)>();
                for (int i = 0; i < node.choices.Count; i++)
                {
                    var ch = node.choices[i];
                    if (!ChoicePasses(ch)) continue;

                    opts.Add((ch.text, () => ApplyChoiceAndGo(ch)));
                }

                if (opts.Count == 0) EndDialogue();
                else ui.DisplayChoices(opts);
                return;
            }
        }

        // 4) acabou
        EndDialogue();
    }

    bool ChoicePasses(DialogueChoice c)
    {
        if (c == null) return false;
        if (c.conditions == null || c.conditions.Count == 0) return true;
        for (int i = 0; i < c.conditions.Count; i++)
            if (!variables.Evaluate(c.conditions[i])) return false;
        return true;
    }

    void ApplyChoiceAndGo(DialogueChoice c)
    {
        if (c.onChooseActions != null)
            for (int i = 0; i < c.onChooseActions.Count; i++)
                c.onChooseActions[i]?.Execute(variables);

        ContinueToNode(c.nextNodeId);
    }

    public void ContinueToNode(string nodeId)
    {
        if (string.IsNullOrEmpty(nodeId)) { EndDialogue(); return; }
        if (!index.TryGetValue(nodeId, out var node))
        {
            Debug.LogError("[DialogueManager] Nó não encontrado: " + nodeId);
            EndDialogue();
            return;
        }
        ShowNode(node);
    }

    void EndDialogue()
    {
        IsActive = false;
        if (ui) ui.Hide();      // fecha painel
        OnDialogueEnd?.Invoke();
        Debug.Log("[DialogueManager] Diálogo terminado.");
    }

    // ---------- Retratos / Voz ----------
    Sprite GetPortrait(DialogueNode node)
    {
        // 1) Se o próprio nó fornece sprite, usa-o
        if (node != null && node.portrait) return node.portrait;

        // 2) SpeakerDB
        if (speakerDB && node != null && !string.IsNullOrEmpty(node.speaker))
        {
            var s = speakerDB.GetPortrait(node.speaker);
            if (s) return s;
        }

        // 3) Fallback para Resources (opcional)
        if (fallbackToResources && node != null && !string.IsNullOrEmpty(node.speaker))
            return Resources.Load<Sprite>($"{portraitsFolder}/{node.speaker}");

        return null;
    }

    AudioClip GetVoiceClip(DialogueNode node)
    {
        if (node != null && node.voiceClip) return node.voiceClip;

        if (speakerDB && node != null && !string.IsNullOrEmpty(node.speaker))
        {
            var c = speakerDB.GetVoice(node.speaker);
            if (c) return c;
        }

        if (fallbackToResources && node != null && !string.IsNullOrEmpty(node.speaker))
            return Resources.Load<AudioClip>($"{voicesFolder}/{node.speaker}");

        return null;
    }
}
