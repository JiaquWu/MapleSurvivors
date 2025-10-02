using System;
using System.Collections.Generic;
using UnityEngine;

//public enum LevelSignal { WaveStarted, WaveCleared }
//public readonly struct WaveStarted { public readonly int Index; public WaveStarted(int i) => Index = i; }
//public readonly struct PlayerLeveledUp
//{
//  public readonly int PlayerId, NewLevel;
//  public PlayerLeveledUp(int id, int lv) { PlayerId = id; NewLevel = lv; }
//}

//static class GameSignals
//{
//  static readonly Dictionary<LevelSignal, Delegate> _map = new();
//  static readonly Dictionary<LevelSignal, Type> _payload = new() {
//    { LevelSignal.WaveStarted, typeof(WaveStarted) },
//    { LevelSignal.WaveCleared, typeof(ValueTuple) } // 无载荷
//  };

//  public static IDisposable StartListening<T>(LevelSignal key, Action<T> h)
//  {
//    Ensure<T>(key);
//    if (_map.TryGetValue(key, out var d)) _map[key] = (Action<T>)d + h;
//    else _map[key] = h;
//    return new Sub<T>(key, h);
//  }
//  public static void StopListening<T>(LevelSignal key, Action<T> h)
//  {
//    if (_map.TryGetValue(key, out var d)) _map[key] = (Action<T>)d - h;
//  }
//  public static void Raise<T>(LevelSignal key, in T payload = default)
//  {
//    Ensure<T>(key);
//    if (_map.TryGetValue(key, out var d)) ((Action<T>)d)?.Invoke(payload);
//  }
//  static void Ensure<T>(LevelSignal key)
//  {
//    var want = _payload[key];
//    if (want != typeof(T)) throw new InvalidOperationException($"Payload type mismatch for {key}: expect {want.Name}, got {typeof(T).Name}");
//  }
//  struct Sub<T> : IDisposable
//  {
//    readonly LevelSignal k; readonly Action<T> h;
//    public Sub(LevelSignal k, Action<T> h) { this.k = k; this.h = h; }
//    public void Dispose() => StopListening(k, h);
//  }
//}

public readonly struct WaveStarted
{
  public int Index { get; }
  public WaveStarted(int index) { Index = index; }
  public override string ToString() => $"WaveStarted(Index={Index})";
}

public readonly struct NewGameStarted
{
  public int Seed { get; }
  public NewGameStarted(int seed) { Seed = seed; }
  public override string ToString()
  {
    return $"NewGameStarted(Seed={Seed})";
  }
}


//public record GameOver();

//public record WaveStarted(int Index);
//public record WaveCleared(int Index);

//public record PlayerLeveledUp(int PlayerId, int NewLevel);
//public record UpgradeOptionsReady(int PlayerId, UpgradeOption[] Options);
//public record UpgradeChosen(int PlayerId, UpgradeOption Option);

//public record PauseChanged(bool IsPaused);


public static class GameSignals
{
  static readonly Dictionary<Type, Delegate> _map = new();

  // 订阅：强类型、返回 IDisposable，省心退订
  public static IDisposable Subscribe<T>(Action<T> handler)
  {
    if (_map.TryGetValue(typeof(T), out var del))
      _map[typeof(T)] = (Action<T>)del + handler;
    else
      _map[typeof(T)] = handler;
    return new Sub<T>(handler);
  }
  public static void Unsubscribe<T>(Action<T> handler)
  {
    if (_map.TryGetValue(typeof(T), out var del)) _map[typeof(T)] = (Action<T>)del - handler;
  }
  public static void Publish<T>(in T payload)
  {
    if (_map.TryGetValue(typeof(T), out var del)) ((Action<T>)del)?.Invoke(payload);
  }
  struct Sub<T> : IDisposable
  {
    readonly Action<T> h; public Sub(Action<T> h) => this.h = h;
    public void Dispose() => Unsubscribe(h);
  }

  /*
   * // 订阅（自动托管退订）
IDisposable _token;
void OnEnable()  { _token = SignalBus.Subscribe<WaveStarted>(OnWaveStarted); }
void OnDisable() { _token?.Dispose(); }

void OnWaveStarted(WaveStarted s) => ui.ShowWaveIntro(s.Index);

// 触发
SignalBus.Publish(new WaveStarted(0));
SignalBus.Publish(new PlayerLeveledUp(playerId, newLevel));
   */
}
