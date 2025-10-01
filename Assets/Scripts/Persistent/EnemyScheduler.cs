using System.Collections.Generic;
using UnityEngine;

public struct PlayerSnapshot { public Vector2 Pos; public int Id; }

public class EnemyScheduler : MonoBehaviour
{
  readonly List<EnemyBrain> brains = new();
  readonly List<SpriteAnimatorLite> anims = new();
  PlayerSnapshot[] players;

  [Range(1, 8)] public int updateBuckets = 3; // 分帧
  int frameIndex;

  void LateUpdate()
  {
    // 1) 每帧缓存玩家位置（一次）
    players = SnapshotPlayers();

    // 2) 分桶更新 AI（降低尖峰）
    float dt = Time.deltaTime;
    for (int i = frameIndex; i < brains.Count; i += updateBuckets)
      brains[i].Tick(dt, players);

    // 3) 动画全部 tick（可不分桶）
    for (int i = 0; i < anims.Count; i++) anims[i].Tick(dt);

    frameIndex = (frameIndex + 1) % updateBuckets;
  }

  public void Register(EnemyBrain b, SpriteAnimatorLite a) { brains.Add(b); anims.Add(a); }
  public void Unregister(EnemyBrain b, SpriteAnimatorLite a) { brains.Remove(b); anims.Remove(a); }

  PlayerSnapshot[] SnapshotPlayers()
  {
    var list = HandleRegistry.Instance.Players;
    var arr = players == null || players.Length != list.Count ? new PlayerSnapshot[list.Count] : players;
    for (int i = 0; i < list.Count; i++) arr[i] = new PlayerSnapshot { Pos = list[i].Root.position, Id = list[i].Id };
    return arr;
  }
}
