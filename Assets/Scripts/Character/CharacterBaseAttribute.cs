using UnityEngine;
using System.Collections.Generic;

public enum StatId
{
    Attack,
    Defense,
    MagicAttack,
    MagicDefense,
    MaxHealth,
    MaxMana,
    HealthRegen,
    ManaRegen,
    MoveSpeed,
    AttackSpeed,
    CritChance,
    CritDamage
}

public enum StatModType
{
    Add,
    Multiply,
    FinalMultiply
}

[System.Serializable]
public struct StatEntry
{
    public StatId Id;
    public float Value;
}

[CreateAssetMenu(fileName = "CharacterBaseAttribute", menuName = "Scriptable Objects/CharacterBaseAttribute")]
public class CharacterBaseAttribute : ScriptableObject
{
    [SerializeField] List<StatEntry> baseStats = new List<StatEntry>();
}
