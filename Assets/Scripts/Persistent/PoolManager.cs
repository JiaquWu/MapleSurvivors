using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonManager<PoolManager>
{
  // 通过 prefab 做 key
  readonly Dictionary<GameObject, PrefabPool> _pools = new();

  PrefabPool GetPool(GameObject prefab)
  {
    if (!_pools.TryGetValue(prefab, out var pool))
    {
      pool = new PrefabPool(prefab, transform);
      _pools.Add(prefab, pool);
    }
    return pool;
  }

  public void Prewarm(GameObject prefab, int count) => GetPool(prefab).Prewarm(count);

  public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
      => GetPool(prefab).Get(pos, rot);

  public static void Release(GameObject go)
  {
    var tag = go.GetComponent<PooledTag>();
    if (tag != null && tag.Owner != null) { tag.Owner.Release(go); return; }
    // 非池对象也安全处理
    go.SetActive(false);
    Destroy(go);
  }
}
