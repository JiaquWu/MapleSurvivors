using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

class PrefabPool
{
  readonly GameObject _prefab;
  readonly Transform _root;
  readonly Stack<GameObject> _stack = new();

  public PrefabPool(GameObject prefab, Transform parent)
  {
    _prefab = prefab;
    _root = new GameObject($"Pool<{prefab.name}>").transform;
    _root.SetParent(parent, false);
  }

  public void Prewarm(int count)
  {
    for (int i = 0; i < count; i++)
    {
      var go = Create();
      go.SetActive(false);
      _stack.Push(go);
    }
  }

  GameObject Create()
  {
    var go = Object.Instantiate(_prefab, _root);
    // 让 PoolManager 能反查归还到哪个池
    go.AddComponent<PooledTag>().Owner = this;
    return go;
  }

  public GameObject Get(Vector3 pos, Quaternion rot)
  {
    var go = _stack.Count > 0 ? _stack.Pop() : Create();
    var t = go.transform;
    t.SetPositionAndRotation(pos, rot);
    go.SetActive(true);
    go.GetComponent<Poolable>()?.OnSpawned();
    return go;
  }

  public void Release(GameObject go)
  {
    go.GetComponent<Poolable>()?.OnDespawned();
    go.SetActive(false);
    go.transform.SetParent(_root, false);
    _stack.Push(go);
  }
}

class PooledTag : MonoBehaviour
{
  public PrefabPool Owner;
}
