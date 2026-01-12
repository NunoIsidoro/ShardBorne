using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<TKey, TValue>>
{
    [SerializeField] private List<TKey> keys = new();
    [SerializeField] private List<TValue> values = new();

    [NonSerialized] private Dictionary<TKey, TValue> dict;

    private Dictionary<TKey, TValue> Dict
    {
        get { if (dict == null) dict = new Dictionary<TKey, TValue>(); return dict; }
    }

    // ----- Unity serialization -----
    public void OnBeforeSerialize()
    {
        keys.Clear(); values.Clear();
        if (dict == null) return;
        foreach (var kv in dict) { keys.Add(kv.Key); values.Add(kv.Value); }
    }

    public void OnAfterDeserialize()
    {
        dict = new Dictionary<TKey, TValue>();
        int n = Math.Min(keys.Count, values.Count);
        for (int i = 0; i < n; i++) dict[keys[i]] = values[i];
    }

    // ----- API mínima usada -----
    public bool TryGetValue(TKey key, out TValue value) => Dict.TryGetValue(key, out value);
    public bool ContainsKey(TKey key) => Dict.ContainsKey(key);
    public TValue this[TKey key] { get => Dict[key]; set => Dict[key] = value; }
    public void Clear() => Dict.Clear();

    // Iteração
    public IEnumerable<KeyValuePair<TKey, TValue>> Pairs() => Dict;
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Dict.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // Helpers ocasionais
    public Dictionary<TKey, TValue> ToDictionary() => new Dictionary<TKey, TValue>(Dict);
    public void SetFrom(IDictionary<TKey, TValue> src)
    {
        Dict.Clear();
        foreach (var kv in src) Dict[kv.Key] = kv.Value;
    }
}
