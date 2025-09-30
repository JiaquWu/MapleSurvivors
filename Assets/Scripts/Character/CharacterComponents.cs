using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class CharacterComponents
{
  private readonly Dictionary<System.Type, object> map = new();

  public void Register(System.Type t, object instance)
  {
    if (t == null || instance == null) return;
    map[t] = instance;
  }

  public T Require<T>() where T : class
  {
    if (map.TryGetValue(typeof(T), out var o)) return (T)o;
    foreach (var kv in map)
    {
      if (kv.Key.IsAssignableFrom(typeof(T)))
        return (T)kv.Value;
    }
    throw new System.InvalidOperationException($"[CharacterServices] Missing: {typeof(T).Name}");
  }

  public bool TryGet<T>(out T value) where T : class
  {
    if (map.TryGetValue(typeof(T), out var o)) { value = (T)o; return true; }
    foreach (var kv in map)
    {
      if (kv.Key.IsAssignableFrom(typeof(T)))
      {
        value = (T)kv.Value;
        return true;
      }
    }
    value = null; return false;
  }

  public bool Contains(System.Type t)
  {
    for (int i = 0; i < map.Count; i++)
    {
      var key = map.ElementAt(i).Key;
      if (key.IsAssignableFrom(t))
        return true;
    }
    return false;
    //return map.Any(keyValue => t.IsAssignableFrom(keyValue.Key));
  }
}
