using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMotor2D : MonoBehaviour
{
  [SerializeField] float radiusEpsilon = 0.01f;   // 抗穿透余量
  [SerializeField] int castBufferSize = 8;      // 命中缓存
  [SerializeField] LayerMask collisionMask;       // Walls 等

  Rigidbody2D rb;
  RaycastHit2D[] hits;

  void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
    hits = new RaycastHit2D[castBufferSize];
  }

  /// <summary>
  /// 以“期望位移”推动刚体，遇到碰撞会沿法线滑动
  /// </summary>
  public void Move(Vector2 desiredDelta)
  {
    if (desiredDelta.sqrMagnitude < Mathf.Epsilon) return;

    // 尝试主方向位移
    Vector2 delta = desiredDelta;
    if (TryMove(delta, out var remainder))
    {
      // 完全移动成功
      return;
    }

    // 被挡：沿切线方向滑动（去掉法线分量）
    Vector2 along = Vector3.Project(remainder, Vector2.Perpendicular(GetHitNormal()));
    TryMove(along, out _);

    
  }
  // —— 本地函数 —— //
  bool TryMove(Vector2 delta, out Vector2 remain)
  {
    remain = delta;
    if (delta.sqrMagnitude < Mathf.Epsilon) return true;

    // Cast：预测位移是否会撞到东西
    int count = rb.Cast(delta.normalized, hits, delta.magnitude + radiusEpsilon);
    if (count == 0)
    {
      rb.position += delta; // 没撞就直接平移
      remain = Vector2.zero;
      return true;
    }

    // 找最近的命中
    float minDist = delta.magnitude;
    Vector2 hitNormal = Vector2.zero;
    for (int i = 0; i < count; i++)
    {
      if (hits[i].distance < minDist)
      {
        minDist = hits[i].distance;
        hitNormal = hits[i].normal;
      }
    }

    // 贴着碰撞体前移（留一点余量避免抖动）
    float moveDist = Mathf.Max(0f, minDist - radiusEpsilon);
    rb.position += delta.normalized * moveDist;

    // 计算剩余位移：去掉法线分量，实现“滑动”
    Vector2 remaining = delta - delta.normalized * moveDist;
    remain = Vector3.ProjectOnPlane(remaining, hitNormal);

    // 记录法线，供外部投影（仅示例）
    _lastHitNormal = hitNormal;

    return false;
  }

  Vector2 _lastHitNormal = Vector2.up; // 做个缓存供投影函数使用
  Vector2 GetHitNormal() => _lastHitNormal;
}