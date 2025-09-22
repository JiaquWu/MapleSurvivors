using UnityEngine;

[System.Serializable]
public class StatModifier
{
    public StatId Stat;
    public StatModType modType;
    public float Value;     // Mul/FinalMul 用 1.10 表示 +10%
    public int Stacks = 1;
    public float? Duration = null; // 秒；null=永久
    public string SourceId;  // 唯一来源
}
