using UnityEngine;

public class Poolable : MonoBehaviour
{
  // 归还时调用（可覆写做重置）
  public virtual void OnSpawned() { }
  public virtual void OnDespawned() { }

  // 方便物体内部自行归还（如子弹命中、特效播放完）
  public void ReturnToPool() => PoolManager.Release(gameObject);
}
