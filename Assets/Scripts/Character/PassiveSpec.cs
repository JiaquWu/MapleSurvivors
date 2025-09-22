using System.Collections.Generic;
using UnityEngine;


// 每级一组修正
[System.Serializable]
public class PassiveLevel
{
    public List<StatModifier> modifiers = new List<StatModifier>();
}

[CreateAssetMenu(fileName = "PassiveSpec", menuName = "Scriptable Objects/PassiveSpec")]
public class PassiveSpec : ScriptableObject
{
    [Tooltip("每一级的修正定义"),SerializeField]
    private List<PassiveLevel> levels = new();

    // 转成运行时的 StatModifier（带上 sourceId）
    public List<StatModifier> GetLevelMods(int level, string sourceId)
    {
        var list = new List<StatModifier>();
        if (level < 1 || level > levels.Count) return list;

        var defs = levels[level - 1].modifiers;
        list.Capacity = defs.Count;

        for (int i = 0; i < defs.Count; i++)
        {
            var d = defs[i];
            list.Add(new StatModifier
            {
                Stat = d.Stat,
                modType = d.modType,
                Value = d.Value,
                Stacks = Mathf.Max(1, d.Stacks),
                Duration = d.Duration,
                SourceId = sourceId
            });
        }
        return list;
    }
}
