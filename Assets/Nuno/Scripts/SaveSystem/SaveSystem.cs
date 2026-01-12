using System;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    // --- Save da cena ---
    const string KEY_HAS_SAVE = "sb_has_save";
    const string KEY_LAST_SCENE = "sb_last_scene";

    // --- Save das DialogueVariables ---
    const string KEY_VARS = "sb_vars";

    public static bool HasSave() =>
        PlayerPrefs.GetInt(KEY_HAS_SAVE, 0) == 1 && !string.IsNullOrEmpty(GetLastScene());

    public static string GetLastScene() => PlayerPrefs.GetString(KEY_LAST_SCENE, "");

    public static void SaveAtScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;
        PlayerPrefs.SetString(KEY_LAST_SCENE, sceneName);
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 1);
        PlayerPrefs.Save();
    }

    public static void DeleteSave()
    {
        PlayerPrefs.DeleteKey(KEY_LAST_SCENE);
        PlayerPrefs.SetInt(KEY_HAS_SAVE, 0);
        PlayerPrefs.Save();
    }

    // ---------- BLOCO DE VARIÁVEIS DE DIÁLOGO ----------
    [Serializable] struct KVF { public string k; public float v; }
    [Serializable] struct KVB { public string k; public bool v; }
    [Serializable] struct KVS { public string k; public string v; }

    [Serializable]
    class VarsSave
    {
        public List<KVF> numbers = new();
        public List<KVB> flags = new();
        public List<KVS> texts = new();
    }

    /// <summary>Grava todas as DialogueVariables em PlayerPrefs (JSON).</summary>
    public static void SaveAll(DialogueVariables v)
    {
        if (v == null) return;

        var data = new VarsSave();

        // IMPORTANTe: iterar com .Pairs() (os teus dicionários são serializáveis, não Dictionary<,>)
        foreach (var kv in v.numbers.Pairs()) data.numbers.Add(new KVF { k = kv.Key, v = kv.Value });
        foreach (var kv in v.flags.Pairs()) data.flags.Add(new KVB { k = kv.Key, v = kv.Value });
        foreach (var kv in v.texts.Pairs()) data.texts.Add(new KVS { k = kv.Key, v = kv.Value });

        PlayerPrefs.SetString(KEY_VARS, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }

    /// <summary>Carrega as DialogueVariables de PlayerPrefs (JSON) para o objeto passado.</summary>
    public static void LoadAll(DialogueVariables v)
    {
        if (v == null) return;

        var json = PlayerPrefs.GetString(KEY_VARS, "");
        if (string.IsNullOrEmpty(json)) return;

        var data = JsonUtility.FromJson<VarsSave>(json);
        if (data == null) return;

        
        v.numbers.Clear();
        v.flags.Clear();
        v.texts.Clear();
        
        if (data.numbers != null) foreach (var p in data.numbers) v.numbers[p.k] = p.v;
        if (data.flags != null) foreach (var p in data.flags) v.flags[p.k] = p.v;
        if (data.texts != null) foreach (var p in data.texts) v.texts[p.k] = p.v;
    }

    /// <summary>Apaga apenas o save das DialogueVariables.</summary>
    public static void DeleteVars()
    {
        PlayerPrefs.DeleteKey(KEY_VARS);
        PlayerPrefs.Save();
    }

    public static void EnsureDefaultVars(DialogueVariables v)
    {
        if (v == null) return;

        // Do JSON (act1_dialogues_races_v2.json)
        if (!v.flags.ContainsKey("hasIntroPlayed")) v.flags["hasIntroPlayed"] = false;
        if (!v.texts.ContainsKey("quest_FarmVeggies")) v.texts["quest_FarmVeggies"] = "not_taken";
        if (!v.flags.ContainsKey("combat_TutorialDone")) v.flags["combat_TutorialDone"] = false;
        if (!v.flags.ContainsKey("hasFirstShard")) v.flags["hasFirstShard"] = false;
        if (!v.flags.ContainsKey("spoke_Weaver")) v.flags["spoke_Weaver"] = false;
        if (!v.flags.ContainsKey("hint_StrangerHouse")) v.flags["hint_StrangerHouse"] = false;
        if (!v.flags.ContainsKey("ability_HeavyStrike")) v.flags["ability_HeavyStrike"] = false;
        if (!v.flags.ContainsKey("unlocked_Subterranean")) v.flags["unlocked_Subterranean"] = false;
        if (!v.numbers.ContainsKey("morality_Purity")) v.numbers["morality_Purity"] = 0;



        // variaveies de quests e afins para a cena do estranho
        if (!v.texts.ContainsKey("quest_HeavyTraining"))
            v.texts["quest_HeavyTraining"] = "not_taken";
        if (!v.numbers.ContainsKey("count.rocksBroken"))
            v.numbers["count.rocksBroken"] = 0;



        // auxiliares
        if (!v.numbers.ContainsKey("count.potato")) 
            v.numbers["count.potato"] = 0;
        if (!v.flags.ContainsKey("enemy.farm.spawned")) 
            v.flags["enemy.farm.spawned"] = false;
    }

    public static void HardResetVars(DialogueVariables v, bool setNewGameDefaults = true)
    {
        // limpa só as variáveis (não mexe no last scene)
        PlayerPrefs.DeleteKey("sb_vars");
        PlayerPrefs.Save();

        if (v == null) return;

        // limpa o ScriptableObject em memória
        v.numbers.Clear();
        v.flags.Clear();
        v.texts.Clear();

        if (!setNewGameDefaults) return;

        // estado inicial do Acto 1
        v.texts["quest_FarmVeggies"] = "collect"; // not_taken → collect → return → done
        v.numbers["count.potato"] = 0;
        v.flags["enemy.farm.spawned"] = false;

        v.flags["hasIntroPlayed"] = false;
        v.flags["combat_TutorialDone"] = false;
        v.flags["hasFirstShard"] = false;
        v.flags["spoke_Weaver"] = false;
        v.flags["hint_StrangerHouse"] = false;
        v.flags["ability_HeavyStrike"] = false;
        v.flags["unlocked_Subterranean"] = false;
        v.numbers["morality_Purity"] = 0;
    }
}
