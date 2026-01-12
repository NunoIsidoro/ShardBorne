using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ShardBorne/Dialogue/Speaker DB", fileName = "SpeakerDB")]
public class SpeakerDB : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        [Tooltip("ID do orador exatamente como no campo 'speaker' dos nós (ex.: Tharion, Mira)")]
        public string id;

        [Header("UI")]
        public Sprite portrait;
        public Color uiColor = Color.white;

        [Header("Áudio (opcional)")]
        public AudioClip voiceClip;

        [Header("Meta (opcional)")]
        public string race;
    }

    public List<Entry> entries = new();

    Dictionary<string, Entry> map;

    void OnEnable() => Rebuild();
#if UNITY_EDITOR
    void OnValidate() => Rebuild();
#endif

    public void Rebuild()
    {
        // Usa case-insensitive para evitar problemas com maiúsculas/minúsculas nos IDs
        map = new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
        foreach (var e in entries)
        {
            if (e == null || string.IsNullOrWhiteSpace(e.id)) continue;
            map[e.id] = e; // último vence
        }
    }

    public bool TryGet(string id, out Entry entry)
    {
        if (map == null) { entry = null; return false; }
        return map.TryGetValue(id, out entry);
    }

    public Sprite GetPortrait(string id)
        => TryGet(id, out var e) ? e.portrait : null;

    public AudioClip GetVoice(string id)
        => TryGet(id, out var e) ? e.voiceClip : null;

    public Color GetUiColor(string id)
        => TryGet(id, out var e) ? e.uiColor : Color.white;
}
