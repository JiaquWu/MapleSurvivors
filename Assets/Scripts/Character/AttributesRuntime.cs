using System;
using System.Collections.Generic;
using UnityEngine;

public class AttributesRuntime : MonoBehaviour, IStatModifierSink, IAttributesQuery, ICharacterComponent
{
    public Type[] Requirements => new Type[] { typeof(CharacterEventHub) };

    public Type[] Provides => new Type[] { typeof(IAttributesQuery), typeof(IStatModifierSink) };

    // бн baseValues / modifiers / finalValues / dirty бн

    public void AddOrUpdate(string sourceId, IEnumerable<StatModifier> mods)
    {

        Debug.Log("sourceId: " + sourceId + mods.ToString());
    }

    public void RemoveBySource(string sourceId)
    {
        //
    }

    public float Get(StatId id)
    {
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

