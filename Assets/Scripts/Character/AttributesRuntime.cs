using System;
using System.Collections.Generic;
using UnityEngine;

public class AttributesRuntime : MonoBehaviour, IStatModifierSink, IAttributesQuery, ICharacterComponent
{
    public Type[] Requirements => new Type[] { typeof(PlayerEventHub) };

    public Type[] Provides => new Type[] { typeof(IAttributesQuery), typeof(IStatModifierSink) };

    // �� baseValues / modifiers / finalValues / dirty ��

    public void AddOrUpdate(string sourceId, IEnumerable<StatModifier> mods)
    {
        // 1) ���� sourceId ����/����
        // 2) ���Ӱ�쵽�� StatId Ϊ dirty
        // 3) ��ѡ������/�������㲢���� OnStatChanged
        Debug.Log("sourceId: " + sourceId + mods.ToString());
    }

    public void RemoveBySource(string sourceId)
    {
        // �Ƴ�����Դ���������� + ����
    }

    public float Get(StatId id)
    { /* ��ȡǰ�� dirty �� �ֲ����� */
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

