using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpec", menuName = "Scriptable Objects/EnemySpec")]
public class EnemySpec : ScriptableObject
{
  [Header("Stats")]
  public CharacterBaseAttribute baseAttribute;     // 组合：引用基础属性包
  //public List<ModifierDef> onSpawnModifiers = new(); // 出生时附加的差异（可为空）

  [Header("Behaviour")]
  public float moveSpeed = 2f;
  public float attackCooldown = 1.5f;
  //public AttackType attackType;

  [Header("Presentation")]
  public Sprite Sprite;
  public SpriteClip moveClip;
  public SpriteClip hurtClip;
  public SpriteClip deathClip;

  [Header("Rewards")]
  public int xpReward = 1;
  public int goldReward = 0;

  // 便捷：把“最终初始修正”吐出来，供运行时拷贝使用
  //public IEnumerable<StatEntry> GetBaseStats() => baseAttribute != null ? baseAttribute.BaseStats : Array.Empty<StatEntry>();
  //public IEnumerable<StatModifier> GetSpawnMods(string sourceId)
  //{
  //  foreach (var d in onSpawnModifiers)
  //    yield return new StatModifier { Stat = d.stat, Op = d.op, Value = d.value, Stacks = d.stacks, Duration = d.duration, Priority = d.priority, SourceId = sourceId };
  //}
}
