using System.Collections.Generic;
using UnityEngine;

public interface IStatModifierSink
{
    void AddOrUpdate(string sourceId, IEnumerable<StatModifier> mods);
    void RemoveBySource(string sourceId);
}
