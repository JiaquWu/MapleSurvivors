using System.Collections.Generic;
using UnityEngine;

public class LevelManager: SingletonManager<LevelManager>
{
  [SerializeField] EnemySpawner enemySpawner;
  //[SerializeField] LevelEventBus levelBus; // SO 事件总线（下节讲）

  int alive;
  List<EnemyHandle> current = new();

  //public void StartWave(WaveConfig cfg)
  //{
  //  Clear();
  //  current = enemySpawner.SpawnWave(cfg, out alive);
  //  foreach (var e in current) e.Hub.OnDie += HandleEnemyDie;
  //  levelBus.RaiseWaveStarted(cfg.index);
  //}

  void HandleEnemyDie()
  {
    //if (--alive <= 0) levelBus.RaiseWaveCleared();
  }

  void Clear()
  {
    foreach (var e in current) if (e != null) e.Hub.OnDie -= HandleEnemyDie;
    current.Clear(); alive = 0;
  }
}
