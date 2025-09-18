using UnityEngine;
using System;
using System.Collections.Generic;

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
    throw new System.InvalidOperationException($"[CharacterServices] Missing: {typeof(T).Name}");
  }

  public bool TryGet<T>(out T value) where T : class
  {
    if (map.TryGetValue(typeof(T), out var o)) { value = (T)o; return true; }
    value = null; return false;
  }

  public bool Contains(System.Type t) => map.ContainsKey(t);
}
