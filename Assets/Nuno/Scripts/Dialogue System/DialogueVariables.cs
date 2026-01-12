using MoreMountains.Tools;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueVariables", menuName = "ShardBorne/Dialogue/Dialogue Variables")]
public class DialogueVariables : ScriptableObject
{  
    [Serializable] public class FloatDict : SerializableDictionary<string, float> { }
    [Serializable] public class BoolDict : SerializableDictionary<string, bool> { }
    [Serializable] public class StringDict : SerializableDictionary<string, string> { }

    [Header("Armazenamento")]
    public FloatDict numbers = new();
    public BoolDict flags = new();
    public StringDict texts = new();

    // ---- API ----
    public bool TryGet(string key, out object value)
    {
        if (numbers.TryGetValue(key, out var f)) { value = f; return true; }
        if (flags.TryGetValue(key, out var b)) { value = b; return true; }
        if (texts.TryGetValue(key, out var s)) { value = s; return true; }
        value = null; return false;
    }

    public void Set(string key, string sVal, float fVal, bool bVal)
    {
        if (numbers.ContainsKey(key)) numbers[key] = fVal;
        else if (flags.ContainsKey(key)) flags[key] = bVal;
        else if (texts.ContainsKey(key)) texts[key] = sVal;
        else
        {
            if (!string.IsNullOrEmpty(sVal)) texts[key] = sVal;
            else if (Mathf.Abs(fVal) > 0.0001f) numbers[key] = fVal;
            else flags[key] = bVal;
        }
    }

    public void Add(string key, float delta)
    {
        if (!numbers.TryGetValue(key, out var cur)) cur = 0f;
        numbers[key] = cur + delta;
    }

    public void Toggle(string key)
    {
        if (!flags.TryGetValue(key, out var cur)) cur = false;
        flags[key] = !cur;
    }

    public bool Evaluate(DialogueCondition c)
    {
        if (c == null) return true;

        switch (c.type)
        {
            case ConditionType.Bool:
                bool b = flags.TryGetValue(c.variable, out var bv) && bv;
                return CompareBool(b, c.boolValue, c.comparison);

            case ConditionType.Number:
                float f = numbers.TryGetValue(c.variable, out var fv) ? fv : 0f;
                return CompareNumber(f, c.numberValue, c.comparison);

            default:
                texts.TryGetValue(c.variable, out var sv);
                return CompareString(sv, c.stringValue, c.comparison);
        }
    }
   
    bool CompareBool(bool a, bool b, ComparisonType cmp) =>
        cmp switch
        {
            ComparisonType.Equal => a == b,
            ComparisonType.NotEqual => a != b,
            ComparisonType.Exists => true,
            ComparisonType.NotExists => false,
            _ => a == b
        };

    bool CompareNumber(float a, float b, ComparisonType cmp) =>
        cmp switch
        {
            ComparisonType.Equal => Mathf.Approximately(a, b),
            ComparisonType.NotEqual => !Mathf.Approximately(a, b),
            ComparisonType.Greater => a > b,
            ComparisonType.Less => a < b,
            ComparisonType.GreaterOrEqual => a >= b,
            ComparisonType.LessOrEqual => a <= b,
            ComparisonType.Exists => true,
            ComparisonType.NotExists => false,
            _ => false
        };

    bool CompareString(string a, string b, ComparisonType cmp) =>
        cmp switch
        {
            ComparisonType.Equal => string.Equals(a, b, StringComparison.Ordinal),
            ComparisonType.NotEqual => !string.Equals(a, b, StringComparison.Ordinal),
            ComparisonType.Exists => a != null,
            ComparisonType.NotExists => a == null,
            _ => string.Equals(a, b, StringComparison.Ordinal)
        };
}
