using System;
using System.Collections.Generic;
using UnityEngine;


public interface IHandleRegistry
{
  event Action<PlayerHandle> OnPlayerSpawned;
  event Action<PlayerHandle> OnPlayerDespawned;
  event Action<EnemyHandle> OnEnemySpawned;
  event Action<EnemyHandle> OnEnemyDespawned;

  IReadOnlyList<PlayerHandle> Players { get; }
  IReadOnlyList<EnemyHandle> Enemies { get; }

  bool TryGetPlayer(int id, out PlayerHandle h);
  bool TryGetEnemy(int id, out EnemyHandle h);
}

public class HandleRegistry : SingletonManager<HandleRegistry>, IHandleRegistry
{

  readonly List<PlayerHandle> _players = new();
  readonly List<EnemyHandle> _enemies = new();
  public IReadOnlyList<PlayerHandle> Players => _players;
  public IReadOnlyList<EnemyHandle> Enemies => _enemies;

  public event Action<PlayerHandle> OnPlayerSpawned, OnPlayerDespawned;
  public event Action<EnemyHandle> OnEnemySpawned, OnEnemyDespawned;

  public void Register(PlayerHandle h) { _players.Add(h); OnPlayerSpawned?.Invoke(h); }
  public void Unregister(PlayerHandle h) { if (_players.Remove(h)) OnPlayerDespawned?.Invoke(h); }

  public void Register(EnemyHandle h) { _enemies.Add(h); OnEnemySpawned?.Invoke(h); }
  public void Unregister(EnemyHandle h) { if (_enemies.Remove(h)) OnEnemyDespawned?.Invoke(h); }

  public bool TryGetPlayer(int id, out PlayerHandle h) { h = _players.Find(p => p.Id == id); return h != null; }
  public bool TryGetEnemy(int id, out EnemyHandle h) { h = _enemies.Find(e => e.Id == id); return h != null; }
}

public sealed class PlayerHandle
{
  public readonly int Id;
  public readonly Transform Root;
  public readonly CharacterEventHub Hub;
  public readonly IAttributesQuery Attr;
  public readonly IStatModifierSink StatSink;

  public PlayerHandle(int id, Transform root, CharacterEventHub hub,
                      IAttributesQuery attr, IStatModifierSink sink)
  { Id = id; Root = root; Hub = hub; Attr = attr; StatSink = sink; }
}

public sealed class EnemyHandle
{
  public readonly int Id;
  public readonly Transform Root;
  public readonly CharacterEventHub Hub;
  public readonly IAttributesQuery Attr;
  public EnemyHandle(int id, Transform root, CharacterEventHub hub, IAttributesQuery attr)
  { Id = id; Root = root; Hub = hub; Attr = attr; }
}
