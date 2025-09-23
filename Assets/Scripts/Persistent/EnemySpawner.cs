using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
  public event System.Action OnAllEnemiesCleared;

  int aliveCount;
  readonly List<CharacterEventHub> hubs = new();

  public void SpawnWave(GameObject[] prefabs)
  {
    ClearOld(); 
    aliveCount = 0;
    foreach (var p in prefabs)
    {
      var enemy = Instantiate(p, GetSpawnPos(), Quaternion.identity);
      var hub = enemy.GetComponent<CharacterEventHub>();
      hubs.Add(hub);
      aliveCount++;
      hub.OnDie += HandleDeath;
    }
  }

  Vector3 GetSpawnPos()
  {
    return Vector3.zero; // You can implement your own spawn logic here
  }

  void HandleDeath()
  {
    aliveCount--;
    if (aliveCount <= 0) OnAllEnemiesCleared?.Invoke();
  }

  void ClearOld()
  {
    foreach (var h in hubs) if (h) h.OnDie -= HandleDeath;
    hubs.Clear(); aliveCount = 0;
  }
}
