using System;
using System.Collections.Generic;
using UnityEngine;

public class AttributesRuntime : MonoBehaviour, IStatModifierSink, IAttributesQuery, ICharacterComponent
{
    public Type[] Requirements => new Type[] { typeof(PlayerEventHub) };

    public Type[] Provides => new Type[] { typeof(IAttributesQuery), typeof(IStatModifierSink) };

    // … baseValues / modifiers / finalValues / dirty …

    public void AddOrUpdate(string sourceId, IEnumerable<StatModifier> mods)
    {
        // 1) 根据 sourceId 覆盖/叠层
        // 2) 标记影响到的 StatId 为 dirty
        // 3) 可选：立即/按需重算并触发 OnStatChanged
        Debug.Log("sourceId: " + sourceId + mods.ToString());
    }

    public void RemoveBySource(string sourceId)
    {
        // 移除该来源的所有修正 + 标脏
    }

    public float Get(StatId id)
    { /* 读取前若 dirty → 局部重算 */
        return 0f;
    }

    public void Init(CharacterComponents components)
    {
        //throw new NotImplementedException();
    }

    public void PostInit(CharacterComponents components)
    {
        //throw new NotImplementedException();
    }
}

