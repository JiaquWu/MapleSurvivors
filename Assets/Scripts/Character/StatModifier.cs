using UnityEngine;

[System.Serializable]
public class StatModifier
{
    public StatId Stat;
    public StatModType modType;
    public float Value;     // Mul/FinalMul �� 1.10 ��ʾ +10%
    public int Stacks = 1;
    public float? Duration = null; // �룻null=����
    public string SourceId;  // Ψһ��Դ
}
