#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

[System.Serializable] class DB { public Meta metadata; public Dictionary<string, object> variables; public List<Scene> scenes; }
[System.Serializable] class Meta { public string title; public Dictionary<string, Speaker> speakers; }
[System.Serializable] class Speaker { public string race, portrait, voice, cloak_hint; }
[System.Serializable] class Scene { public string id; public List<Node> nodes; }

[System.Serializable]
class Node
{
    public string id, scene, speaker, speaker_id, speaker_race, text, next;
    public List<Choice> choices;
    public List<Condition> conditions;
    public List<KV> set;
    public List<EventAction> on_enter;
}

[System.Serializable]
class Choice
{
    public string id, label;
    [JsonProperty("goto")] public string gotoNode;   // <- mapeia "goto" do JSON para um nome seguro em C#
    public List<Condition> conditions;
    public List<KV> set;
}

[System.Serializable] class Condition { public string var, op, value; }
[System.Serializable] class KV { public string key, value; }

[System.Serializable]
class EventAction
{
    public string type, text, cue, id, item_id, enemy_id, style;
    public float intensity, duration;
    public int count;
}

public static class DialogueJsonImporter
{
    [MenuItem("Dialogue/Import JSON → DialogueAsset")]
    public static void ImportJson()
    {
        var path = EditorUtility.OpenFilePanel("Seleciona act1_dialogues_races.json", Application.dataPath, "json");
        if (string.IsNullOrEmpty(path)) return;

        string json = File.ReadAllText(path);
        var db = JsonConvert.DeserializeObject<DB>(json);
        if (db == null || db.scenes == null)
        {
            Debug.LogError("JSON inválido ou sem cenas.");
            return;
        }

        var asset = ScriptableObject.CreateInstance<DialogueAsset>();
        asset.startNodeId = "A0_Narrador";
        asset.nodes = new List<DialogueNode>();

        foreach (var sc in db.scenes)
        {
            foreach (var n in sc.nodes)
            {
                var dn = new DialogueNode();
                dn.id = n.id;
                dn.speaker = n.speaker;
                dn.speakerRace =
                    (string.IsNullOrEmpty(n.speaker_race) && db.metadata != null && db.metadata.speakers != null &&
                     !string.IsNullOrEmpty(n.speaker) && db.metadata.speakers.ContainsKey(n.speaker))
                    ? db.metadata.speakers[n.speaker].race
                    : n.speaker_race;

                dn.text = n.text;
                dn.nextNodeId = n.next;
                dn.type = (n.choices != null && n.choices.Count > 0) ? NodeType.Choice : NodeType.Line;

                // Choices
                if (n.choices != null)
                {
                    foreach (var c in n.choices)
                    {
                        var ch = new DialogueChoice();
                        ch.text = c.label;
                        ch.nextNodeId = c.gotoNode; // <- usa o campo seguro

                        if (c.set != null)
                        {
                            foreach (var kv in c.set)
                            {
                                var act = KVToAction(kv);
                                if (act != null) ch.onChooseActions.Add(act);
                            }
                        }
                        if (c.conditions != null)
                        {
                            foreach (var cond in c.conditions)
                            {
                                var dc = OpToCondition(cond);
                                if (dc != null) ch.conditions.Add(dc);
                            }
                        }
                        dn.choices.Add(ch);
                    }
                }

                // Condições do nó como branches simples
                if (n.conditions != null && n.conditions.Count > 0)
                {
                    foreach (var cond in n.conditions)
                    {
                        var dc = OpToCondition(cond);
                        if (dc != null) dn.branches.Add(new DialogueConditionBranch { condition = dc, nextNodeId = dn.nextNodeId });
                    }
                    if (dn.branches.Count > 0) dn.type = NodeType.Branch;
                }

                // 'set' do nó → onEnterActions
                if (n.set != null)
                {
                    foreach (var kv in n.set)
                    {
                        var act = KVToAction(kv);
                        if (act != null) dn.onEnterActions.Add(act);
                    }
                }

                asset.nodes.Add(dn);
            }
        }

        var save = EditorUtility.SaveFilePanelInProject("Guardar DialogueAsset", "Act1_DialogueAsset", "asset", "Onde guardar o asset?");
        if (!string.IsNullOrEmpty(save))
        {
            AssetDatabase.CreateAsset(asset, save);
            AssetDatabase.SaveAssets();
            Selection.activeObject = asset;
            Debug.Log($"Importadas {asset.nodes.Count} falas/nós para {save}");
        }
    }

    static DialogueAction KVToAction(KV kv)
    {
        if (kv == null || string.IsNullOrEmpty(kv.key)) return null;
        var a = new DialogueAction();

        // "+1"/"-1" → Add
        if (!string.IsNullOrEmpty(kv.value) && (kv.value.StartsWith("+") || kv.value.StartsWith("-")))
        {
            if (float.TryParse(kv.value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var delta))
            {
                a.actionType = ActionType.Add;
                a.variableName = kv.key;
                a.numberValue = delta;
                return a;
            }
        }

        // "true"/"false" → Set bool
        if (kv.value == "true" || kv.value == "false")
        {
            a.actionType = ActionType.Set;
            a.variableName = kv.key;
            a.boolValue = (kv.value == "true");
            return a;
        }

        // número → Set number
        if (float.TryParse(kv.value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var num))
        {
            a.actionType = ActionType.Set;
            a.variableName = kv.key;
            a.numberValue = num;
            return a;
        }

        // string → Set texto
        a.actionType = ActionType.Set;
        a.variableName = kv.key;
        a.stringValue = kv.value;
        return a;
    }

    static DialogueCondition OpToCondition(Condition c)
    {
        if (c == null || string.IsNullOrEmpty(c.var)) return null;
        var dc = new DialogueCondition();
        dc.variable = c.var;

        if (c.value == "true" || c.value == "false")
        {
            dc.type = ConditionType.Bool;
            dc.boolValue = (c.value == "true");
        }
        else if (float.TryParse(c.value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var num))
        {
            dc.type = ConditionType.Number;
            dc.numberValue = num;
        }
        else
        {
            dc.type = ConditionType.String;
            dc.stringValue = c.value;
        }

        switch (c.op)
        {
            case "==": dc.comparison = ComparisonType.Equal; break;
            case "!=": dc.comparison = ComparisonType.NotEqual; break;
            case ">": dc.comparison = ComparisonType.Greater; break;
            case "<": dc.comparison = ComparisonType.Less; break;
            case ">=": dc.comparison = ComparisonType.GreaterOrEqual; break;
            case "<=": dc.comparison = ComparisonType.LessOrEqual; break;
            default: dc.comparison = ComparisonType.Equal; break;
        }
        return dc;
    }
}
#endif
