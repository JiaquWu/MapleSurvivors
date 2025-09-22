using System.Collections.Generic;
using UnityEngine;


// ÿ��һ������
[System.Serializable]
public class PassiveLevel
{
    public List<StatModifier> modifiers = new List<StatModifier>();
}

[CreateAssetMenu(fileName = "PassiveSpec", menuName = "Scriptable Objects/PassiveSpec")]
public class PassiveSpec : ScriptableObject
{
    [Tooltip("ÿһ������������"),SerializeField]
    private List<PassiveLevel> levels = new();

    // ת������ʱ�� StatModifier������ sourceId��
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
